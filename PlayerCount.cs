using System.Linq;
using Oxide.Core.Libraries.Covalence;
using System.Collections.Generic;
using Oxide.Core.Plugins;
using Oxide.Core;
using Newtonsoft.Json;
using System.IO;

namespace Oxide.Plugins
{
    [Info("PlayerCount", "[4ga] RAR", "2.0.0")]
    [Description("Shows players count")]
    class PlayerCount : CovalencePlugin
    {
        #region Init
        [PluginReference]
        private Plugin BetterChat;

        private Settings settings;

        public class Settings
        {
            [JsonProperty(PropertyName = "Online Message")]
            public string OnlineMessage { get; set; } = "{onlinePlayers}/{maxPlayers} Online ({joiningPlayers} Joining {queuedPlayers} Queued)";

            [JsonProperty(PropertyName = "Prefix")]
            public string Prefix { get; set; } = "<color=#FFA500>[4ga]</color>";

            [JsonProperty(PropertyName = "Steam Avatar Id")]
            public ulong SteamAvatarId { get; set; } = 76561198273760551;

            [JsonProperty(PropertyName = "Command List")]
            public List<string> CommandList { get; set; } = new List<string>(new string[] { "online", "players", "pop"});
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                settings = Config.ReadObject<Settings>();
                if (settings == null)
                {
                    LoadDefaultConfig();
                }
            }
            catch
            {
                LoadDefaultConfig();
            }
            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            string configPath = $"{Interface.Oxide.ConfigDirectory}{Path.DirectorySeparatorChar}{Name}.json";
            LogWarning($"Could not load a valid configuration file, creating a new configuration file at {configPath}");
            settings = new Settings();
        }

        protected override void SaveConfig() => Config.WriteObject(settings);

        private void Init()
        {
            foreach (var command in settings.CommandList)
            {
                AddCovalenceCommand(command, "OnlineCommand"); //Register all commands
            }
        }
        #endregion

        #region Commands
        private void OnlineCommand(IPlayer player)
        {
            string displayText = settings.OnlineMessage.Replace("{onlinePlayers}", players.Connected.Count().ToString())
                .Replace("{maxPlayers}", server.MaxPlayers.ToString())
                .Replace("{queuedPlayers}", ServerMgr.Instance.connectionQueue.Queued.ToString())
                .Replace("{joiningPlayers}", ServerMgr.Instance.connectionQueue.Joining.ToString());

            Message(player, settings.Prefix, displayText, settings.SteamAvatarId);
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
                foreach (var command in settings.CommandList)
                {
                    if (string.Compare(command, message) == 0)
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
        private void Message(IPlayer player, string prefix, string message, ulong userId) //TODO when possible switch to Universal API
        {
            var basePlayer = player.Object as BasePlayer;
            string formatted = prefix != null ? $"{prefix} {message}" : message;
            basePlayer.SendConsoleCommand("chat.add", 2, userId, formatted);
        }
        #endregion
    }
}