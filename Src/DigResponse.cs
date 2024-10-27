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
    public string? version { get; set; }
    public List<string> players { get; set; }
    public string? connect { get; set; }
    public int ping { get; set; }
    public int queryPort { get; set; }
    public Dictionary<string, object>? raw { get; set; }

    [JsonIgnore]
    public string? detailedstatus { get; set;}
    
    public virtual void ParseRaw(ILogger logger) {}
}

public sealed class PalworldDigResponse : DigResponse {
    
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

public sealed class SatisfactoryDigResponse : DigResponse {

    [JsonIgnore]
    public string? sessionname { get; set; }

    [JsonIgnore]
    public int techtier { get; set; }

    [JsonIgnore]
    public bool paused { get; set; }

    [JsonIgnore]
    public bool running { get; set; }

    [JsonIgnore]
    public string? sessionstate { get; set; }

    public Dictionary<string,object>? http { get; set; }
    public Dictionary<string,object>? serverGameState { get; set; }

    public override void ParseRaw(ILogger logger) {
        if (raw != null && raw.TryGetValue("http", out var jsonObjectHttp)) {
            http = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObjectHttp.ToString());
            if (http != null && http.TryGetValue("serverGameState", out var jsonObjectState)) {
                serverGameState = JsonSerializer.Deserialize<Dictionary<string,object>>(jsonObjectState.ToString());
                sessionname = serverGameState["activeSessionName"].ToString();
                techtier = int.Parse(serverGameState["techTier"].ToString());
                paused = bool.Parse(serverGameState["isGamePaused"].ToString());
                running = bool.Parse(serverGameState["isGameRunning"].ToString());
                if(running) {
                    if (paused) {
                        sessionstate = "Session is paused";
                        detailedstatus = "[:pause_button: Paused]";
                    } else {
                        sessionstate = "Session is running";
                        detailedstatus = "";
                    }
                } else {
                    sessionstate = "Session not started";
                    detailedstatus = "[:gear: Not started]";
                }
            }
        }
    }
}