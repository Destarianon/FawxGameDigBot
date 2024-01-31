# FAWX GameDig Bot

---
This Discord bot provides a configurable solution for game server montioring
by providing a unified embedded status message for all your
servers, as well as up/down notifications in game specific text channels.

![Embedded Message Sample](Docs/Images/sample-embed.png?raw=true "The Status Message")

### Built using:
+ .Net 8.0
+ NodeJS LTS (20.x at time of writing)
+ Discord.Net
+ GameDig
+ Docker


# Features:
+ Utilizes the GameDig library which supports hundreds of games and services. More info [here](https://github.com/gamedig/node-gamedig/tree/master)
+ Highly configurable, including a customizable statistics format per server.
+ Optionally sends up/down notifications to a game-specific channel.
+ Easy to deploy and lightweight.

---
# Usage:
This will outline the basics of deploying the bot to a single docker host.
> Support for K8S, Swarm or other advanced configurations will **not** be provided.


## Create the Bot registration
First, create a Discord bot registration. Instructions to do so are [here](https://discordpy.readthedocs.io/en/stable/discord.html). When selecting scopes and permissions use the following settings:
+ Scopes: `bot`


![Scopes example](Docs/Images/sample-scopes.png?raw=true "Bot scopes settings")


+ Bot Permissions: `Send Messages`, `Embed Links`, `Attach Files`, `Read Message History`


![Permissions example](Docs/Images/sample-permissions.png?raw=true "Bot permissions settings")

## Add it to the server
After inviting the bot to your server, add it to a `status` channel and any `game` channels. The bot will use the `status` channel to post a unified status message for all servers, and will use the `game` channels to post up/down notifications for each server.
+ The `status` channel should not be used for anything other than status messages. If there are too many messages in this channel, the bot will lose track of the existing statues message and may create duplicates.
+ The `game` channels are optional. If they are not configured for a game server, no up/down notifications will be sent.
+ You can specify the same `game` channel for all servers to send up/down notifications to a single location if you want.

Record the IDs of the channels you are using for the bot. You will need them shortly.
To get the Channel Id, open Discord, go to Settings > Advanced and enable developer mode. Then, right-click on the channel and select "Copy ID" to get the Channel Id.

### Permissions
The bot will need the following permissions to function properly:
+ Send Messages
+ Embed Links
+ Attach Files
+ Read Message History

## Create a configuration file
1. Create an `appsetting.json` file using the contents of the provided [appsettings.example.json](appsettings.example.json) file as an example. Store this configuration in a secure place on your docker host.
2. In the .json file update the value of `BotToken` with the Bot Token from your registration.
> [!WARNING]
> Do not use the Client Secret from your App registration. They are not the same thing!*
3. Update the `EmbedChannelId` with the channel id of your status channel.
4. Customize the `EmbedMessageTitle` or `EmbedMessageColor` to adjust the appearance of the status message.
5. Define the servers the bot will be watching using the format described [here](Docs/Servers.MD)

## Deploy
The **recommended** way to deploy the bot is using docker compose. A sample docker compose file is provided [here](docker-compose.yml). You will need to update the `volumes` section to point to the location of your `appsettings.json` file.
```yaml
services:
  fawxgamedigbot:
    container_name: gamedigbot
    image: destarianon/fawxgamedigbot
    restart: unless-stopped
    volumes:
      - ./appsettings.json:/app/appsettings.json #Assumes appsettings.json is in the same directory as the docker-compose.yml file
```

Once you've created your `docker-compose.yml` file, you can deploy it using the following command
```shell
docker-compose up -d
```

---
# FAQ:
### My bot is creating multiple embedded status messages.
+ Make sure the status channel is not being used for anything other than dynamic status messages. If there are too many messages in this channel, the bot will lose track and may create duplicates.

### My bot is crashing on startup.
+ Make sure you have an `appsettings.json` file mounted to the docker container in the correct place.
+ Make sure your configuration file is valid JSON and all the settings are correct. You can use a tool like [JSONLint](https://jsonlint.com/) to validate your configuration file.
+ You can use the `docker logs` command to view the logs from the bot container. This may provide more information about the error.
+ You can update the "Default" log level in the `appsettings.json` file to `Debug` to get more detailed logging.

### My bot is failing to contact my servers.
+ Make sure the docker host has network access to the servers you are monitoring.
+ Make sure you are using the right port number for the server you are monitoring.
+ In some cases, you made need to configure docker to use "host networking" because the bot requires an open UDP port to receive game responses. 

Details of host networking can be found [here](https://docs.docker.com/network/host/). You can enable host networking for the bot container by adding the following to the container definition in your `docker-compose.yml` file:
```yaml 
network_mode: "host"
```

### My bot is not sending any messages.
+ Make sure the bot has the correct permissions in the channels you are using.
+ Make sure the configuration file contains the correct Channel IDs.
+ Make sure the bot is not muted.

### My server is missing from the status message.
+ Make sure the server is properly configured in the `appsettings.json` file.
+ If you have disabled per-game notification, make sure you did so as described in the [server configuration guide](Docs/Servers.MD#server-configuration).