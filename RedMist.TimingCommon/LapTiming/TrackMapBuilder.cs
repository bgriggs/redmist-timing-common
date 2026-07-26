using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.LapTiming;

/// <summary>
/// Builds a <see cref="TrackMap"/> from a stream of GPS samples for a single car. Samples are tagged
/// with the car's completed-lap count; the builder buffers samples for the current lap and, on each
/// lap rollover, offers the lap that just completed as a candidate map.
///
/// A candidate is not taken on trust. The lap count comes from the timing feed, and when it arrives
/// late or irregularly - which it does, especially early in a session - a single buffer can span two
/// or three real laps and produce a map far longer than the circuit.
///
/// Two properties make that self-correcting without knowing the circuit in advance. A buffer can
/// only ever be too long: it spans at least one lap, and swallowing a missed crossing adds another,
/// so the shortest candidate seen is the best estimate of a single lap and every later candidate is
/// measured against that rather than against whatever came immediately before. And corroboration
/// compares shape as well as length, because two laps of the same circuit trace the same line while
/// a lap that cut a corner - a dropout bridged by a straight, a detour - does not, however close its
/// length happens to land.
///
/// Comparing only consecutive candidates is not enough: a feed running consistently late produces
/// consecutive buffers that are both two laps long and agree with each other perfectly.
///
/// The first observed lap is discarded outright because a car is usually joined mid-lap. Laps during
/// which the car left the racing surface are discarded too - a lap through the pits cuts a corner
/// the racing line does not have. Samples are decimated by a minimum spacing so the polyline stays
/// compact regardless of the feed's update rate.
///
/// Pure and timing-source agnostic: feed it the corrected/snapped positions from any source.
/// </summary>
public class TrackMapBuilder
{
    /// <summary>Minimum points a lap buffer must have to be considered a usable map.</summary>
    public const int MinPoints = 20;
    /// <summary>Minimum plausible lap length in meters; shorter buffers are rejected as noise.</summary>
    public const double MinLapLengthMeters = 200.0;
    /// <summary>
    /// Longest plausible lap. Comfortably past the longest circuits raced anywhere, so it says
    /// nothing about a real track and only bounds a buffer that has run away entirely.
    /// </summary>
    public const double MaxLapLengthMeters = 30_000.0;
    /// <summary>
    /// How closely two candidate laps must agree on length to corroborate each other, as a fraction
    /// of the shortest seen. Wide enough for the sampling to land differently around the corners
    /// from one lap to the next, far tighter than the gap between one lap and two.
    /// </summary>
    public const double LengthAgreementTolerance = 0.05;

    /// <summary>
    /// How far a corroborating lap may stray from the one it is confirming before the two are
    /// treated as different lines rather than the same one driven twice. Covers the width of a
    /// track and the scatter of the fixes describing it; a lap that skipped part of the circuit
    /// departs from it by far more.
    /// </summary>
    public const double ShapeAgreementMeters = 30.0;

    /// <summary>How many laps must agree before a length is believed.</summary>
    public const int RequiredAgreeingLaps = 2;

    private readonly int eventId;
    private readonly double minSpacingMeters;

    private int? currentLap;
    private bool seenRollover;
    private bool currentLapLeftTrack;
    private readonly List<TrackMapPoint> currentLapPoints = [];
    private TrackMap? completed;

    /// <summary>
    /// The shortest plausible candidate seen so far - the best estimate of a single lap - and how
    /// many laps have corroborated it.
    /// </summary>
    private TrackMap? shortestCandidate;
    private int agreeingLaps;

