using MessagePack;

namespace RedMist.TimingCommon.Models.InCarVideo;

/// <summary>
/// Status of a car's video system.
/// </summary>
[MessagePackObject]
public class VideoStatus
{
    /// <summary>
    /// Brand/model of the car's video system.
    /// </summary>
    [Key(0)]
    public VideoSystemType VideoSystemType { get; set; }
    /// <summary>
    /// Gets or sets the destination configuration consuming video.
    /// </summary>
    [Key(1)]
    public VideoDestination VideoDestination { get; set; } = new VideoDestination();
}
