using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.Configuration;

public class EventScheduleEntry
{
    [JsonPropertyName("d")]
    public DateTime DayOfEvent { get; set; }
    [JsonPropertyName("s")]
    public DateTime StartTime { get; set; }
    [JsonPropertyName("e")]
    public DateTime EndTime { get; set; }
    [JsonPropertyName("n")]
    public string Name { get; set; } = string.Empty;
}
