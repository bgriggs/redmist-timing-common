using MessagePack;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarDriverMode;

/// <summary>
/// Update for in-car driver mode payload.
/// </summary>
[MessagePackObject]
public class InCarPayload
{
    [JsonPropertyName("n")]
    [MessagePack.Key(0)]
    public string CarNumber { get; set; } = string.Empty;
    [JsonPropertyName("p")]
    [MessagePack.Key(1)]
    public string PositionInClass { get; set; } = string.Empty;
    [JsonPropertyName("o")]
    [MessagePack.Key(2)]
    public string PositionOverall { get; set; } = string.Empty;
    [JsonPropertyName("f")]
    [MessagePack.Key(3)]
    public Flags Flag { get; set; } = Flags.Unknown;
    [JsonPropertyName("c")]
    [MessagePack.Key(4)]
    public List<CarStatus?> Cars { get; set; } = [];
}
