using RedMist.TimingCommon.LapTiming;
using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Tests.LapTiming;

[TestClass]
public class TrackGeometryPointAtDistanceTests
{
    private const double Radius = 250.0;
    private const int PointCount = 72;

    private static TrackMap Circle() => TrackTestData.CircleMap(Radius, PointCount);

    [TestMethod]
    public void PointAtDistance_Zero_IsThePathOrigin()
    {
        var map = Circle();

        var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, 0);

        Assert.IsNotNull(result);
        Assert.AreEqual(map.Points[0].Latitude, result.Value.Latitude, 1e-9);
        Assert.AreEqual(map.Points[0].Longitude, result.Value.Longitude, 1e-9);
    }

    [TestMethod]
    public void PointAtDistance_RoundTripsEveryMapPoint()
    {
        var map = Circle();

        foreach (var point in map.Points)
        {
            var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, point.CumulativeDistanceMeters);

            Assert.IsNotNull(result);
            var error = TrackGeometry.DistanceMeters(point.Latitude, point.Longitude, result.Value.Latitude, result.Value.Longitude);
            Assert.AreEqual(0.0, error, 0.1, $"Point at {point.CumulativeDistanceMeters:F1} m should come back where it started");
        }
    }

    [TestMethod]
    public void PointAtDistance_MidSegment_InterpolatesBetweenTheEnds()
    {
        var map = Circle();
        var a = map.Points[10];
        var b = map.Points[11];
        var midpoint = (a.CumulativeDistanceMeters + b.CumulativeDistanceMeters) / 2;

        var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, midpoint);

        Assert.IsNotNull(result);
        var segmentLength = TrackGeometry.DistanceMeters(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        var toA = TrackGeometry.DistanceMeters(a.Latitude, a.Longitude, result.Value.Latitude, result.Value.Longitude);
        var toB = TrackGeometry.DistanceMeters(b.Latitude, b.Longitude, result.Value.Latitude, result.Value.Longitude);

        // Equidistant alone would admit any point on the segment's perpendicular bisector, including
        // ones nowhere near the track, so pin the actual distance too.
        Assert.AreEqual(segmentLength / 2, toA, 0.1);
        Assert.AreEqual(segmentLength / 2, toB, 0.1);
    }

    [TestMethod]
    public void PointAtDistance_ReportsTheForwardDirectionOfTravel()
    {
        var map = Circle();

        // The circle is laid out counterclockwise from due east, so at its northernmost point a car
        // is heading west.
        var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, map.TotalLengthMeters / 4);

        Assert.IsNotNull(result);
        Assert.AreEqual(270.0, result.Value.HeadingDegrees, 5.0,
            "A reversed heading would read 90 here, and a swapped atan2 would read 180");
    }

    [TestMethod]
    public void PointAtDistance_ExactlyALap_IsTheOrigin()
    {
        var map = Circle();

        var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, map.TotalLengthMeters);

        Assert.IsNotNull(result);
        Assert.AreEqual(map.Points[0].Latitude, result.Value.Latitude, 1e-9);
        Assert.AreEqual(map.Points[0].Longitude, result.Value.Longitude, 1e-9);
    }

    [TestMethod]
    public void PointAtDistance_AHairBehindTheOrigin_DoesNotFallThrough()
    {
        var map = Circle();

        // Wrapping a tiny negative leaves a value that rounds back up to exactly the lap length -
        // the one distance the normalized range excludes, and the case that would fall off the end
        // of the segment walk if it were not clamped.
        var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, -1e-18);

        Assert.IsNotNull(result);
        Assert.AreEqual(map.Points[0].Latitude, result.Value.Latitude, 1e-9);
        Assert.AreEqual(map.Points[0].Longitude, result.Value.Longitude, 1e-9);
    }

    [TestMethod]
    public void PointAtDistance_DuplicatedCumulativeDistances_StaysOnThePath()
    {
        var map = Circle();
        // A stalled car repeats a fix, so two points can share a distance. The walk must not divide
        // by the zero-length segment between them.
        map.Points[20].CumulativeDistanceMeters = map.Points[19].CumulativeDistanceMeters;

        var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters,
            map.Points[19].CumulativeDistanceMeters);

        Assert.IsNotNull(result);
        Assert.IsTrue(double.IsFinite(result.Value.Latitude) && double.IsFinite(result.Value.Longitude));
    }

    [TestMethod]
    public void NormalizeDistance_BringsAnyDistanceIntoTheLap()
    {
        const double lap = 1000.0;

        Assert.AreEqual(0.0, TrackGeometry.NormalizeDistance(0, lap), 1e-9);
        Assert.AreEqual(250.0, TrackGeometry.NormalizeDistance(250, lap), 1e-9);
        Assert.AreEqual(0.0, TrackGeometry.NormalizeDistance(lap, lap), 1e-9);
        Assert.AreEqual(250.0, TrackGeometry.NormalizeDistance(lap + 250, lap), 1e-9);
        Assert.AreEqual(950.0, TrackGeometry.NormalizeDistance(-50, lap), 1e-9);
        Assert.AreEqual(0.0, TrackGeometry.NormalizeDistance(-1e-18, lap), 1e-9);
        Assert.AreEqual(0.0, TrackGeometry.NormalizeDistance(double.NaN, lap), 1e-9);
        Assert.AreEqual(0.0, TrackGeometry.NormalizeDistance(100, 0), 1e-9);
    }

    [TestMethod]
    public void NormalizeDistance_IsAlwaysInsideTheLap()
    {
        const double lap = 1885.0;
        double[] inputs = [-1e9, -1885.0, -0.5, 0, 1884.999, 1885.0, 1e9];

        foreach (var input in inputs)
        {
            var d = TrackGeometry.NormalizeDistance(input, lap);
            Assert.IsTrue(d >= 0 && d < lap, $"{input} normalized to {d}, outside [0, {lap})");
        }
    }

    [TestMethod]
    public void PointAtDistance_InTheClosingSegment_StaysOnThePath()
    {
        var map = Circle();
        var last = map.Points[^1].CumulativeDistanceMeters;
        var distance = (last + map.TotalLengthMeters) / 2; // halfway round the closing leg

        var result = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, distance);

        Assert.IsNotNull(result);
        var snap = TrackGeometry.Snap(map.Points, map.TotalLengthMeters, result.Value.Latitude, result.Value.Longitude);
        Assert.IsNotNull(snap);
        Assert.AreEqual(0.0, snap.Value.LateralOffsetMeters, 1.0, "The closing leg is part of the path");
        Assert.AreEqual(distance, snap.Value.DistanceAlongMeters, 1.0);
    }

    [TestMethod]
    public void PointAtDistance_BeyondALap_WrapsAround()
    {
        var map = Circle();
        var inside = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, 137.0);
        var wrapped = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, map.TotalLengthMeters + 137.0);

        Assert.IsNotNull(inside);
        Assert.IsNotNull(wrapped);
        Assert.AreEqual(inside.Value.Latitude, wrapped.Value.Latitude, 1e-9);
        Assert.AreEqual(inside.Value.Longitude, wrapped.Value.Longitude, 1e-9);
    }

    [TestMethod]
    public void PointAtDistance_Negative_WrapsBackwards()
    {
        var map = Circle();
        var expected = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, map.TotalLengthMeters - 50.0);
        var negative = TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, -50.0);

        Assert.IsNotNull(expected);
        Assert.IsNotNull(negative);
        Assert.AreEqual(expected.Value.Latitude, negative.Value.Latitude, 1e-9);
        Assert.AreEqual(expected.Value.Longitude, negative.Value.Longitude, 1e-9);
    }

    [TestMethod]
    public void PointAtDistance_TooFewPointsOrNoLength_ReturnsNull()
    {
        var map = Circle();
        Assert.IsNull(TrackGeometry.PointAtDistance([], map.TotalLengthMeters, 10));
        Assert.IsNull(TrackGeometry.PointAtDistance([map.Points[0]], map.TotalLengthMeters, 10));
        Assert.IsNull(TrackGeometry.PointAtDistance(null!, map.TotalLengthMeters, 10));
        Assert.IsNull(TrackGeometry.PointAtDistance(map.Points, 0, 10));
        Assert.IsNull(TrackGeometry.PointAtDistance(map.Points, -100, 10));
    }

    [TestMethod]
    public void PointAtDistance_NonFiniteLength_ReturnsNullRatherThanNaNCoordinates()
    {
        var map = Circle();

        Assert.IsNull(TrackGeometry.PointAtDistance(map.Points, double.NaN, 100));
        Assert.IsNull(TrackGeometry.PointAtDistance(map.Points, double.PositiveInfinity, 100));
    }

    [TestMethod]
    public void PointAtDistance_NonFiniteDistance_ReturnsNull()
    {
        var map = Circle();
        Assert.IsNull(TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, double.NaN));
        Assert.IsNull(TrackGeometry.PointAtDistance(map.Points, map.TotalLengthMeters, double.PositiveInfinity));
    }

    [TestMethod]
    public void BearingDegrees_CardinalDirections()
    {
        var (north, _) = TrackTestData.PointOnCircle(100, Math.PI / 2);
        var (_, east) = TrackTestData.PointOnCircle(100, 0);
        var (south, _) = TrackTestData.PointOnCircle(100, -Math.PI / 2);
        var (_, west) = TrackTestData.PointOnCircle(100, Math.PI);

        Assert.AreEqual(0.0, TrackGeometry.BearingDegrees(TrackTestData.Lat0, TrackTestData.Lon0, north, TrackTestData.Lon0), 0.1);
        Assert.AreEqual(90.0, TrackGeometry.BearingDegrees(TrackTestData.Lat0, TrackTestData.Lon0, TrackTestData.Lat0, east), 0.1);
        Assert.AreEqual(180.0, TrackGeometry.BearingDegrees(TrackTestData.Lat0, TrackTestData.Lon0, south, TrackTestData.Lon0), 0.1);
        Assert.AreEqual(270.0, TrackGeometry.BearingDegrees(TrackTestData.Lat0, TrackTestData.Lon0, TrackTestData.Lat0, west), 0.1);
    }

    [TestMethod]
    public void BearingDegrees_IsAlwaysInRange()
    {
        for (int deg = 0; deg < 360; deg += 13)
        {
            var (lat, lon) = TrackTestData.PointOnCircle(100, deg * Math.PI / 180.0);
            var bearing = TrackGeometry.BearingDegrees(TrackTestData.Lat0, TrackTestData.Lon0, lat, lon);
            Assert.IsTrue(bearing >= 0 && bearing < 360, $"Bearing {bearing} out of range for {deg} degrees");
        }
    }

    [TestMethod]
    public void BearingDegrees_AHairWestOfNorth_WrapsToZeroNotThreeSixty()
    {
        // A bearing this close to due north rounds to exactly 360 when a negative angle is wrapped,
        // which is the one value the documented [0, 360) range excludes. Real track segments never
        // get this close, but the range is public contract.
        double[] bearings =
        [
            TrackGeometry.BearingDegrees(0.0, 1e-300, 1.0, 0.0),
            TrackGeometry.BearingDegrees(40.0, -1.0, 41.0, -1.0 - 2.2e-16),
        ];

        foreach (var bearing in bearings)
            Assert.IsTrue(bearing >= 0 && bearing < 360, $"Bearing {bearing} is outside [0, 360)");
    }

    [TestMethod]
    public void BearingDegrees_CoincidentPoints_IsZero()
    {
        Assert.AreEqual(0.0, TrackGeometry.BearingDegrees(40, -86, 40, -86), 1e-12);
    }
}
