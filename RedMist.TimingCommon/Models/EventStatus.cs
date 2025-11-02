using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

[Obsolete("Use RedMist.TimingCommon.Models.SessionState instead")]
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
    public string LocalTimeOfDay { get; set; } = string.Empty;
    [JsonPropertyName("rt")]
    public string RunningRaceTime { get; set; } = string.Empty;
    [JsonPropertyName("pq")]
    public bool IsPracticeQualifying { get; set; }
}
