using MessagePack;
using RedMist.TimingCommon.Models.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents a summary of an event for display in event listings.
/// </summary>
[MessagePackObject]
public class EventListSummary
{
    /// <summary>
    /// Gets or sets the unique identifier for this event.
    /// </summary>
    [JsonPropertyName("eid")]
    [Key(0)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the organization hosting this event.
    /// </summary>
    [JsonPropertyName("oid")]
    [Key(1)]
    public int OrganizationId { get; set; }

    /// <summary>
    /// Gets or sets the name of the organization hosting this event.
    /// Maximum length: 255 characters.
    /// </summary>
    [JsonPropertyName("on")]
    [Key(2)]
    [MaxLength(255)]
    public string OrganizationName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the event.
    /// Maximum length: 512 characters.
    /// </summary>
    [JsonPropertyName("en")]
    [Key(3)]
    [MaxLength(512)]
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the event as a formatted string.
    /// Maximum length: 80 characters.
    /// </summary>
    [JsonPropertyName("ed")]
    [Key(4)]
    [MaxLength(80)]
    public string EventDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this event is currently live.
    /// </summary>
    [JsonPropertyName("l")]
    [Key(5)]
    public bool IsLive { get; set; }

    /// <summary>
    /// Gets or sets the name of the track where this event is held.
    /// Maximum length: 128 characters.
    /// </summary>
    [JsonPropertyName("t")]
    [Key(6)]
    [MaxLength(128)]
    public string TrackName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the schedule for this event, or <c>null</c> if no schedule is available.
    /// </summary>
    [JsonPropertyName("s")]
    [Key(7)]
    public EventSchedule? Schedule { get; set; }

    /// <summary>
    /// Indicates whether this is a real event or a simulation/replay/test event.
    /// </summary>
    [MessagePack.Key(8)]
    [JsonPropertyName("sim")]
    public bool IsSimulation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this session is historical or archived.
    /// </summary>
    [MessagePack.Key(9)]
    [JsonPropertyName("arch")]
    public bool IsArchived { get; set; }
}
