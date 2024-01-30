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
    public string? connect { get; set; }
    public int ping { get; set; }
    public int queryPort { get; set; }
    public Dictionary<string, object>? raw { get; set; }
    
    public virtual void ParseRaw() {}
}

public sealed class PalworldDigResponse : DigResponse {
    
    [JsonIgnore]
    public string? version { get; set; }
    
    [JsonIgnore]
    public int days { get; set; }
    
    public Dictionary<string,object>? attributes { get; set; }

    public override void ParseRaw() {
        if (raw != null && raw.TryGetValue("attributes", out var jsonObject)) {
            attributes = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObject.ToString());
            numplayers = (int)attributes["numplayers"];
            version = (string)attributes["version"];
            days = (int)attributes["days"];
        }
    }
}