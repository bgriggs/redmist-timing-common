namespace RedMist.TimingCommon.Models;

/// <summary>
/// Identifies how an event's timing data is produced.
/// </summary>
public enum TimingSource
{
    /// <summary>
    /// Data arrives from a track-side relay (the default). Value: 0
    /// </summary>
    Relay,
    /// <summary>
    /// Data is produced by an external source service that pushes pre-formed patches. Value: 1
    /// </summary>
    External
}
