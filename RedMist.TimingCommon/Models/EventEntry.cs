using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Competitor entry information for an event.
/// </summary>
[MessagePackObject]
public class EventEntry
{
    /// <summary>
    /// Car's number which can include letters, such as 99x.
    /// </summary>
    [JsonPropertyName("no")]
    [MaxLength(5)]
    [MessagePack.Key(0)]
    public string Number { get; set; } = string.Empty;
    /// <summary>
    /// Typically associated with First or Last name depending on configuration of the timing system.
    /// </summary>
    [JsonPropertyName("nm")]
    [MaxLength(30)]
    [MessagePack.Key(1)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Typically the name of the team depending on configuration of the timing system.
    /// </summary>
    [JsonPropertyName("t")]
    [MaxLength(30)]
    [MessagePack.Key(2)]
    public string Team { get; set; } = string.Empty;
    /// <summary>
    /// Car's class. This can be empty.
    /// </summary>
    [JsonPropertyName("c")]
    [MaxLength(40)]
    [MessagePack.Key(3)]
    public string Class { get; set; } = string.Empty;
}
