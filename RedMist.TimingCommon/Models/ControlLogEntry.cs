using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents a control log entry for tracking race control actions and incidents.
/// </summary>
[MessagePackObject]
public class ControlLogEntry
{
    /// <summary>
    /// Gets or sets the order identifier for this log entry.
    /// </summary>
    [JsonPropertyName("o")]
    [Key(0)]
    public int OrderId { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when this log entry was created.
    /// </summary>
    [JsonPropertyName("t")]
    [Key(1)]
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Gets or sets the corner or location where the incident occurred.
    /// Maximum length: 128 characters.
    /// </summary>
    [JsonPropertyName("cor")]
    [Key(2)]
    [MaxLength(128)]
    public string Corner { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the first car number involved in the incident.
    /// Maximum length: 5 characters.
    /// </summary>
    [JsonPropertyName("c1")]
    [Key(3)]
    [MaxLength(5)]
    public string Car1 { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the first car is highlighted, indicating who to apply a penality or at fault.
    /// </summary>
    [JsonPropertyName("c1h")]
    [Key(4)]
    public bool IsCar1Highlighted { get; set; }
    
    /// <summary>
    /// Gets or sets the second car number involved in the incident.
    /// Maximum length: 5 characters.
    /// </summary>
    [JsonPropertyName("c2")]
    [Key(5)]
    [MaxLength(5)]
    public string Car2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the second car is highlighted, indicating who to apply a penality or at fault.
    /// </summary>
    [JsonPropertyName("c2h")]
    [Key(6)]
    public bool IsCar2Highlighted { get; set; }
    
    /// <summary>
    /// Gets or sets notes about the incident or action.
    /// Maximum length: 128 characters.
    /// </summary>
    [JsonPropertyName("n")]
    [Key(7)]
    [MaxLength(128)]
    public string Note { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the status of the incident or action.
    /// Maximum length: 30 characters.
    /// </summary>
    [JsonPropertyName("s")]
    [Key(8)]
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the penalty action taken or applied.
    /// Maximum length: 30 characters.
    /// </summary>
    [JsonPropertyName("a")]
    [Key(9)]
    [MaxLength(30)]
    public string PenalityAction { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets additional notes or remarks.
    /// Maximum length: 128 characters.
    /// </summary>
    [JsonPropertyName("on")]
    [Key(10)]
    [MaxLength(128)]
    public string OtherNotes { get; set; } = string.Empty;
}
