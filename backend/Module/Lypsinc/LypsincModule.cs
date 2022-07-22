using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Lypsinc
{
    class LypsincModule : Module<LypsincModule>
    {
        private static string TeamspeakQueryAddress { get; set; }
        private static short TeamspeakQueryPort { get; set; }
        private static string TeamspeakPort { get; set; }
        private static string TeamspeakLogin { get; set; }
        private static string TeamspeakPassword { get; set; }
        private static string TeamspeakChannel { get; set; }

        public override bool Load(bool reload = false)
        {
            /*if (Configurations.Configuration.Instance.DevMode) return false;
            TeamspeakQueryAddress = Configurations.Configuration.Instance.TeamspeakQueryAddress;

            short port = 0;
            if (short.TryParse(Configurations.Configuration.Instance.TeamspeakQueryPort, out port))
            {
                TeamspeakQueryPort = port;
            }
            else
            {
                Logging.Logger.Debug("Failed Convert Port");
                return false;
            }

            TeamspeakPort = Configurations.Configuration.Instance.TeamspeakPort;
            TeamspeakLogin = Configurations.Configuration.Instance.TeamspeakLogin;
            TeamspeakPassword = Configurations.Configuration.Instance.TeamspeakPassword;
            TeamspeakChannel = Configurations.Configuration.Instance.VoiceChannel;
            
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (Configurations.Configuration.Instance.LipsyncActive)
                        CheckSpeakingPlayers().Wait();

                    Thread.Sleep(2000);
                }
            }, TaskCreationOptions.LongRunning);

            /*Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    CheckForLogins().Wait();
                    Thread.Sleep(2000);
                }
            }, TaskCreationOptions.LongRunning);*/

            return true;
        }

        private async Task CheckForLogins()
        {
            try
            {
                using (var qc = new QueryClient(TeamspeakQueryAddress, TeamspeakQueryPort))
                {
                    await qc.Connect();
                    await qc.Send("login", new Parameter("Player_login_name", TeamspeakLogin), new Parameter("Player_login_password", TeamspeakPassword));
                    await qc.Send("use", new Parameter("port", TeamspeakPort));

                    Logging.Logger.Debug("Connected to Teamspeak3.");

                    while (qc.IsConnected && qc.Client.Connected)
                    {
                        List<String> speakingPlayers = new List<String>();

                        var channel = await qc.Send("channelfind", new Parameter("pattern", TeamspeakChannel));
                        String channelId = channel.First()["cid"].ToString();
                        var Playerlist = await qc.Send("Playerlist", new Parameter("-voice", ""));

                        var playerList = new List<DbPlayer>();
                        foreach (var iPlayer in Players.Players.Instance.GetValidPlayers().ToList())
                        {
                            if (iPlayer.RankId > 0)
                                continue;

                            if (!iPlayer.HasData("login_time"))
                                continue;

                            DateTime LoginTime = iPlayer.GetData("login_time");
                            if (LoginTime.AddSeconds(120) <= DateTime.Now)
                            {
                                playerList.Add(iPlayer);
                                iPlayer.ResetData("login_time");
                                continue;
                            }
                        }

                        foreach (var player in playerList)
                        {
                            if (Playerlist.Where(ic => ReplaceStr(ic["Player_nickname"].ToString()).Contains(player.VoiceHash)).Count() > 0)
                                continue;
                            else
                            {
                                player.Player.SendNotification("Du wurdest aufgrund von nicht Anwesenheit im Voice-Server vom Spielserver gekickt.");
                                player.Kick();
                            }
                        }

                        Thread.Sleep(60000);
                    }
                    Logging.Logger.Debug("Task is over...");
                }
            }
            catch (Exception e)
            {
                Logging.Logger.Debug("Voice Anwesenheit Check failed...");
                Logging.Logger.Debug(e.ToString());
            }
        }

        private async Task CheckSpeakingPlayers()
        {
            if (Configurations.Configuration.Instance.IsServerOpen)
            {
                try
                {
                    using (var qc = new QueryClient(TeamspeakQueryAddress, TeamspeakQueryPort))
                    {
                        await qc.Connect();
                        await qc.Send("login", new Parameter("Player_login_name", TeamspeakLogin), new Parameter("Player_login_password", TeamspeakPassword));
                        await qc.Send("use", new Parameter("port", TeamspeakPort));

                        Logging.Logger.Debug("Connected to Teamspeak3.");

                        while (qc.IsConnected && qc.Client.Connected)
                        {
                            List<String> speakingPlayers = new List<String>();

                            var channel = await qc.Send("channelfind", new Parameter("pattern", TeamspeakChannel));
                            String channelId = channel.First()["cid"].ToString();

                            var Playerlist = await qc.Send("Playerlist", new Parameter("-voice", ""));
                            foreach (var Player in Playerlist)
                            {
                                if (Player["Player_flag_talking"].ToString() == "1" && Player["cid"].ToString() == channelId)
                                {
                                    speakingPlayers.Add(ReplaceStr(Player["Player_nickname"].ToString()));
                                }
                            }

                            List<DbPlayer> players = Players.Players.Instance.GetValidPlayers();

                            foreach (var player in players)
                            {
                                // Spieler spricht?
                                if (speakingPlayers.Where(ic => ic.Contains(player.VoiceHash)).Count() > 0 && !(player.HasData("speaking") && player.GetData("speaking")))
                                {
                                    foreach (DbPlayer rangePlayer in Players.Players.Instance.GetValidPlayers().Where(d => d.Player.Position.DistanceTo(player.Player.Position) < 20.0f).ToList())
                                    {
                                        rangePlayer.Player.TriggerEvent("startPlayerSpeak", player.Player);
                                    }
                                    player.SetData("speaking", true);
                                }

                                if (speakingPlayers.Where(ic => ic.Contains(player.VoiceHash)).Count() <= 0 && (player.HasData("speaking") && player.GetData("speaking")))
                                {
                                    foreach (DbPlayer rangePlayer in Players.Players.Instance.GetValidPlayers().Where(d => d.Player.Position.DistanceTo(player.Player.Position) < 20.0f).ToList())
                                    {
                                        rangePlayer.Player.TriggerEvent("stopPlayerSpeak", player.Player);
                                    }
                                    player.SetData("speaking", false);
                                }
                            }

                            Thread.Sleep(1250);
                        }
                        Logging.Logger.Debug("Task is over...");
                    }
                }
                catch (Exception e)
                {
                    Logging.Logger.Debug("Lypsync Prob... could be ignores");
                    Logging.Logger.Debug(e.ToString());
                }
            }
        }

        public string ReplaceStr(string str)
        {
            str = str.Replace("\\\\", "\\");
            str = str.Replace("\\/", "/");
            str = str.Replace("\\s", " ");
            str = str.Replace("\\p", "|");
            str = str.Replace("\\a", "\a");
            str = str.Replace("\\b", "\b");
            str = str.Replace("\\f", "\f");
            str = str.Replace("\\n", "\n");
            str = str.Replace("\\r", "\r");
            str = str.Replace("\\t", "\t");
            str = str.Replace("\\v", "\v");
            return str;
        }
    }
}
