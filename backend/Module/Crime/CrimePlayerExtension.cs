using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Email;
using VMP_CNR.Module.FIB;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Events;
using VMP_CNR.Module.Staatskasse;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Weapons.Component;

namespace VMP_CNR.Module.Crime
{
    public static class CrimePlayerExtension
    {

        public static void AddCrime(this DbPlayer dbPlayer, DbPlayer Cop, CrimeReason crime, string notice = "")
        {
            if (!Cop.IsValid() || !dbPlayer.IsValid() || crime == null) return;


            string reporter = Cop.GetName();

            if (crime.Jailtime <= 0)
            {
                EmailModule.Instance.SendPlayerEmail(dbPlayer, "Ticket erhalten", EmailTemplates.GetTicketTemplate(CrimeModule.Instance.GetCrimeCosts(new CrimePlayerReason(crime, notice), dbPlayer.EconomyIndex), crime.Name));
            }

            dbPlayer.AddCrimeLogical(reporter, new CrimePlayerReason(crime, notice));
        }

        public static void AddCrime(this DbPlayer dbPlayer, string reporter, CrimeReason crime, string notice = "")
        {
            if (!dbPlayer.IsValid() || crime == null) return;

            dbPlayer.AddCrimeLogical(reporter, new CrimePlayerReason(crime, notice));

            if (crime.Jailtime <= 0)
            {
                EmailModule.Instance.SendPlayerEmail(dbPlayer, "Ticket erhalten", EmailTemplates.GetTicketTemplate(CrimeModule.Instance.GetCrimeCosts(new CrimePlayerReason(crime, notice), dbPlayer.EconomyIndex), crime.Name));
            }
        }

        private static void AddCrimeLogical(this DbPlayer dbPlayer, string reporter, CrimePlayerReason crime, string notice = "")
        {
            if (notice == "")
            {
                notice = $"Beamter {reporter} am {DateTime.Now.ToString("dd/MM/yyyy")} um {DateTime.Now.ToString("HH:mm")} Uhr";
            }
            crime.Notice = notice;
            
            dbPlayer.Crimes.Add(crime);
            dbPlayer.AddDbCrime(crime);
        }

        public static void RemoveCrime(this DbPlayer dbPlayer, CrimePlayerReason crime, string officer = "")
        {
            if (dbPlayer.Crimes.Contains(crime))
                dbPlayer.Crimes.Remove(crime);

            // dbPlayer.SendNewNotification($"Dir wurde das Verbrechen {crime.Name} erlassen.");
            if (dbPlayer.TeamId != (int)teams.TEAM_FIB && !dbPlayer.IsNSADuty) Teams.TeamModule.Instance.SendChatMessageToDepartments($"{dbPlayer.GetName()} wurde {crime.Name} {(officer != "" ? "von " + officer : "")} erlassen!");
            dbPlayer.RemoveSingleDBCrime(crime);
        }

        public static void RemoveAllCrimes(this DbPlayer dbPlayer, string officer = "")
        {
            dbPlayer.Crimes.Clear();
            dbPlayer.ResetDbCrimes();
            dbPlayer.UHaftTime = 0;

            if(dbPlayer.TeamId != (int)teams.TEAM_FIB && !dbPlayer.IsNSADuty) Teams.TeamModule.Instance.SendChatMessageToDepartments($"{dbPlayer.GetName()} wurde die Akte {(officer != "" ? "von " + officer : "")} erlassen!");
        }
        
        private static void AddDbCrime(this DbPlayer iPlayer, CrimePlayerReason crime)
        {
            // Insert into DB
            string query =
                $"INSERT INTO `player_crime` (`player_id`, `crime_reason_id`, `notice`) VALUES ('{iPlayer.Id}', '{crime.Id}', '{crime.Notice}');";
            MySQLHandler.ExecuteAsync(query);
        }

        private static void RemoveSingleDBCrime(this DbPlayer iPlayer, CrimePlayerReason crime)
        {
            // Insert into DB
            string query =
                $"DELETE FROM `player_crime` WHERE player_id = '{iPlayer.Id}' AND crime_reason_id = '{crime.Id}' AND notice LIKE '%{crime.Notice}%';";
            MySQLHandler.ExecuteAsync(query);
        }

