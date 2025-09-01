using ProtoBuf;
using RedMist.TimingCommon.Models.InCarVideo;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Status data for a car.
/// </summary>
[ProtoContract]
public class CarPosition
{
    /// <summary>
    /// Used to indicate position information is not available.
    /// </summary>
    public const int InvalidPosition = -999;

    /// <summary>
    /// Redmist Event ID.
    /// </summary>
    [JsonPropertyName("eid")]
    [MaxLength(11)]
    [ProtoMember(1)]
    public string? EventId { get; set; }
    /// <summary>
    /// Current session ID as reported by the timing system.
    /// </summary>
    [JsonPropertyName("sid")]
    [MaxLength(11)]
    [ProtoMember(2)]
    public string? SessionId { get; set; }
    /// <summary>
    /// Car number which can include letters such as 99x.
    /// </summary>
    [JsonPropertyName("n")]
    [MaxLength(5)]
    [ProtoMember(3)]
    public string? Number { get; set; }
    /// <summary>
    /// Car's main transponder ID as indicated by the timing system.
    /// </summary>
    [JsonPropertyName("tp")]
    [ProtoMember(4)]
    public uint TransponderId { get; set; }
    /// <summary>
    /// Car's class as indicated by the timing system.
    /// </summary>
    [JsonPropertyName("class")]
    [MaxLength(40)]
    [ProtoMember(5)]
    public string? Class { get; set; }
    /// <summary>
    /// Car's best lap time formatted as HH:mm:ss.fff.
    /// </summary>
    [JsonPropertyName("bt")]
    [MaxLength(12)]
    [ProtoMember(6)]
    public string? BestTime { get; set; }
    /// <summary>
    /// Car's best lap number.
    /// </summary>
    [JsonPropertyName("bl")]
    [ProtoMember(7)]
    public int BestLap { get; set; }
    /// <summary>
    /// Whether the car set the best lap time of the session.
    /// </summary>
    [JsonPropertyName("ibt")]
    [ProtoMember(8)]
    public bool IsBestTime { get; set; }
    /// <summary>
    /// Whether the car set the best lap time in its class.
    /// </summary>
    [JsonPropertyName("btc")]
    [ProtoMember(9)]
    public bool IsBestTimeClass { get; set; }
    /// <summary>
    /// Time to the next car in the same class formatted as HH:mm:ss.fff.
    /// </summary>
    [JsonPropertyName("cg")]
    [MaxLength(12)]
    [ProtoMember(10)]
    public string? InClassGap { get; set; }
    /// <summary>
    /// Time to the in-class leader formatted as HH:mm:ss.fff.
    /// </summary>
    [JsonPropertyName("cd")]
    [MaxLength(12)]
    [ProtoMember(11)]
    public string? InClassDifference { get; set; }
    /// <summary>
    /// Time to the next car overall formatted as HH:mm:ss.fff.
    /// </summary>
    [JsonPropertyName("og")]
    [MaxLength(12)]
    [ProtoMember(12)]
    public string? OverallGap { get; set; }
    /// <summary>
    /// Time to the overall leader formatted as HH:mm:ss.fff.
    /// </summary>
    [JsonPropertyName("od")]
    [MaxLength(12)]
    [ProtoMember(13)]
    public string? OverallDifference { get; set; }
    /// <summary>
    /// Total race time formatted as HH:mm:ss.fff.
    /// </summary>
    [JsonPropertyName("ttm")]
    [MaxLength(12)]
    [ProtoMember(14)]
    public string? TotalTime { get; set; }
    /// <summary>
    /// Car's last lap time formatted as HH:mm:ss.fff.
    /// </summary>
    [JsonPropertyName("ltm")]
    [MaxLength(12)]
    [ProtoMember(15)]
    public string? LastLapTime { get; set; }
    /// <summary>
    /// Last lap number.
    /// </summary>
    [JsonPropertyName("llc")]
    [ProtoMember(16)]
    public int LastLapCompleted { get; set; }
    /// <summary>
    /// Number of times the car pitted. Null if not supported by the timing system.
    /// </summary>
    [JsonPropertyName("psc")]
    [ProtoMember(17)]
    public int? PitStopCount { get; set; }
    /// <summary>
    /// Last lap number the car pitted. Null if not supported by the timing system.
    /// </summary>
    [JsonPropertyName("llp")]
    [ProtoMember(18)]
    public int? LastLapPitted { get; set; }
    /// <summary>
    /// Laps completed by the car. Null if not supported by the timing system.
    /// </summary>
    [JsonPropertyName("llo")]
    [ProtoMember(19)]
    public int? LapsLedOverall { get; set; }
    /// <summary>
    /// Laps completed by the car in class. Null if not supported by the timing system.
    /// </summary>
    [JsonPropertyName("llic")]
    [ProtoMember(20)]
    public int? LapsLedInClass { get; set; }
    /// <summary>
    /// Car's overall position in the race by laps and as reported by the timing system.
    /// </summary>
    [JsonPropertyName("ovp")]
    [ProtoMember(21)]
    public int OverallPosition { get; set; }
    /// <summary>
    /// Car's position in-class.
    /// </summary>
    [JsonPropertyName("clp")]
    [ProtoMember(22)]
    public int ClassPosition { get; set; }
    /// <summary>
    /// Car's starting overall position inferred by the order the cars pass S/F at the start of the race or by the multiloop timing system if available.
    /// </summary>
    [JsonPropertyName("osp")]
    [ProtoMember(23)]
    public int OverallStartingPosition { get; set; }
    /// <summary>
    /// Number of position the car has gained overall. Negative number means positions lost.
    /// Value of -999 means not available.
    /// </summary>
    [JsonPropertyName("opg")]
    [ProtoMember(24)]
    public int OverallPositionsGained { get; set; } = InvalidPosition;
    /// <summary>
    /// Car's starting position in-class inferred by the order the cars pass S/F at the start of the race or by the multiloop timing system if available.
    /// </summary>
    [JsonPropertyName("icsp")]
    [ProtoMember(25)]
    public int InClassStartingPosition { get; set; }
    /// <summary>
    /// Number of position the car has gained in-class. Negative number means positions lost.
    /// Value of -999 means not available.
    /// </summary>
    [JsonPropertyName("cpg")]
    [ProtoMember(26)]
    public int InClassPositionsGained { get; set; } = InvalidPosition;
    /// <summary>
    /// This car has gained the most positions overall.
    /// </summary>
    [JsonPropertyName("ompg")]
    [ProtoMember(27)]
    public bool IsOverallMostPositionsGained { get; set; }
    /// <summary>
    /// This car has gained the most positions in-class.
    /// </summary>
    [JsonPropertyName("cmpg")]
    [ProtoMember(28)]
    public bool IsClassMostPositionsGained { get; set; }
    /// <summary>
    /// Laps lost due to penalties.
    /// </summary>
    [JsonPropertyName("pl")]
    [ProtoMember(29)]
    public int PenalityLaps { get; set; }
    /// <summary>
    /// Number of warnings issued.
    /// </summary>
    [JsonPropertyName("pw")]
    [ProtoMember(30)]
    public int PenalityWarnings { get; set; }
    /// <summary>
    /// Number of black flags applied to the car.
    /// </summary>
    [JsonPropertyName("bf")]
    [ProtoMember(31)]
    public int BlackFlags { get; set; }
    /// <summary>
    /// Car just passed the pit lane entry line.
    /// </summary>
    [JsonPropertyName("enp")]
    [ProtoMember(32)]
    public bool IsEnteredPit { get; set; }
    /// <summary>
    /// Car is just passed the pit lane start/finish line.
    /// </summary>
    [JsonPropertyName("psf")]
    [ProtoMember(33)]
    public bool IsPitStartFinish { get; set; }
    /// <summary>
    /// Car just passed the pit lane exit line.
    /// </summary>
    [JsonPropertyName("exp")]
    [ProtoMember(34)]
    public bool IsExitedPit { get; set; }
    /// <summary>
    /// Car is currently in pits.
    /// </summary>
    [JsonPropertyName("ip")]
    [ProtoMember(35)]
    public bool IsInPit { get; set; }
    /// <summary>
    /// Current lap includes a pit stop.
    /// </summary>
    [JsonPropertyName("lip")]
    [ProtoMember(36)]
    public bool LapIncludedPit { get; set; }
    /// <summary>
    /// Name of the last timing loop passed.
    /// </summary>
    [JsonPropertyName("ln")]
    [ProtoMember(37)]
    public string LastLoopName { get; set; } = string.Empty;
    /// <summary>
    /// Car position is stale and has not been updated for a while.
    /// </summary>
    [JsonPropertyName("st")]
    [ProtoMember(38)]
    public bool IsStale { get; set; }
    /// <summary>
    /// Flag active for the overall track.
    /// </summary>
    [JsonPropertyName("flg")]
    [ProtoMember(39)]
    public Flags TrackFlag { get; set; }
    /// <summary>
    /// Current local flag for the car. Requires specific in-car equipment.
    /// </summary>
    [JsonPropertyName("lflg")]
    [ProtoMember(40)]
    public Flags LocalFlag { get; set; }
    /// <summary>
    /// This lap had a local flag for the car. Requires specific in-car equipment.
    /// </summary>
    [JsonPropertyName("lhlf")]
    [ProtoMember(41)]
    public bool? LapHadLocalFlag { get; set; }
    /// <summary>
    /// Car's completed section for the current lap.
    /// </summary>
    [JsonPropertyName("csec")]
    [ProtoMember(42)]
    public List<CompletedSection> CompletedSections { get; set; } = [];
    /// <summary>
    /// Estimated lap time for the car.
    /// </summary>
    [JsonPropertyName("plt")]
    [ProtoMember(43)]
    public int ProjectedLapTimeMs { get; set; }
    /// <summary>
    /// Time at which the current lap started in UTC.
    /// </summary>
    [JsonPropertyName("lstt")]
    [ProtoMember(44)]
    public TimeOnly LapStartTime { get; set; }
    /// <summary>
    /// Current name of the driver.
    /// </summary>
    [JsonPropertyName("dn")]
    [ProtoMember(45)]
    public string DriverName { get; set; } = string.Empty;
    /// <summary>
    /// Current ID of the driver.
    /// </summary>
    [JsonPropertyName("did")]
    [ProtoMember(46)]
    public string DriverId { get; set; } = string.Empty;
    /// <summary>
    /// In-car video details if available.
    /// </summary>
    [JsonPropertyName("vid")]
    [ProtoMember(47)]
    public VideoStatus? InCarVideo { get; set; }
    /// <summary>
    /// Last reported latitude of the car in WGS84 spherical Mercator.
    /// </summary>
    [JsonPropertyName("lat")]
    [ProtoMember(48)]
    public double? Latitude { get; set; }
    /// <summary>
    /// Last reported longitude of the car in WGS84 spherical Mercator.
    /// </summary>
    [JsonPropertyName("lon")]
    [ProtoMember(49)]
    public double? Longitude { get; set; }
    /// <summary>
    /// Car may have been involved in an incident. Only available with certain in-car systems.
    /// </summary>
    [JsonPropertyName("iw")]
    [ProtoMember(50)]
    public bool ImpactWarning { get; set; }
}
