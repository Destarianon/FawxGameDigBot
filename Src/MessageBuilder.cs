﻿using System.Runtime.CompilerServices;
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

        // Game specific fields
        if (response is PalworldDigResponse tresponse) {
            builder.Replace("{version}", tresponse.version);
            builder.Replace("{days}", tresponse.days.ToString());
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

        embed.WithColor(Color.Orange);
        embed.WithCurrentTimestamp();
        return embed.Build();
    }
}