using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Message to convey to team, drivers, spectators, etc.
/// </summary>
[ProtoContract]
public class Announcement
{
    /// <summary>
    /// Time at which the announcement was made.
    /// </summary>
    [ProtoMember(1)]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// Announcement priority ("Urgent", "High", "Normal", "Low").
    /// </summary>
    [MaxLength(6)]
    [ProtoMember(2)]
    public string Priority { get; set; } = string.Empty;
    /// <summary>
    /// The message.
    /// </summary>
    [MaxLength(200)]
    [ProtoMember(3)]
    public string Text { get; set; } = string.Empty;
}
