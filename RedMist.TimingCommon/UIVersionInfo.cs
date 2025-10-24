using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon;

/// <summary>
/// Provides version information for the UI application.
/// </summary>
[MessagePackObject]
public class UIVersionInfo
{
    /// <summary>
    /// Gets or sets the latest iOS version identifier that is available.
    /// </summary>
    [MaxLength(15)]
    [MessagePack.Key(0)] 
    public string LatestIOSVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum required iOS version for the application.
    /// The application may enforce this minimum version for compatibility.
    /// </summary>
    [MaxLength(15)]
    [MessagePack.Key(1)]
    public string MinimumIOSVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether an iOS update is recommended for the device.
    /// </summary>
    [MessagePack.Key(2)]
    public bool RecommendIOSUpdate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the minimum iOS version requirement is mandatory.
    /// The application may prevent usage if the device does not meet this requirement.
    /// </summary>
    [MessagePack.Key(3)]
    public bool IsIOSMinimumMandatory { get; set; }

    /// <summary>
    /// Gets or sets the latest available version of the Android operating system.
    /// </summary>
    [MaxLength(15)]
    [MessagePack.Key(4)]
    public string LatestAndroidVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum required Android version for the application.
    /// </summary>
    [MaxLength(15)]
    [MessagePack.Key(5)]
    public string MinimumAndroidVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the application should recommend an update for Android devices.
    /// </summary>
    [MessagePack.Key(6)]
    public bool RecommendAndroidUpdate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the minimum Android version requirement is mandatory.
    /// </summary>
    [MessagePack.Key(7)] 
    public bool IsAndroidMinimumMandatory { get; set; }

    /// <summary>
    /// Gets or sets the latest available version of the web application.
    /// </summary>
    [MaxLength(15)]
    [MessagePack.Key(8)]
    public string LatestWebVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum supported version of the web client required for compatibility.
    /// </summary>
    [MaxLength(15)]
    [MessagePack.Key(9)]
    public string MinimumWebVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether a web update is recommended.
    /// </summary>
    [MessagePack.Key(10)] 
    public bool RecommendWebUpdate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the minimum web requirement is mandatory.
    /// </summary>
    [MessagePack.Key(11)] 
    public bool IsWebMinimumMandatory { get; set; }
}
