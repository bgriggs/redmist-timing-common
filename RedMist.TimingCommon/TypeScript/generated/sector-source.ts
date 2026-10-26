/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Where a session's sector times come from. The values are part of the wire contract.
 */
export enum SectorSource {
    /**
     * Value: 0. The session has no sectors. Kept at 0: the native app decoders read a value they do
     * not know as this, and other clients should treat one the same way.
     */
    None = 0,
    /**
     * Value: 1. X2 timing loops the organizer has set as sector boundaries.
     */
    X2Loops = 1,
    /**
     * Value: 2. The timing system's own sections, over the Multiloop protocol.
     */
    Multiloop = 2,
    /**
     * Value: 3. An external timing feed.
     */
    External = 3,
    /**
     * Value: 4. In-car GPS positions crossing sector lines on the track map. Less precise than a
     * timing loop.
     */
    Gps = 4,
}
