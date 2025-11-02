using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// Represents a single entry in an event schedule, defining a scheduled activity with start and end times.
/// </summary>
[MessagePackObject]
public class EventScheduleEntry
{
    /// <summary>
    /// Gets or sets the day of the event for this schedule entry.
    /// </summary>
    [JsonPropertyName("d")]
    [Key(0)]
    public DateTime DayOfEvent { get; set; }
    
    /// <summary>
    /// Gets or sets the start time of the scheduled activity.
    /// </summary>
    [JsonPropertyName("s")]
    [Key(1)]
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets the end time of the scheduled activity.
    /// </summary>
    [JsonPropertyName("e")]
    [Key(2)]
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// Gets or sets the name or description of the scheduled session, such as qualifying, race 1, etc.
    /// Maximum length: 128 characters.
    /// </summary>
    [JsonPropertyName("n")]
    [Key(3)]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;
}
