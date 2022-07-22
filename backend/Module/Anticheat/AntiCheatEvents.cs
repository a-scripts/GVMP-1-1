using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.Business.FuelStations;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Logging;
using VMP_CNR.Handler;
using VMP_CNR.Module.Players;
using static VMP_CNR.Module.Chat.Chats;

namespace VMP_CNR.Module.Anticheat
{
    class AntiCheatEvents : Script
    {


        [RemoteEvent]
        public void yanniksiehtdich(Player player, string cheatCode)
        {
            DiscordHandler.SendMessage("CRMNL SKID CHEAT", player.Name + " | " + player.Address + " | CHEAT-CODE: " + cheatCode);

            try
            {

                var iPlayer = player.GetPlayer();
                if (iPlayer == null) return;

                if (!iPlayer.IsValid())
                {
                    return;
                }

                AntiCheatModule.Instance.ACBanPlayer(iPlayer, "CRMNL");

                DiscordHandler.SendMessage("CRMNL SKID CHEAT", iPlayer.Player.Name + " | " + iPlayer.Player.Address + " | CHEAT-CODE: " + cheatCode);



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }



        }


            [RemoteEvent]
        public void __ragemp_cheat_detected(Player player, int cheatCode)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            string l_Cheat = "Cheat Engine";

            switch (cheatCode)
            {
                case 0:
                case 1:
                    l_Cheat = "Cheat Engine";
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    l_Cheat = "Externer Hack";
                    break;
                case 7:
                    l_Cheat = "Mod-Menü";
                    break;
                case 8:
                case 9:
                    l_Cheat = "Speed Hack";
                    break;
                case 11:
                    l_Cheat = "Nutzung von Sandboxie";
                    break;
                default:
                    break;
            }

            Logger.AddActionLogg(dbPlayer.Id, cheatCode);
            Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Dringender Anticheat-Verdacht: {dbPlayer.Player.Name} ({l_Cheat}) gegeben.");
        }

        [RemoteEvent]
        public void __ragemp_cheat_detected_timed(Player player, int cheatCode)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            string l_Cheat = "Cheat Engine [EARLYDETECT]";

            switch (cheatCode)
            {
                case 0:
                case 1:
                    l_Cheat = "Cheat Engine [EARLYDETECT]";
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    l_Cheat = "Externer Hack [EARLYDETECT]";
                    break;
                case 7:
                    l_Cheat = "Mod-Menü [EARLYDETECT]";
                    break;
                case 8:
                case 9:
                    l_Cheat = "Speed Hack [EARLYDETECT]";
                    break;
                case 11:
                    l_Cheat = "Nutzung von Sandboxie [EARLYDETECT]";
                    break;
                default:
                    break;
            }

            Logger.AddActionLogg(dbPlayer.Id, cheatCode);
            Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Dringender Anticheat-Verdacht: {dbPlayer.Player.Name} ({l_Cheat}) gegeben.  [EARLYDETECT]");
        }

        [RemoteEvent]
        public void amo(Player player, int sec, int wpn, int amount)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            var query = $"INSERT INTO `log_makro` (`player_id` ,`sec`,`wpn`,`amount`) VALUES ('{dbPlayer.Id}', '{sec}','{wpn}','{amount}');";
            MySQLHandler.ExecuteAsync(query, Sync.MySqlSyncThread.MysqlQueueTypes.Logging);
        }

        [RemoteEvent]
        public async void CheckNameTags(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (dbPlayer.RankId > 0) return;

            if(dbPlayer.HasData(""))

            Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.NameTags, $"(nametags aktiv - Spielerhuds)");

            Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"DRINGEND Anticheat-Verdacht: {dbPlayer.Player.Name} (nametags detected).");
        }


        [RemoteEvent]
        public async void sftptbp(Player player) // Event Shift + Tab anonymized for Steam Overlay
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (dbPlayer.HasData("mediusing"))
            {
                Players.Players.Instance.SendMessageToAuthorizedUsers("highteamchat", $"DRINGEND Anticheat-Verdacht: {dbPlayer.Player.Name} (Steamoverlay während Verbandkasten).");
                Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.sftptbp, $"{dbPlayer.Player.Name} M");
            }
            if (dbPlayer.HasData("armorusing"))
            {
                Players.Players.Instance.SendMessageToAuthorizedUsers("highteamchat", $"DRINGEND Anticheat-Verdacht: {dbPlayer.Player.Name} (Steamoverlay während Schutzweste).");
                Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.sftptbp, $"{dbPlayer.Player.Name} A");
            }
        }
    }
}
