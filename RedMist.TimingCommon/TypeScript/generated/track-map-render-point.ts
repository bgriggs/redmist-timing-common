/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * A @see {@link RedMist.TimingCommon.Models.TrackMap} point carrying both its geographic position and a planar position ready
 * to draw. See @see {@link RedMist.TimingCommon.Models.TrackMapRender} for what the planar frame is and how to scale it.
 */
export interface TrackMapRenderPoint {
    /**
     * Latitude in decimal degrees (WGS84).
     */
    latitude: number;
    /**
     * Longitude in decimal degrees (WGS84).
     */
    longitude: number;
    /**
     * Meters east of the map's western edge. Increases left to right, as on screen.
     */
    x: number;
    /**
     * Meters south of the map's northern edge. Increases downward, matching screen coordinates, so
     * a consumer scales and translates but never has to flip an axis.
     */
    y: number;
    /**
     * Distance in meters from the path origin to this point, measured along the path. Carried
     * through from @see {@link RedMist.TimingCommon.Models.TrackMapPoint.CumulativeDistanceMeters}, so it is measured from the
     * path origin rather than the start/finish line.
     */
    cumulativeDistanceMeters: number;
}
