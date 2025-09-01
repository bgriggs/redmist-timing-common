using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents a completed section in the timing system for a given competitor.
/// </summary>
[ProtoContract]
public class CompletedSection
{
    /// <summary>
    /// Car number.
    /// </summary>
    /// <remarks>Max length is 4 if multiloop is in use</remarks>
    [MaxLength(5)]
    [ProtoMember(1)]
    public string Number { get; set; } = string.Empty;
    /// <summary>
    /// Section ID from the timing system.
    /// </summary>
    [MaxLength(5)]
    [ProtoMember(2)]
    public string SectionId { get; set; } = string.Empty;
    /// <summary>
    /// Section time in milliseconds.
    /// </summary>
    [ProtoMember(3)]
    public int ElapsedTimeMs { get; set; }
    /// <summary>
    /// Previous section time in milliseconds.
    /// </summary>
    [ProtoMember(4)]
    public int LastSectionTimeMs { get; set; }
    /// <summary>
    /// Lap number for the last completed section.
    /// </summary>
    [ProtoMember(5)]
    public int LastLap { get; set; }
}
