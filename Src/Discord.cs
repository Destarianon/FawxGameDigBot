using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace FawxGameDigBot;

public class Discord {
    private DiscordSocketClient _client { get; set; }
    public bool IsReady { get; private set; }
    public ulong BotId { get; private set; }
    public ulong EmbedMessageId { get; private set; }

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

        IsReady = false;
    }
    
    private Task Log(LogMessage msg) {
        _logger.LogDebug(msg.ToString());
        return Task.CompletedTask;
    }

    private Task Ready() {
        IsReady = true;
        BotId = _client.CurrentUser.Id;
        _logger.LogDebug($"Connected to Discord with id {BotId}");
        GetExistingEmbedMessage();
        return Task.CompletedTask;
    }

    public async Task Start() {
        var token = _settings.Value.Discord.BotToken;
        _logger.LogDebug($"Using bot token ending in '******{token.Substring(token.Length-5)}'");
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await _client.SetCustomStatusAsync(_settings.Value.Discord.StatusMessage ?? "Digging...");
    }

    public async Task Stop() {
        await _client.StopAsync();
        await _client.LogoutAsync();
    }

    private async void GetExistingEmbedMessage() {
        _logger.LogDebug("Looking for an existing status message...");
        var channel = _client.GetChannel(_settings.Value.Discord.EmbedChannelId);
        if (channel is IMessageChannel mchannel) {
            var latestMessage = await mchannel.GetMessagesAsync(1).FlattenAsync();
            _logger.LogDebug($"Latest message in channel {channel.Id} is {latestMessage.Last().Id}");
            var searchMessages = await mchannel.GetMessagesAsync(100).FlattenAsync();

            var foundLast = false;
            foreach (var message in searchMessages) {
                if (message.Author.Id == BotId) {
                    foundLast = true;
                    EmbedMessageId = message.Id;
                    _logger.LogDebug($"Found existing status message {EmbedMessageId} in channel {channel.Id}");
                    break;
                }
            }
            if (!foundLast) {
                _logger.LogDebug($"Did not find an existing status message in channel {channel.Id}");
            }
        }
    }
    
    public async Task<ulong> SendMessage(ulong channelId, string messageContent) {
        if (channelId != 0) {
            var channel = _client.GetChannel(channelId);
            if (channel is IMessageChannel mchannel) {
                var message = await mchannel.SendMessageAsync(text: messageContent);
                _logger.LogDebug($"Sent message {message.Id} in channel {channelId}");
                return message.Id;  
            }
            _logger.LogError($"Failed to send a message in a non-text channel!");
        } else {
            _logger.LogDebug("Did not send a Discord message because no Channel Id was provided");
        }
        return 0;
    }
    
    private async Task<ulong> SendEmbedMessage(ulong channelId, Embed embed) {
        if (channelId != 0) {
            var channel = _client.GetChannel(channelId);
            if (channel is IMessageChannel mchannel) {
                var message = await mchannel.SendMessageAsync(embed: embed);
                _logger.LogDebug($"Sent embedded message {message.Id} in channel {channelId}");
                return message.Id;
            }
            _logger.LogError($"Failed to send embedded message in a non-text channel!");
        } else {
            _logger.LogWarning("Did not create an embed status message because no Channel Id was provided!");
        }
        return 0;
    }

    private async void UpdateEmbedMessage(ulong channelId, ulong messageId, Embed embed) {
        if (channelId != 0) {
            var channel = _client.GetChannel(channelId);
            if (channel is IMessageChannel mchannel) {
                var message = await mchannel.GetMessageAsync(messageId);
                if (message is IUserMessage umessage) {
                    await umessage.ModifyAsync(x => x.Embed = embed);
                    _logger.LogDebug($"Updated embedded message {message.Id} in channel {channelId}");
                }
            } else {
                _logger.LogError($"Failed to update a message in a non-text channel!");
            }
        } else {
            _logger.LogWarning("Did not update embed status message because no Channel Id was provided!");
        }
    }

    public async void UpdateStatusMessage(Embed embed) {
        if (EmbedMessageId != 0) {
            UpdateEmbedMessage(_settings.Value.Discord.EmbedChannelId, EmbedMessageId, embed);
        } else {
            _logger.LogInformation("No existing status message found, creating a new one!");
            EmbedMessageId = await SendEmbedMessage(_settings.Value.Discord.EmbedChannelId, embed);
        }
    }
}
