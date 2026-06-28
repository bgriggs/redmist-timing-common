using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.LapTiming;

/// <summary>
/// Builds a <see cref="TrackMap"/> from a stream of GPS samples for a single car. Samples are tagged
/// with the car's completed-lap count; the builder buffers samples for the current lap and, on each
/// lap rollover, finalizes a map from the lap that just completed.
///
/// The first observed lap is discarded because a car is usually joined mid-lap, so its buffer is a
/// partial lap. The first <em>full</em> lap (the span between two consecutive lap rollovers) becomes
/// the map. Samples are decimated by a minimum spacing so the polyline stays compact regardless of
/// the feed's update rate.
///
/// Pure and timing-source agnostic: feed it the corrected/snapped positions from any source.
/// </summary>
public class TrackMapBuilder
{
    /// <summary>Minimum points a lap buffer must have to be considered a usable map.</summary>
    public const int MinPoints = 20;
    /// <summary>Minimum plausible lap length in meters; shorter buffers are rejected as noise.</summary>
    public const double MinLapLengthMeters = 200.0;

    private readonly int eventId;
    private readonly double minSpacingMeters;

    private int? currentLap;
    private bool seenRollover;
    private readonly List<TrackMapPoint> currentLapPoints = [];
    private TrackMap? completed;

    /// <param name="eventId">Event the resulting map belongs to.</param>
    /// <param name="minSpacingMeters">Drop samples closer than this to the previous kept sample.</param>
    public TrackMapBuilder(int eventId, double minSpacingMeters = 5.0)
    {
        this.eventId = eventId;
        this.minSpacingMeters = minSpacingMeters;
    }

    /// <summary>True once a full clean lap has been captured and a map is available.</summary>
    public bool IsComplete => completed != null;

    /// <summary>
    /// Adds a GPS sample for the car. <paramref name="completedLaps"/> is the car's completed-lap
    /// count (e.g. CarPosition.LastLapCompleted); an increment marks a start/finish crossing.
    /// </summary>
    public void AddSample(double latitude, double longitude, int completedLaps)
    {
        if (completed != null)
            return;

        if (currentLap == null)
            currentLap = completedLaps;

        if (completedLaps != currentLap)
        {
            // Start/finish crossing. The buffer holds the lap that just completed. Skip the very first
            // rollover (its buffer is the partial join-in lap); use the first full lap after that.
            if (seenRollover)
            {
                var map = BuildFrom(currentLapPoints);
                if (map != null)
                {
                    completed = map;
                    return;
                }
            }
            seenRollover = true;
            currentLap = completedLaps;
            currentLapPoints.Clear();
        }

        // Decimate: keep the sample only if it is far enough from the last kept one.
        if (currentLapPoints.Count == 0)
        {
            currentLapPoints.Add(new TrackMapPoint { Latitude = latitude, Longitude = longitude });
            return;
        }

        var last = currentLapPoints[^1];
        if (TrackGeometry.DistanceMeters(last.Latitude, last.Longitude, latitude, longitude) >= minSpacingMeters)
            currentLapPoints.Add(new TrackMapPoint { Latitude = latitude, Longitude = longitude });
    }

    /// <summary>
    /// Returns the completed map stamped with the source session and build time, or null if a full
    /// lap has not been captured yet.
    /// </summary>
    public TrackMap? Build(int sessionId, DateTime builtUtc)
    {
        if (completed == null)
            return null;
        completed.SessionId = sessionId;
        completed.BuiltUtc = builtUtc;
        return completed;
    }

    /// <summary>
    /// Computes cumulative distances and total (closed) length for a buffer of ordered points and
    /// returns a map, or null if the buffer is too small/short to be a real lap.
    /// </summary>
    private TrackMap? BuildFrom(List<TrackMapPoint> pts)
    {
        if (pts.Count < MinPoints)
            return null;

        var points = new List<TrackMapPoint>(pts.Count);
        double cumulative = 0;
        for (int i = 0; i < pts.Count; i++)
        {
            if (i > 0)
                cumulative += TrackGeometry.DistanceMeters(
                    pts[i - 1].Latitude, pts[i - 1].Longitude, pts[i].Latitude, pts[i].Longitude);

            points.Add(new TrackMapPoint
            {
                Latitude = pts[i].Latitude,
                Longitude = pts[i].Longitude,
                CumulativeDistanceMeters = cumulative,
            });
        }

        // Closing segment from the last point back to the first.
        var closing = TrackGeometry.DistanceMeters(
            pts[^1].Latitude, pts[^1].Longitude, pts[0].Latitude, pts[0].Longitude);
        var totalLength = cumulative + closing;

        if (totalLength < MinLapLengthMeters)
            return null;

        return new TrackMap
        {
            EventId = eventId,
            Points = points,
            TotalLengthMeters = totalLength,
            Version = 1,
        };
    }
}