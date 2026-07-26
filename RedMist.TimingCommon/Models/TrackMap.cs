using MessagePack;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// A single point on a track's centerline path, ordered around the lap from the path origin (see
/// <see cref="TrackMap"/>).
/// </summary>
[MessagePackObject]
public class TrackMapPoint
{
    /// <summary>
    /// Latitude in decimal degrees (WGS84).
    /// </summary>
    [JsonPropertyName("lat")]
    [MessagePack.Key(0)]
    public double Latitude { get; set; }
    /// <summary>
    /// Longitude in decimal degrees (WGS84).
    /// </summary>
    [JsonPropertyName("lon")]
    [MessagePack.Key(1)]
    public double Longitude { get; set; }
    /// <summary>
    /// Distance in meters from the first point of the path to this point, measured along the path.
    /// The first point is 0. Distances are relative to the path origin, not the start/finish line;
    /// see <see cref="TrackMap.StartFinishOffsetMeters"/>.
    /// </summary>
    [JsonPropertyName("d")]
    [MessagePack.Key(2)]
    public double CumulativeDistanceMeters { get; set; }
}

/// <summary>
/// A learned model of a track's geometry: an ordered, closed polyline of centerline points with
/// cumulative distances. Built from GPS position data over a clean lap and reused to compute how far
/// a car is around the lap (see <see cref="RedMist.TimingCommon.LapTiming.TrackGeometry"/> and
/// <see cref="RedMist.TimingCommon.LapTiming.GpsLapProjector"/>). Generic and timing-source agnostic.
///
/// The path origin (index 0) is wherever the builder's first sample landed, which is near but not on
/// the start/finish line; <see cref="StartFinishOffsetMeters"/> locates the line itself once calibrated.
/// </summary>
[MessagePackObject]
public class TrackMap
{
    /// <summary>
    /// RedMist event this map was built for.
    /// </summary>
    [JsonPropertyName("eid")]
    [MessagePack.Key(0)]
    public int EventId { get; set; }
    /// <summary>
    /// Session the map was built from.
    /// </summary>
    [JsonPropertyName("sid")]
    [MessagePack.Key(1)]
    public int SessionId { get; set; }
    /// <summary>
    /// Ordered centerline points around the lap, starting at the path origin (index 0). The path is
    /// closed: the last point connects back to the first.
    /// </summary>
    [JsonPropertyName("pts")]
    [MessagePack.Key(2)]
    public List<TrackMapPoint> Points { get; set; } = [];
    /// <summary>
    /// Total lap length in meters, including the closing segment from the last point back to the first.
    /// </summary>
    [JsonPropertyName("len")]
    [MessagePack.Key(3)]
    public double TotalLengthMeters { get; set; }
    /// <summary>
    /// When the map was built, in UTC.
    /// </summary>
    [JsonPropertyName("built")]
    [MessagePack.Key(4)]
    public DateTime BuiltUtc { get; set; }
    /// <summary>
    /// Schema/build version, allowing consumers to detect a rebuilt or upgraded map.
    /// </summary>
    [JsonPropertyName("ver")]
    [MessagePack.Key(5)]
    public int Version { get; set; } = 1;
    /// <summary>
    /// Distance in meters along the path, from <see cref="Points"/>[0], to the start/finish line.
    /// The polyline is anchored wherever the builder's first sample landed, which trails the real
    /// crossing by the timing feed's lap-count latency, so the origin is not the line itself. This
    /// offset is calibrated by observing where cars are when their completed-lap count increments
    /// (see <see cref="RedMist.TimingCommon.LapTiming.StartFinishCalibrator"/>) and is applied when
    /// converting a distance along the path to a lap fraction.
    ///
    /// Null until calibrated. A calibrated offset is legitimately small - the origin is already
    /// anchored to a lap rollover - so zero cannot serve as the "not yet calibrated" marker.
    /// </summary>
    [JsonPropertyName("sfo")]
    [MessagePack.Key(6)]
    public double? StartFinishOffsetMeters { get; set; }
}