using RedMist.TimingCommon.Models;

namespace RedMist.TimingCommon.LapTiming;

/// <summary>
/// Turns a learned <see cref="TrackMap"/> into a <see cref="TrackMapRender"/>: the same centerline,
/// projected onto a planar frame in meters that a consumer can scale straight into a viewport,
/// together with the map's extent and its start/finish line resolved to a position.
///
/// The projection is equirectangular about the map's own center, the same approximation the rest of
/// <see cref="TrackGeometry"/> uses. Across the largest circuits raced anywhere it departs from true
/// distance by a couple of meters, and - the property that matters for drawing - it is uniform: the
/// scale is the same in both axes and everywhere on the map, so the track keeps its true shape and a
/// single scale factor fits it to any viewport.
///
/// Pure and timing-source agnostic; it reads a map and returns a new object, touching nothing.
/// </summary>
public static class TrackMapProjector
{
    /// <summary>
    /// Projects <paramref name="map"/> for drawing, or returns null when it does not describe a
    /// track that can be drawn: fewer than two points, no length, values that are not finite, or an
    /// extent of zero. A caller treats null as "no map yet" rather than as an error - an event has
    /// none until enough clean laps have been driven to learn one.
    /// </summary>
    /// <param name="map">The learned map to project.</param>
    /// <param name="trackName">Track name to stamp on the result, when the event records one.</param>
    public static TrackMapRender? Project(TrackMap? map, string? trackName = null)
    {
        // Points is a nil member away from being null on a deserialized map, and a stored length can
        // be NaN or infinite, neither of which a bare sign test catches.
        if (map is null || map.Points is not { Count: >= 2 } || !TrackGeometry.IsUsableLength(map.TotalLengthMeters))
            return null;

        var points = map.Points;
        double minLat = double.MaxValue, maxLat = double.MinValue;
        double minLon = double.MaxValue, maxLon = double.MinValue;

        foreach (var p in points)
        {
            // A persisted map is only as good as whatever wrote it. One corrupt value would
            // otherwise poison the bounds and collapse every projected point to NaN, so the map is
            // rejected outright rather than returned unusable. The distance along the path is
            // checked with the coordinates because consumers place cars with it.
            if (!double.IsFinite(p.Latitude) || !double.IsFinite(p.Longitude) ||
                !double.IsFinite(p.CumulativeDistanceMeters))
                return null;

            if (p.Latitude < minLat) minLat = p.Latitude;
            if (p.Latitude > maxLat) maxLat = p.Latitude;
            if (p.Longitude < minLon) minLon = p.Longitude;
            if (p.Longitude > maxLon) maxLon = p.Longitude;
        }

        // Every point in the same place. There is no outline to draw, and a consumer fitting it to a
        // viewport would divide by a zero extent and scale the whole map to NaN.
        if (minLat == maxLat && minLon == maxLon)
            return null;

        var centerLat = (minLat + maxLat) * 0.5;
        var centerLon = (minLon + maxLon) * 0.5;

        // One scale for the whole map, taken at its center. Using each point's own latitude would
        // stretch the north end of the track relative to the south.
        var metersPerDegreeLat = TrackGeometry.EarthRadiusMeters * Math.PI / 180.0;
        var metersPerDegreeLon = metersPerDegreeLat * Math.Cos(centerLat * Math.PI / 180.0);

        // Origin is the north-west corner and Y grows downward, so the result is already in screen
        // orientation and a consumer never has to flip an axis.
        double ToX(double lon) => (lon - minLon) * metersPerDegreeLon;
        double ToY(double lat) => (maxLat - lat) * metersPerDegreeLat;

        var rendered = new List<TrackMapRenderPoint>(points.Count);
        foreach (var p in points)
        {
            rendered.Add(new TrackMapRenderPoint
            {
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                X = ToX(p.Longitude),
                Y = ToY(p.Latitude),
                CumulativeDistanceMeters = p.CumulativeDistanceMeters,
            });
        }

        var render = new TrackMapRender
        {
            EventId = map.EventId,
            SessionId = map.SessionId,
            TrackName = trackName,
            Points = rendered,
            LengthMeters = map.TotalLengthMeters,
            BuiltUtc = map.BuiltUtc,
            Version = map.Version,
            Bounds = new TrackMapBounds
            {
                MinLatitude = minLat,
                MaxLatitude = maxLat,
                MinLongitude = minLon,
                MaxLongitude = maxLon,
                WidthMeters = ToX(maxLon),
                HeightMeters = ToY(minLat),
                CenterLatitude = centerLat,
                CenterLongitude = centerLon,
            },
        };

        if (map.StartFinishOffsetMeters is double offset &&
            TrackGeometry.PointAtDistance(points, map.TotalLengthMeters, offset) is { } sf)
        {
            render.StartFinish = new TrackMapStartFinish
            {
                Latitude = sf.Latitude,
                Longitude = sf.Longitude,
                X = ToX(sf.Longitude),
                Y = ToY(sf.Latitude),
                // The wrapped offset, not the raw one: the position was resolved from the wrapped
                // value, and a consumer compares this against a snapped car's distance along the
                // path, which is always inside the lap.
                DistanceAlongMeters = TrackGeometry.NormalizeDistance(offset, map.TotalLengthMeters),
                HeadingDegrees = sf.HeadingDegrees,
            };
        }

        return render;
    }
}
