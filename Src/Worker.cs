using Discord;
using Microsoft.Extensions.Options;

namespace FawxGameDigBot;

public class Worker(ILogger<Worker> logger, IOptions<AppSettings> settings, Discord discord, GameDig dig) : BackgroundService {
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {

        //wait forever ONLY FOR TESTING
        //await Task.Delay(-1, stoppingToken);
        
        await discord.Start();
        while (!discord.IsReady && !stoppingToken.IsCancellationRequested) {
            await Task.Delay(500, stoppingToken);
        }

        await discord.GetExistingEmbedMessage();
        
        var servers = settings.Value.GameServers;
        logger.LogInformation($"Found {servers.Count} servers to monitor");
        
        while (!stoppingToken.IsCancellationRequested) {
            
            foreach (GameServer server in servers) {
                var response = await server.Dig(dig);
                if (server.DiscordChannelId != 0 && server.HasStatusChanged()) {
                    if (server.Online()) {
                        logger.LogInformation($"Sending server restored message for '{server.Name}'");
                        await discord.SendMessage(server.DiscordChannelId, $"{server.Name} is back online!");
                    } else {
                        logger.LogWarning($"Sending server down message for '{server.Name}'");
                        await discord.SendMessage(server.DiscordChannelId, $"{server.Name} is offline!");
                    }
                }
            }

            var embed = MessageBuilder.BuildEmbed(servers, settings.Value);
            await discord.UpdateStatusMessage(embed);

            await Task.Delay((int)(settings.Value.RefreshInterval * 1000), stoppingToken);
        }
        
        await discord.Stop();
    }
}