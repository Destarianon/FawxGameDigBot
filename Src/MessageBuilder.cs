using System.Text;
using Discord;

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
        builder.Replace("{version}", response.version);
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
        if (response is PalworldDigResponse palworld_response) {
            builder.Replace("{serverfps}", palworld_response.serverfps.ToString());
            builder.Replace("{uptime}", palworld_response.uptime.ToString());
        } else if (response is SatisfactoryDigResponse satisfactory_response) {
            builder.Replace("{sessionname}", satisfactory_response.sessionname);
            builder.Replace("{techtier}", satisfactory_response.techtier.ToString());
            builder.Replace("{sessionstate}", satisfactory_response.sessionstate);
            builder.Replace("{running}", satisfactory_response.running.ToString());
            builder.Replace("{paused}", satisfactory_response.paused.ToString());
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

            // Detailed status message
            string detailedstatus = "";
            if (server.ShowDetailedStatus) {
                if (server.CurrentResponse != null && server.CurrentResponse.detailedstatus != null) {
                    detailedstatus = server.CurrentResponse.detailedstatus;
                }
            }

            // Lock icon based on password protection status
            string locked = "";
            if(server.ShowLocked != null && server.CurrentResponse != null) {
                locked = server.ShowLocked switch {
                    "hide" => "",
                    "locked" => server.CurrentResponse.password ? ":lock:" : "",
                    "dynamic" => server.CurrentResponse.password ? ":lock:" : ":unlock:",
                    "alwayslocked" => ":lock:",
                    "alwaysunlocked" => ":unlock:",
                    "text" => server.CurrentResponse.password ? "Password protected" : "Open",
                    "text_locked" => server.CurrentResponse.password ? "Password protected" : "",
                    _ => ""
                };
            }

            embed.AddField($"{server.Name}: {status} {detailedstatus} {locked}", $"```{server.GetEmbedString(server.StatusTemplate,server.CurrentResponse)}```");
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