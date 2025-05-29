using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarDriverMode;

/// <summary>
/// Represents the current status of a car, including its position and lap metadata like gap and diff.
/// </summary>
public class CarStatus
{
    /// <summary>
    /// Car number.
    /// </summary>
    [JsonPropertyName("n")]
    public string Number { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the class identifier.
    /// </summary>
    [JsonPropertyName("c")]
    public string Class { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    [JsonPropertyName("t")]
    public string Team { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the time of the last completed lap, represented as a string mm:ss.ttt.
    /// </summary>
    [JsonPropertyName("l")]
    public string LastLap { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the gain or loss of the last lap.
    /// </summary>
    [JsonPropertyName("gl")]
    public string GainLoss { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the gap to the next car.
    /// </summary>
    [JsonPropertyName("g")]
    public string Gap { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the type of the car if available from competitor metadata.
    /// </summary>
    [JsonPropertyName("ct")]
    public string CarType { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the driver if available.
    /// </summary>
    [JsonPropertyName("d")]
    public string Driver { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the unique identifier for the transponder.
    /// </summary>
    [JsonPropertyName("id")]
    public uint TransponderId { get; set; }
    /// <summary>
    /// Gets or sets the URL of the car image if available.
    /// </summary>
    [JsonPropertyName("i")]
    public string CarImageUrl { get; set; } = string.Empty;
}
