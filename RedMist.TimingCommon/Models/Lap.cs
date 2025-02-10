using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class Lap
{
    [JsonPropertyName("ts")]
    public string Timestamp { get; set; } = string.Empty;
    [JsonPropertyName("l")]
    public int LapNumber { get; set; }
    [JsonPropertyName("lt")]
    public string TimeMs { get; set; } = string.Empty;
    [JsonPropertyName("f")]
    public bool IsFastest { get; set; }
    [JsonPropertyName("p")]
    public bool IsPit { get; set; }
    [JsonPropertyName("lsp")]
    public int LapsSinceLastPit { get; set; }
    [JsonPropertyName("cp")]
    public int ClassPosition { get; set; }
    [JsonPropertyName("op")]
    public int OverallPosition { get; set; }
    [JsonPropertyName("lp")]
    public string LapPercentile { get; set; } = string.Empty;
}
