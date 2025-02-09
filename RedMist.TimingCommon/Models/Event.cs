using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class Event
{
    [JsonPropertyName("eid")]
    public int EventId { get; set; }
    [JsonPropertyName("n")]
    public string EventName { get; set; } = string.Empty;
    [JsonPropertyName("d")]
    public string EventDate { get; set; } = string.Empty;
}
