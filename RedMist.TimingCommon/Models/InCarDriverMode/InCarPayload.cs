using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarDriverMode;

/// <summary>
/// Update for in-car driver mode payload.
/// </summary>
public class InCarPayload
{
    [JsonPropertyName("f")]
    public Flags Flags { get; set; } = Flags.Unknown;
    [JsonPropertyName("c")]
    public List<CarStatus> Cars { get; set; } = [];
}
