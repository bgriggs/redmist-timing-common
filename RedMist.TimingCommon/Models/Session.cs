using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

[PrimaryKey(nameof(Id), nameof(EventId))]
public class Session
{
    [JsonPropertyName("sid")]
    public int Id { get; set; }

    [JsonPropertyName("eid")]
    public int EventId { get; set; }

    [JsonPropertyName("n")]
    [MaxLength(64)] // RMonitor max is 40
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("st")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("et")]
    public DateTime? EndTime { get; set; }

    [JsonPropertyName("lu")]
    public DateTime? LastUpdated { get; set; }

    [JsonPropertyName("il")]
    public bool IsLive { get; set; }
}
