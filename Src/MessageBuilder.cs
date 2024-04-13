using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Discord;
using Microsoft.Extensions.Options;

namespace FawxGameDigBot;

public static class MessageBuilder {
    public static string BuildString<T> (T response, string statusTemplate) where T : DigResponse {
        var builder = new StringBuilder(statusTemplate);
        builder.Replace("{name}", response.name);
        builder.Replace("{map}", response.map);
        builder.Replace("{numplayers}", response.numplayers.ToString());
        builder.Replace("{maxplayers}", response.maxplayers.ToString());
        builder.Replace("{connect}", response.connect);
        builder.Replace("{ping}", response.ping.ToString());
        builder.Replace("{queryPort}", response.queryPort.ToString());
        var playersString = "[";
        for (int i = 0; i < response.players.Count; i++) {
            playersString += $"{response.players[i]}";
            if (!(i == response.players.Count - 1)) {
                playersString += ", ";
            }
        }
        playersString += "]";
        builder.Replace("{players}", playersString);

        // Game specific fields
        if (response is PalworldDigResponse tresponse) {
            builder.Replace("{version}", tresponse.version);
            builder.Replace("{serverfps}", tresponse.serverfps.ToString());
            builder.Replace("{uptime}", tresponse.uptime.ToString());
        }

        return builder.ToString();
    }

    public static Embed BuildEmbed(IEnumerable<GameServer> servers,  AppSettings settings) {
        DateTimeOffset dto = new DateTimeOffset(DateTime.Now);
        var embed = new EmbedBuilder {
            Title = settings.Discord.EmbedMessageTitle,
            Description =
                $"Next update <t:{dto.AddSeconds(settings.RefreshInterval + 1).ToUnixTimeSeconds()}:R>"
        };
        foreach (var server in servers) {
            string status;
            status = server.Online() ? ":green_square: Online" : ":red_square: Offline";

            embed.AddField($"{server.Name}: {status}", $"```{server.GetEmbedString(server.StatusTemplate,server.LastResponse)}```");
        }

        var colorString = "E67E22";
        if (settings.Discord.EmbedMessageColor != null) {
            colorString = settings.Discord.EmbedMessageColor.Replace("#", "").Trim();
        }
        var color = uint.Parse(colorString, System.Globalization.NumberStyles.HexNumber);
        
        embed.WithColor(new Color(color));
        embed.WithCurrentTimestamp();
        return embed.Build();
    }
}