namespace RedMist.TimingCommon.Models;

/// <summary>
/// Where a car is in a pit visit, for showing the visit as a sequence rather than a single in-pit
/// flag. Published by whichever source owns the car's pit state: in-car GPS telemetry can follow
/// the whole sequence, while timing loops only know which loop the car last crossed and so report
/// <see cref="EnteredPit"/>, <see cref="InPit"/> and <see cref="ExitedPit"/>.
///
/// Clients should fall back to <see cref="CarPosition.IsInPit"/> and the other pit flags when this
/// is <see cref="None"/> or missing, since a timing source with no pit sequence, and a server that
/// predates it, leave it there throughout. The values are part of the wire contract.
/// </summary>
public enum PitPhase
{
    /// <summary>
    /// Value: 0. Not in a pit visit, or no source reports one. Kept at 0: the native app decoders
    /// read a value they do not know as this, and other clients should treat one the same way.
    /// </summary>
    None = 0,
    /// <summary>
    /// Value: 1. In the pit lane, driving to the pit box.
    /// </summary>
    EnteredPit = 1,
    /// <summary>
    /// Value: 2. Stopped in the pit box, or the garage. <see cref="CarPosition.PitStopStartTime"/>
    /// says since when.
    /// </summary>
    InPitBox = 2,
    /// <summary>
    /// Value: 3. Moving again after a stop, or driving straight through, heading for the pit exit.
    /// </summary>
    ExitingPit = 3,
    /// <summary>
    /// Value: 4. Just left the pit lane. Shown briefly before returning to <see cref="None"/>.
    /// </summary>
    ExitedPit = 4,
    /// <summary>
    /// Value: 5. In the pit, with no finer detail available - the car crossed a pit start/finish
    /// loop, or a loop that only says it is in the pit. Shown the same way as
    /// <see cref="InPitBox"/>.
    /// </summary>
    InPit = 5,
}
