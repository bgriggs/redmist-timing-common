using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.X2;

public class Passing
{
    [JsonPropertyName("i")]
    public uint Id { get; set; }
    [JsonPropertyName("l")]
    public uint LoopId { get; set; }
    [JsonPropertyName("t")]
    public uint TransponderId { get; set; }
    [JsonPropertyName("ts")]
    public uint TransponderShortId { get; set; }
    [JsonPropertyName("h")]
    public ushort Hits { get; set; }
    [JsonPropertyName("tl")]
    public DateTime TimestampLocal { get; set; }
    [JsonPropertyName("tu")]
    public DateTime TimestampUtc { get; set; }
    [JsonPropertyName("p")]
    public bool IsInPit { get; set; }
    [JsonPropertyName("r")]
    public bool IsResend { get; set; }
}
