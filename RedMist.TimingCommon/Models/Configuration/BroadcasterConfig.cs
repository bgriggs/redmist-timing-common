using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// Details about the broadcaster of the event.
/// </summary>
[MessagePackObject]
public class BroadcasterConfig
{
    /// <summary>
    /// Name of the Broadcaster company.
    /// </summary>
    [Key(0)]
    [JsonPropertyName("c")]
    [MaxLength(128)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL where the broadcast is located.
    /// </summary>
    [Key(1)]
    [JsonPropertyName("u")]
    [MaxLength(2083)]
    public string Url { get; set; } = string.Empty;
}
