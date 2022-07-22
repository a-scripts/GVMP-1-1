using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Logging;

namespace VMP_CNR.Handler
{
    public class DiscordHandler
    {
        private static string m_LiveWebhookURL = "https://canary.discord.com/api/webhooks/973676619019464795/FSzxS2xb8RYdM5YFnDk1naN7w1rUPIuzaA6vB8alfu2D8p49C1I57xoBPHwroDJSSw74";
        private static string m_TestWebhookURL = "https://canary.discord.com/api/webhooks/973676619019464795/FSzxS2xb8RYdM5YFnDk1naN7w1rUPIuzaA6vB8alfu2D8p49C1I57xoBPHwroDJSSw74";

        public DiscordHandler()
        {
        }

        public static void SendMessage(string p_Message, string p_Description = "")
        {
            try
            {
                DiscordMessage l_Message = new DiscordMessage($"{ NAPI.Server.GetServerName() + NAPI.Server.GetServerPort()} - {p_Message}", p_Description);

                using (WebClient l_WC = new WebClient())
                {
                    l_WC.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    l_WC.Encoding = Encoding.UTF8;

                    string l_Upload = JsonConvert.SerializeObject(l_Message);
                    if (Configuration.Instance.DevMode)
                        l_WC.UploadString(m_TestWebhookURL, l_Upload);
                    else
                        l_WC.UploadString(m_LiveWebhookURL, l_Upload);
                }
            }
            catch (Exception e)
            {
                Logger.Crash(e);
                // Muss funktionieren amk, coded by Euka
            }
        }
    }

    public class DiscordMessage
    {
        public string content { get; private set; }
        public bool tts { get; private set; }
        public EmbedObject[] embeds { get; private set; }

        public DiscordMessage(string p_Message, string p_EmbedContent)
        {
            content = p_Message;
            tts = true;

            EmbedObject l_Embed = new EmbedObject(p_EmbedContent);
            embeds = new EmbedObject[] { l_Embed };
        }
    }

    public class EmbedObject
    {
        public string title { get; private set; }
        public string description { get; private set; }

        public EmbedObject(string p_Desc)
        {
            title = DateTime.Now.ToString();
            description = p_Desc;
        }
    }
}
