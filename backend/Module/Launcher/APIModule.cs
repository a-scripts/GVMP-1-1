using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Helper;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Zone;

namespace VMP_CNR.Module.Launcher
{
    public sealed class APIModule : Module<APIModule>
    {
        public override Type[] RequiredModules()
        {
            return new[] { typeof(ConfigurationModule) };
        }

        protected override bool OnLoad()
        {
            //Task.Run(async () => { await LockLauncher(); });
            //Task.Run(async () => { await ClearWhitelist(); });

            return base.OnLoad();
        }

        public override void OnPlayerDisconnected(DbPlayer dbPlayer, string reason)
        {
            /*Task.Run(async () =>
            {
                try
                {
                    if (!Configurations.Configuration.Instance.DevMode)
                    {
                        using (WebPlayer l_WebPlayer = new WebPlayer())
                        {
                            l_WebPlayer.Encoding = System.Text.Encoding.UTF8;
                            l_WebPlayer.Headers["Content-Type"] = "application/json";

                            ResetWhitelistData l_ResetData = new ResetWhitelistData(dbPlayer.ForumId.ToString());
                            string l_Response = await l_WebPlayer.UploadStringTaskAsync("https://launcher.gvmp.de:5002/player/whitelist/reset", JsonConvert.SerializeObject(l_ResetData));
                            ResetWhitelistDataAnswer l_Answer = JsonConvert.DeserializeObject<ResetWhitelistDataAnswer>(l_Response);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.Logger.Crash(e);
                }
            }); TODO*/
        }

        public async Task<bool> ClearWhitelist()
        {
            return true;
            /*if (Configuration.Instance.DevMode)
                return false;

            using (WebPlayer l_Player = new WebPlayer())
            {
                l_Player.Encoding = System.Text.Encoding.UTF8;
                l_Player.Headers["Content-Type"] = "application/json";

                ClearWhitelistData l_Data = new ClearWhitelistData();
                var l_Response = await l_Player.UploadStringTaskAsync("https://launcher.gvmp.de:5002/player/whitelist/clear", JsonConvert.SerializeObject(l_Data));
                var l_ResponseData = JsonConvert.DeserializeObject<ClearWhitelistDataAnswer>(l_Response);

                if (!l_ResponseData.success)
                    return false;

                return true;
            }*/
        }

        public async Task<bool> IsWhitelisted(uint p_ForumID)
        {
            /*using (WebPlayer l_Player = new WebPlayer())
            {
                try
                {
                    string l_Response = await l_Player.DownloadStringTaskAsync("https://launcher.gvmp.de:5002/player/whitelist/" + p_ForumID.ToString());
                    WhitelistDataAnswer l_Answer = JsonConvert.DeserializeObject<WhitelistDataAnswer>(l_Response);
                    return l_Answer.success;
                }
                catch (Exception e)
                {
                    Logger.Crash(e);
                    return false;
                }
            }*/

            return true;
        }

        public async Task<bool> IsUsingCorrectProxy(uint p_ForumID, string p_IP)
        {
            /*using (WebPlayer l_WebPlayer = new WebPlayer())
            {
                l_WebPlayer.Encoding = System.Text.Encoding.UTF8;
                l_WebPlayer.Headers["Content-Type"] = "application/json";

                VerifyData l_VerifyData = new VerifyData(p_ForumID, p_IP);
                string l_Response = await l_WebPlayer.UploadStringTaskAsync("https://launcher.gvmp.de:5002/launcher/whitelist/verify", JsonConvert.SerializeObject(l_VerifyData));
                VerifyDataAnswer l_Answer = JsonConvert.DeserializeObject<VerifyDataAnswer>(l_Response);

                return l_Answer.success;
            }*/

            return true;
        }

        public async Task LockLauncher()
        {
            /*if (Configuration.Instance.DevMode)
                return;

            using (WebPlayer l_WebPlayer = new WebPlayer())
            {
                l_WebPlayer.Encoding = System.Text.Encoding.UTF8;
                l_WebPlayer.Headers["Content-Type"] = "application/json";

                CommunicationsData l_ResetData = new CommunicationsData(Configurations.Configuration.Instance.MAINTENACE_API_KEY, CommType.Maintenance, true, "Der Server ist aktuell nicht erreichbar. Der Login ist verfügbar, wenn der Gameserver wieder online ist.");
                string l_Response = await l_WebPlayer.UploadStringTaskAsync("https://launcher.gvmp.de:5002/launcher/communications", JsonConvert.SerializeObject(l_ResetData));
                CommunicationsDataAnswer l_Answer = JsonConvert.DeserializeObject<CommunicationsDataAnswer>(l_Response);
            }*/
        }

        public async Task UnlockLauncher()
        {
           /* if (Configuration.Instance.DevMode)
                return;

            using (WebPlayer l_WebPlayer = new WebPlayer())
            {
                l_WebPlayer.Encoding = System.Text.Encoding.UTF8;
                l_WebPlayer.Headers["Content-Type"] = "application/json";

                CommunicationsData l_ResetData = new CommunicationsData(Configurations.Configuration.Instance.MAINTENACE_API_KEY, CommType.Maintenance, false, "");
                string l_Response = await l_WebPlayer.UploadStringTaskAsync("https://launcher.gvmp.de:5002/launcher/communications", JsonConvert.SerializeObject(l_ResetData));
                CommunicationsDataAnswer l_Answer = JsonConvert.DeserializeObject<CommunicationsDataAnswer>(l_Response);
            }*/
        }
    }
}
