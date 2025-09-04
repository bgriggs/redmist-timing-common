using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Track segment or section information, e.g., for a pit lane or track sector.
/// </summary>
[MessagePackObject]
public class Section
{
    /// <summary>
    /// Section name from the timing system.
    /// </summary>
    [MaxLength(3)]
    [MessagePack.Key(0)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Section length in inches from the timing system.
    /// </summary>
    [MessagePack.Key(1)]
    public int LengthInches { get; set; }
    /// <summary>
    /// Name of the section start point from the timing system.
    /// </summary>
    [MaxLength(4)]
    [MessagePack.Key(2)]
    public string StartLabel { get; set; } = string.Empty;
    /// <summary>
    /// Name of the end of the section from the timing system
    /// </summary>
    [MaxLength(4)]
    [MessagePack.Key(3)]
    public string EndLabel { get; set; } = string.Empty;
}
