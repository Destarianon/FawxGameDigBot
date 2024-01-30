using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace FawxGameDigBot;

public class Discord {
    private DiscordSocketClient _client { get; set; }
    public bool IsReady { get; private set; } = false;
    public ulong BotId { get; private set; }
    public ulong StatusMessageId { get; private set; }

    private ILogger<Discord> _logger;
    private IOptions<AppSettings> _settings;
    
    public Discord(ILogger<Discord> logger, IOptions<AppSettings> settings) {
        _logger = logger;
        _settings = settings;
        
        var discordSocketConfig = new DiscordSocketConfig {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 1000,
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
        };
        
        _client = new DiscordSocketClient(discordSocketConfig);
        _client.Log += Log;
        _client.Ready += Ready;
    }
    
    private Task Log(LogMessage msg) {
        _logger.LogDebug(msg.ToString());
        return Task.CompletedTask;
    }

    private Task Ready() {
        IsReady = true;
        BotId = _client.CurrentUser.Id;
        _logger.LogDebug($"Connected to Discord with id {BotId}");
        return Task.CompletedTask;
    }

    public async Task Start() {
        await _client.LoginAsync(TokenType.Bot, _settings.Value.Discord.BotToken);
        await _client.StartAsync();
        await _client.SetCustomStatusAsync(_settings.Value.Discord.StatusMessage ?? "Digging...");
    }
}