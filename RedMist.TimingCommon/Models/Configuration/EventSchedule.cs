using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.Configuration;

public class EventSchedule
{
    [JsonPropertyName("n")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("s")]
    public List<EventScheduleEntry> Entries { get; set; } = new List<EventScheduleEntry>();
}
