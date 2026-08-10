using MessagePack;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// A <see cref="TrackMap"/> point carrying both its geographic position and a planar position ready
/// to draw. See <see cref="TrackMapRender"/> for what the planar frame is and how to scale it.
/// </summary>
[MessagePackObject]
public class TrackMapRenderPoint
{
    /// <summary>Latitude in decimal degrees (WGS84).</summary>
    [JsonPropertyName("lat")]
    [Key(0)]
    public double Latitude { get; set; }

    /// <summary>Longitude in decimal degrees (WGS84).</summary>
    [JsonPropertyName("lon")]
    [Key(1)]
    public double Longitude { get; set; }

    /// <summary>Meters east of the map's western edge. Increases left to right, as on screen.</summary>
    [JsonPropertyName("x")]
    [Key(2)]
    public double X { get; set; }

    /// <summary>
    /// Meters south of the map's northern edge. Increases downward, matching screen coordinates, so
    /// a consumer scales and translates but never has to flip an axis.
    /// </summary>
    [JsonPropertyName("y")]
    [Key(3)]
    public double Y { get; set; }

    /// <summary>
    /// Distance in meters from the path origin to this point, measured along the path. Carried
    /// through from <see cref="TrackMapPoint.CumulativeDistanceMeters"/>, so it is measured from the
    /// path origin rather than the start/finish line.
    /// </summary>
    [JsonPropertyName("d")]
    [Key(4)]
    public double CumulativeDistanceMeters { get; set; }
}

/// <summary>
/// The geographic extent of a <see cref="TrackMapRender"/>, and the size of the planar frame its
/// points are expressed in.
/// </summary>
[MessagePackObject]
public class TrackMapBounds
{
    /// <summary>Southern edge, in decimal degrees.</summary>
    [JsonPropertyName("minLat")]
    [Key(0)]
    public double MinLatitude { get; set; }

    /// <summary>Northern edge, in decimal degrees.</summary>
    [JsonPropertyName("maxLat")]
    [Key(1)]
    public double MaxLatitude { get; set; }

    /// <summary>Western edge, in decimal degrees.</summary>
    [JsonPropertyName("minLon")]
    [Key(2)]
    public double MinLongitude { get; set; }

    /// <summary>Eastern edge, in decimal degrees.</summary>
    [JsonPropertyName("maxLon")]
    [Key(3)]
    public double MaxLongitude { get; set; }

    /// <summary>
    /// Width of the planar frame in meters: the largest <see cref="TrackMapRenderPoint.X"/> on the
    /// map. Divide a viewport's width by this to get the scale that fits the track horizontally.
    /// </summary>
    [JsonPropertyName("w")]
    [Key(4)]
    public double WidthMeters { get; set; }

    /// <summary>
    /// Height of the planar frame in meters: the largest <see cref="TrackMapRenderPoint.Y"/> on the
    /// map. Divide a viewport's height by this to get the scale that fits the track vertically.
    /// </summary>
    [JsonPropertyName("h")]
    [Key(5)]
    public double HeightMeters { get; set; }

    /// <summary>Latitude of the center of the extent, in decimal degrees.</summary>
    [JsonPropertyName("cLat")]
    [Key(6)]
    public double CenterLatitude { get; set; }

    /// <summary>Longitude of the center of the extent, in decimal degrees.</summary>
    [JsonPropertyName("cLon")]
    [Key(7)]
    public double CenterLongitude { get; set; }
}

/// <summary>
/// Where the start/finish line sits on a rendered map, and which way cars are travelling as they
/// cross it, so a consumer can mark the line and orient the track without re-deriving either.
/// </summary>
[MessagePackObject]
public class TrackMapStartFinish
{
    /// <summary>Latitude of the line, in decimal degrees.</summary>
    [JsonPropertyName("lat")]
    [Key(0)]
    public double Latitude { get; set; }

    /// <summary>Longitude of the line, in decimal degrees.</summary>
    [JsonPropertyName("lon")]
    [Key(1)]
    public double Longitude { get; set; }

    /// <summary>Position of the line in the map's planar frame; see <see cref="TrackMapRenderPoint.X"/>.</summary>
    [JsonPropertyName("x")]
    [Key(2)]
    public double X { get; set; }

