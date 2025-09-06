using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents a completed section in the timing system for a given competitor.
/// </summary>
[MessagePackObject]
public class CompletedSection
{
    /// <summary>
    /// Car number.
    /// </summary>
    /// <remarks>Max length is 4 if multiloop is in use</remarks>
    [MaxLength(5)]
    [MessagePack.Key(0)]
    public string Number { get; set; } = string.Empty;
    /// <summary>
    /// Section ID from the timing system.
    /// </summary>
    [MaxLength(5)]
    [MessagePack.Key(1)]
    public string SectionId { get; set; } = string.Empty;
    /// <summary>
    /// Section time in milliseconds.
    /// </summary>
    [MessagePack.Key(2)]
    public int ElapsedTimeMs { get; set; }
    /// <summary>
    /// Previous section time in milliseconds.
    /// </summary>
    [MessagePack.Key(3)]
    public int LastSectionTimeMs { get; set; }
    /// <summary>
    /// Lap number for the last completed section.
    /// </summary>
    [MessagePack.Key(4)]
    public int LastLap { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CompletedSection section &&
               Number == section.Number &&
               SectionId == section.SectionId &&
               ElapsedTimeMs == section.ElapsedTimeMs &&
               LastSectionTimeMs == section.LastSectionTimeMs &&
               LastLap == section.LastLap;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Number, SectionId, ElapsedTimeMs, LastSectionTimeMs, LastLap);
    }
}
