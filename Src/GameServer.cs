using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace FawxGameDigBot;

public sealed class GameServer() {
    public string? Name { get; set; }
    public string? GameType { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public ulong DiscordChannelId { get; set; } = 0;
    public string StatusTemplate { get; set; } = "";
    
    [JsonIgnore]
    public DigResponse? LastResponse { get; set; }
    
    [JsonIgnore]
    public DigResponse? CurrentResponse { get; set; }

    public string ConnectionString() {
        return $"{Host}:{Port}";
    }
    
    public bool Online() {
        return CurrentResponse?.online ?? false;
    }

    public string GetEmbedString(string statusTemplate, DigResponse? response) {

        DigResponse _response;
        if (response == null) {
            if (CurrentResponse == null) {
                return "No response yet";
            } else {
                _response = CurrentResponse;
            }
        } else {
            _response = response;
        }
        
        return Online() ? MessageBuilder.BuildString(_response, statusTemplate) : "Server is offline";
    }

    public async Task<DigResponse> Dig(GameDig dig) {
        LastResponse = CurrentResponse;
        CurrentResponse = await dig.Dig(this);
        return CurrentResponse;
    }

    public bool HasStatusChanged() {
        if (CurrentResponse != null && LastResponse != null) {
            return CurrentResponse.online != LastResponse.online;
        }
        return false;
    }
}