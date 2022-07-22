﻿using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Players.Db;
using static VMP_CNR.Module.Players.Events.EventStateModule;

namespace VMP_CNR.Module.Players.Events
{
    public class EventStateModule : Module<EventStateModule>
    {
        public enum EventListIds
        {
            EASTER = 1,
            EASTER2021 = 2,
        }

        public static UInt32 EasterEgg1 = 1034;
        public static UInt32 EasterEgg2 = 1035;
        public static UInt32 EasterEgg3 = 1036;

        public override void OnPlayerLoadData(DbPlayer dbPlayer, MySqlDataReader reader)
        {
            dbPlayer.EventDoneList = new Dictionary<EventListIds, int>();

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT * FROM player_eventstate WHERE player_id = '{dbPlayer.Id}';";
                using (var eventreader = cmd.ExecuteReader())
                {
                    if (eventreader.HasRows)
                    {
                        while (eventreader.Read())
                        {
                            dbPlayer.EventDoneList.Add((EventListIds)eventreader.GetInt32("event_id"), eventreader.GetInt32("state"));
                        }
                    }
                }
                conn.Close();
            }

            Console.WriteLine("EventStateModule");

        }
    }

    public static class PlayerEventExtension
    {
        public static bool HasEventDone(this DbPlayer dbPlayer, EventListIds eventListId)
        {
            return (dbPlayer.EventDoneList.ContainsKey(eventListId) && dbPlayer.EventDoneList[eventListId] == 1);
        }

        public static void ChangeEventState(this DbPlayer dbPlayer, EventListIds eventListId, int StateValue)
        {
            if(dbPlayer.EventDoneList.ContainsKey(eventListId))
            {
                dbPlayer.EventDoneList[eventListId] = StateValue;
                MySQLHandler.ExecuteAsync($"UPDATE `player_eventstate` SET state = '{StateValue}' WHERE player_id = '{dbPlayer.Id}' AND event_id = '{eventListId}';");
            }
            else
            {
                dbPlayer.EventDoneList.Add(eventListId, StateValue);
                MySQLHandler.ExecuteAsync($"INSERT INTO `player_eventstate` (`event_id`, `player_id`, `state`) VALUES ('{(int)eventListId}', '{dbPlayer.Id}', '{StateValue}');");
            }
        }
    }
}
