using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class EventEntries
{
    [JsonPropertyName("no")]
    public string Number { get; set; } = string.Empty;
    
    [JsonPropertyName("nm")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("t")]
    public string Team { get; set; } = string.Empty;

    [JsonPropertyName("c")]
    public string Class { get; set; } = string.Empty;
}
