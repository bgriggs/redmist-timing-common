using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class CompetitorMetadata
{
    [JsonPropertyName("e")]
    public int EventId { get; set; }
    [Required]
    [MaxLength(16)]
    [JsonPropertyName("n")]
    public string CarNumber { get; set; } = string.Empty;
    [JsonPropertyName("lu")]
    public DateTime LastUpdated { get; set; }
    [JsonPropertyName("t")]
    public uint Transponder { get; set; }
    [JsonPropertyName("t2")]
    public uint Transponder2 { get; set; }
    [MaxLength(32)]
    [JsonPropertyName("cl")]
    public string Class { get; set; } = string.Empty;
    [MaxLength(64)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(64)]
    public string LastName { get; set; } = string.Empty;
    [MaxLength(128)]
    public string NationState { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Sponsor { get; set; } = string.Empty;
    [MaxLength(48)]
    public string Make { get; set; } = string.Empty;
    [MaxLength(128)]
    public string Hometown { get; set; } = string.Empty;
    [MaxLength(64)]
    public string Club { get; set; } = string.Empty;
    [MaxLength(48)]
    public string ModelEngine { get; set; } = string.Empty;
    [MaxLength(48)]
    public string Tires { get; set; } = string.Empty;
    [MaxLength(128)]
    public string Email { get; set; } = string.Empty;
}
