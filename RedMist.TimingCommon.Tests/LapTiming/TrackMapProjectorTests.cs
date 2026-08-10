using RedMist.TimingCommon.LapTiming;
using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Tests.LapTiming;

[TestClass]
public class TrackMapProjectorTests
{
    private const double Radius = 300.0;
    private const int PointCount = 72;

    private static TrackMap Circle() => TrackTestData.CircleMap(Radius, PointCount);

    [TestMethod]
    public void Project_NullMap_ReturnsNull()
    {
        Assert.IsNull(TrackMapProjector.Project(null));
    }

    [TestMethod]
    public void Project_TooFewPoints_ReturnsNull()
    {
        var map = new TrackMap { TotalLengthMeters = 1000 };
        map.Points.Add(new TrackMapPoint { Latitude = 40, Longitude = -86 });

        Assert.IsNull(TrackMapProjector.Project(map));
    }

    [TestMethod]
    public void Project_ZeroLength_ReturnsNull()
    {
        var map = Circle();
        map.TotalLengthMeters = 0;

        Assert.IsNull(TrackMapProjector.Project(map));
    }

    [TestMethod]
    public void Project_NonFiniteCoordinate_ReturnsNull()
    {
        var map = Circle();
        map.Points[10].Latitude = double.NaN;

        Assert.IsNull(TrackMapProjector.Project(map),
            "A corrupt coordinate collapses the bounds, so the map should be rejected rather than returned unusable");
    }

    [TestMethod]
    public void Project_KeepsEveryPointInPathOrder()
    {
        var map = Circle();

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.AreEqual(map.Points.Count, render.Points.Count);
        for (int i = 0; i < map.Points.Count; i++)
        {
            Assert.AreEqual(map.Points[i].Latitude, render.Points[i].Latitude, 1e-12);
            Assert.AreEqual(map.Points[i].Longitude, render.Points[i].Longitude, 1e-12);
            Assert.AreEqual(map.Points[i].CumulativeDistanceMeters, render.Points[i].CumulativeDistanceMeters, 1e-9);
        }
    }

    [TestMethod]
    public void Project_CircleBounds_AreOneDiameterSquare()
    {
        var render = TrackMapProjector.Project(Circle());

        Assert.IsNotNull(render);
        Assert.AreEqual(2 * Radius, render.Bounds.WidthMeters, 1.0);
        Assert.AreEqual(2 * Radius, render.Bounds.HeightMeters, 1.0);
    }

    [TestMethod]
    public void Project_OriginIsNorthWestCorner_AndYGrowsDownward()
    {
        var render = TrackMapProjector.Project(Circle());

        Assert.IsNotNull(render);
        Assert.AreEqual(0.0, render.Points.Min(p => p.X), 1e-6, "The western edge should sit at x = 0");
        Assert.AreEqual(0.0, render.Points.Min(p => p.Y), 1e-6, "The northern edge should sit at y = 0");

        var northernmost = render.Points.OrderByDescending(p => p.Latitude).First();
        var southernmost = render.Points.OrderBy(p => p.Latitude).First();
        Assert.IsTrue(northernmost.Y < southernmost.Y,
            "Y must increase southward so the frame is already in screen orientation");

        var westernmost = render.Points.OrderBy(p => p.Longitude).First();
        var easternmost = render.Points.OrderByDescending(p => p.Longitude).First();
        Assert.IsTrue(westernmost.X < easternmost.X, "X must increase eastward");
    }

    [TestMethod]
    public void Project_PlanarDistances_MatchGeographicDistances()
    {
        var map = Circle();
        var render = TrackMapProjector.Project(map);
        Assert.IsNotNull(render);

        // The whole point of a uniform projection: measuring in the planar frame gives meters, in
        // both axes and anywhere on the map. Sample pairs spread around the lap.
        for (int i = 0; i < PointCount; i += 7)
        {
            var j = (i + 19) % PointCount;
            var a = render.Points[i];
            var b = render.Points[j];

            var planar = Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
            var geographic = TrackGeometry.DistanceMeters(a.Latitude, a.Longitude, b.Latitude, b.Longitude);

            Assert.AreEqual(geographic, planar, 0.5, $"Points {i} and {j} should be the same distance apart in both frames");
        }
    }

