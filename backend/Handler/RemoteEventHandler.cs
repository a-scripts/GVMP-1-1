using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;

namespace VMP_CNR.Handler
{
    public class RemoteEventHandler : Script
    {
        [RemoteEvent]
        public void triggerServerEvent(Player player, string p_EventData)
        {
            try
            {
                Logger.Debug(p_EventData);
                EventData l_Data = JsonConvert.DeserializeObject<EventData>(p_EventData);

                DbPlayer dbPlayer = player.GetPlayer();


                string authKey = l_Data.AuthKey;
                string eventName = l_Data.EventName;


                SecureEventThread.Instance.AddToQueue(new EventInformation(dbPlayer, l_Data));
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }
    }

    public static class RemoteEventPlayerExtensions
    {
        public static bool IsLegitEvent(this DbPlayer dbPlayer, string authKey, bool p_LoginEvent = false)
        {

            return true;
        }
    }

    public class SecureEventAttribute : Attribute
    {

    }

    public sealed class SecureEventHandler
    {
        public IEnumerable<System.Reflection.MethodInfo> SecureEventMethods;
        public SecureEventHandler()
        {
        }

        public void Init()
        {
            SecureEventMethods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(typeof(SecureEventAttribute), false).FirstOrDefault() != null);

            foreach (var l_Event in SecureEventMethods)
            {
                Console.WriteLine($"LOADED SECURE-EVENT: {l_Event.Name}");
            }
        }

        public void ExecuteEvent(Player player, string eventName, object[] args)
        {
            try
            {
                bool l_Found = false;
                foreach (var method in SecureEventMethods)
                {
                    if (method.Name != eventName)
                        continue;

                    Logger.Debug($"Triggered SecureEvent: {eventName}");
                    l_Found = true;

                    List<object> newArgs = new List<object>();
                    newArgs.Add(player);
                    foreach (var x in args)
                    {
                        if (x.GetType() == typeof(Int32))
                        {
                            int l_NewXBastard = Convert.ToInt32(x);
                            newArgs.Add(l_NewXBastard);
                        }
                        else if (x.GetType() == typeof(Int64))
                        {
                            int l_NewXBastard = Convert.ToInt32((Int64)x);
                            newArgs.Add(l_NewXBastard);
                        }
                        else
                        {
                            newArgs.Add(x);
                        }
                    }

                    Logger.Debug($"SecureEvent {eventName} args:");

                    var obj = Activator.CreateInstance(method.DeclaringType);
                    method.Invoke(obj, newArgs.ToArray());
                    break;
                }

                if (!l_Found)
                {
                    Console.WriteLine($"Es wurde kein SecureEvent Methode gefunden für das Event: {eventName}");
                }
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }
    }

    public class EventData
    {
        public string AuthKey { get; set; }
        public string EventName { get; set; }
        public object[] Args { get; set; }
    }

    public class EventInformation
    {
        public DbPlayer dbPlayer { get; private set; }
        public EventData data { get; private set; }

        public EventInformation(DbPlayer p_DbPlayer, EventData p_EventData)
        {
            dbPlayer = p_DbPlayer;
            data = p_EventData;
        }
    }
}