        public static async Task LoadCrimes(this DbPlayer iPlayer)
        {
            iPlayer.Crimes.Clear();

            // Loading Wanted for Player
            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                cmd.CommandText =
                    $"SELECT crime_reason_id, notice FROM `player_crime` WHERE player_id = '{iPlayer.Id}';";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var reason = CrimeReasonModule.Instance.Get(reader.GetUInt32("crime_reason_id"));
                            if (reason == null) continue;

                            string notice = reader.GetString("notice");
                            iPlayer.Crimes.Add(new CrimePlayerReason(reason, notice));
                        }
                    }
                }
                await conn.CloseAsync();
            }

            // Crime History
            iPlayer.CrimeHistories.Clear();

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                cmd.CommandText =
                    $"SELECT crimes, date FROM `player_crime_history` WHERE player_id = '{iPlayer.Id}';";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            iPlayer.CrimeHistories.Add(new CrimePlayerHistory(reader.GetString("crimes"), reader.GetDateTime("date")));
                        }
                    }
                }
                await conn.CloseAsync();
            }
        }

        public static void AddToCrimeHistory(this DbPlayer iPlayer, string crimeString)
        {
            iPlayer.CrimeHistories.Add(new CrimePlayerHistory(crimeString, DateTime.Now));
            MySQLHandler.ExecuteAsync($"INSERT INTO player_crime_history (`player_id`, `crimes`, `date`) VALUES ('{iPlayer.Id}', '{crimeString}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');");
        }
        
        private static void ResetDbCrimes(this DbPlayer iPlayer)
        {
            MySQLHandler.ExecuteAsync($"DELETE FROM `player_crime` WHERE `player_id` = '{iPlayer.Id}'");
        }
        
        public static void ArrestPlayer(this DbPlayer iPlayer, DbPlayer iPlayerCop, bool SpawnPlayer = true)
        {
            if (iPlayer.IsACop()  && iPlayer.IsInDuty()) return;

            var wanteds = iPlayer.wanteds[0];
            if (iPlayer.TempWanteds > 0 && iPlayer.wanteds[0] < 30) wanteds = 30;

            int jailtime = CrimeModule.Instance.CalcJailTime(iPlayer.Crimes);
            int jailcosts = CrimeModule.Instance.CalcJailCosts(iPlayer.Crimes, iPlayer.EconomyIndex);

            iPlayer.jailtime[0] = jailtime;

            // Checke auf Jailtime
            if (iPlayerCop != null && iPlayer.jailtime[0] == 0)
            {
                iPlayerCop.SendNewNotification(iPlayer.GetName() + " hat keine Haftzeit offen!");
                return;
            }

            // Uhaft
            if(iPlayer.UHaftTime > 0)
            {
                //Limit auf 60min
                int uHaftJailTimeReverse = iPlayer.UHaftTime > 60 ? 60 : iPlayer.UHaftTime;

                // Maximal bis auf 10 min runter
                iPlayer.jailtime[0] = iPlayer.jailtime[0] - uHaftJailTimeReverse <= 10 ? 10 : iPlayer.jailtime[0] - iPlayer.UHaftTime;
            }

            EmailModule.Instance.SendPlayerEmail(iPlayer, "Inhaftierung", EmailTemplates.GetArrestTemplate(iPlayer.Crimes, jailcosts, jailtime));

            string JailStringHistroy = $"Inhaftierung ({jailtime} | $ {jailcosts}):";

            string ListCrimes = "Sie wurden wegen folgenden Verbrechen Inhaftiert: ";

            foreach(CrimePlayerReason crime in iPlayer.Crimes)
            {
                ListCrimes += crime.Name + ",";

                if(crime.Jailtime > 0)
                {
                    JailStringHistroy += crime.Name + ",";
                }
            }
            
            iPlayer.RemoveAllCrimes();
            iPlayer.TempWanteds = 0;

            iPlayer.ResetData("follow");

            if (iPlayerCop != null)
            {
                iPlayerCop.ResetData("follow");
                iPlayerCop.SendNewNotification(MSG.General.hasArrested(iPlayer.GetName(), iPlayer.jailtime[0] - 1));
                iPlayer.SendNewNotification(MSG.General.isArrested(iPlayerCop.GetName(), iPlayer.jailtime[0] - 1));
            }

            iPlayer.TakeAnyMoney((int)jailcosts, true);
            KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, jailcosts);

            iPlayer.SendNewNotification(
                "Durch Ihre Inhaftierung wurde Ihnen eine Strafzahlung von $" + jailcosts + " in Rechnung gestellt!");

            TeamModule.Instance.SendChatMessageToDepartments("An Alle Einheiten, " + iPlayer.GetName() +
                " sitzt nun hinter Gittern!");

            iPlayer.jailtimeReducing[0] = Convert.ToInt32(iPlayer.jailtime[0] / 3);

            iPlayer.SendNewNotification(ListCrimes);
            iPlayer.AddToCrimeHistory(JailStringHistroy);

            // Set Voice To Normal
            iPlayer.Player.SetSharedData("voiceRange", (int)VoiceRange.whisper);
            iPlayer.SetData("voiceType", 3);
            iPlayer.Player.TriggerEvent("setVoiceType", 3);

            if (SpawnPlayer)
            {
                iPlayer.RemoveWeapons();
                iPlayer.ResetAllWeaponComponents();

                iPlayer.Player.SetPosition(new Vector3());
                PlayerSpawn.OnPlayerSpawn(iPlayer.Player);
            }

            iPlayer.Save();
        }
    }
}
