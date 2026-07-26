using RedMist.TimingCommon.LapTiming;
using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Tests.LapTiming;

[TestClass]
public class GpsLapProjectorTests
{
    [TestMethod]
    public void Project_HalfwayAtHalfTime_ProjectsFullLap()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, Math.PI); // half a lap

        var p = GpsLapProjector.Project(map, lat, lon, TimeSpan.FromSeconds(45));

        Assert.IsNotNull(p);
        Assert.AreEqual(0.5, p.Value.Fraction, 0.01);
        Assert.AreEqual(90_000, p.Value.ProjectedLapTimeMs, 2_000);
    }

    [TestMethod]
    public void Project_QuarterAtQuarterTime_ProjectsFullLap()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, Math.PI / 2); // quarter lap

        var p = GpsLapProjector.Project(map, lat, lon, TimeSpan.FromSeconds(22.5));

        Assert.IsNotNull(p);
        Assert.AreEqual(0.25, p.Value.Fraction, 0.01);
        Assert.AreEqual(90_000, p.Value.ProjectedLapTimeMs, 2_500);
    }

    [TestMethod]
    public void Project_TooEarlyInLap_ReturnsNull()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, 2 * Math.PI * 0.01); // 1% in

        var p = GpsLapProjector.Project(map, lat, lon, TimeSpan.FromSeconds(1), minFraction: 0.05);

        Assert.IsNull(p);
    }

    [TestMethod]
    public void Project_CalibratedMap_MeasuresFractionFromStartFinish()
    {
        // The line sits a quarter of the way along the path, so a car three quarters along the path
        // is only half a lap in.
        var map = TrackTestData.CircleMap(200, 72);
        map.StartFinishOffsetMeters = map.TotalLengthMeters * 0.25;
        var (lat, lon) = TrackTestData.PointOnCircle(200, 2 * Math.PI * 0.75);

        var p = GpsLapProjector.Project(map, lat, lon, TimeSpan.FromSeconds(45));

        Assert.IsNotNull(p);
        Assert.AreEqual(0.5, p.Value.Fraction, 0.01);
        Assert.AreEqual(90_000, p.Value.ProjectedLapTimeMs, 2_000);
    }

    [TestMethod]
    public void Project_CalibratedMap_WrapsForACarBehindTheLine()
    {
        // Car is at 10% of the path with the line at 25%, so it is 85% of the way round the lap.
        var map = TrackTestData.CircleMap(200, 72);
        map.StartFinishOffsetMeters = map.TotalLengthMeters * 0.25;
        var (lat, lon) = TrackTestData.PointOnCircle(200, 2 * Math.PI * 0.10);

        var p = GpsLapProjector.Project(map, lat, lon, TimeSpan.FromSeconds(85));

        Assert.IsNotNull(p);
        Assert.AreEqual(0.85, p.Value.Fraction, 0.01);
        Assert.AreEqual(100_000, p.Value.ProjectedLapTimeMs, 3_000);
    }

    [TestMethod]
    public void Project_CalibratedMap_DistanceAndFractionShareTheSameOrigin()
    {
        var map = TrackTestData.CircleMap(200, 72);
        map.StartFinishOffsetMeters = map.TotalLengthMeters * 0.25;
        var (lat, lon) = TrackTestData.PointOnCircle(200, 2 * Math.PI * 0.10);

        var p = GpsLapProjector.Project(map, lat, lon, TimeSpan.FromSeconds(85));

        Assert.IsNotNull(p);
        Assert.AreEqual(p.Value.Fraction, p.Value.DistanceAlongMeters / map.TotalLengthMeters, 1e-9,
            "Distance and fraction must be measured from the same place");
    }

    [TestMethod]
    public void Project_CalibratedMap_MinFractionGatesFromTheLine()
    {
        // Just past the calibrated line is too early in the lap, even though the raw path fraction
        // is well above the minimum.
        var map = TrackTestData.CircleMap(200, 72);
        map.StartFinishOffsetMeters = map.TotalLengthMeters * 0.25;
        var (lat, lon) = TrackTestData.PointOnCircle(200, 2 * Math.PI * 0.26);

        Assert.IsNull(GpsLapProjector.Project(map, lat, lon, TimeSpan.FromSeconds(1), minFraction: 0.05));
    }

    [TestMethod]
    public void Project_NonFinitePosition_ReturnsNull()
    {
        var map = TrackTestData.CircleMap(200, 72);
        Assert.IsNull(GpsLapProjector.Project(map, double.NaN, -86, TimeSpan.FromSeconds(45)));
    }

    [TestMethod]
    public void Project_NullMap_ReturnsNull()
    {
        Assert.IsNull(GpsLapProjector.Project(null, 40, -86, TimeSpan.FromSeconds(45)));
    }

    [TestMethod]
    public void Project_NonPositiveElapsed_ReturnsNull()
    {
        var map = TrackTestData.CircleMap(200, 72);
        var (lat, lon) = TrackTestData.PointOnCircle(200, Math.PI);
        Assert.IsNull(GpsLapProjector.Project(map, lat, lon, TimeSpan.Zero));
    }

    [TestMethod]
    public void Project_EmptyMap_ReturnsNull()
    {
        var map = new TrackMap { TotalLengthMeters = 0 };
        Assert.IsNull(GpsLapProjector.Project(map, 40, -86, TimeSpan.FromSeconds(45)));
    }
}