using System.Text.Json.Serialization;

namespace RedMist.TimingCommon.Models;

public class CarHistory
{
    [JsonPropertyName("cid")]
    public string CarId { get; set; } = string.Empty;
    [JsonPropertyName("laps")]
    public List<Lap> Laps { get; set; } = [];
}
