using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class FlagDuration
{
    [JsonPropertyName("f")]
    public Flags Flags { get; set; }
    [JsonPropertyName("s")]
    public DateTime StartTime { get; set; }
    [JsonPropertyName("e")]
    public DateTime? EndTime { get; set; }
}
