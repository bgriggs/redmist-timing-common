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

    [TestMethod]
    public void SnapNear_PositionInsideWindow_SnapsSameAsGlobal()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, Math.PI);
        var expected = map.TotalLengthMeters * 0.48; // last known position, just behind the car

        var snap = TrackGeometry.SnapNear(map.Points, map.TotalLengthMeters, lat, lon, expected, windowMeters: 100);

        Assert.IsNotNull(snap);
        Assert.AreEqual(0.5, snap.Value.Fraction, 0.01);
    }

    [TestMethod]
    public void SnapNear_PositionFarFromWindow_StaysInWindowWithLargeLateralOffset()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, Math.PI); // half a lap away from expected

        var snap = TrackGeometry.SnapNear(map.Points, map.TotalLengthMeters, lat, lon,
            expectedDistanceAlongMeters: 0, windowMeters: 50);

        // The window is honoured, and the lateral offset - not a null result - is what tells the
        // caller the position is inconsistent with where the car was last seen.
        Assert.IsNotNull(snap);
        Assert.IsTrue(TrackGeometry.CircularDistanceMeters(snap.Value.DistanceAlongMeters, 0, map.TotalLengthMeters) <= 50);
        Assert.AreEqual(400, snap.Value.LateralOffsetMeters, 10, "Should report the full diameter of the circle");
    }

    [TestMethod]
    public void SnapNear_WindowWrapsPastOrigin_StillMatches()
    {
        var map = TrackTestData.CircleMap(200, 72);
        // Car is just past start/finish; its last known position was just before it.
        var (lat, lon) = TrackTestData.PointOnCircle(200, 2 * Math.PI * 0.01);
        var expected = map.TotalLengthMeters * 0.99;

        var snap = TrackGeometry.SnapNear(map.Points, map.TotalLengthMeters, lat, lon, expected,
            windowMeters: map.TotalLengthMeters * 0.05);

        Assert.IsNotNull(snap);
        Assert.AreEqual(0.01, snap.Value.Fraction, 0.01);
    }

    [TestMethod]
    public void SnapNear_PrefersNearerLegOfACrossover()
    {
        // Two parallel straights 20 m apart, as at a crossover: out along y = 0, back along y = 20.
        var local = new List<(double east, double north)>();
        for (int i = 0; i <= 39; i++)
            local.Add((i * 10.0, 0));
        for (int i = 39; i >= 0; i--)
            local.Add((i * 10.0, 20.0));
        var (points, totalLength) = TrackTestData.ClosedPath(local);

        // 12 m north of the outbound straight at x = 200, so only 8 m from the return straight:
        // geometrically nearest is the return leg, but the car was last seen on the outbound one.
        var (qLat, qLon) = TrackTestData.PointAt(200, 12);

        var global = TrackGeometry.Snap(points, totalLength, qLat, qLon);
        var constrained = TrackGeometry.SnapNear(points, totalLength, qLat, qLon,
            expectedDistanceAlongMeters: 200, windowMeters: 40);

        Assert.IsNotNull(global);
        Assert.IsNotNull(constrained);
        Assert.IsTrue(global.Value.DistanceAlongMeters > 400,
            $"Unconstrained snap should pick the geometrically nearer return leg; was {global.Value.DistanceAlongMeters:F0} m");
        Assert.AreEqual(200, constrained.Value.DistanceAlongMeters, 40,
            "Constrained snap should stay on the leg the car was last known to be on");
    }

    [TestMethod]
    public void SnapNear_NonPositiveWindow_ReturnsNull()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, Math.PI);
        Assert.IsNull(TrackGeometry.SnapNear(map.Points, map.TotalLengthMeters, lat, lon, 0, 0));
    }

    [TestMethod]
    public void CircularDistance_WrapsAtOrigin()
    {
        Assert.AreEqual(20, TrackGeometry.CircularDistanceMeters(990, 10, 1000), 1e-9);
        Assert.AreEqual(20, TrackGeometry.CircularDistanceMeters(10, 990, 1000), 1e-9);
        Assert.AreEqual(500, TrackGeometry.CircularDistanceMeters(0, 500, 1000), 1e-9);
        Assert.AreEqual(0, TrackGeometry.CircularDistanceMeters(250, 250, 1000), 1e-9);
    }

    [TestMethod]
    public void FractionFromStartFinish_ShiftsAndWrapsByOffset()
    {
        // With the line 100 m along a 1000 m path, the origin itself is 90% of the way round.
        Assert.AreEqual(0.9, TrackGeometry.FractionFromStartFinish(0, 1000, 100), 1e-9);
        Assert.AreEqual(0.0, TrackGeometry.FractionFromStartFinish(100, 1000, 100), 1e-9);
        Assert.AreEqual(0.5, TrackGeometry.FractionFromStartFinish(600, 1000, 100), 1e-9);
    }

    [TestMethod]
    public void FractionFromStartFinish_UncalibratedMap_MatchesRawFraction()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var half = map.TotalLengthMeters * 0.5;
        Assert.IsNull(map.StartFinishOffsetMeters);
        Assert.AreEqual(0.5, TrackGeometry.FractionFromStartFinish(map, half), 1e-9);
    }

    [TestMethod]
    public void FractionFromStartFinish_JustBehindTheLine_WrapsToZeroNotOne()
    {
        // The rounding here lands exactly on the total length, which would report a car sitting on
        // the line as a full lap in rather than none.
        var fraction = TrackGeometry.FractionFromStartFinish(128.0, 4000.0, 128.00000000000003);

        Assert.IsTrue(fraction >= 0 && fraction < 1.0, $"Fraction must stay in [0, 1); was {fraction}");
        Assert.AreEqual(0.0, fraction, 1e-9);
    }

    [TestMethod]
    public void FractionFromStartFinish_OutOfRangeInputs_Normalise()
    {
        // Offset past the end of the path, and a distance past the end of the path.
        Assert.AreEqual(0.5, TrackGeometry.FractionFromStartFinish(600, 1000, 5100), 1e-9);
        Assert.AreEqual(0.5, TrackGeometry.FractionFromStartFinish(5600, 1000, 100), 1e-9);
        // Negative offset behaves as a shift the other way.
        Assert.AreEqual(0.7, TrackGeometry.FractionFromStartFinish(600, 1000, -100), 1e-9);
        // Unusable path length.
        Assert.AreEqual(0.0, TrackGeometry.FractionFromStartFinish(600, 0, 100), 1e-9);
    }

    [TestMethod]
    public void CircularDistance_OutOfRangeAndDegenerateInputs()
    {
        // Callers pass raw observations and expected positions that can sit outside [0, length).
        Assert.AreEqual(90, TrackGeometry.CircularDistanceMeters(4100, 10, 4000), 1e-9);
        Assert.AreEqual(20, TrackGeometry.CircularDistanceMeters(-10, 10, 4000), 1e-9);
        Assert.AreEqual(0, TrackGeometry.CircularDistanceMeters(10, 20, 0), 1e-9);
    }

    [TestMethod]
    public void Snap_CorruptPointInMap_DoesNotDisplaceTheMatch()
    {
        // One bad coordinate in a persisted map must not relocate every car on it.
        var map = TrackTestData.CircleMap(200, 72);
        var truth = TrackGeometry.Snap(map.Points, map.TotalLengthMeters,
            map.Points[18].Latitude, map.Points[18].Longitude);
        map.Points[40].Longitude = double.NaN;

        var snap = TrackGeometry.Snap(map.Points, map.TotalLengthMeters,
            map.Points[18].Latitude, map.Points[18].Longitude);

        Assert.IsNotNull(truth);
        Assert.IsNotNull(snap);
        Assert.AreEqual(truth.Value.DistanceAlongMeters, snap.Value.DistanceAlongMeters, 1e-6);
    }

    [TestMethod]
    public void Snap_NonFinitePosition_ReturnsNull()
    {
        var map = TrackTestData.CircleMap(200, 72);
        Assert.IsNull(TrackGeometry.Snap(map.Points, map.TotalLengthMeters, double.NaN, -86));
        Assert.IsNull(TrackGeometry.Snap(map.Points, map.TotalLengthMeters, 40, double.PositiveInfinity));
    }
}