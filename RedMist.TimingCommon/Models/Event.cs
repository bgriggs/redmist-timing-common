using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class Event
{
    [JsonPropertyName("e")]
    public int EventId { get; set; }
    [JsonPropertyName("n")]
    public string EventName { get; set; } = string.Empty;
    [JsonPropertyName("d")]
    public string EventDate { get; set; } = string.Empty;
    [JsonPropertyName("s")]
    public Session[] Sessions { get; set; } = [];
    public string OrganizationName { get; set; } = string.Empty;
    public string? OrganizationWebsite { get; set; }
    public byte[]? OrganizationLogo { get; set; }
}
