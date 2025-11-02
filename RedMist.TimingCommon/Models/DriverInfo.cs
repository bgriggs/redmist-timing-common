using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Represents identifying and descriptive information for a driver entry in an event.
/// </summary>
/// <remarks>Either the combination of <c>EventId</c> and <c>CarNumber</c>, or <c>TransponderId</c>, must be
/// provided to uniquely identify a driver entry</remarks>
[MessagePackObject]
public class DriverInfo
{
    /// <summary>
    /// Redmist Event ID.
    /// Either EventId and CarNumber or TransponderId must be provided.
    /// </summary>
    [MaxLength(11)]
    [MessagePack.Key(0)]
    public int EventId { get; set; }

    /// <summary>
    /// Car number which can include letters such as 99x.
    /// Either EventId and CarNumber or TransponderId must be provided.
    /// </summary>
    [MaxLength(5)]
    [MessagePack.Key(1)]
    public string CarNumber { get; set; } = string.Empty;

    /// <summary>
    /// Car's main transponder ID as indicated by the timing system.
    /// Either EventId and CarNumber or TransponderId must be provided.
    /// </summary>
    [MessagePack.Key(2)]
    public uint TransponderId { get; set; }

    /// <summary>
    /// Gets or sets the name of the driver. Leave empty if to clear out.
    /// </summary>
    [MessagePack.Key(3)]
    [MaxLength(25)]
    public string DriverName { get; set; } = string.Empty;

    /// <summary>
    /// Optional ID of the driver to track across events.
    /// </summary>
    [MessagePack.Key(4)]
    [MaxLength(36)]
    public string DriverId { get; set; } = string.Empty;
}
