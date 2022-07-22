using System;
using MySql.Data.MySqlClient;
using GTANetworkAPI;
namespace VMP_CNR
{
    public class LogHandler
    {
        public static void LogDeath(string u1, uint userId, string u2, string weapon, string type)
        {
            NAPI.Task.Run(() =>
            {
                u1 = u1 ?? "undefined";
            u2 = u2 ?? "undefined";
            weapon = weapon ?? "undefined";

            String Query = $"INSERT INTO `log_death` (`user`, `killer_id`, `killer`, `weapon`, `type`) VALUES('{MySqlHelper.EscapeString(u1)}', '{userId}', '{MySqlHelper.EscapeString(u2)}', '{MySqlHelper.EscapeString(weapon)}', '{type}')";

            MySQLHandler.ExecuteAsync(Query);
            });

        }

        public static void LogKilled(string u1, string u2, string weapon)
        {
                NAPI.Task.Run(() =>
                {
                    u1 = u1 ?? "undefined";
            u2 = u2 ?? "undefined";
            weapon = weapon ?? "undefined";
            if (u2 == "") u2 = "None";
            if (weapon == "") weapon = "None";

            String Query = $"INSERT INTO `log_killed` (`user`, `killer`, `weapon`) VALUES('{MySqlHelper.EscapeString(u1)}', '{MySqlHelper.EscapeString(u2)}', '{MySqlHelper.EscapeString(weapon)}')";

            MySQLHandler.ExecuteAsync(Query);
                });

            }

            public static void LogAsi(string username, string asi)
                {
                    NAPI.Task.Run(() =>
                    {
                        username = username ?? "undefined";
            asi = asi ?? "undefined";

            MySQLHandler.ExecuteAsync($"INSERT INTO `log_asi` (`name`, `asi`) VALUES('{MySqlHelper.EscapeString(username)}', '{MySqlHelper.EscapeString(asi)}')");

                    });

                }

                public static void LogFakename(uint p_PlayerID, string p_PlayerName, string p_FakeName)
                    {
                        NAPI.Task.Run(() =>
                        {
                            string p_ID = p_PlayerID.ToString() ?? "undefined";
            p_PlayerName = p_PlayerName ?? "undefined";
            p_FakeName = p_FakeName ?? "undefined";
            
            string l_Query =
                $"INSERT INTO `log_fakename` (`player_id`, `player_name`, `fake_name`) VALUES ('{MySqlHelper.EscapeString(p_ID)}', '{MySqlHelper.EscapeString(p_PlayerName)}', '{MySqlHelper.EscapeString(p_FakeName)}');";

            MySQLHandler.ExecuteAsync(l_Query);
                        });

                    }

                    public static void LogFactionAction(uint p_PlayerID, string p_PlayerName, uint p_TeamID, bool p_Invite)
                        {
                            NAPI.Task.Run(() =>
                            {
                                string l_PlayerID = p_PlayerID.ToString() ?? "undefined";
                                string l_PlayerName = p_PlayerName ?? "undefined";
                                string l_TeamID = p_TeamID.ToString() ?? "undefined";

                                string l_Query = $"INSERT INTO `log_faction` (`team_id`, `invite`, `player_id`, `player_name`) VALUES ('{MySqlHelper.EscapeString(l_TeamID)}', '{(p_Invite ? "1" : "0")}', '{MySqlHelper.EscapeString(l_PlayerID)}', '{MySqlHelper.EscapeString(l_PlayerName)}');";

                                MySQLHandler.InsertAsync(l_Query);
                            });
        }
    }
}