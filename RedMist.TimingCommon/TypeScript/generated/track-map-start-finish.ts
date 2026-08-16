/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * Where the start/finish line sits on a rendered map, and which way cars are travelling as they
 * cross it, so a consumer can mark the line and orient the track without re-deriving either.
 */
export interface TrackMapStartFinish {
    /**
     * Latitude of the line, in decimal degrees.
     */
    latitude: number;
    /**
     * Longitude of the line, in decimal degrees.
     */
    longitude: number;
    /**
     * Position of the line in the map's planar frame; see @see {@link RedMist.TimingCommon.Models.TrackMapRenderPoint.X}.
     */
    x: number;
    /**
     * Position of the line in the map's planar frame; see @see {@link RedMist.TimingCommon.Models.TrackMapRenderPoint.Y}.
     */
    y: number;
    /**
     * Distance in meters from the path origin to the line, along the path. The same figure as
     * @see {@link RedMist.TimingCommon.Models.TrackMap.StartFinishOffsetMeters}.
     */
    distanceAlongMeters: number;
    /**
     * Direction of travel across the line as a compass bearing in degrees: 0 is north, 90 east,
     * measured clockwise. A line marker is drawn perpendicular to this.
     */
    headingDegrees: number;
}
