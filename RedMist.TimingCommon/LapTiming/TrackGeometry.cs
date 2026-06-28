using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.LapTiming;

/// <summary>
/// The result of snapping a geographic point onto a <see cref="TrackMap"/>'s centerline path.
/// </summary>
/// <param name="SegmentIndex">Index of the path segment (point i to point i+1) the point snapped to.</param>
/// <param name="DistanceAlongMeters">Distance from start/finish to the snapped position, along the path.</param>
/// <param name="Fraction">Fraction of the lap completed, in [0, 1).</param>
/// <param name="LateralOffsetMeters">Perpendicular distance from the point to the path (how far off the centerline).</param>
public readonly record struct TrackSnapResult(
    int SegmentIndex,
    double DistanceAlongMeters,
    double Fraction,
    double LateralOffsetMeters);

/// <summary>
/// Pure geometry helpers for working with <see cref="TrackMap"/> data: distance between geographic
/// points and snapping a point onto the track centerline. Uses an equirectangular (flat-earth)
/// projection, which is cheap and accurate to well under a meter over the area of a race track.
/// </summary>
public static class TrackGeometry
{
    public const double EarthRadiusMeters = 6_371_000.0;
    private const double DegToRad = Math.PI / 180.0;

    /// <summary>
    /// Great-circle-ish distance in meters between two lat/lon points using an equirectangular
    /// approximation. Accurate to sub-meter at track scale.
    /// </summary>
    public static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        var meanLatRad = (lat1 + lat2) * 0.5 * DegToRad;
        var x = (lon2 - lon1) * DegToRad * Math.Cos(meanLatRad);
        var y = (lat2 - lat1) * DegToRad;
        return Math.Sqrt(x * x + y * y) * EarthRadiusMeters;
    }

    /// <summary>
    /// Snaps a geographic point onto the closed centerline path, returning the distance along the
    /// path from start/finish and the corresponding lap fraction. Returns null when the map has too
    /// few points or zero length.
    /// </summary>
    public static TrackSnapResult? Snap(IReadOnlyList<TrackMapPoint> points, double totalLengthMeters, double lat, double lon)
    {
        if (points.Count < 2 || totalLengthMeters <= 0)
            return null;

        // Project everything to a local planar frame (meters) referenced to the first point, so we can
        // do straight-line segment projection. cos(lat0) is effectively constant over a track.
        var lat0 = points[0].Latitude;
        var lon0 = points[0].Longitude;
        var cosLat0 = Math.Cos(lat0 * DegToRad);

        (double x, double y) ToLocal(double la, double lo) =>
            ((lo - lon0) * DegToRad * cosLat0 * EarthRadiusMeters,
             (la - lat0) * DegToRad * EarthRadiusMeters);

        var (qx, qy) = ToLocal(lat, lon);

        var bestLateral = double.MaxValue;
        var bestSegment = 0;
        var bestDistanceAlong = 0.0;

        var n = points.Count;
        for (int i = 0; i < n; i++)
        {
            var a = points[i];
            var b = points[(i + 1) % n];           // wrap to close the loop
            var (ax, ay) = ToLocal(a.Latitude, a.Longitude);
            var (bx, by) = ToLocal(b.Latitude, b.Longitude);

            var dx = bx - ax;
            var dy = by - ay;
            var segLenSq = dx * dx + dy * dy;

            // Parametric projection of Q onto segment AB, clamped to the segment.
            var t = segLenSq > 0 ? ((qx - ax) * dx + (qy - ay) * dy) / segLenSq : 0.0;
            t = Math.Clamp(t, 0.0, 1.0);

            var px = ax + t * dx;
            var py = ay + t * dy;
            var lateral = Math.Sqrt((qx - px) * (qx - px) + (qy - py) * (qy - py));

            if (lateral < bestLateral)
            {
                bestLateral = lateral;
                bestSegment = i;
                // Use the map's stored cumulative distances for along-path distance so it stays
                // consistent with TotalLengthMeters (including the closing segment).
                var segStart = a.CumulativeDistanceMeters;
                var segEnd = i + 1 < n ? points[i + 1].CumulativeDistanceMeters : totalLengthMeters;
                bestDistanceAlong = segStart + t * (segEnd - segStart);
            }
        }

        var fraction = bestDistanceAlong / totalLengthMeters;
        if (fraction < 0) fraction = 0;
        if (fraction >= 1) fraction %= 1.0;

        return new TrackSnapResult(bestSegment, bestDistanceAlong, fraction, bestLateral);
    }
}