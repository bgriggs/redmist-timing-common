using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarVideo;

public class VideoDestination
{
    [JsonPropertyName("t")]
    public VideoDestinationType Type { get; set; } = VideoDestinationType.None;
    [JsonPropertyName("u")]
    public string Url { get; set; } = string.Empty;
    [JsonPropertyName("h")]
    public string HostName { get; set; } = string.Empty;
    [JsonPropertyName("p")]
    public ushort Port { get; set; } = 0;
    [JsonPropertyName("pa")]
    public string Parameters { get; set; } = string.Empty;
}
