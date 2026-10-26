/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * A car's most recent time through one sector of the lap. A car carries at most one of these per
 * sector, replaced each time it completes that sector again, so while it is partway round a lap the
 * sectors it has not reached yet still hold its previous lap's times. Compare @see {@link RedMist.TimingCommon.Models.SectorTime.Lap}
 * with @see {@link RedMist.TimingCommon.Models.CarPosition.LastLapCompleted} to tell them apart: a sector whose lap is greater
 * belongs to the lap the car is on now.
 * 
 * The best flags are worked out on the server for every source alike, and they nest: an overall
 * best is also the best in its class, and a class best is also the car's own best. Clients color a
 * time as the session best (purple) when @see {@link RedMist.TimingCommon.Models.SectorTime.IsOverallBest} is set and the field is shown
 * overall, or when @see {@link RedMist.TimingCommon.Models.SectorTime.IsClassBest} is set and it is grouped by class; failing that, as the
 * car's own best (green) when @see {@link RedMist.TimingCommon.Models.SectorTime.IsPersonalBest} is set.
 */
export interface SectorTime {
    /**
     * Which sector this is, from 1 to the number of sectors in
     * @see {@link RedMist.TimingCommon.Models.SessionState.SectorLayout}.
     */
    index: number;
    /**
     * The lap this sector time belongs to: the lap the car was on when it started the sector.
     */
    lap: number;
    /**
     * Time through the sector in milliseconds.
     */
    timeMs: number;
    /**
     * The time is shown but does not count towards any best: the car went through the pit lane
     * during the sector, or the source marked the time as not valid. None of the best flags are
     * set when this is.
     */
    isInvalid: boolean;
    /**
     * This is the car's own best time in this sector this session. Also set whenever
     * @see {@link RedMist.TimingCommon.Models.SectorTime.IsClassBest} is.
     */
    isPersonalBest: boolean;
    /**
     * This is the best time in this sector this session across all classes. When set,
     * @see {@link RedMist.TimingCommon.Models.SectorTime.IsClassBest} and @see {@link RedMist.TimingCommon.Models.SectorTime.IsPersonalBest} are too.
     */
    isOverallBest: boolean;
    /**
     * This is the best time in this sector this session within the car's class. Also set whenever
     * @see {@link RedMist.TimingCommon.Models.SectorTime.IsOverallBest} is.
     */
    isClassBest: boolean;
}
