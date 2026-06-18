using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// Represents the configuration for an event, including scheduling, track details, and broadcast settings.
/// </summary>
[MessagePackObject]
public class Event
{
    /// <summary>
    /// Gets or sets the unique identifier for this event.
    /// </summary>
    [Key(0)]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the organization that owns this event.
    /// </summary>
    [Key(1)]
    [Required]
    public int OrganizationId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the event.
    /// Maximum length: 512 characters.
    /// </summary>
    [Key(2)]
    [MaxLength(512)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the start date and time of the event.
    /// </summary>
    [Key(3)]
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Gets or sets the end date and time of the event.
    /// </summary>
    [Key(4)]
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether this event is active, 
    /// i.e. is selected in the Relay as the event to send data to.
    /// </summary>
    [Key(5)]
    public bool IsActive { get; set; }
  
    /// <summary>
    /// Gets or sets a value indicating whether this event is currently live.
    /// </summary>
    [Key(6)]
    public bool IsLive { get; set; }
 
    /// <summary>
    /// Gets or sets the URL for event-specific information or registration.
    /// Maximum length: 512 characters.
    /// </summary>
    [Key(7)]
    [MaxLength(512)]
    public string EventUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the schedule for this event.
    /// </summary>
    [Key(8)]
    public EventSchedule Schedule { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether source data logging is enabled for this event.
    /// This will log all raw data received from the timing system.
    /// Default is <c>true</c>.
    /// </summary>
    [Key(9)]
    public bool EnableSourceDataLogging { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the name of the track where this event is held.
    /// Maximum length: 128 characters.
    /// </summary>
    [Key(10)]
    [MaxLength(128)]
    public string TrackName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the course configuration or layout variant being used.
    /// Maximum length: 64 characters.
    /// </summary>
    [Key(11)]
    [MaxLength(64)]
    public string CourseConfiguration { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the track distance or length.
    /// Maximum length: 20 characters.
    /// </summary>
    [Key(12)]
    [MaxLength(20)]
    public string Distance { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the broadcaster configuration for this event.
    /// </summary>
    [Key(13)]
    public BroadcasterConfig Broadcast { get; set; } = new BroadcasterConfig();
    
    /// <summary>
    /// Gets or sets the list of loop metadata for timing loops used in this event.
    /// </summary>
    [Key(14)]
    public List<LoopMetadata> LoopsMetadata { get; set; } = [];
    
    /// <summary>
    /// Gets or sets a value indicating whether this event has been deleted.
    /// Default is <c>false</c>.
    /// </summary>
    [Key(15)]
    [Required]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Indicates whether this is a real event or a simulation/replay/test event.
    /// </summary>
    [MessagePack.Key(16)]
    [Required]
    public bool IsSimulation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this session is historical or archived.
    /// </summary>
    [MessagePack.Key(17)]
    [Required]
    public bool IsArchived { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this event is private and requires an access code to view.
    /// </summary>
    [MessagePack.Key(18)]
    [Required]
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the event name should be hidden from public responses.
    /// When true, listings and event details return an empty name; consumers should display their own placeholder.
    /// </summary>
    [MessagePack.Key(19)]
    [Required]
    public bool HideName { get; set; }

    /// <summary>
    /// Gets or sets the 1-7 digit numeric access code required to view a private event.
    /// Null when <see cref="IsPrivate"/> is false.
    /// </summary>
    [MessagePack.Key(20)]
    [MaxLength(7)]
    public string? AccessCode { get; set; }

    /// <summary>
    /// Gets or sets how this event's timing data is produced. Defaults to <see cref="TimingSource.Relay"/>.
    /// </summary>
    [MessagePack.Key(21)]
    [Required]
    public TimingSource TimingSource { get; set; }

    /// <summary>
    /// Gets or sets opaque, source-specific configuration for an external timing source.
    /// Interpreted only by the external source service; null for relay events.
    /// </summary>
    [MessagePack.Key(22)]
    public string? ExternalConfig { get; set; }
}
