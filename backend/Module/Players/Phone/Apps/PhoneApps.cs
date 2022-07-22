using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Module.Players.Phone.Apps;
using System;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Voice;

namespace VMP_CNR.Module.Players.Phone.Apps
{
    public class PhoneApps
    {
        private Dictionary<string, PhoneApp> InitialApps { get; }

        private Dictionary<string, PhoneApp> Apps { get; }

        private uint PlayerId { get; }

        public PhoneApps(DbPlayer dbPlayer)
        {
            PlayerId = dbPlayer.Id;
            Apps = new Dictionary<string, PhoneApp>();
            InitialApps = new Dictionary<string, PhoneApp>();
            
            if (dbPlayer.TeamId != 0)
            {
                AddDefault("TeamApp");
            }

            if (dbPlayer.ActiveBusiness != null)
            {
                AddDefault("BusinessApp");
            }

            if (VoiceModule.Instance.hasPlayerRadio(dbPlayer))
            {
                AddDefault("FunkApp");
            }
            
            AddDefault("GpsApp");
            AddDefault("ContactsApp");
            AddDefault("LifeInvaderApp");
            AddDefault("TaxiApp");
            AddDefault("NewsApp");
            AddDefault("TelefonApp");
            AddDefault("ProfileApp");
            AddDefault("VehicleTracker");
            AddDefault("MessengerApp");
            AddDefault("SettingsApp");
            AddDefault("CalculatorApp");
            AddDefault("ServiceRequestApp");
            AddDefault("BankingApp");
        }

        private void AddDefault(string appName)
        {
            Console.WriteLine(appName);
            var phoneApp = PhoneAppsModule.Instance[appName];
            if (phoneApp == null) return;
            Console.WriteLine(phoneApp.Name);

            InitialApps.Add(phoneApp.Name, phoneApp);
            Add(phoneApp);
        }

        public void Add(string appName)
        {
            Console.WriteLine(appName);

            var phoneApp = PhoneAppsModule.Instance[appName];
            Console.WriteLine(phoneApp.Name);
            if (phoneApp == null) return;
            Add(phoneApp);
        }

        public void Add(PhoneApp phoneApp)
        {
            Console.WriteLine(phoneApp.Name);

            if (Apps.ContainsKey(phoneApp.Id)) return;
            Apps.Add(phoneApp.Id, phoneApp);
        }

        public void Remove(string appName)
        {
            if (!Apps.ContainsKey(appName)) return;
            Apps.Remove(appName);
        }

        public string GetJson()
        {
            DbPlayer dbPlayer = Players.Instance.GetByDbId(PlayerId);
            if (dbPlayer != null && dbPlayer.IsValid() && dbPlayer.phoneSetting.flugmodus)
            {
                //keine SMS, Telefon app
                var temp = Apps.Values.ToList();
                temp.Remove(PhoneAppsModule.Instance["BusinessApp"]);
                temp.Remove(PhoneAppsModule.Instance["TelefonApp"]);
                temp.Remove(PhoneAppsModule.Instance["MessengerApp"]);
                temp.Remove(PhoneAppsModule.Instance["TaxiApp"]);
                temp.Remove(PhoneAppsModule.Instance["NewsApp"]);
                temp.Remove(PhoneAppsModule.Instance["LifeInvaderApp"]);
                if (VoiceModule.Instance.hasPlayerRadio(dbPlayer))
                {
                    temp.Remove(PhoneAppsModule.Instance["FunkApp"]);
                }
                return JsonConvert.SerializeObject(temp);
            }
            else
            {
                //ganz normal alle Apps
                Console.WriteLine(JsonConvert.SerializeObject(Apps.Values.ToList()));
                return JsonConvert.SerializeObject(Apps.Values.ToList());
            }
        }
    }
}