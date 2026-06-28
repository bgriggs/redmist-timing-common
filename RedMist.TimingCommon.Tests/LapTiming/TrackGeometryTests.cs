using RedMist.TimingCommon.LapTiming;

namespace RedMist.TimingCommon.Tests.LapTiming;

[TestClass]
public class TrackGeometryTests
{
    [TestMethod]
    public void DistanceMeters_OneDegreeOfLatitude_IsAbout111Km()
    {
        var d = TrackGeometry.DistanceMeters(40.0, -86.0, 41.0, -86.0);
        // One degree of latitude is ~111.195 km (R * pi/180).
        Assert.AreEqual(111_195, d, 50);
    }

    [TestMethod]
    public void DistanceMeters_KnownNorthOffset_MatchesMeters()
    {
        var (lat, lon) = TrackTestData.PointOnCircle(100.0, Math.PI / 2); // 100 m due north
        var d = TrackGeometry.DistanceMeters(TrackTestData.Lat0, TrackTestData.Lon0, lat, lon);
        Assert.AreEqual(100.0, d, 0.5);
    }

    [TestMethod]
    public void DistanceMeters_SamePoint_IsZero()
    {
        Assert.AreEqual(0.0, TrackGeometry.DistanceMeters(40, -86, 40, -86), 1e-9);
    }

    [TestMethod]
    public void Snap_PointHalfwayAroundCircle_ReturnsHalfFraction()
    {
        var map = TrackTestData.CircleMap(radiusMeters: 200, count: 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, Math.PI); // opposite start (angle 0)

        var snap = TrackGeometry.Snap(map.Points, map.TotalLengthMeters, lat, lon);

        Assert.IsNotNull(snap);
        Assert.AreEqual(0.5, snap.Value.Fraction, 0.01);
        Assert.AreEqual(0.0, snap.Value.LateralOffsetMeters, 1.0, "Point on the circle should snap onto the path");
    }

    [TestMethod]
    public void Snap_PointAtStart_ReturnsNearZeroFraction()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, 0);

        var snap = TrackGeometry.Snap(map.Points, map.TotalLengthMeters, lat, lon);

        Assert.IsNotNull(snap);
        Assert.IsTrue(snap.Value.Fraction < 0.02 || snap.Value.Fraction > 0.98,
            $"Start point should be near 0 (or wrap near 1); was {snap.Value.Fraction}");
    }

    [TestMethod]
    public void Snap_PointOffsetFromPath_ReportsLateralOffset()
    {
        var map = TrackTestData.CircleMap(200, 72);
        // A point at the same bearing but 10 m further out (radius 210 at quarter lap).
        var (lat, lon) = TrackTestData.PointOnCircle(210, Math.PI / 2);

        var snap = TrackGeometry.Snap(map.Points, map.TotalLengthMeters, lat, lon);

        Assert.IsNotNull(snap);
        Assert.AreEqual(10.0, snap.Value.LateralOffsetMeters, 1.5);
        Assert.AreEqual(0.25, snap.Value.Fraction, 0.01);
    }

    [TestMethod]
    public void Snap_TooFewPoints_ReturnsNull()
    {
        var map = TrackTestData.CircleMap(200, 72);
        Assert.IsNull(TrackGeometry.Snap(map.Points.Take(1).ToList(), map.TotalLengthMeters, 40, -86));
        Assert.IsNull(TrackGeometry.Snap(map.Points, 0, 40, -86));
    }
}