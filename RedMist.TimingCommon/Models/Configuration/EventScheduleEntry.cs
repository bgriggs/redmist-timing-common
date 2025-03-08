namespace RedMist.TimingCommon.Models.Configuration;

public class EventScheduleEntry
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Name { get; set; } = string.Empty;
}
