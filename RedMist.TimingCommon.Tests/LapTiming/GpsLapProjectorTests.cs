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