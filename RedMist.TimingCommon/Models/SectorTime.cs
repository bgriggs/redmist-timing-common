using MessagePack;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// A car's most recent time through one sector of the lap. A car carries at most one of these per
/// sector, replaced each time it completes that sector again, so while it is partway round a lap the
/// sectors it has not reached yet still hold its previous lap's times. Compare <see cref="Lap"/>
/// with <see cref="CarPosition.LastLapCompleted"/> to tell them apart: a sector whose lap is greater
/// belongs to the lap the car is on now.
///
/// The best flags are worked out on the server for every source alike, and they nest: an overall
/// best is also the best in its class, and a class best is also the car's own best. Clients color a
/// time as the session best (purple) when <see cref="IsOverallBest"/> is set and the field is shown
/// overall, or when <see cref="IsClassBest"/> is set and it is grouped by class; failing that, as the
/// car's own best (green) when <see cref="IsPersonalBest"/> is set.
/// </summary>
[MessagePackObject]
public class SectorTime
{
    /// <summary>
    /// Which sector this is, from 1 to the number of sectors in
    /// <see cref="SessionState.SectorLayout"/>.
    /// </summary>
    [JsonPropertyName("i")]
    [Key(0)]
    public int Index { get; set; }
    /// <summary>
    /// The lap this sector time belongs to: the lap the car was on when it started the sector.
    /// </summary>
    [JsonPropertyName("l")]
    [Key(1)]
    public int Lap { get; set; }
    /// <summary>
    /// Time through the sector in milliseconds.
    /// </summary>
    [JsonPropertyName("t")]
    [Key(2)]
    public int TimeMs { get; set; }
    /// <summary>
    /// The time is shown but does not count towards any best: the car went through the pit lane
    /// during the sector, or the source marked the time as not valid. None of the best flags are
    /// set when this is.
    /// </summary>
    [JsonPropertyName("inv")]
    [Key(3)]
    public bool IsInvalid { get; set; }
    /// <summary>
    /// This is the car's own best time in this sector this session. Also set whenever
    /// <see cref="IsClassBest"/> is.
    /// </summary>
    [JsonPropertyName("pb")]
    [Key(4)]
    public bool IsPersonalBest { get; set; }
    /// <summary>
    /// This is the best time in this sector this session across all classes. When set,
    /// <see cref="IsClassBest"/> and <see cref="IsPersonalBest"/> are too.
    /// </summary>
    [JsonPropertyName("ob")]
    [Key(5)]
    public bool IsOverallBest { get; set; }
    /// <summary>
    /// This is the best time in this sector this session within the car's class. Also set whenever
    /// <see cref="IsOverallBest"/> is.
    /// </summary>
    [JsonPropertyName("cb")]
    [Key(6)]
    public bool IsClassBest { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is SectorTime other &&
               Index == other.Index &&
               Lap == other.Lap &&
               TimeMs == other.TimeMs &&
               IsInvalid == other.IsInvalid &&
               IsPersonalBest == other.IsPersonalBest &&
               IsOverallBest == other.IsOverallBest &&
               IsClassBest == other.IsClassBest;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index, Lap, TimeMs, IsInvalid, IsPersonalBest, IsOverallBest, IsClassBest);
    }
}
