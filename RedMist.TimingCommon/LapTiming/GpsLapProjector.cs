using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.LapTiming;

/// <summary>
/// The result of projecting a car's lap time from its position on a <see cref="TrackMap"/>.
/// </summary>
/// <param name="ProjectedLapTimeMs">Estimated total lap time in milliseconds.</param>
/// <param name="Fraction">Fraction of the lap completed at the sampled position, in (0, 1).</param>
/// <param name="DistanceAlongMeters">Distance from start/finish to the sampled position.</param>
public readonly record struct LapProjection(
    int ProjectedLapTimeMs,
    double Fraction,
    double DistanceAlongMeters);

/// <summary>
/// Projects a car's total lap time from where it currently is on the track and how long the current
/// lap has been running: <c>projected = elapsed / fraction</c>, where <c>fraction</c> comes from
/// snapping the car's GPS position onto the <see cref="TrackMap"/>.
///
/// This is intentionally a thin, pure calculation. Validity gating (flag, pit, GPS staleness) and
/// sanity bounds (e.g. relative to a reference lap) are the caller's responsibility — the projection
/// is unreliable very early in a lap, which the <c>minFraction</c> guard reflects.
/// </summary>
public static class GpsLapProjector
{
    /// <summary>
    /// Computes a projected lap time, or null when no map is available, the position cannot be
    /// snapped, the car is too early in the lap (fraction below <paramref name="minFraction"/>), or
    /// elapsed time is non-positive.
    /// </summary>
    public static LapProjection? Project(TrackMap? map, double latitude, double longitude,
        TimeSpan elapsedSinceLapStart, double minFraction = 0.05)
    {
        if (map == null || map.Points.Count < 2 || map.TotalLengthMeters <= 0)
            return null;
        if (elapsedSinceLapStart <= TimeSpan.Zero)
            return null;

        var snap = TrackGeometry.Snap(map.Points, map.TotalLengthMeters, latitude, longitude);
        if (snap == null || snap.Value.Fraction < minFraction)
            return null;

        var ms = elapsedSinceLapStart.TotalMilliseconds / snap.Value.Fraction;
        if (ms <= 0 || ms > int.MaxValue)
            return null;

        return new LapProjection((int)ms, snap.Value.Fraction, snap.Value.DistanceAlongMeters);
    }
}