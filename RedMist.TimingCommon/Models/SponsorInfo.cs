using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models;

[MessagePackObject]
public class SponsorInfo
{
    [Key(0)]
    public int Id { get; set; }
    
    [Key(1)]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Key(2)]
    [MaxLength(1024)]
    public required string ImageUrl { get; set; }

    [Key(3)]
    [MaxLength(1024)]
    public required string TargetUrl { get; set; }

    [Key(4)]
    [MaxLength(200)]
    public string? AltText { get; set; } = string.Empty;
    [Key(5)]
    public int DisplayDurationMs { get; set; }
    [Key(6)]
    public int DisplayPriority { get; set; }
}
