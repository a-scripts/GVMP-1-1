using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players;

namespace VMP_CNR.Module.NutritionPlayer
{
    /// Nutrition v0.1
    /// - Wird in DbPlayers eingebunden
    /// - Table Nutrition

    public class Nutrition
    {
        public float Kcal { get; set; } //Kohlenhydrate
        public float Fett { get; set; }
        public float Wasser { get; set; }
        public float Zucker { get; set; }
        public float avg { get; set; }

        public uint PlayerId { get; set; }
        public DateTime lastFood { get; set; }

        public Nutrition(DbPlayer iplayer)
        {
            PlayerId = iplayer.Id;
        }

        public void save()
        {
            if (!NutritionModule.Instance.NutritionActive) return;

            try
            {
                MySQLHandler.ExecuteAsync($"UPDATE `nutrition` SET wasser='{this.Wasser.ToString().Replace(",", ".")}',zucker='{this.Zucker.ToString().Replace(",", ".")}',fett='{this.Fett.ToString().Replace(",", ".")}',kcal='{this.Kcal.ToString().Replace(",", ".")}' WHERE player_id={PlayerId}", Sync.MySqlSyncThread.MysqlQueueTypes.Nutrition);
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }


        public void GetNutritionBySQL()
        {
            if (!NutritionModule.Instance.NutritionActive) return;

            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                try
                {
                    string query =$"SELECT * FROM `nutrition` WHERE player_id='{PlayerId}' LIMIT 1;";
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = @query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {

                                    this.Kcal = reader.GetFloat("kcal");
                                    this.Wasser = reader.GetFloat("wasser");
                                    this.Fett = reader.GetFloat("fett");
                                    this.Zucker = reader.GetFloat("zucker");
                                }
                            }
                            else
                            {
                                MySQLHandler.ExecuteAsync($"INSERT INTO `nutrition` (`player_id`, `kcal`,`fett`,`wasser`,`zucker`) VALUES ({PlayerId}, {NutritionModule.Instance.StandardKcal},{NutritionModule.Instance.StandardFett}, {NutritionModule.Instance.StandardWasser}, {NutritionModule.Instance.StandardZucker});", Sync.MySqlSyncThread.MysqlQueueTypes.Nutrition);
                                //default
                                this.Kcal = NutritionModule.Instance.StandardKcal;
                                this.Fett = NutritionModule.Instance.StandardFett;
                                this.Wasser = NutritionModule.Instance.StandardWasser;
                                this.Zucker = NutritionModule.Instance.StandardZucker;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Crash(e);
                }
            }));

            return;
        }
    }
}
