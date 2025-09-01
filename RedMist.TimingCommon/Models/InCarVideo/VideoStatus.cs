using ProtoBuf;

namespace RedMist.TimingCommon.Models.InCarVideo;

/// <summary>
/// Status of a car's video system.
/// </summary>
[ProtoContract]
public class VideoStatus
{
    /// <summary>
    /// Brand/model of the car's video system.
    /// </summary>
    [ProtoMember(1)]
    public VideoSystemType VideoSystemType { get; set; }
    /// <summary>
    /// Gets or sets the destination configuration consuming video.
    /// </summary>
    [ProtoMember(2)]
    public VideoDestination VideoDestination { get; set; } = new VideoDestination();
}