    [TestMethod]
    public void Project_CarriesMapMetadata()
    {
        var map = Circle();
        map.EventId = 42;
        map.SessionId = 7;
        map.Version = 3;
        map.BuiltUtc = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

        var render = TrackMapProjector.Project(map, "Test Raceway");

        Assert.IsNotNull(render);
        Assert.AreEqual(42, render.EventId);
        Assert.AreEqual(7, render.SessionId);
        Assert.AreEqual(3, render.Version);
        Assert.AreEqual(map.BuiltUtc, render.BuiltUtc);
        Assert.AreEqual(map.TotalLengthMeters, render.LengthMeters, 1e-9);
        Assert.AreEqual("Test Raceway", render.TrackName);
    }

    [TestMethod]
    public void Project_UncalibratedMap_HasNoStartFinish()
    {
        var map = Circle();
        map.StartFinishOffsetMeters = null;

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.IsNull(render.StartFinish, "An uncalibrated map is drawable but its line is unknown");
        Assert.AreNotEqual(0, render.Points.Count);
    }

    [TestMethod]
    public void Project_CalibratedMap_PlacesStartFinishOnThePath()
    {
        var map = Circle();
        map.StartFinishOffsetMeters = map.TotalLengthMeters / 4;

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.IsNotNull(render.StartFinish);
        Assert.AreEqual(map.TotalLengthMeters / 4, render.StartFinish.DistanceAlongMeters, 1e-9);

        // A quarter of the way around a circle laid out counterclockwise from due east is due north
        // of the center.
        var (expectedLat, expectedLon) = TrackTestData.PointOnCircle(Radius, Math.PI / 2);
        Assert.AreEqual(expectedLat, render.StartFinish.Latitude, 1e-6);
        Assert.AreEqual(expectedLon, render.StartFinish.Longitude, 1e-6);

        // ...where a counterclockwise car is travelling west.
        Assert.AreEqual(270.0, render.StartFinish.HeadingDegrees, 5.0);
    }

    [TestMethod]
    public void Project_StartFinishXY_AgreesWithItsLatLon()
    {
        var map = Circle();
        map.StartFinishOffsetMeters = map.TotalLengthMeters * 0.6;

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.IsNotNull(render.StartFinish);

        // The line is on the path, so the nearest rendered point must be nearly on top of it in the
        // planar frame too - the same projection has to have been applied to both.
        var sf = render.StartFinish;
        var nearest = render.Points
            .OrderBy(p => TrackGeometry.DistanceMeters(p.Latitude, p.Longitude, sf.Latitude, sf.Longitude))
            .First();

        var geographic = TrackGeometry.DistanceMeters(nearest.Latitude, nearest.Longitude, sf.Latitude, sf.Longitude);
        var planar = Math.Sqrt((nearest.X - sf.X) * (nearest.X - sf.X) + (nearest.Y - sf.Y) * (nearest.Y - sf.Y));
        Assert.AreEqual(geographic, planar, 0.5);

        // The check above compares a separation, which is blind to the frame's origin: an X measured
        // from the map's center rather than its western edge would still pass. The line is on the
        // track, so it has to land inside the extent the points define.
        Assert.IsTrue(sf.X >= 0 && sf.X <= render.Bounds.WidthMeters, $"Start/finish X {sf.X} is outside the map");
        Assert.IsTrue(sf.Y >= 0 && sf.Y <= render.Bounds.HeightMeters, $"Start/finish Y {sf.Y} is outside the map");
    }

