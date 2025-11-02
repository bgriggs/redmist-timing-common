using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents a collection of control log entries associated with a specific car.
/// </summary>
[MessagePackObject]
public class CarControlLogs
{
    /// <summary>
    /// Gets or sets the car number.
    /// Maximum length: 5 characters.
    /// </summary>
    [JsonPropertyName("cn")]
    [Key(0)]
    [MaxLength(5)]
    public string CarNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of control log entries for this car.
    /// Maximum number of entries: 2000.
    /// </summary>
    [JsonPropertyName("entries")]
    [Key(1)]
    [MaxLength(2000)]
    public List<ControlLogEntry> ControlLogEntries { get; set; } = [];
}
