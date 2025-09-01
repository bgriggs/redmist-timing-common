using ProtoBuf;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Competitor entry information for an event.
/// </summary>
[ProtoContract]
public class EventEntry
{
    /// <summary>
    /// Car's number which can include letters, such as 99x.
    /// </summary>
    [JsonPropertyName("no")]
    [MaxLength(5)]
    [ProtoMember(1)]
    public string Number { get; set; } = string.Empty;
    /// <summary>
    /// Typically associated with First or Last name depending on configuration of the timing system.
    /// </summary>
    [JsonPropertyName("nm")]
    [MaxLength(30)]
    [ProtoMember(2)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Typically the name of the team depending on configuration of the timing system.
    /// </summary>
    [JsonPropertyName("t")]
    [MaxLength(30)]
    [ProtoMember(3)]
    public string Team { get; set; } = string.Empty;
    /// <summary>
    /// Car's class. This can be empty.
    /// </summary>
    [JsonPropertyName("c")]
    [MaxLength(40)]
    [ProtoMember(4)]
    public string Class { get; set; } = string.Empty;
}
