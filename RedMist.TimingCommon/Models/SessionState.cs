using MessagePack;
using System.ComponentModel.DataAnnotations;
using RedMist.TimingCommon.Attributes;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// The overall state of a race session at any given time. 
/// This includes data needed to fully represent the race.
/// </summary>
[MessagePackObject]
[GeneratePatch]
public class SessionState
{
    /// <summary>
    /// Redmist ID for the event.
    /// </summary>
    [MessagePack.Key(0)]
    public int EventId { get; set; }
    /// <summary>
    /// Event name as provided by the organizer.
    /// </summary>
    [MaxLength(512)]
    [MessagePack.Key(1)]
    public string EventName { get; set; } = string.Empty;
    /// <summary>
    /// Session, or run, is the current part of the event being timed such as in individual race, practice, or qualifying session.
    /// This is the ID provided by the timing system.
    /// </summary>
    [MessagePack.Key(2)]
    public int SessionId { get; set; }
    /// <summary>
    /// Session name as provided by the timing system.
    /// </summary>
    [MaxLength(40)]
    [MessagePack.Key(3)]
    public string SessionName { get; set; } = string.Empty;
    /// <summary>
    /// Optional number of laps to go if the race is lap based.
    /// </summary>
    [MessagePack.Key(4)]
    public int LapsToGo { get; set; }
    /// <summary>
    /// Optional time of the session remaining if the event is time based.
    /// Format is HH:mm:ss.
    /// </summary>
    /// <remarks>
    /// Orbits has been seen to have a negative seconds value preceding the start of a race, i.e. HH:mm:-ss
    /// </remarks>
    [MaxLength(10)]
    [MessagePack.Key(5)]
    public string TimeToGo { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the local time of day as a string. This is 24 hour format HH:mm:ss.
    /// </summary>
    [MaxLength(8)]
    [MessagePack.Key(6)]
    public string LocalTimeOfDay { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the amount of time a race has been running. Format is HH:mm:ss.
    /// </summary>
    [MaxLength(8)]
    [MessagePack.Key(7)]
    public string RunningRaceTime { get; set; } = string.Empty;
    /// <summary>
    /// Whether the current session is a practice qualifying session. This is is not guaranteed 
    /// to be accurate and comes from the session name.
    /// </summary>
    [MessagePack.Key(8)]
    public bool IsPracticeQualifying { get; set; }
    /// <summary>
    /// When the session started according to the the first time data was received from the timing system for the session.
    /// </summary>
    [MessagePack.Key(9)]
    public DateTime SessionStartTime { get; set; }
    /// <summary>
    /// When the session ended according to the timing system change to another session or timeout of data being received, such as at the end of the race.
    /// </summary>
    [MessagePack.Key(10)]
    public DateTime? SessionEndTime { get; set; }
    /// <summary>
    /// The event's local time zone offset from UTC in hours as indicated by the organizer's system time.
    /// </summary>
    [MessagePack.Key(11)]
    public double LocalTimeZoneOffset { get; set; }
    /// <summary>
    /// Indicates whether the organizer is connected and sending data for the event.
    /// </summary>
    [MessagePack.Key(12)]
    public bool IsLive { get; set; }
    /// <summary>
    /// These are the signed up entries indicated by the timing system.
    /// They may not be the same as the cars that actually participated in the event.
    /// </summary>
    [MessagePack.Key(13)]
    [MaxLength(500)]
    public List<EventEntry> EventEntries { get; set; } = [];
    /// <summary>
    /// Represents the current position and status of each car in the event.
    /// </summary>
    [MessagePack.Key(14)]
    [MaxLength(500)]
    public List<CarPosition> CarPositions { get; set; } = [];
    /// <summary>
    /// Current flag state for the event.
    /// </summary>
    [MessagePack.Key(15)]
    public Flags CurrentFlag { get; set; }
    /// <summary>
    /// Flag states for the event, including durations.
    /// </summary>
    [MessagePack.Key(16)]
    [MaxLength(100)]
    public List<FlagDuration> FlagDurations { get; set; } = [];
    /// <summary>
    /// Amount of time in milliseconds the session has been under green. Available with Multiloop timing systems.
    /// </summary>
    [MessagePack.Key(17)]
    public int? GreenTimeMs { get; set; }
    /// <summary>
    /// Number of laps the session has been under green. Available with Multiloop timing systems.
    /// </summary>
    [MessagePack.Key(18)]
    public int? GreenLaps { get; set; }
    /// <summary>
    /// Amount of time in milliseconds the session has been under yellow. Available with Multiloop timing systems.
    /// </summary>
    [MessagePack.Key(19)]
    public int? YellowTimeMs { get; set; }
    /// <summary>
    /// Number of laps the session has been under yellow. Available with Multiloop timing systems.
    /// </summary>
    [MessagePack.Key(20)]
    public int? YellowLaps { get; set; }
    /// <summary>
    /// Number of yellow flag periods in the session. Available with Multiloop timing systems.
    /// </summary>
    [MessagePack.Key(21)]
    public int? NumberOfYellows { get; set; }
    /// <summary>
    /// Amount of time in milliseconds the session has been under red. Available with Multiloop timing systems.
    /// </summary>
    [MessagePack.Key(22)]
    public int? RedTimeMs { get; set; }
    /// <summary>
    /// Gets or sets the average speed of the race, expressed as a string, e.g. 130.456 mph.
    /// </summary>
    [MaxLength(11)]
    [MessagePack.Key(23)]
    public string? AverageRaceSpeed { get; set; }
    /// <summary>
    /// Count of overall lead changes in the session. Available with Multiloop timing systems.
    /// </summary>
    [MessagePack.Key(24)]
    public int? LeadChanges { get; set; }
    /// <summary>
    /// Track sections as indicated by the timing system.
    /// </summary>
    [MessagePack.Key(25)]
    [MaxLength(5)]
    public List<Section> Sections { get; set; } = [];
    /// <summary>
    /// Class colors in hexadecimal format #RRGGBB (e.g., "#FF0000" for red).
    /// Each color corresponds to a racing class for visual identification.
    /// </summary>
    [MessagePack.Key(26)]
    [MaxLength(50)]
    public Dictionary<string, string> ClassColors { get; set; } = [];
    /// <summary>
    /// Session announcements as indicated by the timing system.
    /// </summary>
    [MessagePack.Key(27)]
    [MaxLength(500)]
    public List<Announcement> Announcements { get; set; } = [];
    /// <summary>
    /// Last time the session state was updated.
    /// </summary>
    [MessagePack.Key(28)]
    public DateTime? LastUpdated { get; set; }
    /// <summary>
    /// Gets or sets the mapping of class names to their corresponding order values.
    /// </summary>
    [MessagePack.Key(29)]
    [MaxLength(50)]
    public Dictionary<string, string> ClassOrder { get; set; } = [];
    /// <summary>
    /// Indicates whether this is a real event or a simulation/replay/test event.
    /// </summary>
    [MessagePack.Key(30)]
    public bool IsSimulation { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this session is historical or archived.
    /// </summary>
    [MessagePack.Key(31)]
    public bool IsArchived { get; set; }
}
