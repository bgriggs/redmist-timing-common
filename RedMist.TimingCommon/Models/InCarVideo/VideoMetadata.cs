using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.InCarVideo;

/// <summary>
/// Details about the in-car video.
/// </summary>
[MessagePackObject]
public class VideoMetadata
{
    /// <summary>
    /// Associated event ID.
    /// </summary>
    [JsonPropertyName("eid")]
    [Key(0)]
    public int EventId { get; set; }

    /// <summary>
    /// Car number if available. Specific to the event.
    /// </summary>
    [JsonPropertyName("cn")]
    [Key(1)]
    [MaxLength(5)]
    public string CarNumber { get; set; } = string.Empty;

    /// <summary>
    /// Car's transponder ID. Not specific to the event.
    /// </summary>
    [JsonPropertyName("tp")]
    [Key(2)]
    public uint TransponderId { get; set; }

    /// <summary>
    /// Indicates if the transponder ID should be used instead of the car number.
    /// </summary>
    [JsonPropertyName("utp")]
    [Key(3)]
    public bool UseTransponderId { get; set; } = false;

    /// <summary>
    /// Source of the video, e.g., Sentinel.
    /// </summary>
    [JsonPropertyName("t")]
    [Key(4)]
    public VideoSystemType SystemType { get; set; } = VideoSystemType.None;

    /// <summary>
    /// Destination details for the video stream.
    /// </summary>
    [JsonPropertyName("d")]
    [Key(5)]
    [MaxLength(5)]
    public List<VideoDestination> Destinations { get; set; } = [];

    /// <summary>
    /// Indicates if the video is currently live.
    /// </summary>
    [JsonPropertyName("l")]
    [Key(6)]
    public bool IsLive { get; set; } = false;

    /// <summary>
    /// DEPRECATED. Name of the driver if available.
    /// </summary>
    [JsonPropertyName("dn")]
    [Key(7)]
    [MaxLength(25)]
    [Obsolete("DriverName is deprecated and will be removed in future versions.")]
    public string DriverName { get; set; } = string.Empty;
}
