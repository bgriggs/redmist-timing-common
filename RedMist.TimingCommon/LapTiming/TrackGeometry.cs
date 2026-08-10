using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.LapTiming;

/// <summary>
/// The result of snapping a geographic point onto a <see cref="TrackMap"/>'s centerline path.
/// </summary>
/// <param name="SegmentIndex">Index of the path segment (point i to point i+1) the point snapped to.</param>
/// <param name="DistanceAlongMeters">Distance from the path origin to the snapped position, along the path.</param>
/// <param name="Fraction">Fraction of the path covered, in [0, 1), measured from the path origin.</param>
/// <param name="LateralOffsetMeters">Perpendicular distance from the point to the path (how far off the centerline).</param>
/// <remarks>
/// Distances and fractions are relative to the path origin (<c>Points[0]</c>), not the start/finish
/// line. Use <see cref="TrackGeometry.FractionFromStartFinish(double, double, double)"/> to convert
/// to a lap fraction once a map's start/finish offset is known.
/// </remarks>
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
    /// Whether a lap length can be measured against at all. A stored map can carry NaN or an
    /// infinity - nothing upstream of it rejects one - and both slip past a bare sign test.
    /// </summary>
    public static bool IsUsableLength(double totalLengthMeters) =>
        double.IsFinite(totalLengthMeters) && totalLengthMeters > 0;

    /// <summary>
    /// Shortest distance between two points measured around a closed path of
    /// <paramref name="totalLengthMeters"/>, i.e. taking the wrap at the origin into account.
    /// </summary>
    public static double CircularDistanceMeters(double aMeters, double bMeters, double totalLengthMeters)
    {
        if (totalLengthMeters <= 0)
            return 0;
        var d = Math.Abs(aMeters - bMeters) % totalLengthMeters;
        return Math.Min(d, totalLengthMeters - d);
    }

    /// <summary>
    /// Converts a distance along the path into a lap fraction in [0, 1) measured from the
    /// start/finish line, shifting by the map's calibrated offset and wrapping at the line.
    /// </summary>
    public static double FractionFromStartFinish(double distanceAlongMeters, double totalLengthMeters,
        double startFinishOffsetMeters)
    {
        if (totalLengthMeters <= 0)
            return 0;
        var d = (distanceAlongMeters - startFinishOffsetMeters) % totalLengthMeters;
        if (d < 0)
        {
            d += totalLengthMeters;
            // A distance a hair behind the line leaves a tiny negative that rounds back up to the
            // full length, which would report a car on the line as a whole lap in rather than none.
            if (d >= totalLengthMeters)
                d = 0;
        }
        return d / totalLengthMeters;
    }

    /// <summary>
    /// Converts a distance along <paramref name="map"/> into a lap fraction in [0, 1) measured from
    /// the start/finish line. An uncalibrated map measures from its path origin instead.
    /// </summary>
    public static double FractionFromStartFinish(TrackMap map, double distanceAlongMeters) =>
        FractionFromStartFinish(distanceAlongMeters, map.TotalLengthMeters, map.StartFinishOffsetMeters ?? 0);

    /// <summary>
    /// The position a given distance along the closed path from its origin, interpolated within the
    /// segment the distance falls in, together with the direction of travel there as a compass
    /// bearing in degrees (0 north, 90 east, clockwise).
    ///
    /// The inverse of <see cref="Snap"/>: that turns a position into a distance, this turns a
    /// distance back into a position. Distances outside the lap wrap around it, so a caller need not
    /// normalize first. Returns null when the path is missing or has fewer than two points, has no
    /// usable length, or when the distance is not finite.
    /// </summary>
    public static (double Latitude, double Longitude, double HeadingDegrees)? PointAtDistance(
        IReadOnlyList<TrackMapPoint> points, double totalLengthMeters, double distanceMeters)
    {
        // The length is checked for finiteness, not just sign: a persisted map can carry NaN or an
        // infinity, because the builder's plausibility check is a pair of comparisons that a NaN
        // fails silently, and either one would otherwise yield NaN coordinates.
        if (points is not { Count: >= 2 } || !IsUsableLength(totalLengthMeters) || !double.IsFinite(distanceMeters))
            return null;

        var d = NormalizeDistance(distanceMeters, totalLengthMeters);

        // Find the segment containing d. Cumulative distances ascend, so a linear walk over a
        // decimated polyline is cheap and avoids assuming they are exactly sorted.
        var n = points.Count;
        for (int i = 0; i < n; i++)
        {
            var a = points[i];
            var b = points[(i + 1) % n];
            var segStart = a.CumulativeDistanceMeters;

            // The final segment closes the loop, so it ends at the lap length rather than at the
            // first point's cumulative distance of zero.
            var segEnd = i + 1 < n ? points[i + 1].CumulativeDistanceMeters : totalLengthMeters;
            if (d > segEnd && i + 1 < n)
                continue;

            var segLength = segEnd - segStart;
            var t = segLength > 0 ? Math.Clamp((d - segStart) / segLength, 0.0, 1.0) : 0.0;

            var lat = a.Latitude + t * (b.Latitude - a.Latitude);
            var lon = a.Longitude + t * (b.Longitude - a.Longitude);
            return (lat, lon, BearingDegrees(a.Latitude, a.Longitude, b.Latitude, b.Longitude));
        }

        // Unreachable: the last iteration cannot continue, because its guard requires i + 1 < n.
        // The compiler cannot see that, so the loop needs an exit.
        return null;
    }

    /// <summary>
    /// A distance along a closed path, brought into [0, totalLengthMeters) by wrapping at the
    /// origin. Distances beyond a lap and negative distances both land where they would if the path
    /// were walked round; a caller therefore never has to normalize before measuring or comparing.
    /// </summary>
    public static double NormalizeDistance(double distanceMeters, double totalLengthMeters)
    {
        if (!IsUsableLength(totalLengthMeters) || !double.IsFinite(distanceMeters))
            return 0;

        var d = distanceMeters % totalLengthMeters;
        if (d < 0)
        {
            d += totalLengthMeters;
            // A distance a hair behind the origin leaves a tiny negative that rounds back up to the
            // full length, which is the one value the range excludes.
            if (d >= totalLengthMeters)
                d = 0;
        }
        return d;
    }

    /// <summary>
    /// Compass bearing in degrees from one point to another: 0 is north, 90 east, measured
    /// clockwise and normalized to [0, 360). Uses the same flat-earth approximation as the rest of
    /// this class, which is exact enough over a segment of a track. Two coincident points have no
    /// direction, and report 0.
    /// </summary>
    public static double BearingDegrees(double lat1, double lon1, double lat2, double lon2)
    {
        var meanLatRad = (lat1 + lat2) * 0.5 * DegToRad;
        var east = (lon2 - lon1) * DegToRad * Math.Cos(meanLatRad);
        var north = (lat2 - lat1) * DegToRad;
        if (east == 0 && north == 0)
            return 0;

        var degrees = Math.Atan2(east, north) / DegToRad;
        if (degrees < 0)
            degrees += 360.0;

        // A bearing a hair west of north leaves a tiny negative that rounds up to exactly 360 when
        // wrapped, which is the one value the documented range excludes.
        return degrees >= 360.0 ? 0.0 : degrees;
    }

    /// <summary>
    /// Snaps a geographic point onto the closed centerline path, returning the distance along the
    /// path from its origin and the corresponding fraction. Returns null when the map has too
    /// few points or zero length.
    /// </summary>
    public static TrackSnapResult? Snap(IReadOnlyList<TrackMapPoint> points, double totalLengthMeters, double lat, double lon) =>
        SnapCore(points, totalLengthMeters, lat, lon, expectedDistanceAlongMeters: null, windowMeters: 0);

    /// <summary>
    /// Snaps a geographic point onto the path, considering only positions within
    /// <paramref name="windowMeters"/> of <paramref name="expectedDistanceAlongMeters"/> (measured
    /// around the loop, so the window wraps at the origin).
    ///
    /// A plain <see cref="Snap"/> takes the geometrically nearest point on the whole path, which is
    /// ambiguous wherever the track passes close to itself - crossovers, hairpins, and parallel
    /// straights - and a few meters of GPS error there puts a car on the wrong leg. Anchoring the
    /// search to where the car was last known to be, widened by how far it could plausibly have
    /// travelled since, resolves that ambiguity.
    ///
    /// This only narrows where along the path to look; it does not judge whether the car is
    /// anywhere near the path. A position nowhere near the window still snaps to the closest
    /// position inside it, reported with a correspondingly large
    /// <see cref="TrackSnapResult.LateralOffsetMeters"/> - checking that is how a caller rejects a
    /// position inconsistent with the car's recent history. Null means no position could be
    /// produced at all: a non-positive window, an unusable map (fewer than two points or no
    /// length), a window narrower than the map's point spacing, or a non-finite position.
    /// </summary>
    public static TrackSnapResult? SnapNear(IReadOnlyList<TrackMapPoint> points, double totalLengthMeters,
        double lat, double lon, double expectedDistanceAlongMeters, double windowMeters)
    {
        if (windowMeters <= 0)
            return null;
        return SnapCore(points, totalLengthMeters, lat, lon, expectedDistanceAlongMeters, windowMeters);
    }

    /// <summary>
    /// Shared snap implementation. When <paramref name="expectedDistanceAlongMeters"/> is supplied,
    /// candidate positions further than <paramref name="windowMeters"/> from it around the loop are
    /// rejected. Segments are short (the builder decimates to a few meters), so a segment is judged
    /// by its projected point rather than by clamping the projection into the window.
    /// </summary>
    private static TrackSnapResult? SnapCore(IReadOnlyList<TrackMapPoint> points, double totalLengthMeters,
        double lat, double lon, double? expectedDistanceAlongMeters, double windowMeters)
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

            // Written as a negated less-than rather than >= so a non-finite lateral (a corrupt point
            // in a persisted map, or a NaN query) is rejected instead of displacing the best match.
            if (!(lateral < bestLateral))
                continue;

            // Use the map's stored cumulative distances for along-path distance so it stays
            // consistent with TotalLengthMeters (including the closing segment).
            var segStart = a.CumulativeDistanceMeters;
            var segEnd = i + 1 < n ? points[i + 1].CumulativeDistanceMeters : totalLengthMeters;
            var distanceAlong = segStart + t * (segEnd - segStart);

            if (expectedDistanceAlongMeters is double expected &&
                CircularDistanceMeters(distanceAlong, expected, totalLengthMeters) > windowMeters)
            {
                continue;
            }

            bestLateral = lateral;
            bestSegment = i;
            bestDistanceAlong = distanceAlong;
        }

        // No candidate was accepted: either a window excluded the whole path, or the position and
        // path never produced a finite distance to compare.
        if (bestLateral == double.MaxValue)
            return null;

        var fraction = bestDistanceAlong / totalLengthMeters;
        if (fraction < 0) fraction = 0;
        if (fraction >= 1) fraction %= 1.0;

        return new TrackSnapResult(bestSegment, bestDistanceAlong, fraction, bestLateral);
    }
}