﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using VMP_CNR.Handler;
using VMP_CNR.Module.Doors;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerTask;
using VMP_CNR.Module.Players.Sync;
using VMP_CNR.Module.Weather;

namespace VMP_CNR.Module
{
    public sealed class Modules
    {
        public static Modules Instance { get; } = new Modules();

        private readonly Dictionary<Type, BaseModule> modules;

        private Modules()
        {
            modules = new Dictionary<Type, BaseModule>();

            var objects = Assembly.GetAssembly(typeof(BaseModule))
                .GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseModule)) &&
                                 myType.GetCustomAttribute<DisabledModuleAttribute>() == null)
                .Select(type => (BaseModule) Activator.CreateInstance(type))
                .ToList();
            objects.Sort(new ModuleComparer());
            objects.Reverse();

            foreach (var module in objects)
            {
                Register(module);
            }
        }

        public MethodInfo GetCommand(string methodName)
        {
            // For Using Syntax if /mychatcommand then void Commandmychatcommand
            try
            {
                foreach (var module in modules)
                {
                    if (module.Value.GetType().GetMethod("Command" + methodName) != null)
                    {
                        return module.Value.GetType().GetMethod("Command" + methodName);
                    }
                }
            }
            catch(Exception e)
            {
                Logging.Logger.Crash(e);
            }
            return null;
        }

        public dynamic GetModuleByCommand(string methodName)
        {
            // For Using Syntax if /mychatcommand then void Commandmychatcommand
            try 
            { 
                foreach (var module in modules)
                {
                    if (module.Value.GetType().GetMethod("Command" + methodName) != null)
                    {
                        return module.Value;
                    }
                }
            }
            catch(Exception e)
            {
                Logging.Logger.Crash(e);
            }
            return null;
        }
        
        public void OnPlayerWeaponSwitch(DbPlayer dbPlayer, WeaponHash oldGun, WeaponHash newGun)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerWeaponSwitch(dbPlayer, oldGun, newGun);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }


        public bool HasDoorAccess(DbPlayer dbPlayer, Door door)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    if (module.HasDoorAccess(dbPlayer, door)) return true;
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }

            return false;
        }

        public bool OnPlayerConnected(Player Player)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    if (!module.OnPlayerConnected(Player)) return false;
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }

            return true;
        }
        
        public void OnPlayerFirstSpawn(DbPlayer dbPlayer)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerFirstSpawn(dbPlayer);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }


        public void OnVehicleSpawn(SxVehicle sxvehicle)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnVehicleSpawn(sxvehicle);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnDailyReset()
        {
            if (Settings.SettingsModule.Instance.CanDailyReset())
            {

                foreach (var module in modules.Values)
                {
                    try
                    {
                        module.OnDailyReset();
                    }
                    catch (Exception e)
                    {
                        Logger.Print(e.ToString());
                    }
                }

                // just saving will set onupdated value
                Settings.Setting setting = Settings.SettingsModule.Instance.GetSetting(Settings.Settings.DailyReset);

                setting.Updated = DateTime.Now;

                Settings.SettingsModule.Instance.SaveSetting(setting);
            }
        }

        public void OnServerBeforeRestart()
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnServerBeforeRestart();
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnPlayerFirstSpawnAfterSync(DbPlayer dbPlayer)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerFirstSpawnAfterSync(dbPlayer);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnPlayerSpawn(DbPlayer dbPlayer)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerSpawn(dbPlayer);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnPlayerEnterVehicle(DbPlayer dbPlayer, Vehicle vehicle, sbyte seat)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerEnterVehicle(dbPlayer, vehicle, seat);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnPlayerExitVehicle(DbPlayer dbPlayer, Vehicle vehicle)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerExitVehicle(dbPlayer, vehicle);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnTenSecUpdate()
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnTenSecUpdate();
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public async Task OnTenSecUpdateAsync()
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    await module.OnTenSecUpdateAsync();
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnFiveSecUpdate()
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnFiveSecUpdate();
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnVehicleDeleteTask(SxVehicle sxVehicle)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnVehicleDeleteTask(sxVehicle);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnPlayerConnect(DbPlayer dbPlayer)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerConnected(dbPlayer);
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }
        }

        public void OnPlayerLoadData(DbPlayer dbPlayer, MySqlDataReader reader)
        {
            foreach (var module in modules.Values)
            {
                Console.WriteLine(module.ToString());
                try
                {
                    // Data dont need loaded sync
                    module.OnPlayerLoadData(dbPlayer, reader);
                }
                catch (Exception e)
                {
                    Logger.Print(e.Message);
                }
            }
        }

        public void OnPlayerDeath(DbPlayer dbPlayer, NetHandle killer, uint weapon)
        {
            bool IgnoreFutureDeath = false;
            foreach (var module in modules.Values)
            {
                try
                {
                    IgnoreFutureDeath = (!IgnoreFutureDeath) ? module.OnPlayerDeathBefore(dbPlayer, killer, weapon) : IgnoreFutureDeath;
                }
                catch (Exception e)
                {
                    Logger.Print(e.Message);
                }
            }

            if (!IgnoreFutureDeath)
            {
                foreach (var module in modules.Values)
                {
                    try
                    {
                        module.OnPlayerDeath(dbPlayer, killer, weapon);
                    }
                    catch (Exception e)
                    {
                        Logger.Print(e.Message);
                    }
                }
            }
        }
        
        public void OnPlayerLoggedIn(DbPlayer dbPlayer)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnPlayerLoggedIn(dbPlayer);
                }
                catch (Exception e)
                {
                    Logger.Print(e.Message);
                }
            }
        }

        public void OnPlayerDisconnected(DbPlayer dbPlayer, string reason)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                foreach (var module in modules.Values)
                {
                    try
                    {
                        module.OnPlayerDisconnected(dbPlayer, reason);
                    }
                    catch (Exception e)
                    {
                        Logger.Print(e.Message);
                    }
                }
            }));
        }

        public async Task<bool> OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            bool lol = false;
       
                return await Task.Run<bool>(() =>
            {
                foreach (var module in modules.Values)
                {

                    try
                    {
                        NAPI.Task.Run(() =>
                        {
                            if (dbPlayer == null || !dbPlayer.IsValid()) lol = true;

                            if (dbPlayer.HasData("blockKeyPress") && dbPlayer.GetData("blockKeyPress")) lol = true;

                            if (module.OnKeyPressed(dbPlayer, key)) lol = true;
                        });
                    }
                    catch (Exception e)
                    {
                        Logger.Print(e.ToString());
                    }
                }

                return lol;
            });
        }

        public bool OnChatCommand(DbPlayer dbPlayer, string commandString)
        {
            try
            {
                var commandSplit = commandString.Split(' ');
                if (commandSplit.Length == 0) return false;
                var command = commandSplit[0];
                if (!command.StartsWith("/"))
                {
                    return false;
                }

                command = command.Replace("/", "");

                var size = commandSplit.Count(splittedCommand => !string.IsNullOrEmpty(splittedCommand));
                size--;

                var args = size > 0 ? new string[size] : null;
                if (args != null)
                {
                    var argIndex = 0;
                    for (var i = 1; i < commandSplit.Length; i++)
                    {
                        if (string.IsNullOrEmpty(commandSplit[i])) continue;
                        args[argIndex++] = commandSplit[i];
                    }
                }

                foreach (var module in modules.Values)
                {
                    try
                    {
                        if (module.OnChatCommand(dbPlayer, command, args)) return true;
                    }
                    catch (Exception e)
                    {
                        Logger.Print(commandString);
                        Logger.Print(e.StackTrace);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Print(commandString);
                Logger.Print(e.StackTrace);
            }

            return false;
        }

        public bool OnColShapeEvent(DbPlayer dbPlayer, ColShape colShape, ColShapeState colShapeState)
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    if (!dbPlayer.IsValid()) return true;
                    if (module.OnColShapeEvent(dbPlayer, colShape, colShapeState)) return true;
                }
                catch (Exception e)
                {
                    Logger.Print(e.ToString());
                }
            }

            return false;
        }

        public void LoadAll()
        {
            foreach (var module in modules.Values)
            {
                module.Load();
            }
        }

        public void Load(Type moduleType, bool reload = false)
        {
            if (!modules.ContainsKey(moduleType))
            {
                Logger.Print($"Module not found: {moduleType}");
                return;
            }

            if (reload)
            {
                Logger.Print($"Module reloaded: {moduleType}");

            }

            modules[moduleType].Load(reload);
        }

        public bool Reload(string name)
        {
            foreach (var module in modules)
            {
                if (!module.Value.GetType().ToString().Equals(name)) continue;
                module.Value.Load(true);
                return true;
            }

            return false;
        }

        public void OnMinuteUpdate()
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    module.OnMinuteUpdate();
                }
                catch (Exception e)
                {
                    Logging.Logger.Crash(e);
                }
            }
        }

        public async Task OnMinuteUpdateAsync()
        {
            foreach (var module in modules.Values)
            {
                try
                {
                    await module.OnMinuteUpdateAsync();
                }
                catch (Exception e)
                {
                    Logging.Logger.Crash(e);
                }
            }
        }

        public void OnTwoMinutesUpdate()
        {
            try
            {
                foreach (var l_Module in modules.Values)
                {
                    l_Module.OnTwoMinutesUpdate();
                }
            }
            catch (Exception e)
            {
                Logging.Logger.Crash(e);
            }
        }

        public void OnFiveMinuteUpdate()
        {
            try 
            { 
                foreach (var module in modules.Values)
                {
                     module.OnFiveMinuteUpdate();
                }
            }
            catch (Exception e)
            {
                Logging.Logger.Crash(e);
            }
}

        public void OnFifteenMinuteUpdate()
        {
            try 
            { 
                foreach(var module in modules.Values)
                {
                    module.OnFifteenMinuteUpdate();
                }
            }
            catch (Exception e)
            {
                Logging.Logger.Crash(e);
            }
        }

        public void OnPlayerMinuteUpdate()
        {
            try 
            { 
                foreach (var module in modules.Values)
                {
                    foreach(DbPlayer iPlayer in Players.Players.Instance.GetValidPlayers())
                    {
                        if (iPlayer == null) continue;
                        module.OnPlayerMinuteUpdate(iPlayer);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Logger.Crash(e);
            }
        }

        public Dictionary<Type, BaseModule> GetAll()
        {
            return modules;
        }

        private void Register(BaseModule module)
        {
            modules.Add(module.GetType(), module);
        }
    }
}