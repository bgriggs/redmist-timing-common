using RedMist.TimingCommon.LapTiming;

namespace RedMist.TimingCommon.Tests.LapTiming;

[TestClass]
public class TrackMapBuilderTests
{
    private const double Radius = 200;      // circumference ~1257 m
    private const int PointsPerLap = 72;    // ~17 m spacing, all kept past the 5 m decimation

    private static void FeedLap(TrackMapBuilder builder, int completedLaps, double startRad = 0)
    {
        foreach (var (lat, lon) in TrackTestData.Circle(Radius, PointsPerLap, startRad))
            builder.AddSample(lat, lon, completedLaps);
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
    public void Build_DiscardsPartialJoinLap_UsesFirstFullLap()
    {
        var builder = new TrackMapBuilder(eventId: 7);

        // Joined mid-lap: only half of lap 5 is seen.
        foreach (var (lat, lon) in TrackTestData.Circle(Radius, PointsPerLap / 2))
            builder.AddSample(lat, lon, 5);

        // Rollover into lap 6 (discards the partial buffer), then a full lap 6.
        FeedLap(builder, completedLaps: 6);

        // Rollover into lap 7 triggers the build from the full lap 6 buffer.
        var (lat0, lon0) = TrackTestData.PointOnCircle(Radius, 0);
        builder.AddSample(lat0, lon0, 7);

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
    public void Build_DecimatesDenseSamples()
    {
        var builder = new TrackMapBuilder(eventId: 1, minSpacingMeters: 5.0);

        // Discard a first partial lap to arm the builder.
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);

        // Feed a dense full lap (many more samples than needed); decimation should thin it out.
        foreach (var (lat, lon) in TrackTestData.Circle(Radius, 1000))
            builder.AddSample(lat, lon, 1);

        var (lat0, lon0) = TrackTestData.PointOnCircle(Radius, 0);
        builder.AddSample(lat0, lon0, 2);

        var map = builder.Build(1, DateTime.UnixEpoch);
        Assert.IsNotNull(map);
        // ~1257 m / 5 m spacing => on the order of 250 points, far fewer than 1000.
        Assert.IsTrue(map.Points.Count < 400, $"Expected decimation; got {map.Points.Count} points");
        // Cumulative distances must be strictly increasing.
        for (int i = 1; i < map.Points.Count; i++)
            Assert.IsTrue(map.Points[i].CumulativeDistanceMeters > map.Points[i - 1].CumulativeDistanceMeters);
    }

    [TestMethod]
    public void Build_TooFewPoints_RejectsLap()
    {
        var builder = new TrackMapBuilder(eventId: 1);

        // Arm with a partial lap, then a "full" lap with too few points to be credible.
        builder.AddSample(TrackTestData.Lat0, TrackTestData.Lon0, 0);
        foreach (var (lat, lon) in TrackTestData.Circle(Radius, 5))
            builder.AddSample(lat, lon, 1);
        var (lat0, lon0) = TrackTestData.PointOnCircle(Radius, 0);
        builder.AddSample(lat0, lon0, 2);

        Assert.IsFalse(builder.IsComplete);
    }
}