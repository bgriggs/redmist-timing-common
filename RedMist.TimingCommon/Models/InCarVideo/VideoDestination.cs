using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.InCarVideo;

/// <summary>
/// Location where a car's video is being sent or can be accessed.
/// </summary>
[MessagePackObject]
public class VideoDestination
{
    /// <summary>
    /// Destination such as YouTube.
    /// </summary>
    [JsonPropertyName("t")]
    [Key(0)]
    public VideoDestinationType Type { get; set; } = VideoDestinationType.None;
    /// <summary>
    /// Destination's URL.
    /// </summary>
    [JsonPropertyName("u")]
    [Key(1)]
    [MaxLength(2083)] 
    public string Url { get; set; } = string.Empty;
    /// <summary>
    /// Destination URL's host name or IP address.
    /// </summary>
    [JsonPropertyName("h")]
    [Key(2)]
    [MaxLength(2083)]
    public string HostName { get; set; } = string.Empty;
    /// <summary>
    /// Destination's port.
    /// </summary>
    [JsonPropertyName("p")]
    [Key(3)]
    public ushort Port { get; set; } = 0;
    /// <summary>
    /// Gets or sets the parameters associated with the operation.
    /// </summary>
    [JsonPropertyName("pa")]
    [Key(4)]
    [MaxLength(30)]
    public string Parameters { get; set; } = string.Empty;
}
