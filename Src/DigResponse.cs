using System.Text.Json;
using System.Text.Json.Serialization;

namespace FawxGameDigBot;

public class DigResponse {
    
    [JsonIgnore]
    public bool online { get; set; }
    public string? name { get; set; }
    public string? map { get; set; }
    public bool password { get; set; }
    public int numplayers { get; set; }
    public int maxplayers { get; set; }
    public List<string> players { get; set; }
    public string? connect { get; set; }
    public int ping { get; set; }
    public int queryPort { get; set; }
    public Dictionary<string, object>? raw { get; set; }
    
    public virtual void ParseRaw(ILogger logger) {}
}

public sealed class PalworldDigResponse : DigResponse {
    
    public string? version { get; set; }
    
    [JsonIgnore]
    public int serverfps { get; set; }
    
    [JsonIgnore]
    public int uptime { get; set; }
    
    public Dictionary<string,object>? metrics { get; set; }
    
    public Dictionary<string,object>? settings { get; set; }

    public override void ParseRaw(ILogger logger) {
        if (raw != null && raw.TryGetValue("metrics", out var jsonObject)) {
            //logger.LogDebug($"attributes json: {jsonObject}");
            metrics = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject.ToString());
            serverfps = int.Parse(metrics["serverfps"].ToString());
            uptime = int.Parse(metrics["uptime"].ToString());
        }
    }
}