    /// <summary>Position of the line in the map's planar frame; see <see cref="TrackMapRenderPoint.Y"/>.</summary>
    [JsonPropertyName("y")]
    [Key(3)]
    public double Y { get; set; }

    /// <summary>
    /// Distance in meters from the path origin to the line, along the path. The same figure as
    /// <see cref="TrackMap.StartFinishOffsetMeters"/>.
    /// </summary>
    [JsonPropertyName("d")]
    [Key(4)]
    public double DistanceAlongMeters { get; set; }

    /// <summary>
    /// Direction of travel across the line as a compass bearing in degrees: 0 is north, 90 east,
    /// measured clockwise. A line marker is drawn perpendicular to this.
    /// </summary>
    [JsonPropertyName("hdg")]
    [Key(5)]
    public double HeadingDegrees { get; set; }
}

/// <summary>
/// A <see cref="TrackMap"/> prepared for drawing: the same learned centerline, plus a planar
/// projection, the map's extent, and the start/finish line resolved to a position.
///
/// <para>
/// <see cref="TrackMapRenderPoint.X"/> and <see cref="TrackMapRenderPoint.Y"/> are meters in a
/// flat-earth frame whose origin is the map's north-west corner, with Y increasing downward. The
/// frame is uniformly scaled - one meter east and one meter south are the same number of units - so
/// the track keeps its true shape at any zoom. A consumer fits it to a viewport with a single
/// scale factor:
/// </para>
/// <code>
/// var scale = Math.Min(viewportWidth / map.Bounds.WidthMeters, viewportHeight / map.Bounds.HeightMeters);
/// // then, per point: (point.X * scale, point.Y * scale)
/// </code>
/// <para>
/// One of the two extents can legitimately be zero - a track running due north-south has no width -
/// which leaves that ratio infinite and the other one deciding the scale, as it should. Both are
/// never zero: a map with no extent at all is not projected in the first place.
/// </para>
/// <para>
/// Latitude and longitude are carried alongside for consumers drawing over a real map instead, and
/// <see cref="Points"/> stays in path order, so it can be stroked as a polyline directly. The path
/// is closed: the last point connects back to the first, and that closing segment is not repeated
/// in the list.
/// </para>
/// </summary>
[MessagePackObject]
public class TrackMapRender
{
    /// <summary>RedMist event the map was learned for.</summary>
    [JsonPropertyName("eid")]
    [Key(0)]
    public int EventId { get; set; }

    /// <summary>Session the map was learned from.</summary>
    [JsonPropertyName("sid")]
    [Key(1)]
    public int SessionId { get; set; }

    /// <summary>Name of the track, when the event records one.</summary>
    [JsonPropertyName("track")]
    [Key(2)]
    public string? TrackName { get; set; }

    /// <summary>
    /// Ordered centerline points around the lap, starting at the path origin. See the class remarks
    /// for the coordinate frame.
    /// </summary>
    [JsonPropertyName("pts")]
    [Key(3)]
    public List<TrackMapRenderPoint> Points { get; set; } = [];

    /// <summary>The map's extent and the size of its planar frame.</summary>
    [JsonPropertyName("bounds")]
    [Key(4)]
    public TrackMapBounds Bounds { get; set; } = new();

    /// <summary>
    /// Lap length in meters, including the closing segment back to the first point.
    /// </summary>
    [JsonPropertyName("len")]
    [Key(5)]
    public double LengthMeters { get; set; }

    /// <summary>
    /// Where the start/finish line sits, or null when the map has not been calibrated yet. A map
    /// without it is still drawable - the outline is complete - it just cannot be marked or
    /// oriented, which is why this is null rather than a guess at the path origin.
    /// </summary>
    [JsonPropertyName("sf")]
    [Key(6)]
    public TrackMapStartFinish? StartFinish { get; set; }

    /// <summary>When the underlying map was learned, in UTC.</summary>
    [JsonPropertyName("built")]
    [Key(7)]
    public DateTime BuiltUtc { get; set; }

    /// <summary>
    /// Schema/build version of the underlying <see cref="TrackMap"/>, for a consumer that needs to
    /// tell one map format from another. It does not change when a map is relearned - compare
    /// <see cref="BuiltUtc"/> to spot that.
    /// </summary>
    [JsonPropertyName("ver")]
    [Key(8)]
    public int Version { get; set; }
}
