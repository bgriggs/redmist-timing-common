using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class CarControlLogs
{
    [JsonPropertyName("cn")]
    public string CarNumber { get; set; } = string.Empty;
    [JsonPropertyName("entries")]
    public List<ControlLogEntry> ControlLogEntries { get; set; } = [];
}
