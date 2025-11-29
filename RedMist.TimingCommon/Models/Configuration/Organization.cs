using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// Represents an organization, series, or league that hosts racing events, including configuration for timing systems and branding.
/// </summary>
[MessagePackObject]
public class Organization
{
    /// <summary>
    /// Gets or sets the unique identifier for this organization.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Key]
    [Key(0)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique authentication identifier for the client.
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Key(1)]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name of the organization.
    /// Maximum length: 255 characters.
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Key(2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the abbreviated name of the organization.
    /// Maximum length: 8 characters.
    /// </summary>
    [Required]
    [MaxLength(8)]
    [Key(3)]
    public string ShortName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the organization's website URL, or <c>null</c> if not available.
    /// Maximum length: 1024 characters.
    /// </summary>
    [MaxLength(1024)]
    [Key(4)]
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the organization's logo as a byte array, or <c>null</c> if not available.
    /// Maximum size: 5242880 bytes (5 MB).
    /// </summary>
    [Key(5)]
    [MaxLength(5242880)] // 5MB
    public byte[]? Logo { get; set; }

    /// <summary>
    /// Gets or sets the type of control log system used by this organization.
    /// Maximum length: 40 characters.
    /// </summary>
    [Key(6)]
    [MaxLength(40)]
    public string ControlLogType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parameters for the control log system.
    /// Maximum length: 512 characters.
    /// </summary>
    [Key(7)]
    [MaxLength(512)]
    public string ControlLogParams { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Orbits timing system configuration. 
    /// This will be removed in future versions. Use the fields directly in the Organization class instead.
    /// </summary>
    [Key(8)]
    public OrbitsConfiguration Orbits { get; set; } = new OrbitsConfiguration();

    /// <summary>
    /// Gets or sets the X2 decoder server configuration.
    /// </summary>
    [Key(9)]
    public X2Configuration X2 { get; set; } = new X2Configuration();

    /// <summary>
    /// Gets or sets the IP address for the RMonitor timing system, or <c>null</c> if not configured.
    /// Maximum length: 40 characters.
    /// </summary>
    [Key(10)]
    [MaxLength(40)]
    public string? RMonitorIp { get; set; }
    
    /// <summary>
    /// Gets or sets the port number for the RMonitor timing system.
    /// Default is 50000.
    /// </summary>
    [Key(11)]
    public int RMonitorPort { get; set; } = 50000;
    
    /// <summary>
    /// Gets or sets the IP address for the Multiloop timing system, or <c>null</c> if not configured.
    /// Maximum length: 40 characters.
    /// </summary>
    [Key(12)]
    [MaxLength(40)]
    public string? MultiloopIp { get; set; }
    
    /// <summary>
    /// Gets or sets the port number for the Multiloop timing system.
    /// Default is 50004.
    /// </summary>
    [Key(13)]
    public int MultiloopPort { get; set; } = 50004;
    
    /// <summary>
    /// Gets or sets the file system path to Orbits logs, or <c>null</c> if not configured.
    /// Maximum length: 512 characters.
    /// </summary>
    [Key(14)]
    [MaxLength(512)]
    public string? OrbitsLogsPath { get; set; }

    /// <summary>
    /// Gets or sets the URL used to connect to the Flagtronics service.
    /// Example: http://192.168.1.100:52733/api/driverid/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
    /// </summary>
    [Key(15)]
    [MaxLength(512)]
    public string FlagtronicsUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets the API key used to authenticate requests to the Flagtronics service.
    /// </summary>
    [Key(16)]
    [MaxLength(42)]
    public string FlagtronicsApiKey { get; } = string.Empty;
}
