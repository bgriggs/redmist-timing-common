using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class ControlLogEntry
{
    [JsonPropertyName("o")]
    public int OrderId { get; set; }
    [JsonPropertyName("t")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("cor")]
    public string Corner { get; set; } = string.Empty;
    [JsonPropertyName("c1")]
    public string Car1 { get; set; } = string.Empty;
    [JsonPropertyName("c1h")]
    public bool IsCar1Highlighted { get; set; }
    [JsonPropertyName("c2")]
    public string Car2 { get; set; } = string.Empty;
    [JsonPropertyName("c2h")]
    public bool IsCar2Highlighted { get; set; }
    [JsonPropertyName("n")]
    public string Note { get; set; } = string.Empty;
    [JsonPropertyName("s")]
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("a")]
    public string PenalityAction { get; set; } = string.Empty;
    [JsonPropertyName("on")]
    public string OtherNotes { get; set; } = string.Empty;
}
