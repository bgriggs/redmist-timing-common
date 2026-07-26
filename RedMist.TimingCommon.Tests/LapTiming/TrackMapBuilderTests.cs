using RedMist.TimingCommon.LapTiming;

namespace RedMist.TimingCommon.Tests.LapTiming;

[TestClass]
public class TrackMapBuilderTests
{
    private const double Radius = 200;      // circumference ~1257 m
    private const int PointsPerLap = 72;    // ~17 m spacing, all kept past the 5 m decimation

    private static void FeedLap(TrackMapBuilder builder, int completedLaps, double startRad = 0,
        double radius = Radius, int points = PointsPerLap, bool onTrack = true)
    {
        foreach (var (lat, lon) in TrackTestData.Circle(radius, points, startRad))
            builder.AddSample(lat, lon, completedLaps, onTrack);
    }

    /// <summary>Crosses start/finish, which is what offers the buffered lap as a candidate.</summary>
    private static void Rollover(TrackMapBuilder builder, int completedLaps)
    {
        var (lat, lon) = TrackTestData.PointOnCircle(Radius, 0);
        builder.AddSample(lat, lon, completedLaps);
    }

    /// <summary>
    /// The join-in lap plus two agreeing laps: the minimum that produces a map.
    /// </summary>
    private static TrackMapBuilder BuiltFromTwoCleanLaps(int eventId = 7)
    {
        var builder = new TrackMapBuilder(eventId);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 4);   // partial join-in lap
        FeedLap(builder, completedLaps: 5);
        FeedLap(builder, completedLaps: 6);
        Rollover(builder, completedLaps: 7);
        return builder;
    }

    [TestMethod]
    public void Build_BeforeFullLap_ReturnsNull()
    {
        var builder = new TrackMapBuilder(eventId: 7);
        FeedLap(builder, completedLaps: 5);

        Assert.IsFalse(builder.IsComplete);
        Assert.IsNull(builder.Build(sessionId: 3, builtUtc: DateTime.UnixEpoch));
    }

    [TestMethod]
    public void Build_OneCleanLap_IsNotEnoughOnItsOwn()
    {
        // A single buffer says nothing about whether it holds one lap or two.
        var builder = new TrackMapBuilder(eventId: 7);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 4);
        FeedLap(builder, completedLaps: 5);
        Rollover(builder, completedLaps: 6);

        Assert.IsFalse(builder.IsComplete);
    }

    [TestMethod]
    public void Build_TwoAgreeingLaps_ProducesTheMap()
    {
        var builder = BuiltFromTwoCleanLaps();

        Assert.IsTrue(builder.IsComplete);
        var map = builder.Build(sessionId: 3, builtUtc: DateTime.UnixEpoch);
        Assert.IsNotNull(map);
        Assert.AreEqual(7, map.EventId);
        Assert.AreEqual(3, map.SessionId);

        var expectedCircumference = 2 * Math.PI * Radius;
        Assert.AreEqual(expectedCircumference, map.TotalLengthMeters, expectedCircumference * 0.02);
        Assert.IsTrue(map.Points.Count >= TrackMapBuilder.MinPoints);
    }

    [TestMethod]
    public void Build_LapThatSwallowedAStartFinishCrossing_IsNotCorroborated()
    {
        // The timing feed reported a lap late, so one buffer holds two laps of driving. It is
        // plausible on its own terms - enough points, a sane length - and only fails to match a
        // clean lap. This is the case that put a 10 km map on a 4 km circuit.
        var builder = new TrackMapBuilder(eventId: 1);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        FeedLap(builder, completedLaps: 1);
        FeedLap(builder, completedLaps: 1);   // no rollover between them: one buffer, two laps
        FeedLap(builder, completedLaps: 2);   // a clean lap, roughly half the length
        Rollover(builder, completedLaps: 3);  // offers the clean lap as a candidate

        Assert.IsFalse(builder.IsComplete, "A doubled lap must not corroborate a clean one");
    }

    [TestMethod]
    public void Build_ShorterLapSupersedesTheLongerOnesBeforeIt()
    {
        // A buffer can only ever be too long, so the shortest lap seen is the best estimate and
        // everything longer was covering more than one lap. Here a doubled buffer arrives first and
        // must be given up once a real lap turns up.
        var builder = new TrackMapBuilder(eventId: 1);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        FeedLap(builder, completedLaps: 1);
        FeedLap(builder, completedLaps: 1);         // one buffer holding two laps
        FeedLap(builder, completedLaps: 2);         // a clean lap at last
        FeedLap(builder, completedLaps: 3);         // and another, agreeing with it
        Rollover(builder, completedLaps: 4);

        Assert.IsTrue(builder.IsComplete);
        var map = builder.Build(1, DateTime.UnixEpoch)!;
        var expected = 2 * Math.PI * Radius;
        Assert.AreEqual(expected, map.TotalLengthMeters, expected * 0.05);
    }

    [TestMethod]
    public void Build_EveryBufferDoubled_CorroboratesAndCannotTell()
    {
        // Documents the limit of GPS-only corroboration, which is why TrackMapService checks the
        // length the timing system declares. A feed reporting lap counts consistently late gives
        // every buffer two laps; they agree with each other and trace the same line, and nothing
        // within the samples themselves distinguishes that from a circuit twice as long.
        var builder = new TrackMapBuilder(eventId: 1);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        for (int lap = 1; lap <= 3; lap++)
        {
            FeedLap(builder, completedLaps: lap);
            FeedLap(builder, completedLaps: lap);
        }
        Rollover(builder, completedLaps: 4);

        Assert.IsTrue(builder.IsComplete);
        var map = builder.Build(1, DateTime.UnixEpoch)!;
        Assert.AreEqual(2 * 2 * Math.PI * Radius, map.TotalLengthMeters, 2 * Math.PI * Radius * 0.05,
            "Twice the circuit, indistinguishable from within the GPS");
    }

    [TestMethod]
    public void Build_LapThatSkippedPartOfTheCircuit_DoesNotCorroborate()
    {
        // A dropout bridged by a straight line lands within a few percent of a clean lap's length
        // while describing a different line entirely, so length alone cannot tell them apart.
        var builder = new TrackMapBuilder(eventId: 1);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        FeedLap(builder, completedLaps: 1);

        // Lap 2 cuts straight across a quarter of the circle instead of following it, which leaves
        // it within a few percent of a clean lap's length while describing a different line.
        var arc = TrackTestData.Circle(Radius, PointsPerLap).ToList();
        for (int i = 0; i < arc.Count; i++)
        {
            if (i > PointsPerLap / 8 && i < 3 * PointsPerLap / 8)
                continue;
            builder.AddSample(arc[i].lat, arc[i].lon, 2);
        }
        Rollover(builder, completedLaps: 3);

        Assert.IsFalse(builder.IsComplete, "A lap that cut a corner is not the same line");
    }

    [TestMethod]
    public void Build_CleanLapAfterADoubledOne_EventuallyCorroborates()
    {
        var builder = new TrackMapBuilder(eventId: 1);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        FeedLap(builder, completedLaps: 1);
        FeedLap(builder, completedLaps: 1);   // doubled buffer, offered and held
        FeedLap(builder, completedLaps: 2);   // clean, disagrees with the doubled one
        FeedLap(builder, completedLaps: 3);   // clean, agrees with the previous
        Rollover(builder, completedLaps: 4);

        Assert.IsTrue(builder.IsComplete);
        var map = builder.Build(1, DateTime.UnixEpoch)!;
        var expected = 2 * Math.PI * Radius;
        Assert.AreEqual(expected, map.TotalLengthMeters, expected * 0.05,
            "Should settle on the length two clean laps agreed about");
    }

    [TestMethod]
    public void Build_LapWithAnOffTrackExcursion_IsDiscarded()
    {
        // A lap through the pits cuts geometry the racing line has, so it is not a lap of the track.
        var builder = new TrackMapBuilder(eventId: 1);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        FeedLap(builder, completedLaps: 1);
        // Second lap: same shape, but the car was off the racing surface partway round.
        foreach (var (lat, lon) in TrackTestData.Circle(Radius, PointsPerLap))
            builder.AddSample(lat, lon, 2, onTrack: false);
        Rollover(builder, completedLaps: 3);

        Assert.IsFalse(builder.IsComplete);
    }

    [TestMethod]
    public void Build_OffTrackSamplesAreNotPartOfTheLine()
    {
        var builder = new TrackMapBuilder(eventId: 1);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        // Two clean laps, each with one paddock fix a long way off the circuit dropped in.
        for (int lap = 1; lap <= 2; lap++)
        {
            var (strayLat, strayLon) = TrackTestData.PointAt(5000, 5000);
            builder.AddSample(strayLat, strayLon, lap, onTrack: false);
            FeedLap(builder, completedLaps: lap);
        }
        Rollover(builder, completedLaps: 3);

        // Both laps are abandoned because the car left the track during them.
        Assert.IsFalse(builder.IsComplete);
    }

    [TestMethod]
    public void Build_DecimatesDenseSamples()
    {
        var builder = new TrackMapBuilder(eventId: 1, minSpacingMeters: 5.0);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        foreach (var lap in new[] { 1, 2 })
        {
            foreach (var (lat, lon) in TrackTestData.Circle(Radius, 1000))
                builder.AddSample(lat, lon, lap);
        }
        Rollover(builder, completedLaps: 3);

        var map = builder.Build(1, DateTime.UnixEpoch);
        Assert.IsNotNull(map);
        // ~1257 m / 5 m spacing => on the order of 250 points, far fewer than 1000.
        Assert.IsTrue(map.Points.Count < 400, $"Expected decimation; got {map.Points.Count} points");
        for (int i = 1; i < map.Points.Count; i++)
            Assert.IsTrue(map.Points[i].CumulativeDistanceMeters > map.Points[i - 1].CumulativeDistanceMeters);
    }

    [TestMethod]
    public void Build_TooFewPoints_RejectsLap()
    {
        var builder = new TrackMapBuilder(eventId: 1);

        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);
        foreach (var lap in new[] { 1, 2 })
        {
            foreach (var (lat, lon) in TrackTestData.Circle(Radius, 5))
                builder.AddSample(lat, lon, lap);
        }
        Rollover(builder, completedLaps: 3);

        Assert.IsFalse(builder.IsComplete);
    }

    [TestMethod]
    public void Build_ImplausiblyLongBuffer_RejectsLap()
    {
        // A runaway buffer - the lap count never advanced - is past any real circuit.
        var builder = new TrackMapBuilder(eventId: 1, minSpacingMeters: 1.0);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        foreach (var lap in new[] { 1, 2 })
        {
            for (int i = 0; i < 40; i++)
                FeedLap(builder, completedLaps: lap, radius: 20_000, points: 60);
        }
        Rollover(builder, completedLaps: 3);

        Assert.IsFalse(builder.IsComplete);
    }

    [TestMethod]
    public void Build_KeepsTheDenserOfTwoAgreeingLaps()
    {
        var builder = new TrackMapBuilder(eventId: 1, minSpacingMeters: 1.0);
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        FeedLap(builder, completedLaps: 1, points: 40);
        FeedLap(builder, completedLaps: 2, points: 200);
        Rollover(builder, completedLaps: 3);

        Assert.IsTrue(builder.IsComplete);
        var map = builder.Build(1, DateTime.UnixEpoch)!;
        Assert.IsTrue(map.Points.Count > 100, $"Should keep the finer trace; got {map.Points.Count} points");
    }
}
