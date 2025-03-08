using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models.Configuration;

public class Event
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int OrganizationId { get; set; }
    [MaxLength(512)]
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    [MaxLength(512)]
    public string EventUrl{get;set; } = string.Empty;
    public EventSchedule Schedule { get; set; } = new EventSchedule();
    public bool EnableSourceDataLogging { get; set; } = true;
    [MaxLength(128)]
    public string TrackName { get; set; } = string.Empty;
    [MaxLength(64)]
    public string CourseConfiguration { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Distance { get; set; } = string.Empty;
    public BroadcasterConfig Broadcast { get; set; } = new BroadcasterConfig();
}
