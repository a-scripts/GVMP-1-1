using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using VMP_CNR.Handler;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Helper;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Time;

namespace VMP_CNR.Module.Tasks
{
    public class PlayerLoginTask : SqlResultTask
    {
        private readonly Player player;

        public PlayerLoginTask(Player player)
        {
            this.player = player;
        }

        public override string GetQuery()
        {
     
                return $"SELECT * FROM `player` WHERE `Name` = '{MySqlHelper.EscapeString(player.Name)}' LIMIT 1;";
        }

        public override void OnFinished(MySqlDataReader reader)
        {
            // Check to Avoid Double Login
            DbPlayer checkPlayer = player.GetPlayer();
            if(checkPlayer != null && checkPlayer.IsValid())
            {
                return;
            }

            if (reader.HasRows)
            {
                DbPlayer iPlayer = null;
                while (reader.Read())
                {
                    if (player == null) return;
                    //Bei Warn hau wech
                    if (reader.GetInt32("warns") >= 3)
                    {
                        player.TriggerEvent("freezePlayer", true);
                        //player.Freeze(true);
                        player.CreateUserDialog(Dialogs.menu_register, "banwindow");

                        PlayerLoginDataValidationModule.SyncUserBanToForum(reader.GetInt32("forumid"));

                        player.SendNotification($"Dein GVRP (IC-)Account wurde gesperrt. Melde dich im Teamspeak!");
                        player.Kick();
                        return;
                    }

                    // Check Timeban
                    if (reader.GetInt32("timeban") != 0 && reader.GetInt32("timeban") > DateTime.Now.GetTimestamp())
                    {
                        player.SendNotification("Ban aktiv");
                        player.Kick("Ban aktiv");
                        return;
                    }
                    
                    iPlayer = Players.Players.Instance.Load(reader, player);

                    if (!SocialBanHandler.Instance.IsPlayerWhitelisted(iPlayer))
                    {
                        player.SendNotification("Bitte whitelisten Sie sich im Forum (GVRP-Shield)!");
                        player.Kick();
                        return;
                    }

                    iPlayer.Player.TriggerEvent("sendAuthKey", iPlayer.AuthKey);
                    iPlayer.WatchMenu = 0;
                    iPlayer.Freezed = false;
                    iPlayer.watchDialog = 0;
                    iPlayer.Firstspawn = false;
                    iPlayer.PassAttempts = 0;
                    iPlayer.TempWanteds = 0;

                    iPlayer.PlayerPet = null;
                    
                    iPlayer.adminObject = null;
                    iPlayer.adminObjectSpeed = 0.5f;

                    iPlayer.AccountStatus = AccountStatus.Registered;

                    iPlayer.Character = ClothModule.Instance.LoadCharacter(iPlayer);

                    Task.Run(async() => { await VehicleKeyHandler.Instance.LoadPlayerVehicleKeys(iPlayer); });
                    
                    iPlayer.SetPlayerCurrentJobSkill();
                    //iPlayer.ClearChat();

                    // Check Socialban
                    if (SocialBanHandler.Instance.IsPlayerSocialBanned(iPlayer.Player))
                    {
                        player.SendNotification("Bitte melde dich beim Support im Teamspeak (Social-Ban)");
                        player.Kick();
                        return;
                    }

                    // Check Social Name
                    if (!Configurations.Configuration.Instance.Ptr && iPlayer.SocialClubName != "" && iPlayer.SocialClubName != iPlayer.Player.SocialClubName)
                    {
                        DBLogging.LogAcpAdminAction("System", player.Name, adminLogTypes.perm, $"Social-Club-Name-Changed DB - {iPlayer.SocialClubName} - Player - {iPlayer.Player.SocialClubName}");
                        iPlayer.Player.SendNotification("Bitte melde dich beim Support im Teamspeak (Social-Name-Changed)");
                        iPlayer.Player.Kick("Bitte melde dich beim Support im Teamspeak (Social-Name-Changed)");
                        return;
                    }

                    string queryxd = $"UPDATE `player` SET verified = '0' WHERE id = '{iPlayer.Id}';";
                    MySQLHandler.ExecuteAsync(queryxd);



                    //player.FreezePosition = true;

                    Task.Run(async () =>
                    {
                        NAPI.Task.Run(() =>
                        {
                            if (iPlayer == null) return;
                            player.TriggerEvent("setPlayerHealthRechargeMultiplier");




                            if (iPlayer.AccountStatus == AccountStatus.LoggedIn) return;

                            ComponentManager.Get<LoginWindow>().Show()(iPlayer);

                            if (Configuration.Instance.IsUpdateModeOn)
                            {
                                new LoginWindow().TriggerEvent(iPlayer.Player, "status", "Der Server befindet sich derzeit im Update Modus!");
                                if (iPlayer.Rank.Id < 1) iPlayer.Kick();
                            }
                        });
                    });
                }
            }
            else
            {
                player.SendNotification("Sie benoetigen einen Account (www.gvrp.to)! Name richtig gesetzt? Vorname_Nachname");
                player.Kick(
                    "Sie benoetigen einen Account (www.gvrp.to)! Name richtig gesetzt? Vorname_Nachname");
                Logger.Debug($"Player was kicked, no Account found for {player.Name}");
            }
        }
    }
}