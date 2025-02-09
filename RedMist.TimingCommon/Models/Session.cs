using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class Session
{
    [JsonPropertyName("sid")]
    public int SessionId { get; set; }

    [JsonPropertyName("n")]
    public string SessionName { get; set; } = string.Empty;

    /// <summary>
    /// Session type: e.g. race, practice, qualifying, hpde, etc.
    /// </summary>
    [JsonPropertyName("t")]
    public string SessionType { get; set; } = string.Empty;
}
