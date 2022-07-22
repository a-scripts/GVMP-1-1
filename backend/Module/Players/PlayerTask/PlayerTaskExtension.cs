using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Players.PlayerTask
{
    public static class PlayerTaskExtension
    {
        public static void LoadTasks(this DbPlayer iPlayer)
        {

            iPlayer.PlayerTasks = new Dictionary<uint, PlayerTask>();

            var query = $"SELECT * FROM `tasks` WHERE `owner_id` = '{iPlayer.Id}'";

            iPlayer.PlayerTasks.Clear();
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
                            // Verarbeitung DONE... Load PlayerTask into Players Tasks
                            var pTask = new PlayerTask(
                                reader.GetUInt32(0),
                                reader.GetUInt32(1),
                                iPlayer,
                                reader.GetString(3),
                                reader.GetDateTime(4)
                            );

                            iPlayer.PlayerTasks.Add(pTask.Id, pTask);
                        }
                    }
                }
            }
        }

        public static void AddTask(this DbPlayer iPlayer, PlayerTaskTypeId type, string data = "")
        {
            var pTaskType = PlayerTaskTypeModule.Instance.Get((uint) type);

            if (pTaskType == null) return;
            
            var finishedTime = DateTime.Now.AddMinutes(pTaskType.TaskTime);

            
                using (var connection = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText =
                        $"INSERT INTO `tasks` (type, owner_id, data, finish) VALUES('{(int) pTaskType.Id}', '{(int) iPlayer.Id}', '{data}', '{finishedTime:yyyy-MM-dd H:mm:ss}');";
                    var taskId = Convert.ToUInt32(command.ExecuteScalar());
                    iPlayer.PlayerTasks.Add(taskId, new PlayerTask(taskId, (uint) type, iPlayer, data, finishedTime));
                }
            
        }

        public static void RemoveTask(this DbPlayer dbPlayer, uint taskId)
        {
            MySQLHandler.ExecuteAsync($"DELETE FROM `tasks` WHERE `id` = '{taskId}'");
            dbPlayer.PlayerTasks?.Remove(taskId);
        }

        public static void CheckTasks(this DbPlayer iPlayer)
        {
            if (iPlayer.PlayerTasks == null) return;
            var now = DateTime.Now;
            
            foreach(KeyValuePair<uint, PlayerTask> kvp in iPlayer.PlayerTasks)
            {
                if (kvp.Value.Finish < now) kvp.Value.OnTaskFinish();
            }

            List<uint> toRemove = new List<uint>();
            foreach (KeyValuePair<uint, PlayerTask> pair in iPlayer.PlayerTasks)
            {
                if (pair.Value.Finish < now)
                {
                    toRemove.Add(pair.Key);
                }
            }

            foreach (var key in toRemove)
            {
                iPlayer.RemoveTask(iPlayer.PlayerTasks[key].Id);
            }
            
        }

        public static bool CheckTaskExists(this DbPlayer iPlayer, PlayerTaskTypeId type)
        {
            return iPlayer.PlayerTasks?.FirstOrDefault(task => task.Value.Type.Id == type).Value != null;
        }
    }
}