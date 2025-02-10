using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RedMist.TimingCommon.Models;

public class CarHistory
{
    [JsonPropertyName("cid")]
    public string CarId { get; set; } = string.Empty;
    [JsonPropertyName("laps")]
    public List<Lap> Laps { get; set; } = [];
}
