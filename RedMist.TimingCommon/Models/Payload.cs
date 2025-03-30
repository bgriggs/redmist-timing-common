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
    public List<EventEntry> EventEntries { get; set; } = [];
    [JsonPropertyName("eeu")]
    public List<EventEntry> EventEntryUpdates { get; set; } = [];
    [JsonPropertyName("cps")]
    public List<CarPosition> CarPositions { get; set; } = [];
    [JsonPropertyName("cpu")]
    public List<CarPosition> CarPositionUpdates { get; set; } = [];
    [JsonPropertyName("fd")]
    public List<FlagDuration> FlagDurations { get; set; } = [];
    [JsonPropertyName("r")]
    public bool IsReset { get; set; }
}
