using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;


public class Payload
{
    [JsonPropertyName("e")]
    public int EventId { get; set; }
    [JsonPropertyName("n")]
    public string? EventName { get; set; }
    [JsonPropertyName("es")]
    public EventStatus? EventStatus { get; set; }
    [JsonPropertyName("ee")]
    public List<EventEntry> EventEntries { get; set; } = new();
    [JsonPropertyName("eeu")]
    public List<EventEntry> EventEntryUpdates { get; set; } = new();
    [JsonPropertyName("cps")]
    public List<CarPosition> CarPositions { get; set; } = new();
    [JsonPropertyName("cpu")]
    public List<CarPosition> CarPositionUpdates { get; set; } = new();
    [JsonPropertyName("r")]
    public bool IsReset { get; set; }
}
