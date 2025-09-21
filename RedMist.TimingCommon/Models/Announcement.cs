using MessagePack;
using RedMist.TimingCommon.Models.Mappers;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Message to convey to team, drivers, spectators, etc.
/// </summary>
[MessagePackObject]
public class Announcement
{
    /// <summary>
    /// Time at which the announcement was made.
    /// </summary>
    [MessagePack.Key(0)]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// Announcement priority ("Urgent", "High", "Normal", "Low").
    /// </summary>
    [MaxLength(6)]
    [MessagePack.Key(1)]
    public string Priority { get; set; } = string.Empty;
    /// <summary>
    /// The message.
    /// </summary>
    [MaxLength(200)]
    [MessagePack.Key(2)]
    public string Text { get; set; } = string.Empty;

    public void test()
    {
        //SessionStatePatch
        //CarPositionPatch
        //SessionStateMapper
        //CarPositionMapper
    }

    public override bool Equals(object? obj)
    {
        return obj is Announcement other && Equals(other);
    }

    public bool Equals(Announcement other)
    {
        return Timestamp == other.Timestamp && Priority == other.Priority && Text == other.Text;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Timestamp, Priority, Text);
    }
}
