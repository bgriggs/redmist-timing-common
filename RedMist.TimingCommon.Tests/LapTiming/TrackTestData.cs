using RedMist.TimingCommon.LapTiming;
using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.Tests.LapTiming;

/// <summary>
/// Helpers that synthesize a circular "track" with known geometry so geometry/builder/projector math
/// can be asserted against exact expected values (circumference = 2*pi*r, fraction = angle / 2*pi).
/// </summary>
internal static class TrackTestData
{
    public const double Lat0 = 40.0;
    public const double Lon0 = -86.0;
    private const double DegToRad = Math.PI / 180.0;
    private const double RadToDeg = 180.0 / Math.PI;

    /// <summary>A point on a circle of radius <paramref name="radiusMeters"/> at angle <paramref name="angleRad"/>.</summary>
    public static (double lat, double lon) PointOnCircle(double radiusMeters, double angleRad,
        double centerLat = Lat0, double centerLon = Lon0)
    {
        var r = TrackGeometry.EarthRadiusMeters;
        var cos0 = Math.Cos(centerLat * DegToRad);
        var east = radiusMeters * Math.Cos(angleRad);
        var north = radiusMeters * Math.Sin(angleRad);
        var lat = centerLat + (north / r) * RadToDeg;
        var lon = centerLon + (east / (r * cos0)) * RadToDeg;
        return (lat, lon);
    }

    /// <summary>Evenly spaced points around a circle starting at angle 0.</summary>
    public static List<(double lat, double lon)> Circle(double radiusMeters, int count, double startRad = 0)
    {
        var pts = new List<(double, double)>(count);
        for (int i = 0; i < count; i++)
            pts.Add(PointOnCircle(radiusMeters, startRad + 2 * Math.PI * i / count));
        return pts;
    }

    /// <summary>A <see cref="TrackMap"/> for a circle, with cumulative distances filled in.</summary>
    public static TrackMap CircleMap(double radiusMeters, int count)
    {
        var c = Circle(radiusMeters, count);
        var map = new TrackMap { EventId = 1, SessionId = 1 };
        double cum = 0;
        for (int i = 0; i < c.Count; i++)
        {
            if (i > 0)
                cum += TrackGeometry.DistanceMeters(c[i - 1].lat, c[i - 1].lon, c[i].lat, c[i].lon);
            map.Points.Add(new TrackMapPoint
            {
                Latitude = c[i].lat,
                Longitude = c[i].lon,
                CumulativeDistanceMeters = cum,
            });
        }
        var closing = TrackGeometry.DistanceMeters(c[^1].lat, c[^1].lon, c[0].lat, c[0].lon);
        map.TotalLengthMeters = cum + closing;
        return map;
    }
}