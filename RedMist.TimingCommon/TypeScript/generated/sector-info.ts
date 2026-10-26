/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * One sector of the lap, as listed in @see {@link RedMist.TimingCommon.Models.SessionState.SectorLayout}.
 */
export interface SectorInfo {
    /**
     * Which sector this is, from 1. Matches @see {@link RedMist.TimingCommon.Models.SectorTime.Index}.
     */
    index: number;
    /**
     * Column heading for the sector, normally "S1", "S2" and so on.
     */
    label: string;
    /**
     * Length of the sector in meters, when it is known. Clients use it to size each sector's share
     * of the lap, but only when every sector in the layout has a length; otherwise all sectors are
     * drawn as equal shares.
     */
    lengthMeters: number | null;
}
