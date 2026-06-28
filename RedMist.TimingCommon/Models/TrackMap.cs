using MessagePack;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// A single point on a track's centerline path, ordered from the start/finish line around the lap.
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
    /// Distance in meters from the start/finish line to this point, measured along the path.
    /// The first point is 0.
    /// </summary>
    [JsonPropertyName("d")]
    [MessagePack.Key(2)]
    public double CumulativeDistanceMeters { get; set; }
}

/// <summary>
/// A learned model of a track's geometry: an ordered, closed polyline of centerline points with
/// cumulative distances, anchored at the start/finish line (index 0). Built from GPS position data
/// over a clean lap and reused to compute how far a car is around the lap (see
/// <see cref="RedMist.TimingCommon.LapTiming.TrackGeometry"/> and
/// <see cref="RedMist.TimingCommon.LapTiming.GpsLapProjector"/>). Generic and timing-source agnostic.
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
    /// Ordered centerline points from start/finish (index 0) around the lap. The path is closed: the
    /// last point connects back to the first.
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
}