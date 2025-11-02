using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// X2 loop information.
/// </summary>
[MessagePackObject]
public class LoopMetadata
{
    /// <summary>
    /// Gets or sets the unique identifier for this loop.
    /// </summary>
    [Key(0)]
    public uint Id { get; set; }
    
    /// <summary>
    /// Gets or sets the event identifier this loop is associated with.
    /// </summary>
    [Key(1)]
    public int EventId { get; set; }
    
    /// <summary>
    /// Gets or sets the type of timing loop.
    /// </summary>
    [Key(2)]
    public LoopType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the timing loop.
    /// Maximum length: 64 characters (X2 max is 52).
    /// </summary>
    [Key(3)]
    [MaxLength(64)] // X2 max is 52
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Defines the types of timing loops used in race timing systems.
/// </summary>
public enum LoopType
{
    /// <summary>
    /// Start/Finish line timing loop.
    /// </summary>
    StartFinish,
    
    /// <summary>
    /// Pit lane entry timing loop.
    /// </summary>
    PitIn,
    
    /// <summary>
    /// Pit lane exit timing loop.
    /// </summary>
    PitExit,
    
    /// <summary>
    /// Pit lane start/finish timing loop.
    /// </summary>
    PitStartFinish,
    
    /// <summary>
    /// Other pit related timing loop.
    /// </summary>
    PitOther,
    
    /// <summary>
    /// Other type of timing loop not otherwise categorized.
    /// </summary>
    Other
}
