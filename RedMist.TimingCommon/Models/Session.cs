using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents a racing session/run within an event, including timing information and session characteristics.
/// </summary>
[MessagePackObject]
public class Session
{
    /// <summary>
    /// Gets or sets the unique identifier for this session as provided by the timing system.
    /// </summary>
    [JsonPropertyName("sid")]
    [MessagePack.Key(0)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the event identifier this session belongs to.
    /// </summary>
    [JsonPropertyName("eid")]
    [MessagePack.Key(1)]
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the name of the session.
    /// Maximum length: 64 characters (RMonitor max is 40).
    /// </summary>
    [JsonPropertyName("n")]
    [MaxLength(64)] // RMonitor max is 40
    [MessagePack.Key(2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scheduled start time of the session.
    /// </summary>
    [JsonPropertyName("st")]
    [MessagePack.Key(3)]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the session, or <c>null</c> if the session has not ended.
    /// </summary>
    [JsonPropertyName("et")]
    [MessagePack.Key(4)]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the local time zone offset in hours from UTC.
    /// </summary>
    [JsonPropertyName("tz")]
    [MessagePack.Key(5)]
    public double LocalTimeZoneOffset { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this session was last updated, or <c>null</c> if never updated.
    /// </summary>
    [JsonPropertyName("lu")]
    [MessagePack.Key(6)]
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this session is currently live.
    /// </summary>
    [JsonPropertyName("il")]
    [MessagePack.Key(7)]
    public bool IsLive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is likely a practice or qualifying session. This may not be accurate.
    /// </summary>
    [JsonPropertyName("pq")]
    [MessagePack.Key(8)]
    public bool IsPracticeQualifying { get; set; }
}
