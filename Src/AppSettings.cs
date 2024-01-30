namespace FawxGameDigBot;

public sealed class AppSettings {
    public ulong RefreshInterval { get; set; }
    public DiscordConfig Discord { get; set; }
    public List<GameServer> GameServers { get; set; }

    public sealed class DiscordConfig {
        public string BotToken { get; set; }
        public string? StatusMessage { get; set; }
        public ulong EmbedChannelId { get; set; }
        public string? EmbedMessageTitle { get; set; }
        public string? EmbedMessageColor { get; set; }
    }
}