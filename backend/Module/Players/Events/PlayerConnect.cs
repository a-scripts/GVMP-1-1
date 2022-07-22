using GTANetworkAPI;
using System.Linq;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Tasks;
using System;

namespace VMP_CNR.Module.Players.Events
{
    public class PlayerConnect : Script
    {        
        //[ServerEvent(Event.PlayerConnected)]
        public static void OnPlayerConnected(Player player)
        {
            if (player == null)
            {
                return;
            }

            Console.WriteLine("SS SEX S");
            
            player.SetPosition(new Vector3(17.4809, 637.872, 210.595));

            //Unsichtbar, Freeze
            player.Transparency = 0;
            player.Dimension = 1337; // There is no PlayerID at this point, so count it up

          player.TriggerEvent("OnPlayerReady");

            // Alreade logged in, delete PlayerObject
            var dbPlayer = player.GetPlayer();
            if (dbPlayer != null || dbPlayer.IsValid(true))
            {
                try
                {
                    foreach (var itr in Players.Instance.players.Where(p => p.Value == dbPlayer || p.Value.Player.Name == player.Name))
                    {
                        Players.Instance.players.TryRemove(itr.Key, out DbPlayer tmpDbPlayer);
                    }
                }
                catch (System.Exception e)
                {
                    Logging.Logger.Crash(e);
                }
            }
            
            if (!Configuration.Instance.IsServerOpen)
            {
                player.SendNotification("Server wird heruntergefahren");
                player.Kick();
                return;
            }

            if (!Configuration.Instance.Ptr && SocialBanHandler.Instance.IsPlayerSocialBanned(player))
            {
                player.SendNotification("Bitte melde dich beim Support im Teamspeak (Social-Ban)");
                player.Kick();
                return;
            }
            
            Modules.Instance.OnPlayerConnected(player);

            player.SetData("loginStatusCheck", 1);
            
            //player.Freeze(true);
            player.TriggerEvent("freezePlayer", true);

        

                SynchronizedTaskManager.Instance.Add(new PlayerLoginTask(player));
        }
    }
}
