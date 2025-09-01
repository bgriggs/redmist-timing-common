using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// The overall state of a race session at any given time. 
/// This includes data needed to fully represent the race.
/// </summary>
public class SessionState
{
    /// <summary>
    /// Redmist ID for the event.
    /// </summary>
    [ProtoMember(1)] 
    public int EventId { get; set; }
    /// <summary>
    /// Event name as indicated by the organizer.
    /// </summary>
    [MaxLength(512)]
    [ProtoMember(2)]
    public string EventName { get; set; } = string.Empty;
    /// <summary>
    /// Session, or run, is the current part of the event being timed such as in individual race, practice, or qualifying session.
    /// This is the ID indicated by the timing system.
    /// </summary>
    [ProtoMember(3)] 
    public int SessionId { get; set; }
    /// <summary>
    /// Session name as indicated by the timing system.
    /// </summary>
    [MaxLength(40)]
    [ProtoMember(4)]
    public string SessionName { get; set; } = string.Empty;
    /// <summary>
    /// Optional number of laps to go if the race is lap based.
    /// </summary>
    [ProtoMember(5)]
    public int LapsToGo { get; set; }
    /// <summary>
    /// Optional time of the session remaining if the event is time based.
    /// Format is HH:mm:ss.
    /// </summary>
    /// <remarks>
    /// Orbits has been seen to have a negative seconds value preceding the start of a race, i.e. HH:mm:-ss
    /// </remarks>
    [MaxLength(10)]
    [ProtoMember(6)]
    public string TimeToGo { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the local time of day as a string. This is 24 hour format HH:mm:ss.
    /// </summary>
    [MaxLength(8)]
    [ProtoMember(7)]
    public string LocalTimeOfDay { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the amount of time a race has been running. Format is HH:mm:ss.
    /// </summary>
    [MaxLength(8)]
    [ProtoMember(8)]
    public string RunningRaceTime { get; set; } = string.Empty;
    /// <summary>
    /// Whether the current session is a practice qualifying session. This is is not guaranteed 
    /// to be accurate and comes from the session name.
    /// </summary>
    [ProtoMember(9)] 
    public bool IsPracticeQualifying { get; set; }
    /// <summary>
    /// When the session started according to the the first time data was received from the timing system for the session.
    /// </summary>
    [ProtoMember(10)]
    public DateTime SessionStartTime { get; set; }
    /// <summary>
    /// When the session ended according to the timing system change to another session or timeout of data being received, such as at the end of the race.
    /// </summary>
    [ProtoMember(11)]
    public DateTime? SessionEndTime { get; set; }
    /// <summary>
    /// The event's local time zone offset from UTC in hours as indicated by the organizer's system time.
    /// </summary>
    [ProtoMember(12)]
    public double LocalTimeZoneOffset { get; set; }
    /// <summary>
    /// Indicates whether the organizer is connected and sending data for the event.
    /// </summary>
    [ProtoMember(13)]
    public bool IsLive { get; set; }
    /// <summary>
    /// These are the signed up entries indicated by the timing system.
    /// They may not be the same as the cars that actually participated in the event.
    /// </summary>
    [ProtoMember(14)] 
    public List<EventEntry> EventEntries { get; set; } = [];
    /// <summary>
    /// Represents the current position and status of each car in the event.
    /// </summary>
    [ProtoMember(15)]
    public List<CarPosition> CarPositions { get; set; } = [];
    /// <summary>
    /// Current flag state for the event.
    /// </summary>
    [ProtoMember(16)]
    public Flags CurrentFlag { get; set; }
    /// <summary>
    /// Flag states for the event, including durations.
    /// </summary>
    [ProtoMember(17)]
    public List<FlagDuration> FlagDurations { get; set; } = [];
    /// <summary>
    /// Track sections as indicated by the timing system.
    /// </summary>
    [ProtoMember(18)]
    public List<Section> Sections { get; set; } = [];
    /// <summary>
    /// Class colors in hexadecimal format #RRGGBB (e.g., "#FF0000" for red).
    /// Each color corresponds to a racing class for visual identification.
    /// </summary>
    [ProtoMember(19)]
    public Dictionary<string, string> ClassColors { get; set; } = [];
    /// <summary>
    /// Session announcements as indicated by the timing system.
    /// </summary>
    [ProtoMember(20)]
    public List<Announcement> Announcements { get; set; } = [];
    /// <summary>
    /// Last time the session state was updated.
    /// </summary>
    [ProtoMember(21)]
    public DateTime? LastUpdated { get; set; }
}
