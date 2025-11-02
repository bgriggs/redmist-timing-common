using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

[Obsolete("Use Organiztion configuration fields")]
[MessagePackObject]
public class OrbitsConfiguration
{
    [Key(0)]
    [MaxLength(80)]
    public string IP { get; set; } = string.Empty;
    [Key(1)]
    public int Port { get; set; } = 50000;
    [Key(2)]
    [MaxLength(256)]
    public string? LogsPath { get; set; }
}
