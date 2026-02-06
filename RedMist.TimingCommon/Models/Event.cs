using MessagePack;
using RedMist.TimingCommon.Models.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents a racing event with complete details including sessions, schedule, track information, and organization details.
/// </summary>
[MessagePackObject]
public class Event
{
    /// <summary>
    /// Gets or sets the unique identifier for this event.
    /// </summary>
    [JsonPropertyName("e")]
    [Key(0)]
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the name of the event.
    /// Maximum length: 512 characters.
    /// </summary>
    [JsonPropertyName("n")]
    [Key(1)]
    [MaxLength(512)]
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the event as a formatted string.
    /// Maximum length: 80 characters.
    /// </summary>
    [JsonPropertyName("d")]
    [Key(2)]
    [MaxLength(80)]
    public string EventDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL for event-specific information or registration.
    /// Maximum length: 2083 characters.
    /// </summary>
    [JsonPropertyName("u")]
    [Key(3)]
    [MaxLength(2083)]
    public string EventUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the array of sessions within this event.
    /// Maximum number of sessions: 50.
    /// </summary>
    [JsonPropertyName("s")]
    [Key(4)]
    [MaxLength(50)]
    public Session[] Sessions { get; set; } = [];

    /// <summary>
    /// Gets or sets the name of the organization hosting this event.
    /// Maximum length: 255 characters.
    /// </summary>
    [JsonPropertyName("on")]
    [Key(5)]
    [MaxLength(255)]
    public string OrganizationName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the website URL of the organization hosting this event, or <c>null</c> if not available.
    /// Maximum length: 2083 characters.
    /// </summary>
    [JsonPropertyName("ow")]
    [Key(6)]
    [MaxLength(2083)]
    public string? OrganizationWebsite { get; set; }

    /// <summary>
    /// Gets or sets the organization's logo as a byte array, or <c>null</c> if not available.
    /// Maximum size: 5242880 bytes (5 MB).
    /// </summary>
    [JsonPropertyName("l")]
    [Key(7)]
    [MaxLength(5242880)] // 5MB
    public byte[]? OrganizationLogo { get; set; }

    /// <summary>
    /// Gets or sets the event schedule, or <c>null</c> if no schedule is available.
    /// </summary>
    [JsonPropertyName("sc")]
    [Key(8)]
    public EventSchedule? Schedule { get; set; }

    /// <summary>
    /// Gets or sets the name of the track where this event is held.
    /// Maximum length: 128 characters.
    /// </summary>
    [JsonPropertyName("t")]
    [Key(9)]
    [MaxLength(128)]
    public string TrackName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the course configuration or layout variant being used.
    /// Maximum length: 60 characters.
    /// </summary>
    [JsonPropertyName("cc")]
    [Key(10)]
    [MaxLength(60)]
    public string CourseConfiguration { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the track distance or length.
    /// Maximum length: 20 characters.
    /// </summary>
    [JsonPropertyName("di")]
    [Key(11)]
    [MaxLength(20)]
    public string Distance { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the broadcaster configuration for this event, or <c>null</c> if no broadcast is configured.
    /// </summary>
    [JsonPropertyName("b")]
    [Key(12)]
    public BroadcasterConfig? Broadcast { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this event has a control log available.
    /// </summary>
    [JsonPropertyName("hc")]
    [Key(13)]
    public bool HasControlLog { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this event is currently live.
    /// </summary>
    [JsonPropertyName("il")]
    [Key(14)]
    public bool IsLive { get; set; }
    /// <summary>
    /// Indicates whether this is a real event or a simulation/replay/test event.
    /// </summary>
    [MessagePack.Key(15)]
    public bool IsSimulation { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this session is historical or archived.
    /// </summary>
    [MessagePack.Key(16)]
    public bool IsArchived { get; set; }
    /// <summary>
    /// Organization that owns the event.
    /// </summary>
    [MessagePack.Key(17)]
    public int OrganizationId { get; set; }
}
