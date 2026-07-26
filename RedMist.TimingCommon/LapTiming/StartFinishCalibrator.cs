using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.LapTiming;

/// <summary>
/// Locates the start/finish line on a learned <see cref="TrackMap"/>.
///
/// A map's path origin is wherever the builder's first sample landed after a lap rollover, which
/// trails the real start/finish crossing by however long the timing feed took to report the lap.
/// The line itself is found by observing where cars actually are the moment their completed-lap
/// count increments: each observation is a distance along the path, and the true line is their
/// consensus.
///
/// The consensus is a circular median rather than a mean because the observations wrap at the path
/// origin - a set straddling the wrap (say 5 m and 3990 m on a 4 km lap) averages to the far side of
/// the track, while the median lands correctly. It also shrugs off the occasional bad observation
/// from a car whose lap was reported late.
///
/// What this can and cannot correct is worth being clear about. Every observation is biased forward
/// by however long the feed took to report the lap, and so is the map's own origin, which was
/// recorded at a rollover in the same way - so the estimate mostly confirms the anchor rather than
/// moving it far, and the shared latency bias survives calibration. What it does correct is an
/// origin laid down under unrepresentative conditions: a sparse GPS sample, or a map learned from
/// one car whose report lagged differently from the rest of the field. Consumers displaying a
/// coarse position should expect a residual on the order of the field's reporting spread.
/// </summary>
public static class StartFinishCalibrator
{
    /// <summary>Observations needed before an offset is trusted at all.</summary>
    public const int DefaultMinObservations = 5;

    /// <summary>
    /// Median deviation above which the observations are treated as having no consensus. Sized for
    /// the spread real crossings produce (differing car speeds and reporting latency), so a wider
    /// scatter means something else is wrong - a stale map, a changed course, or bad positions.
    /// </summary>
    public const double DefaultMaxMedianDeviationMeters = 100.0;

    /// <summary>
    /// Estimates the start/finish offset from distances along the path observed at lap rollovers.
    /// Returns null when there are too few observations or they fail to agree, in which case the
    /// caller should keep collecting and leave the map uncalibrated.
    /// </summary>
    /// <param name="observedDistancesAlongMeters">Distance along the path at each observed crossing.</param>
    /// <param name="totalLengthMeters">Total (closed) path length.</param>
    /// <param name="minObservations">Observations required before an estimate is returned.</param>
    /// <param name="maxMedianDeviationMeters">Largest median deviation still considered a consensus.</param>
    public static double? EstimateOffsetMeters(IReadOnlyList<double> observedDistancesAlongMeters,
        double totalLengthMeters, int minObservations = DefaultMinObservations,
        double maxMedianDeviationMeters = DefaultMaxMedianDeviationMeters)
    {
        if (totalLengthMeters <= 0 || observedDistancesAlongMeters.Count < minObservations)
            return null;

        // A single non-finite observation would poison every distance comparison below, leaving the
        // first element to win by default and pass the consensus check as if it were agreed on.
        // The zero check also covers a caller passing a minimum of zero or less.
        var observations = observedDistancesAlongMeters.Where(double.IsFinite).ToList();
        if (observations.Count == 0 || observations.Count < minObservations)
            return null;

        // Circular median: the observation whose total distance to all the others is smallest.
        // O(n^2) over a handful of crossings, and unlike a mean it is well defined across the wrap.
        var best = observations[0];
        var bestCost = double.MaxValue;
        foreach (var candidate in observations)
        {
            double cost = 0;
            foreach (var observation in observations)
                cost += TrackGeometry.CircularDistanceMeters(candidate, observation, totalLengthMeters);

            if (cost < bestCost)
            {
                bestCost = cost;
                best = candidate;
            }
        }

        var deviations = observations
            .Select(o => TrackGeometry.CircularDistanceMeters(best, o, totalLengthMeters))
            .OrderBy(d => d)
            .ToList();
        var medianDeviation = deviations.Count % 2 == 0
            ? (deviations[deviations.Count / 2 - 1] + deviations[deviations.Count / 2]) / 2.0
            : deviations[deviations.Count / 2];

        if (medianDeviation > maxMedianDeviationMeters)
            return null;

        var offset = best % totalLengthMeters;
        if (offset < 0)
            offset += totalLengthMeters;
        return offset;
    }
}
