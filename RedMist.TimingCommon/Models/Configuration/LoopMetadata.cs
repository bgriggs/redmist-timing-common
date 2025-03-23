namespace RedMist.TimingCommon.Models.Configuration;

public class LoopMetadata
{
    public uint Id { get; set; }
    public int EventId { get; set; }
    public LoopType Type { get; set; }
    public string Name { get; set; } = string.Empty;
}

public enum LoopType
{
    StartFinish,
    PitIn,
    PitExit,
    PitStartFinish,
    PitOther,
    Other
}
