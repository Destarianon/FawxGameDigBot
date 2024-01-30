namespace FawxGameDigBot;

public sealed class GameServer {
    public string? Name { get; set; }
    public string? GameType { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public ulong DiscordChannelId { get; set; } = 0;
    public string StatusTemplate { get; set; } = "No status template set";
    public DigResponse? LastResponse { get; set; }

    public string ConnectionString() {
        return $"{Host}:{Port}";
    }
    
    public bool Online() {
        return LastResponse?.online ?? false;
    }

    public string GetEmbedString(string statusTemplate, DigResponse? response) {

        DigResponse _response;
        if (response == null) {
            if (LastResponse == null) {
                return "No response yet";
            } else {
                _response = LastResponse;
            }
        } else {
            _response = response;
        }
        
        return Online() ? EmbedBuilder.BuildString(_response, statusTemplate) : "Server is offline";
    }
}