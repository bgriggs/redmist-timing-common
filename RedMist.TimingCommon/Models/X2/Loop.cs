using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models.X2;

/// <summary>
/// Represents an X2 timing loop.
/// </summary>
[MessagePackObject]
public class Loop
{
    [MessagePack.Key(0)]
    public int OrganizationId { get; set; }
    [MessagePack.Key(1)]
    public int EventId { get; set; }
    [MessagePack.Key(2)]
    public uint Id { get; set; }
    [MaxLength(20)] // X2 max is 16
    [MessagePack.Key(3)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(64)] // X2 max is 52
    [MessagePack.Key(4)]
    public string Description { get; set; } = string.Empty;
    [MessagePack.Key(5)]
    public double Latitude0 { get; set; }
    [MessagePack.Key(6)]
    public double Longitude0 { get; set; }
    [MessagePack.Key(7)]
    public double Latitude1 { get; set; }
    [MessagePack.Key(8)]
    public double Longitude1 { get; set; }
    [MessagePack.Key(9)]
    public uint Order { get; set; }
    [MessagePack.Key(10)]
    public bool IsInPit { get; set; }
    [MessagePack.Key(11)]
    public bool IsOnline { get; set; }
    [MessagePack.Key(12)]
    public bool HasActivity { get; set; }
    [MessagePack.Key(13)]
    public bool IsSyncOk { get; set; }
    [MessagePack.Key(14)]
    public bool HasDeviceWarnings { get; set; }
    [MessagePack.Key(15)]
    public bool HasDeviceErrors { get; set; }
}
