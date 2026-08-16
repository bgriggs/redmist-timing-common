/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

/**
 * The geographic extent of a @see {@link RedMist.TimingCommon.Models.TrackMapRender}, and the size of the planar frame its
 * points are expressed in.
 */
export interface TrackMapBounds {
    /**
     * Southern edge, in decimal degrees.
     */
    minLatitude: number;
    /**
     * Northern edge, in decimal degrees.
     */
    maxLatitude: number;
    /**
     * Western edge, in decimal degrees.
     */
    minLongitude: number;
    /**
     * Eastern edge, in decimal degrees.
     */
    maxLongitude: number;
    /**
     * Width of the planar frame in meters: the largest @see {@link RedMist.TimingCommon.Models.TrackMapRenderPoint.X} on the
     * map. Divide a viewport's width by this to get the scale that fits the track horizontally.
     */
    widthMeters: number;
    /**
     * Height of the planar frame in meters: the largest @see {@link RedMist.TimingCommon.Models.TrackMapRenderPoint.Y} on the
     * map. Divide a viewport's height by this to get the scale that fits the track vertically.
     */
    heightMeters: number;
    /**
     * Latitude of the center of the extent, in decimal degrees.
     */
    centerLatitude: number;
    /**
     * Longitude of the center of the extent, in decimal degrees.
     */
    centerLongitude: number;
}
