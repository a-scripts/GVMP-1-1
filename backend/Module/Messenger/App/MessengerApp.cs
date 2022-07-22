using System.Collections.Generic;
using GTANetworkAPI;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using Newtonsoft.Json;
using System;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Items;

using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Phone;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Messenger.App
{
    public class MessengerApp : SimpleApp
    {
        public MessengerApp() : base("MessengerApp")
        {
        }

        [RemoteEvent]
        public void sendMessage(Player Player, uint number, string messageContent)
        {
        }

        [RemoteEvent]
        public void forwardMessage(Player Player, uint number, uint messageId)
        {
            // Forwars selected message in "original" and fake-proof. TBD later.
        }
    }
}