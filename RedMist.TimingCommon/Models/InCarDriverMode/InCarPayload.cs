using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarDriverMode;

/// <summary>
/// Update for in-car driver mode payload.
/// </summary>
public class InCarPayload
{
    [JsonPropertyName("n")]
    public string CarNumber { get; set; } = string.Empty;
    [JsonPropertyName("p")] 
    public string PositionInClass { get; set; } = string.Empty;
    [JsonPropertyName("o")] 
    public string PositionOverall { get; set; } = string.Empty;
    [JsonPropertyName("f")]
    public Flags Flag { get; set; } = Flags.Unknown;
    [JsonPropertyName("c")]
    public List<CarStatus?> Cars { get; set; } = [];
}
