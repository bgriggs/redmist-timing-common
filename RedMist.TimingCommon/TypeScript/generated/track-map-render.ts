/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { TrackMapRenderPoint } from "./track-map-render-point";
import { TrackMapBounds } from "./track-map-bounds";
import { TrackMapStartFinish } from "./track-map-start-finish";

/**
 * A @see {@link RedMist.TimingCommon.Models.TrackMap} prepared for drawing: the same learned centerline, plus a planar
 * projection, the map's extent, and the start/finish line resolved to a position.
 * 
 * <para>
 * @see {@link RedMist.TimingCommon.Models.TrackMapRenderPoint.X} and @see {@link RedMist.TimingCommon.Models.TrackMapRenderPoint.Y} are meters in a
 * flat-earth frame whose origin is the map's north-west corner, with Y increasing downward. The
 * frame is uniformly scaled - one meter east and one meter south are the same number of units - so
 * the track keeps its true shape at any zoom. A consumer fits it to a viewport with a single
 * scale factor:
 * </para>
 * <code>
 * var scale = Math.Min(viewportWidth / map.Bounds.WidthMeters, viewportHeight / map.Bounds.HeightMeters);
 * // then, per point: (point.X * scale, point.Y * scale)
 * </code>
 * <para>
 * One of the two extents can legitimately be zero - a track running due north-south has no width -
 * which leaves that ratio infinite and the other one deciding the scale, as it should. Both are
 * never zero: a map with no extent at all is not projected in the first place.
 * </para>
 * <para>
 * Latitude and longitude are carried alongside for consumers drawing over a real map instead, and
 * @see {@link RedMist.TimingCommon.Models.TrackMapRender.Points} stays in path order, so it can be stroked as a polyline directly. The path
 * is closed: the last point connects back to the first, and that closing segment is not repeated
 * in the list.
 * </para>
 */
export interface TrackMapRender {
    /**
     * RedMist event the map was learned for.
     */
    eventId: number;
    /**
     * Session the map was learned from.
     */
    sessionId: number;
    /**
     * Name of the track, when the event records one.
     */
    trackName: string | null;
    /**
     * Ordered centerline points around the lap, starting at the path origin. See the class remarks
     * for the coordinate frame.
     */
    points: TrackMapRenderPoint[];
    /**
     * The map's extent and the size of its planar frame.
     */
    bounds: TrackMapBounds;
    /**
     * Lap length in meters, including the closing segment back to the first point.
     */
    lengthMeters: number;
    /**
     * Where the start/finish line sits, or null when the map has not been calibrated yet. A map
     * without it is still drawable - the outline is complete - it just cannot be marked or
     * oriented, which is why this is null rather than a guess at the path origin.
     */
    startFinish: TrackMapStartFinish | null;
    /**
     * When the underlying map was learned, in UTC.
     */
    builtUtc: Date;
    /**
     * Schema/build version of the underlying @see {@link RedMist.TimingCommon.Models.TrackMap}, for a consumer that needs to
     * tell one map format from another. It does not change when a map is relearned - compare
     * @see {@link RedMist.TimingCommon.Models.TrackMapRender.BuiltUtc} to spot that.
     */
    version: number;
}
