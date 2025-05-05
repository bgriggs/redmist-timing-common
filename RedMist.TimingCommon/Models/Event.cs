using RedMist.TimingCommon.Models.Configuration;
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
    [JsonPropertyName("u")]
    public string EventUrl { get; set; } = string.Empty;
    [JsonPropertyName("s")]
    public Session[] Sessions { get; set; } = [];
    [JsonPropertyName("on")]
    public string OrganizationName { get; set; } = string.Empty;
    [JsonPropertyName("ow")]
    public string? OrganizationWebsite { get; set; }
    [JsonPropertyName("l")]
    public byte[]? OrganizationLogo { get; set; }
    [JsonPropertyName("sc")]
    public EventSchedule? Schedule { get; set; }
    [JsonPropertyName("t")]
    public string TrackName { get; set; } = string.Empty;
    [JsonPropertyName("cc")]
    public string CourseConfiguration { get; set; } = string.Empty;
    [JsonPropertyName("di")]
    public string Distance { get; set; } = string.Empty;
    [JsonPropertyName("b")]
    public BroadcasterConfig? Broadcast { get; set; }
    [JsonPropertyName("hc")]
    public bool HasControlLog { get; set; }
    [JsonPropertyName("il")]
    public bool IsLive { get; set; }
}
