using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Crime;
using VMP_CNR.Module.Email;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Computer.Apps.PoliceAktenSearchApp
{
    public class PoliceEditWantedsApp : SimpleApp
    {
        public PoliceEditWantedsApp() : base("PoliceEditWantedsApp")
        {
        }

        [RemoteEvent]
        public void requestWantedCategories(Player p_Player)
        {
            var l_CrimeCategories = CrimeCategoryModule.Instance.GetAll();
            List<CategoryObject> l_List = new List<CategoryObject>();

            foreach (var l_Category in l_CrimeCategories)
            {
                l_List.Add(new CategoryObject() { id = (int)l_Category.Value.Id, name = l_Category.Value.Name });
            }

            var l_Json = NAPI.Util.ToJson(l_List);
            TriggerEvent(p_Player, "responseCategories", l_Json);
        }

        [RemoteEvent]
        public void requestCategoryReasons(Player p_Player, int p_ID)
        {
            var l_CrimeReasons = CrimeReasonModule.Instance.GetAll();
            List<ReasonObject> l_List = new List<ReasonObject>();

            foreach (var l_Reason in l_CrimeReasons)
            {
                if (l_Reason.Value.Category.Id != p_ID)
                    continue;

                l_List.Add(new ReasonObject() { id = (int)l_Reason.Value.Id, name = l_Reason.Value.Name });
            }

            var l_Json = NAPI.Util.ToJson(l_List);
            TriggerEvent(p_Player, "responseCategoryReasons", l_Json);
        }


        [RemoteEvent]
        public void requestPlayerWanteds(Player p_Player, string p_Name)
        {
            Main.m_AsyncThread.AddToAsyncThread(new System.Threading.Tasks.Task(() =>
            {
                var dbPlayer = Players.Players.Instance.FindPlayer(p_Name);
                if (dbPlayer == null || !dbPlayer.IsValid()) return;

                if (!MySQLHandler.IsValidNoSQLi(dbPlayer, p_Name)) return;

                var l_Crimes = dbPlayer.Crimes;
                List<CrimeJsonObject> l_List = new List<CrimeJsonObject>();

                foreach (var l_Reason in l_Crimes)
                {
                    l_List.Add(new CrimeJsonObject() { id = (int)l_Reason.Id, name = l_Reason.Name, description = l_Reason.Description });
                }

                var l_Json = NAPI.Util.ToJson(l_List);

                TriggerEvent(p_Player, "responsePlayerWanteds", l_Json);
            }));
        }

        [RemoteEvent]
        public void removeAllCrimes(Player p_Player, string name)
        {
            if (p_Player == null) return;

            DbPlayer dbPlayer = p_Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.IsACop()) return;

            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, name)) return;

            Main.m_AsyncThread.AddToAsyncThread(new System.Threading.Tasks.Task(() =>
            {
                var suspect = Players.Players.Instance.FindPlayer(name);
                if (suspect == null) return;

                if (suspect.Crimes.Count > 0)
                {
                    EmailModule.Instance.SendPlayerEmail(suspect, "Tickets erlassen", EmailTemplates.GetTicketRemoveListTemplate(suspect.Crimes.ToList().Where(t => t.Jailtime == 0).ToList()));
                    suspect.RemoveAllCrimes(p_Player.Name);   
                }
            }));
        }

        [RemoteEvent]
        public void removePlayerCrime(Player p_Player, string name, int crime)
        {
            if (p_Player == null) return;

            DbPlayer dbPlayer = p_Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.IsACop()) return;

            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, name)) return;

            Main.m_AsyncThread.AddToAsyncThread(new System.Threading.Tasks.Task(() =>
            {
                var suspect = Players.Players.Instance.FindPlayer(name);
                if (suspect == null) return;

                CrimePlayerReason crimePlayerReason = suspect.Crimes.Where(cpr => cpr.Id == (uint)crime).FirstOrDefault();
                if (crimePlayerReason != null)
                {
                    suspect.RemoveCrime(crimePlayerReason, p_Player.Name);

                    if(crimePlayerReason.Jailtime == 0)
                    {
                        EmailModule.Instance.SendPlayerEmail(suspect, "Ticket erlassen", EmailTemplates.GetTicketRemoveTemplate(crimePlayerReason.Name));
                    }
                }
            }));
        }

        [RemoteEvent]
        public void addPlayerWanteds(Player player, string name, string crimes)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, name)) return;
            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, crimes)) return;

            if (!dbPlayer.IsACop()) return;

            List<uint> crimesList = JsonConvert.DeserializeObject<List<uint>>(crimes);

            var suspect = Players.Players.Instance.FindPlayer(name);
            if (suspect == null || crimesList == null || suspect.isInjured()) return;
            foreach(uint crime in crimesList)
            {
                suspect.AddCrime(dbPlayer, CrimeReasonModule.Instance.Get((uint)crime));
            }
            
            Teams.TeamModule.Instance.SendChatMessageToDepartments($"{dbPlayer.GetName()} hat die Akte von {suspect.GetName()} bearbeitet!");
        }
    }

    public class CrimeJsonObject
    {
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }
    }
    public class ReasonObject
    {
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
    }

    public class CategoryObject
    {
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
    }
}
