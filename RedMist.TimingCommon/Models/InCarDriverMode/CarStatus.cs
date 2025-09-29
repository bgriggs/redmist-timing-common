using MessagePack;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models.InCarDriverMode;

/// <summary>
/// Represents the current status of a car, including its position and lap metadata like gap and diff.
/// </summary>
[MessagePackObject]
public class CarStatus
{
    /// <summary>
    /// Car number.
    /// </summary>
    [JsonPropertyName("n")]
    [MessagePack.Key(0)] 
    public string Number { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the class identifier.
    /// </summary>
    [JsonPropertyName("c")]
    [MessagePack.Key(1)]
    public string Class { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    [JsonPropertyName("t")]
    [MessagePack.Key(2)]
    public string Team { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the time of the last completed lap, represented as a string mm:ss.ttt.
    /// </summary>
    [JsonPropertyName("l")]
    [MessagePack.Key(3)]
    public string LastLap { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the gain or loss of the last lap.
    /// </summary>
    [JsonPropertyName("gl")]
    [MessagePack.Key(4)]
    public string GainLoss { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the gap to the next car.
    /// </summary>
    [JsonPropertyName("g")]
    [MessagePack.Key(5)]
    public string Gap { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the type of the car if available from competitor metadata.
    /// </summary>
    [JsonPropertyName("ct")]
    [MessagePack.Key(6)]
    public string CarType { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the name of the driver if available.
    /// </summary>
    [JsonPropertyName("d")]
    [MessagePack.Key(7)]
    public string Driver { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the unique identifier for the transponder.
    /// </summary>
    [JsonPropertyName("id")]
    [MessagePack.Key(8)]
    public uint TransponderId { get; set; }
    /// <summary>
    /// Gets or sets the URL of the car image if available.
    /// </summary>
    [JsonPropertyName("i")]
    [MessagePack.Key(9)]
    public string CarImageUrl { get; set; } = string.Empty;
}
