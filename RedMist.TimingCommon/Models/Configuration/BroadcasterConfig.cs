using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.Configuration;

public class BroadcasterConfig
{
    [JsonPropertyName("c")]
    public string CompanyName { get; set; } = string.Empty;
    [JsonPropertyName("u")]
    public string Url { get; set; } = string.Empty;
}
