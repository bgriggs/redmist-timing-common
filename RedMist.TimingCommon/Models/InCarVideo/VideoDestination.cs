using ProtoBuf;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarVideo;

/// <summary>
/// Location where a car's video is being sent or can be accessed.
/// </summary>
[ProtoContract]
public class VideoDestination
{
    /// <summary>
    /// Destination such as YouTube.
    /// </summary>
    [JsonPropertyName("t")]
    [ProtoMember(1)]
    public VideoDestinationType Type { get; set; } = VideoDestinationType.None;
    /// <summary>
    /// Destination's URL.
    /// </summary>
    [JsonPropertyName("u")]
    [ProtoMember(2)]
    public string Url { get; set; } = string.Empty;
    /// <summary>
    /// Destination URL's host name or IP address.
    /// </summary>
    [JsonPropertyName("h")]
    [ProtoMember(3)]
    public string HostName { get; set; } = string.Empty;
    /// <summary>
    /// Destination's port.
    /// </summary>
    [JsonPropertyName("p")]
    [ProtoMember(4)]
    public ushort Port { get; set; } = 0;
    /// <summary>
    /// Gets or sets the parameters associated with the operation.
    /// </summary>
    [JsonPropertyName("pa")]
    [ProtoMember(5)]
    public string Parameters { get; set; } = string.Empty;
}
