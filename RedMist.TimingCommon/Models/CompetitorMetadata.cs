using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents metadata information for a competitor, including identification, personal details, and vehicle
/// information, used in event management or race tracking systems.
/// </summary>
/// <remarks>This class for competitor details such as car number, event association, transponder IDs, 
/// and personal or team information. The contents of this are subject to the organizer's configuration and may not 
/// corresponding to the field descritions.</remarks>
[MessagePackObject]
public class CompetitorMetadata
{
    /// <summary>
    /// Gets or sets the event identifier this competitor is associated with.
    /// </summary>
    [JsonPropertyName("e")]
    [MessagePack.Key(0)]
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the car number assigned to this competitor.
    /// Maximum length: 16 characters.
    /// </summary>
    [Required]
    [MaxLength(16)]
    [JsonPropertyName("n")]
    [MessagePack.Key(1)]
    public string CarNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when this competitor metadata was last updated.
    /// </summary>
    [JsonPropertyName("lu")]
    [MessagePack.Key(2)]
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the primary transponder identifier for timing and scoring.
    /// </summary>
    [JsonPropertyName("t")]
    [MessagePack.Key(3)]
    public uint Transponder { get; set; }

    /// <summary>
    /// Gets or sets the secondary transponder identifier for timing and scoring.
    /// </summary>
    [JsonPropertyName("t2")]
    [MessagePack.Key(4)]
    public uint Transponder2 { get; set; }

    /// <summary>
    /// Gets or sets the competition class or category.
    /// Maximum length: 32 characters.
    /// </summary>
    [MaxLength(32)]
    [JsonPropertyName("cl")]
    [MessagePack.Key(5)]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the competitor's first name.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 64 characters.
    /// </summary>
    [MaxLength(64)]
    [JsonPropertyName("fn")]
    [MessagePack.Key(6)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the competitor's last name.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 64 characters.
    /// </summary>
    [MaxLength(64)]
    [JsonPropertyName("ln")]
    [MessagePack.Key(7)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the competitor's nation or state.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 128 characters.
    /// </summary>
    [MaxLength(128)]
    [JsonPropertyName("ns")]
    [MessagePack.Key(8)]
    public string NationState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the competitor's sponsor information.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 200 characters.
    /// </summary>
    [MaxLength(200)]
    [JsonPropertyName("s")]
    [MessagePack.Key(9)]
    public string Sponsor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle make or manufacturer.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 48 characters.
    /// </summary>
    [MaxLength(48)]
    [JsonPropertyName("mk")]
    [MessagePack.Key(10)]
    public string Make { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the competitor's hometown.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 128 characters.
    /// </summary>
    [MaxLength(128)]
    [JsonPropertyName("h")]
    [MessagePack.Key(11)]
    public string Hometown { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the club or organization the competitor represents.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 64 characters.
    /// </summary>
    [MaxLength(64)]
    [JsonPropertyName("c")]
    [MessagePack.Key(12)]
    public string Club { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle model or engine information.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 48 characters.
    /// </summary>
    [MaxLength(48)]
    [JsonPropertyName("mo")]
    [MessagePack.Key(13)]
    public string ModelEngine { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tire brand or specification.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 48 characters.
    /// </summary>
    [MaxLength(48)]
    [JsonPropertyName("tr")]
    [MessagePack.Key(14)]
    public string Tires { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the competitor's email address.
    /// The contents of this field are subject to the organizer's configuration and may not corresponding to the field.
    /// Maximum length: 128 characters.
    /// </summary>
    [MaxLength(128)]
    [JsonPropertyName("a")]
    [MessagePack.Key(15)]
    public string Email { get; set; } = string.Empty;
}
