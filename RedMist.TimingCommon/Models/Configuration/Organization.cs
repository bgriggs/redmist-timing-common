using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models.Configuration;

public class Organization
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(8)]
    public string ShortName { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string? Website { get; set; }

    public byte[]? Logo { get; set; }

    [MaxLength(40)]
    public string ControlLogType { get; set; } = string.Empty;

    [MaxLength(512)]
    public string ControlLogParams { get; set; } = string.Empty;

    public OrbitsConfiguration Orbits { get; set; } = new OrbitsConfiguration();

    public X2Configuration X2 { get; set; } = new X2Configuration();

    [MaxLength(40)]
    public string? RMonitorIp { get; set; }
    public int RMonitorPort { get; set; } = 50000;
    [MaxLength(40)]
    public string? MultiloopIp { get; set; }
    public int MultiloopPort { get; set; } = 50004;
    [MaxLength(512)]
    public string? OrbitsLogsPath { get; set; }
}
