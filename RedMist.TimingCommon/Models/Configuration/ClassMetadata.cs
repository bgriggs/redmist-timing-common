using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// Represents metadata information for a class, including its name, display order, and associated color.
/// </summary>
[MessagePackObject]
public class ClassMetadata
{
    /// <summary>
    /// Class name, such as GTO.
    /// </summary>
    [Key(0)]
    [StringLength(40, MinimumLength = 1)]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Display order in the UI when in grouped mode.
    /// </summary>
    [Key(1)]
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the color value as a hexadecimal string to show in the class marker.
    /// </summary>
    [Key(2)]
    public string ColorHex { get; set; } = string.Empty;
}
