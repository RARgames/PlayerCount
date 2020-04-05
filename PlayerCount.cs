using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
//TODO add steam avatar change
//todo add commands with !
namespace Oxide.Plugins
{
    [Info("PlayerCount", "[4ga] RAR", "1.0.0")]
    [Description("Shows players count")]

    class PlayerCount : CovalencePlugin
    {
        #region Initialization
        string onlineMessage;

        void Init()
        {
            LoadDefaultConfig();
        }

        protected override void LoadDefaultConfig()
        {
            Config["Online message"] = onlineMessage = GetConfig("Online message", "{onlinePlayers}/{maxPlayers} Online ({joiningPlayers} Joining {queuedPlayers} Queued)");

            SaveConfig();
        }
        #endregion

        #region Commands
        [Command("online", "players", "pop")]
        void OnlineCommand(IPlayer player, string command, string[] args)
        {
            string displayText = onlineMessage.Replace("{onlinePlayers}", players.Connected.Count().ToString())
                .Replace("{maxPlayers}", server.MaxPlayers.ToString())
                .Replace("{queuedPlayers}", ServerMgr.Instance.connectionQueue.Queued.ToString())
                .Replace("{joiningPlayers}", ServerMgr.Instance.connectionQueue.Joining.ToString());

            player.Reply(displayText);
        }
        #endregion

        #region Helpers
        T GetConfig<T>(string name, T value) => Config[name] == null ? value : (T)Convert.ChangeType(Config[name], typeof(T));
        #endregion
    }
}