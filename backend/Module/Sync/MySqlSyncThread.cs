using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using VMP_CNR.Handler;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.NutritionPlayer;

namespace VMP_CNR.Module.Sync
{
    public class MySqlSyncThread
    {
        public enum MysqlQueueTypes
        {
            Default = 0,
            Inventory = 1,
            Vehicles = 2,
            Logging = 3,
            Nutrition = 4
        }

        public static MySqlSyncThread Instance { get; } = new MySqlSyncThread();

        public readonly ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        public readonly ConcurrentQueue<string> queue2 = new ConcurrentQueue<string>();
        public readonly ConcurrentQueue<string> queue3 = new ConcurrentQueue<string>();
        public readonly ConcurrentQueue<string> InventoryQueue = new ConcurrentQueue<string>();
        public readonly ConcurrentQueue<string> VehiclesQueue = new ConcurrentQueue<string>();
        public readonly ConcurrentQueue<string> LoggingQueue = new ConcurrentQueue<string>();
        public readonly ConcurrentQueue<string> NutritionQueue = new ConcurrentQueue<string>();
        private int index = 1;

        //public ConcurrentDictionary<DateTime, string> LastQueueQuerysAvoidSpam = new ConcurrentDictionary<DateTime, string>();

        private MySqlSyncThread()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    while (queue.IsEmpty)
                    {
                        await Task.Delay(1500);
                    }
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    {
                        try
                        {
                            conn.Open();
                            while (!queue.IsEmpty)
                            {
                                try
                                {
                                    if (!queue.TryDequeue(out var query)) continue;
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = @query;
                                        await cmd.ExecuteNonQueryAsync();
                                        Logger.Debug(@"async task: " + query);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Crash(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);

                            if (e is NullReferenceException)
                            {
                                //DiscordHandler l_Handler = new DiscordHandler("Eine kritische Exception ist aufgetreten!", e.ToString());
                                //l_Handler.Send();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    while (queue2.IsEmpty)
                    {
                        await Task.Delay(1500);
                    }
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    {
                        try
                        {
                            conn.Open();
                            while (!queue2.IsEmpty)
                            {
                                try
                                {
                                    if (!queue2.TryDequeue(out var query)) continue;
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = @query;
                                        await cmd.ExecuteNonQueryAsync();
                                        Logger.Debug(@"async task: " + query);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Crash(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);

                            if (e is NullReferenceException)
                            {
                                //DiscordHandler l_Handler = new DiscordHandler("Eine kritische Exception ist aufgetreten!", e.ToString());
                                //l_Handler.Send();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    while (queue3.IsEmpty)
                    {
                        await Task.Delay(1500);
                    }
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    {
                        try
                        {
                            conn.Open();
                            while (!queue3.IsEmpty)
                            {
                                try
                                {
                                    if (!queue3.TryDequeue(out var query)) continue;
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = @query;
                                        await cmd.ExecuteNonQueryAsync();
                                        Logger.Debug(@"async task: " + query);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Crash(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);

                            if (e is NullReferenceException)
                            {
                                //DiscordHandler l_Handler = new DiscordHandler("Eine kritische Exception ist aufgetreten!", e.ToString());
                                //l_Handler.Send();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    while (InventoryQueue.IsEmpty)
                    {
                        await Task.Delay(1500);
                    }
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    {
                        try
                        {
                            conn.Open();
                            while (!InventoryQueue.IsEmpty)
                            {
                                try
                                {
                                    if (!InventoryQueue.TryDequeue(out var query)) continue;
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = @query;
                                        await cmd.ExecuteNonQueryAsync();
                                        Logger.Debug(@"async task: " + query);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Crash(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);

                            if (e is NullReferenceException)
                            {
                                //DiscordHandler l_Handler = new DiscordHandler("Eine kritische Exception ist aufgetreten!", e.ToString());
                                //l_Handler.Send();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    while (VehiclesQueue.IsEmpty)
                    {
                        await Task.Delay(1500);
                    }
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    {
                        try
                        {
                            conn.Open();
                            while (!VehiclesQueue.IsEmpty)
                            {
                                try
                                {
                                    if (!VehiclesQueue.TryDequeue(out var query)) continue;
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = @query;
                                        await cmd.ExecuteNonQueryAsync();
                                        Logger.Debug(@"async task: " + query);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Crash(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);

                            if (e is NullReferenceException)
                            {
                                //DiscordHandler l_Handler = new DiscordHandler("Eine kritische Exception ist aufgetreten!", e.ToString());
                                //l_Handler.Send();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);

            // MAKE MAKROS GREAT AGAIN!
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    while (LoggingQueue.IsEmpty)
                    {
                        await Task.Delay(1500);
                    }
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    {
                        try
                        {
                            await conn.OpenAsync();
                            while (!LoggingQueue.IsEmpty)
                            {
                                try
                                {
                                    if (!LoggingQueue.TryDequeue(out var query)) continue;
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = @query;
                                        await cmd.ExecuteNonQueryAsync();
                                        Logger.Debug(@"async task: " + query);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Crash(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);

                            if (e is NullReferenceException)
                            {
                                //DiscordHandler l_Handler = new DiscordHandler("Eine kritische Exception ist aufgetreten!", e.ToString());
                                //l_Handler.Send();
                            }
                        }
                        finally
                        {
                            await conn.CloseAsync();
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    while (NutritionQueue.IsEmpty)
                    {
                        await Task.Delay(1500);
                    }
                    using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                    {
                        try
                        {
                            await conn.OpenAsync();
                            while (!NutritionQueue.IsEmpty)
                            {
                                try
                                {
                                    if (!NutritionQueue.TryDequeue(out var query)) continue;
                                    using (var cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = @query;
                                        await cmd.ExecuteNonQueryAsync();
                                        Logger.Debug(@"async task: " + query);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.Crash(e);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);

                            if (e is NullReferenceException)
                            {
                                //DiscordHandler l_Handler = new DiscordHandler("Eine kritische Exception ist aufgetreten!", e.ToString());
                                //l_Handler.Send();
                            }
                        }
                        finally
                        {
                            await conn.CloseAsync();
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Add(string query, MysqlQueueTypes quetype)
        {
            /*if (LastQueueQuerysAvoidSpam.Values.Contains(query))
            {
                return; // protect 4 spamm
            }
            else
            {
                LastQueueQuerysAvoidSpam.TryAdd(DateTime.Now, query);
            }*/

            if (quetype == MysqlQueueTypes.Inventory)
            {
                InventoryQueue.Enqueue(query);
                return;
            }

            if (quetype == MysqlQueueTypes.Vehicles)
            {
                VehiclesQueue.Enqueue(query);
                return;
            }

            if (quetype == MysqlQueueTypes.Logging)
            {
                LoggingQueue.Enqueue(query);
                return;
            }

            if (quetype == MysqlQueueTypes.Nutrition)
            {
                NutritionQueue.Enqueue(query);
                return;
            }

            if (index > 3) index = 1;

            if (index == 1) queue.Enqueue(query);
            else if (index == 2) queue2.Enqueue(query);
            else queue3.Enqueue(query);

            index++;
        }
    }

    public class MysqlSyncThreadModule : Module<MysqlSyncThreadModule>
    {
        /*public override void OnMinuteUpdate()
        {
            List<DateTime> toRemove = MySqlSyncThread.Instance.LastQueueQuerysAvoidSpam.Keys.Where(k => k < DateTime.Now.AddSeconds(30)).ToList();

            List<string> debug = new List<string>();

            foreach (KeyValuePair<DateTime, string> kvp in MySqlSyncThread.Instance.LastQueueQuerysAvoidSpam.Where(q => toRemove.Contains(q.Key)))
            {
                debug.Add(kvp.Value);
            }

            toRemove.ForEach(k => MySqlSyncThread.Instance.LastQueueQuerysAvoidSpam.TryRemove(k, out string lQuery)); /// lQuery ist das Element, das entfernt worden ist. Man kann es dadurch nochmal benutzen, muss aber nicht.

            Logger.Debug("removed from query-stack: " + String.Join(',', debug));
        }*/
    }
}