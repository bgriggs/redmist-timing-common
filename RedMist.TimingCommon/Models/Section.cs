using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Track segment or section information, e.g., for a pit lane or track sector.
/// </summary>
[ProtoContract]
public class Section
{
    /// <summary>
    /// Section name from the timing system.
    /// </summary>
    [MaxLength(3)]
    [ProtoMember(1)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Section length in inches from the timing system.
    /// </summary>
    [ProtoMember(2)]
    public int LengthInches { get; set; }
    /// <summary>
    /// Name of the section start point from the timing system.
    /// </summary>
    [MaxLength(4)]
    [ProtoMember(3)]
    public string StartLabel { get; set; } = string.Empty;
    /// <summary>
    /// Name of the end of the section from the timing system
    /// </summary>
    [MaxLength(4)]
    [ProtoMember(4)]
    public string EndLabel { get; set; } = string.Empty;
}
