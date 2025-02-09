using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class CarPosition
{
    [JsonPropertyName("eid")]
    public string? EventId { get; set; }
    
    [JsonPropertyName("sid")]
    public string? SessionId { get; set; }

    [JsonPropertyName("n")]
    public string? Number { get; set; }

    [JsonPropertyName("bt")]
    public string? BestTime { get; set; }

    [JsonPropertyName("ibt")]
    public bool IsBestTime { get; set; }

    [JsonPropertyName("btc")]
    public bool IsBestTimeClass { get; set; }

    [JsonPropertyName("g")]
    public string? Gap { get; set; }

    [JsonPropertyName("d")]
    public string? Difference { get; set; }

    [JsonPropertyName("ttm")]
    public string? TotalTime { get; set; }

    [JsonPropertyName("ltm")]
    public string? LastTime { get; set; }

    [JsonPropertyName("llp")]
    public int LastLap { get; set; }

    [JsonPropertyName("ovp")]
    public int OverallPosition { get; set; }

    [JsonPropertyName("clp")]
    public int ClassPosition { get; set; }

    [JsonPropertyName("opg")]
    public int OverallPositionsGained { get; set; }

    [JsonPropertyName("cpg")]
    public int InClassPositionsGained { get; set; }

    [JsonPropertyName("ompg")]
    public bool IsOverallMostPositionsGained { get; set; }

    [JsonPropertyName("cmpg")]
    public bool IsClassMostPositionsGained { get; set; }

    [JsonPropertyName("of")]
    public bool IsOverallFastest { get; set; }

    [JsonPropertyName("cf")]
    public bool IsClassFastest { get; set; }

    [JsonPropertyName("pl")]
    public int PenalityLaps { get; set; }

    [JsonPropertyName("pw")]
    public int PenalityWarnings { get; set; }

    [JsonPropertyName("enp")]
    public bool IsEnteredPit { get; set; }

    [JsonPropertyName("exp")]
    public bool IsExistedPit { get; set; }

    [JsonPropertyName("ip")]
    public bool IsInPit { get; set; }
}
