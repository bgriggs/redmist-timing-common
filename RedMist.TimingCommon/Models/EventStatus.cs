using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class EventStatus
{
    [JsonPropertyName("eid")]
    public string? EventId { get; set; }

    [JsonPropertyName("f")]
    public Flags Flag { get; set; }

    [JsonPropertyName("ltg")]
    public int LapsToGo { get; set; }

    [JsonPropertyName("ttg")]
    public string TimeToGo { get; set; } = string.Empty;

    [JsonPropertyName("tt")]
    public string TotalTime { get; set; } = string.Empty;
}
