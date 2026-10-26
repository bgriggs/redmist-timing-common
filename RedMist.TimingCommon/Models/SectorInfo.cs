using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// One sector of the lap, as listed in <see cref="SessionState.SectorLayout"/>.
/// </summary>
[MessagePackObject]
public class SectorInfo
{
    /// <summary>
    /// Which sector this is, from 1. Matches <see cref="SectorTime.Index"/>.
    /// </summary>
    [JsonPropertyName("i")]
    [MessagePack.Key(0)]
    public int Index { get; set; }
    /// <summary>
    /// Column heading for the sector, normally "S1", "S2" and so on.
    /// </summary>
    [JsonPropertyName("lbl")]
    [MaxLength(8)]
    [MessagePack.Key(1)]
    public string Label { get; set; } = string.Empty;
    /// <summary>
    /// Length of the sector in meters, when it is known. Clients use it to size each sector's share
    /// of the lap, but only when every sector in the layout has a length; otherwise all sectors are
    /// drawn as equal shares.
    /// </summary>
    [JsonPropertyName("len")]
    [MessagePack.Key(2)]
    public double? LengthMeters { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is SectorInfo other &&
               Index == other.Index &&
               Label == other.Label &&
               Nullable.Equals(LengthMeters, other.LengthMeters);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index, Label, LengthMeters);
    }
}
