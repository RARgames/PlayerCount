using System;
using System.Linq;
using Oxide.Core.Libraries.Covalence;
using System.Collections.Generic;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("PlayerCount", "[4ga] RAR", "1.5.0")]
    [Description("Shows players count")]
    class PlayerCount : CovalencePlugin
    {
        #region Initialization
        [PluginReference]
        private Plugin BetterChat;
        //TODO change fields to JSON property
        private string onlineMessage;
        private string commands;
        private string prefix;
        private string steamAvatarId;

        private string[] commandsArray;
        private ulong steamAvatarIdFormatted;

        private void Init()
        {
            LoadDefaultConfig();
            commandsArray = commands.Split(';'); //Split commands into string array
            if (commandsArray[commandsArray.Length - 1] == "") //Remove last array element if empty
            {
                commandsArray = commandsArray.Take(commandsArray.Length - 1).ToArray();
            }
            foreach (var c in commandsArray)
            {
                AddCovalenceCommand(c, "OnlineCommand"); //Register all commands
            }
            steamAvatarIdFormatted = ulong.Parse(steamAvatarId);
        }

        protected override void LoadDefaultConfig()
        {
            Config["Message"] = onlineMessage = GetConfig("Online message", "{onlinePlayers}/{maxPlayers} Online ({joiningPlayers} Joining {queuedPlayers} Queued)");
            Config["Command List"] = commands = GetConfig("Command List", "online;players;pop;");
            Config["Prefix"] = prefix = GetConfig("Prefix", "<color=#FFA500>[4ga]</color>");
            Config["Steam Avatar Id"] = steamAvatarId = GetConfig("Steam Avatar Id", "76561198273760551");
            SaveConfig();
        }
        #endregion

        #region Commands
        private void OnlineCommand(IPlayer player)
        {
            string displayText = onlineMessage.Replace("{onlinePlayers}", players.Connected.Count().ToString())
                .Replace("{maxPlayers}", server.MaxPlayers.ToString())
                .Replace("{queuedPlayers}", ServerMgr.Instance.connectionQueue.Queued.ToString())
                .Replace("{joiningPlayers}", ServerMgr.Instance.connectionQueue.Joining.ToString());

            Message(player, prefix, displayText, steamAvatarIdFormatted);
        }

        private object OnUserChat(IPlayer player, string message)
        {
            if (BetterChat != null)
                return true;
            if (CheckIfCommand(player, message))
            {
                return true;
            }
            return null;
        }

        private object OnBetterChat(Dictionary<string, object> data)
        {
            IPlayer player = data["Player"] as IPlayer;
            string message = data["Message"] as string;

            if (CheckIfCommand(player, message))
            {
                return true;
            }
            return data;
        }

        private bool CheckIfCommand(IPlayer player, string message)
        {
            if (message.StartsWith("!"))
            {
                message = message.Substring(1);
                foreach (var c in commandsArray)
                {
                    if (string.Compare(c, message) == 0)
                    {
                        OnlineCommand(player);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Helpers
        T GetConfig<T>(string name, T value) => Config[name] == null ? value : (T)Convert.ChangeType(Config[name], typeof(T));

        private void Message(IPlayer player, string prefix, string message, ulong userId) //TODO when possible switch to Universal API
        {
            var basePlayer = player.Object as BasePlayer;
            string formatted = prefix != null ? $"{prefix} {message}" : message;
            basePlayer.SendConsoleCommand("chat.add", 2, userId, formatted);
        }
        #endregion
    }
}