    [TestMethod]
    public void Project_BoundsMatchTheExtentOfTheProjectedPoints()
    {
        var render = TrackMapProjector.Project(Circle());

        Assert.IsNotNull(render);
        Assert.AreEqual(render.Points.Max(p => p.X), render.Bounds.WidthMeters, 1e-9,
            "Consumers divide a viewport by this, so it has to be the largest X actually present");
        Assert.AreEqual(render.Points.Max(p => p.Y), render.Bounds.HeightMeters, 1e-9);
        Assert.AreEqual(render.Points.Min(p => p.Latitude), render.Bounds.MinLatitude, 1e-12);
        Assert.AreEqual(render.Points.Max(p => p.Latitude), render.Bounds.MaxLatitude, 1e-12);
        Assert.AreEqual(render.Points.Min(p => p.Longitude), render.Bounds.MinLongitude, 1e-12);
        Assert.AreEqual(render.Points.Max(p => p.Longitude), render.Bounds.MaxLongitude, 1e-12);
    }

    [TestMethod]
    public void Project_AsymmetricTrack_KeepsItsProportions()
    {
        // A circle is symmetric about both axes, so it cannot catch a swapped or mis-scaled axis.
        // A rectangle twice as wide as it is tall can.
        const double east = 400.0, north = 200.0;
        var (points, totalLength) = TrackTestData.ClosedPath(
        [
            (0, 0), (east / 2, 0), (east, 0),
            (east, north / 2), (east, north),
            (east / 2, north), (0, north),
            (0, north / 2),
        ]);
        var map = new TrackMap { EventId = 1, Points = points, TotalLengthMeters = totalLength };

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.AreEqual(east, render.Bounds.WidthMeters, 1.0);
        Assert.AreEqual(north, render.Bounds.HeightMeters, 1.0);

        // The south-west corner of the layout is the bottom-left of the drawing.
        var southWest = render.Points.First();
        Assert.AreEqual(0.0, southWest.X, 1.0);
        Assert.AreEqual(north, southWest.Y, 1.0);
    }

    [TestMethod]
    public void Project_MapWithNoExtent_ReturnsNull()
    {
        // Every fix at the same place. There is no outline, and the documented scale formula would
        // divide by zero and take the whole map to NaN.
        var map = new TrackMap { EventId = 1, TotalLengthMeters = 500 };
        for (int i = 0; i < 10; i++)
            map.Points.Add(new TrackMapPoint { Latitude = 40.0, Longitude = -86.0, CumulativeDistanceMeters = i });

        Assert.IsNull(TrackMapProjector.Project(map));
    }

    [TestMethod]
    public void Project_NonFiniteCumulativeDistance_ReturnsNull()
    {
        var map = Circle();
        map.Points[10].CumulativeDistanceMeters = double.NaN;

        Assert.IsNull(TrackMapProjector.Project(map),
            "Consumers place cars with this field, so a corrupt one makes the map unusable too");
    }

    [TestMethod]
    public void Project_NonFiniteLength_ReturnsNull()
    {
        var nan = Circle();
        nan.TotalLengthMeters = double.NaN;
        Assert.IsNull(TrackMapProjector.Project(nan));

        var infinite = Circle();
        infinite.TotalLengthMeters = double.PositiveInfinity;
        Assert.IsNull(TrackMapProjector.Project(infinite));
    }

    [TestMethod]
    public void Project_NullPointsList_ReturnsNull()
    {
        // A nil member on a deserialized map, rather than the empty list the property initializes to.
        var map = new TrackMap { EventId = 1, TotalLengthMeters = 500, Points = null! };

        Assert.IsNull(TrackMapProjector.Project(map));
    }

    [TestMethod]
    public void Project_StartFinishOffsetOutsideTheLap_IsReportedWrapped()
    {
        var map = Circle();
        map.StartFinishOffsetMeters = -50.0;

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.IsNotNull(render.StartFinish);
        Assert.AreEqual(map.TotalLengthMeters - 50.0, render.StartFinish.DistanceAlongMeters, 1e-6,
            "The reported distance must match the position it was resolved from, and stay inside the lap");
        Assert.IsTrue(render.StartFinish.DistanceAlongMeters >= 0
            && render.StartFinish.DistanceAlongMeters < render.LengthMeters);
    }

    [TestMethod]
    public void Project_NaNStartFinishOffset_LeavesTheMapUncalibrated()
    {
        var map = Circle();
        map.StartFinishOffsetMeters = double.NaN;

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.IsNull(render.StartFinish);
        Assert.AreNotEqual(0, render.Points.Count, "The outline is still drawable");
    }

