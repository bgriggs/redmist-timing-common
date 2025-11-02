using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarDriverMode;

/// <summary>
/// Update for in-car driver mode payload.
/// </summary>
[MessagePackObject]
public class InCarPayload
{
    /// <summary>
    /// Gets or sets the car number for the in-car view.
    /// Maximum length: 5 characters.
    /// </summary>
    [JsonPropertyName("n")]
    [MessagePack.Key(0)]
    [MaxLength(5)]
    public string CarNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position in class as a string.
    /// Maximum length: 5 characters.
    /// </summary>
    [JsonPropertyName("p")]
    [MessagePack.Key(1)]
    [MaxLength(5)]
    public string PositionInClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the overall position as a string.
    /// Maximum length: 5 characters.
    /// </summary>
    [JsonPropertyName("o")]
    [MessagePack.Key(2)]
    [MaxLength(5)]
    public string PositionOverall { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current flag status for the session.
    /// </summary>
    [JsonPropertyName("f")]
    [MessagePack.Key(3)]
    public Flags Flag { get; set; } = Flags.Unknown;

    /// <summary>
    /// Gets or sets the list of car statuses for cars around the in-car view car.
    /// Expected 2 cars ahead and 1 car behind. Null is used if no car in that position,
    /// such as when the in-car view car is leading or at the back of the field.
    /// </summary>
    [JsonPropertyName("c")]
    [MessagePack.Key(4)]
    [MaxLength(5)]
    public List<CarStatus?> Cars { get; set; } = [];
}
