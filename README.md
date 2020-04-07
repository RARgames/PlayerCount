#Player Count

Show customizable player count message with custom server icon, prefix and commands.

In `OnlineMessage` you can use tags:
- `{onlinePlayers}` - for number of currently connected players
- `{maxPlayers}` - for number of server max slots
- `{joiningPlayers}` - for number of currently joining players
- `{queuedPlayers}` - for number of currently queued players

As a `Prefix` you can use anything including color codes: `<color=#FFA500></color>`

To change a custom icon use `SteamAvatarId`. It has to be id of a steam profile. 0 for default icon.

`CommandList` allows to define list of commands (F1 console commands, chat `/` commands and chat` !` commands)

You can use this plugin with BetterChat
