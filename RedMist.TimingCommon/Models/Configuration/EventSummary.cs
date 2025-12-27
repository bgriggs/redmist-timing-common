using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace RedMist.TimingCommon.Models.Configuration;

/// <summary>
/// Event summary used for populating selection list/dropdown in the Relay
/// </summary>
[MessagePackObject]
public class EventSummary
{
    /// <summary>
    /// Gets or sets the unique identifier for this event.
    /// </summary>
    [Key(0)]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the event.
    /// Maximum length: 128 characters.
    /// </summary>
    [Key(1)]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the start date and time of the event.
    /// </summary>
    [Key(2)]
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether this event is active, i.e. selected in the relay dropdown.
    /// </summary>
    [Key(3)]
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates whether this is a real event or a simulation/replay/test event.
    /// </summary>
    [Key(4)]
    public bool IsSimulation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this session is historical or archived.
    /// </summary>
    [Key(5)]
    public bool IsArchived { get; set; }
}