    /// <summary>Candidate laps considered, and laps thrown away for leaving the racing surface.</summary>
    public int CandidatesConsidered { get; private set; }
    public int LapsAbandoned { get; private set; }

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
    /// <param name="latitude">Sample latitude in decimal degrees.</param>
    /// <param name="longitude">Sample longitude in decimal degrees.</param>
    /// <param name="completedLaps">The car's completed-lap count when the sample was taken.</param>
    /// <param name="onTrack">
    /// False when the car was off the racing surface - in the pits, the paddock, anywhere its
    /// position does not describe the racing line. The sample is dropped and the whole lap it falls
    /// in is abandoned, since a lap through the pits short-cuts geometry the track really has.
    /// </param>
    public void AddSample(double latitude, double longitude, int completedLaps, bool onTrack = true)
    {
        if (completed != null)
            return;

        if (currentLap == null)
            currentLap = completedLaps;

        if (completedLaps != currentLap)
        {
            // Start/finish crossing. The buffer holds the lap that just completed. Skip the very
            // first rollover (its buffer is the partial join-in lap), and any lap the car spent
            // partly off the racing surface.
            if (seenRollover && currentLapLeftTrack)
                LapsAbandoned++;

            if (seenRollover && !currentLapLeftTrack)
            {
                var candidate = BuildFrom(currentLapPoints);
                if (candidate != null && Consider(candidate))
                    return;
            }
            seenRollover = true;
            currentLap = completedLaps;
            currentLapLeftTrack = false;
            currentLapPoints.Clear();
        }

        if (!onTrack)
        {
            // The buffer is discarded wholesale at the next rollover, so dropping the sample here
            // changes nothing on its own - it just keeps the buffer honest while it lasts.
            currentLapLeftTrack = true;
            return;
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
    /// Weighs a candidate lap against the shortest seen. Returns true once enough laps have
    /// corroborated that shortest one for it to be taken as the map.
    /// </summary>
    private bool Consider(TrackMap candidate)
    {
        CandidatesConsidered++;

        if (shortestCandidate == null)
        {
            shortestCandidate = candidate;
            agreeingLaps = 1;
            return false;
        }

        var shortest = shortestCandidate.TotalLengthMeters;
        var difference = Math.Abs(candidate.TotalLengthMeters - shortest) / shortest;

        if (candidate.TotalLengthMeters < shortest && difference > LengthAgreementTolerance)
        {
            // Shorter than anything seen, so everything before it covered more than one lap.
            shortestCandidate = candidate;
            agreeingLaps = 1;
            return false;
        }

        // Longer than the shortest by more than the tolerance: this buffer swallowed a crossing.
        if (difference > LengthAgreementTolerance)
            return false;

        if (!SameLine(shortestCandidate, candidate))
            return false;

        agreeingLaps++;
        // Both describe the same lap, so keep whichever traced it in more detail.
        if (candidate.Points.Count > shortestCandidate.Points.Count)
            shortestCandidate = candidate;

        if (agreeingLaps < RequiredAgreeingLaps)
            return false;

        completed = shortestCandidate;
        return true;
    }

    /// <summary>
    /// Whether one candidate follows the same line as another, rather than merely covering the same
    /// distance. Two laps of a circuit overlay each other; a lap that missed part of it does not.
    ///
    /// Both directions are checked, because a lap that skipped a section is not betrayed by its own
    /// points - those still sit on the track - but by the section it has nothing to offer against.
    /// </summary>
    private static bool SameLine(TrackMap a, TrackMap b) => LiesAlong(b, a) && LiesAlong(a, b);

    /// <summary>Whether every point of <paramref name="points"/> sits close to <paramref name="path"/>.</summary>
    private static bool LiesAlong(TrackMap points, TrackMap path)
    {
        foreach (var point in points.Points)
        {
            var snap = TrackGeometry.Snap(path.Points, path.TotalLengthMeters, point.Latitude, point.Longitude);
            if (snap == null || snap.Value.LateralOffsetMeters > ShapeAgreementMeters)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Computes cumulative distances and total (closed) length for a buffer of ordered points and
    /// returns a map, or null if the buffer is too small, too short or too long to be a real lap.
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

        if (totalLength < MinLapLengthMeters || totalLength > MaxLapLengthMeters)
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