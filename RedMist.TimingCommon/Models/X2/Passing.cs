using MessagePack;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.X2;

/// <summary>
/// Represents an X2 timing loop passing event, i.e a car went over a loop.
/// </summary>
[MessagePackObject]
public class Passing
{
    [JsonPropertyName("o")]
    [Key(0)]
    public int OrganizationId { get; set; }
    [JsonPropertyName("e")]
    [Key(1)]
    public int EventId { get; set; }
    [JsonPropertyName("i")]
    [Key(2)]
    public uint Id { get; set; }
    [JsonPropertyName("l")]
    [Key(3)]
    public uint LoopId { get; set; }
    [JsonPropertyName("t")]
    [Key(4)]
    public uint TransponderId { get; set; }
    [JsonPropertyName("ts")]
    [Key(5)]
    public uint TransponderShortId { get; set; }
    [JsonPropertyName("h")]
    [Key(6)]
    public ushort Hits { get; set; }
    [JsonPropertyName("tl")]
    [Key(7)]
    public DateTime TimestampLocal { get; set; }
    [JsonPropertyName("tu")]
    [Key(8)]
    public DateTime TimestampUtc { get; set; }
    [JsonPropertyName("p")]
    [Key(9)]
    public bool IsInPit { get; set; }
    [JsonPropertyName("r")]
    [Key(10)]
    public bool IsResend { get; set; }
}
