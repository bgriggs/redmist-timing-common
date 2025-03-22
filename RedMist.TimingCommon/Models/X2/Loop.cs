namespace RedMist.TimingCommon.Models.X2;

public class Loop
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude0 { get; set; }
    public double Longitude0 { get; set; }
    public double Latitude1 { get; set; }
    public double Longitude1 { get; set; }
    public uint Order { get; set; }
    public bool IsInPit { get; set; }
    public bool IsOnline { get; set; }
    public bool HasActivity { get; set; }
    public bool IsSyncOk { get; set; }
    public bool HasDeviceWarnings { get; set; }
    public bool HasDeviceErrors { get; set; }
}
