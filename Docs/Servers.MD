# Server configuration
In your configuration `appsettings.json` file, the `GameServers` section contains an array of objects that define the servers the bot will be watching. Each object has the following properties:
```yaml
# trimmed for brevity
"GameServers": [
      {
        "Name": "Palworld Server 1",
        "GameType": "palworld",
        "Host": "palworld.example.org",
        "Port": 8212,
        "DiscordChannelId": "<discord channel id here>",
        "StatusTemplate": "{name}\nPlayers: {numplayers}/{maxplayers}"
      }
    ]
```
+ `Name` - A friendly name for the server used in status titles and up/down notifications.
+ `GameType` - The "id" of the game server used to determine protocol and available fields. See [Game Types](#game-types) for a list of supported games.
+ `Host` - The hostname or IP address of the server.
+ `Port` - The port the server is listening on. This is the port to query against, so for games with an RCON/API query method this port must be set to the query port.
+ `DiscordChannelId` - The Discord Channel Id of the channel to send up/down notifications to. If this is not defined, no up/down notifications will be sent for this server.
> [!IMPORTANT]
> Do not leave this field blank! To properly disable this functionality, either set the Channel Id to `0` or remove the line entirely.
+ `StatusTemplate` - A template string used to format the status message. See [Status Templates](#status-templates) for more information.
### Rest APIs
For servers which require credentials to query, such as Palworld's REST API, credential variables may be included. These will be ignored on unsupported games.
+ `username` - The username to authenticate for querying.
+ `password` - The password to authenticate for querying.

## Status Templates
The status template is a string that contains variable placeholders which the bot will replace with data from the monitored server. For example,
```yaml
"{name}\nPlayers: {numplayers}/{maxplayers}\nPing: {ping}ms"
```
would create a status message which looks like
```
Server 1
Players: 0/10
Ping: 23ms
```
Variables must be in curly braces `{}` and are case sensitive. The following variables are available:
+ `name` - The actual name of the server returned to a client.
+ `map` - The current map the server is running.
+ `password` - Whether or not the server is password protected.
+ `numplayers` - The number of players currently connected to the server.
+ `maxplayers` - The maximum number of players the server supports.
+ `players` - A list of players currently connected to the server, formatted as a comma-separated list.
+ `ping` - The ping time to the server in milliseconds.
+ `connect` - The connection information for the server. Ex "server.example.org:3000". This will mirror the connection string, so for RCON/API queried servers add this information as text to prevent advertising the wrong port number.
+ `queryPort` - The port number used to query the server for information.

Line breaks can be included in the output of the status information using `\n`.

## Game Types
This bot can query any game supported by GameDig. A list of its supported games and their Type Ids can be found [here](https://github.com/gamedig/node-gamedig/blob/HEAD/GAMES_LIST.md).
Some games have extended support for custom fields unique to that title, listed below.

### Palworld (palworld)
+ `version` - The software version of the server.
+ `uptime` - The amount of time the server has been online (seconds?).
+ `serverfps` - The current server framerate.
+ Notes:
  + The `password` variable is unsupported at this time and will always return `false`