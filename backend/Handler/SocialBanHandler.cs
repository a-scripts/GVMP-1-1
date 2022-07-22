using MySql.Data.MySqlClient;
using System;
using GTANetworkAPI;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players;

namespace VMP_CNR
{
    public sealed class SocialBanHandler
    {
        public static SocialBanHandler Instance { get; } = new SocialBanHandler();

        private SocialBanHandler()
        {
        }

        public void AddEntry(Player player)
        {
            MySQLHandler.ExecuteAsync(
                $"INSERT INTO socialbans (Name) VALUES ('{player.SocialClubName}');");
        }

        public bool IsPlayerSocialBanned(Player player)
        {
            if (player == null || player.SocialClubName == "" || player.SocialClubName == null) return false;
            
            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT * FROM socialbans WHERE Name = '{MySqlHelper.EscapeString(player.SocialClubName)}';";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
                conn.Close();
            }

            return false;
        }

        public bool IsPlayerWhitelisted(DbPlayer iPlayer)
        {
    

            return true;
        }

        public void DeleteEntry(Player player)
        {
            var query =
                $"DELETE FROM socialbans WHERE Name = '{player.SocialClubName}';";
            MySQLHandler.ExecuteAsync(query);
        }
    }
}