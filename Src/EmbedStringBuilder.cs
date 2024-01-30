using System.Text;
using Discord;

namespace FawxGameDigBot;

public static class EmbedBuilder {
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
}