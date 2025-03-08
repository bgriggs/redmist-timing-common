namespace RedMist.TimingCommon.Models.Configuration;

public class EventSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public bool IsActive { get; set; }
}
