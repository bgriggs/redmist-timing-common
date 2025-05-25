using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarVideo;

/// <summary>
/// Details about the in-car video.
/// </summary>
public class VideoMetadata
{
    /// <summary>
    /// Associated event ID.
    /// </summary>
    [JsonPropertyName("eid")]
    public int EventId { get; set; }

    /// <summary>
    /// Car number if available. Specific to the event.
    /// </summary>
    [JsonPropertyName("cn")]
    public string CarNumber { get; set; } = string.Empty;

    /// <summary>
    /// Car's transponder ID. Not specific to the event.
    /// </summary>
    [JsonPropertyName("tp")]
    public uint TransponderId { get; set; }

    /// <summary>
    /// Indicates if the transponder ID should be used instead of the car number.
    /// </summary>
    [JsonPropertyName("utp")]
    public bool UseTransponderId { get; set; } = false;

    /// <summary>
    /// Source of the video, e.g., Sentinel.
    /// </summary>
    [JsonPropertyName("t")]
    public VideoSystemType SystemType { get; set; } = VideoSystemType.None;

    /// <summary>
    /// Destination details for the video stream.
    /// </summary>
    [JsonPropertyName("d")]
    public List<VideoDestination> Destinations { get; set; } = [];

    /// <summary>
    /// Indicates if the video is currently live.
    /// </summary>
    [JsonPropertyName("l")]
    public bool IsLive { get; set; } = false;
}
