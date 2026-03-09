using MessagePack;

namespace RedMist.TimingCommon.Models;

/// <summary>
/// Data for use or display by the relay server. This is not intended to be used for timing data,
/// but rather for other information that may be useful to the relay server and its users.
/// </summary>
[MessagePackObject]
public class RelayTelemetry
{
    [Key(0)]
    public List<ServiceStatus> ServiceStatuses { get; set; } = [];
    [Key(1)]
    public List<EventConnectionStatus> EventConnections { get; set; } = [];
}

[MessagePackObject]
public class ServiceStatus
{
    [Key(0)]
    public required string ServiceName { get; set; }
    [Key(1)]
    public required string Status { get; set; }
}

[MessagePackObject]
public class EventConnectionStatus
{
    [Key(0)]
    public required string ClientApplication { get; set; }
    [Key(1)]
    public int Clients { get; set; }
}