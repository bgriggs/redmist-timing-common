using MessagePack;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents statistics and status information for a control log connection
/// typically used by the relay service.
/// </summary>
[MessagePackObject]
public class ControlLogStatistics
{
    /// <summary>
    /// Control log is connected.
    /// </summary>
    [Key(0)]
    public bool IsConnected { get; set; }

    /// <summary>
    /// Number of entries in the control log.
    /// </summary>
    [Key(1)]
    public int TotalEntries { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the log apears to be left over from a previous event.
    /// </summary>
    [Key(2)]
    public bool IsStaleWarning { get; set; }
}
