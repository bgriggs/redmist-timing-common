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
}
