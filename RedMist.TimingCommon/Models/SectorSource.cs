namespace RedMist.TimingCommon.Models;

/// <summary>
/// Where a session's sector times come from. The values are part of the wire contract.
/// </summary>
public enum SectorSource
{
    /// <summary>
    /// Value: 0. The session has no sectors. Kept at 0: the native app decoders read a value they do
    /// not know as this, and other clients should treat one the same way.
    /// </summary>
    None = 0,
    /// <summary>
    /// Value: 1. X2 timing loops the organizer has set as sector boundaries.
    /// </summary>
    X2Loops = 1,
    /// <summary>
    /// Value: 2. The timing system's own sections, over the Multiloop protocol.
    /// </summary>
    Multiloop = 2,
    /// <summary>
    /// Value: 3. An external timing feed.
    /// </summary>
    External = 3,
    /// <summary>
    /// Value: 4. In-car GPS positions crossing sector lines on the track map. Less precise than a
    /// timing loop.
    /// </summary>
    Gps = 4,
}
