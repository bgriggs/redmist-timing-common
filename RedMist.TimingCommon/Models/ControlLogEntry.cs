namespace RedMist.TimingCommon.Models;

public class ControlLogEntry
{
    public int OrderId { get; set; }

    public DateTime Timestamp { get; set; }

    public string Corner { get; set; } = string.Empty;

    public string Car1 { get; set; } = string.Empty;

    public string Car2 { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string PenalityAction { get; set; } = string.Empty;

    public string OtherNotes { get; set; } = string.Empty;
}
