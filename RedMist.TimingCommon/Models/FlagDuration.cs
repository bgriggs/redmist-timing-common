using MessagePack;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Instance of a flag state during a session.
/// </summary>
[MessagePackObject]
public class FlagDuration
{
    /// <summary>
    /// The flag that is or was active.
    /// </summary>
    [JsonPropertyName("f")]
    [MessagePack.Key(0)]
    public Flags Flag { get; set; }

    /// <summary>
    /// When the flag state started.
    /// </summary>
    [JsonPropertyName("s")]
    [MessagePack.Key(1)]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// When the flag state ended, or null if it is still active.
    /// </summary>
    [JsonPropertyName("e")]
    [MessagePack.Key(2)]
    public DateTime? EndTime { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is FlagDuration other && Equals(other);
    }

    public bool Equals(FlagDuration other)
    {
        return Flag == other.Flag && StartTime == other.StartTime && EndTime == other.EndTime;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Flag, StartTime, EndTime);
    }
}