    [TestMethod]
    public void Project_Result_SurvivesMessagePackAndJsonRoundTrips()
    {
        // TrackMapRender is served over both application/x-msgpack and application/json, and
        // MessagePack decodes positionally: a renumbered or duplicated Key silently shifts every
        // field after it. This pins the contract rather than the values.
        var map = Circle();
        map.StartFinishOffsetMeters = map.TotalLengthMeters * 0.3;
        var render = TrackMapProjector.Project(map, "Test Raceway");
        Assert.IsNotNull(render);

        var viaMsgPack = MessagePack.MessagePackSerializer.Deserialize<TrackMapRender>(
            MessagePack.MessagePackSerializer.Serialize(render));
        var viaJson = System.Text.Json.JsonSerializer.Deserialize<TrackMapRender>(
            System.Text.Json.JsonSerializer.Serialize(render));

        foreach (var round in new[] { viaMsgPack, viaJson })
        {
            Assert.IsNotNull(round);
            Assert.AreEqual(render.EventId, round.EventId);
            Assert.AreEqual(render.SessionId, round.SessionId);
            Assert.AreEqual(render.TrackName, round.TrackName);
            Assert.AreEqual(render.Version, round.Version);
            Assert.AreEqual(render.LengthMeters, round.LengthMeters, 1e-9);
            Assert.AreEqual(render.Points.Count, round.Points.Count);
            Assert.AreEqual(render.Points[7].Latitude, round.Points[7].Latitude, 1e-12);
            Assert.AreEqual(render.Points[7].X, round.Points[7].X, 1e-9);
            Assert.AreEqual(render.Points[7].Y, round.Points[7].Y, 1e-9);
            Assert.AreEqual(render.Points[7].CumulativeDistanceMeters, round.Points[7].CumulativeDistanceMeters, 1e-9);
            Assert.AreEqual(render.Bounds.WidthMeters, round.Bounds.WidthMeters, 1e-9);
            Assert.AreEqual(render.Bounds.HeightMeters, round.Bounds.HeightMeters, 1e-9);
            Assert.AreEqual(render.Bounds.MinLatitude, round.Bounds.MinLatitude, 1e-12);
            Assert.IsNotNull(round.StartFinish);
            Assert.AreEqual(render.StartFinish!.DistanceAlongMeters, round.StartFinish.DistanceAlongMeters, 1e-9);
            Assert.AreEqual(render.StartFinish.HeadingDegrees, round.StartFinish.HeadingDegrees, 1e-9);
            Assert.AreEqual(render.StartFinish.X, round.StartFinish.X, 1e-9);
        }
    }

    [TestMethod]
    public void Project_UncalibratedResult_SurvivesAMessagePackRoundTrip()
    {
        var render = TrackMapProjector.Project(Circle());
        Assert.IsNotNull(render);
        Assert.IsNull(render.StartFinish);

        var round = MessagePack.MessagePackSerializer.Deserialize<TrackMapRender>(
            MessagePack.MessagePackSerializer.Serialize(render));

        Assert.IsNotNull(round);
        Assert.IsNull(round.StartFinish, "A null nested member must survive as null, not as an empty object");
        Assert.IsNull(round.TrackName);
        Assert.AreEqual(render.Points.Count, round.Points.Count);
    }

    [TestMethod]
    public void Project_StartFinishAtOrigin_IsReportedRatherThanTreatedAsUncalibrated()
    {
        var map = Circle();
        map.StartFinishOffsetMeters = 0;

        var render = TrackMapProjector.Project(map);

        Assert.IsNotNull(render);
        Assert.IsNotNull(render.StartFinish, "Zero is a legitimate calibrated offset, not a missing one");
        Assert.AreEqual(map.Points[0].Latitude, render.StartFinish.Latitude, 1e-9);
        Assert.AreEqual(map.Points[0].Longitude, render.StartFinish.Longitude, 1e-9);
    }
}
