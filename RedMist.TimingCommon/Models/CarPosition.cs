﻿using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class CarPosition
{
    [JsonPropertyName("eid")]
    public string? EventId { get; set; }
    
    [JsonPropertyName("sid")]
    public string? SessionId { get; set; }

    [JsonPropertyName("n")]
    public string? Number { get; set; }

    [JsonPropertyName("class")]
    public string? Class { get; set; }

    [JsonPropertyName("bt")]
    public string? BestTime { get; set; }

    [JsonPropertyName("bl")]
    public int BestLap { get; set; }

    [JsonPropertyName("ibt")]
    public bool IsBestTime { get; set; }

    [JsonPropertyName("btc")]
    public bool IsBestTimeClass { get; set; }

    [JsonPropertyName("cg")]
    public string? InClassGap { get; set; }

    [JsonPropertyName("cd")]
    public string? InClassDifference { get; set; }

    [JsonPropertyName("og")]
    public string? OverallGap { get; set; }

    [JsonPropertyName("od")]
    public string? OverallDifference { get; set; }

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

    [JsonPropertyName("osp")]
    public int OverallStartingPosition { get; set; }

    [JsonPropertyName("opg")]
    public int OverallPositionsGained { get; set; }

    [JsonPropertyName("icsp")]
    public int InClassStartingPosition { get; set; }

    [JsonPropertyName("cpg")]
    public int InClassPositionsGained { get; set; }

    [JsonPropertyName("ompg")]
    public bool IsOverallMostPositionsGained { get; set; }

    [JsonPropertyName("cmpg")]
    public bool IsClassMostPositionsGained { get; set; }

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

    [JsonPropertyName("st")]
    public bool IsStale { get; set; }
}
