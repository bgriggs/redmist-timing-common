using ProtoBuf;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Instance of a flag state during a session.
/// </summary>
[ProtoContract]
public class FlagDuration
{
    /// <summary>
    /// The flag that is or was active.
    /// </summary>
    [JsonPropertyName("f")]
    [ProtoMember(1)]
    public Flags Flag { get; set; }
    /// <summary>
    /// When the flag state started.
    /// </summary>
    [JsonPropertyName("s")]
    [ProtoMember(2)]
    public DateTime StartTime { get; set; }
    /// <summary>
    /// When the flag state ended, or null if it is still active.
    /// </summary>
    [JsonPropertyName("e")]
    [ProtoMember(3)]
    public DateTime? EndTime { get; set; }
}
