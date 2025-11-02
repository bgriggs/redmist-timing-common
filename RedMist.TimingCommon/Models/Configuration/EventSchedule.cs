using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// Represents a schedule for an event, containing a collection of scheduled entries.
/// </summary>
[MessagePackObject]
public class EventSchedule
{
    /// <summary>
    /// Gets or sets the name of the schedule.
    /// Maximum length: 128 characters.
    /// </summary>
    [JsonPropertyName("n")]
    [Key(0)]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of schedule entries for the event.
    /// Maximum number of entries: 25.
    /// </summary>
    [JsonPropertyName("s")]
    [Key(1)]
    [MaxLength(25)]
    public List<EventScheduleEntry> Entries { get; set; } = [];
}
