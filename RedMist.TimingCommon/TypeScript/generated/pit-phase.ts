/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Where a car is in a pit visit, for showing the visit as a sequence rather than a single in-pit
 * flag. Published by whichever source owns the car's pit state: in-car GPS telemetry can follow
 * the whole sequence, while timing loops only know which loop the car last crossed and so report
 * @see {@link RedMist.TimingCommon.Models.PitPhase.EnteredPit}, @see {@link RedMist.TimingCommon.Models.PitPhase.InPit} and @see {@link RedMist.TimingCommon.Models.PitPhase.ExitedPit}.
 * 
 * Clients should fall back to @see {@link RedMist.TimingCommon.Models.CarPosition.IsInPit} and the other pit flags when this
 * is @see {@link RedMist.TimingCommon.Models.PitPhase.None} or missing, since a timing source with no pit sequence, and a server that
 * predates it, leave it there throughout. The values are part of the wire contract.
 */
export enum PitPhase {
    /**
     * Value: 0. Not in a pit visit, or no source reports one. Kept at 0: the native app decoders
     * read a value they do not know as this, and other clients should treat one the same way.
     */
    None = 0,
    /**
     * Value: 1. In the pit lane, driving to the pit box.
     */
    EnteredPit = 1,
    /**
     * Value: 2. Stopped in the pit box, or the garage. @see {@link RedMist.TimingCommon.Models.CarPosition.PitStopStartTime}
     * says since when.
     */
    InPitBox = 2,
    /**
     * Value: 3. Moving again after a stop, or driving straight through, heading for the pit exit.
     */
    ExitingPit = 3,
    /**
     * Value: 4. Just left the pit lane. Shown briefly before returning to @see {@link RedMist.TimingCommon.Models.PitPhase.None}.
     */
    ExitedPit = 4,
    /**
     * Value: 5. In the pit, with no finer detail available - the car crossed a pit start/finish
     * loop, or a loop that only says it is in the pit. Shown the same way as
     * @see {@link RedMist.TimingCommon.Models.PitPhase.InPitBox}.
     */
    InPit = 5,
}
