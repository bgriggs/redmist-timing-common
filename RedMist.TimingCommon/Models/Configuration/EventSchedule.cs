namespace RedMist.TimingCommon.Models.Configuration;

public class EventSchedule
{
    public string Name { get; set; } = string.Empty;
    public List<EventScheduleEntry> Entries { get; set; } = new List<EventScheduleEntry>();
}
