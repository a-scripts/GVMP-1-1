using System;
using System.Collections.Generic;
using System.IO;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Paintball.Menu;
using System.Xml;
using System.Linq;
using VMP_CNR.Module.NpcSpawner;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Logging;
using System.Threading.Tasks;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Configurations;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;


namespace VMP_CNR.Module.Paintball
{
    /*
     * 
     * 
      - KnownBugs
      -- 
      -- Weapons after log NOT FIXED
      --
      PAINTBALL v1.0 Stand 20.05.2022
      - Table `paintball` - Einstellungen
      -- Name
      -- Area (Gebietsbegrenzung)
      -- Dimension
      -- Weapons / Ammo
      -- MaxPlayerLobby
      -- EnterLobbyPrice
      -- MaxLife 
      -- RespawnTime (Millisekunden)
      -- SpawnProtection (Millisekunden)
      -- Active (1/0)
      -- Donator rights
      ---
      - Table `paintball_spawns`
      -- Spawns x,y,z
      -- Active (1/0)
      ---
      - Bemerkung:
      -- Jedes Gebiet sollte eine eigene Dimension haben
      -- RespawnTime nicht über 2 Minuten
      -- Richtige Paintball WPNZ kommen mit Rage 1.1
      ---
      - Regeln:
      -- Befehle Deaktiviert: /packgun 
    */

    public class PaintballModule : Module<PaintballModule>
    {

        public static bool PaintballDeactivated = false;
        public static Random Rand = new Random();
        public static Dictionary<string, dynamic> pbLobbies = new Dictionary<string, dynamic>();
        public static Vector3 PaintballMenuPosition = new Vector3(568.955, 2796.59, 42.0183);

        protected override bool OnLoad()
        {
            if (!PaintballDeactivated)
            {

                if (!Main.PaintballMenuLoaded)
                {
                    MenuManager.Instance.AddBuilder(new PaintballEnterMenuBuilder());
                    Main.PaintballMenuLoaded = true;
                new Npc(PedHash.Marine03SMY, new Vector3(568.955, 2796.59, 42.0183), 270, 0);
                }

            }
            return base.OnLoad();
        }

        public override void OnPlayerFirstSpawnAfterSync(DbPlayer dbPlayer)
        {
            if(dbPlayer.Paintball == 1)
            {
                dbPlayer.Dimension[0] = 0;
                dbPlayer.DimensionType[0] = DimensionType.World;
                dbPlayer.Player.SetPosition(PaintballMenuPosition);
                dbPlayer.SetPaintball(0);
            }
        }

        public void StartPaintball(DbPlayer iPlayer, uint id)
        {
            if (PaintballDeactivated) return;
            if (iPlayer != null)
            {
                iPlayer.SetData("ac-ignorews", 4);

                PaintballArea pba = PaintballAreaModule.Instance.Get(id);
                pba.pbPlayers[iPlayer] = new vars { life = pba.MaxLife, kills = 0, deaths = 0, killstreak = 0 };
                iPlayer.SetData("paintball_map", id);
                //REMOVE & LOAD WEAPONZ
                //SAVE WESTE
                iPlayer.SetData("paintball_armor", iPlayer.Player.Armor);
                iPlayer.RemoveAllServerWeapons();
                // SAVE Player 
                iPlayer.SetPaintball(1);

                Spawn(iPlayer);

                iPlayer.Player.TriggerEvent("initializePaintball");
            }
        }



        public void Spawn(DbPlayer iPlayer,bool quit=false,bool colshapeSpawn=false)
        {
            if (iPlayer != null && iPlayer.HasData("paintball_map"))
            {
                var playerMap = iPlayer.GetData("paintball_map");
                PaintballArea pba = PaintballAreaModule.Instance.Get(playerMap);

                if (pba.pbPlayers.ContainsKey(iPlayer)|| quit)
                {

                    if (pba.pbPlayers[iPlayer].life <= 0|| quit)
                    {
                        //FINISH PAINTBALL
                        //PLACE INSERT SQL 4 RANKING HERE

                        //REVIVE IF INJURED


                        pba.pbPlayers.Remove(iPlayer);
                        iPlayer.ResetData("paintball_map");
                        iPlayer.ResetData("paintball_death");
                        //SET END SPAWN 

                        //WORKAROUND PREVENT ALL ACTIONS?
                        //GIVE OLD ARMOR
                        if (iPlayer.HasData("paintball_armor"))
                        {
                          iPlayer.SetArmorPlayer(iPlayer.GetData("paintball_armor"));
                        }
                        //REMOVE WEAPONS
                        iPlayer.RemoveAllServerWeapons();

                        iPlayer.Player.TriggerEvent("emptyWeaponAmmo");
                        if (iPlayer.Injury.Id != 0)
                        {
                            iPlayer.revive();
                        }
                        iPlayer.Player.TriggerEvent("finishPaintball");

                        //GiveOldWeaponz
                        iPlayer.LoadPlayerWeapons();

                        // Just do crazy stuff bra
                        iPlayer.SetTied(false);
                        iPlayer.SetMedicCuffed(false);
                        iPlayer.SetCuffed(false);

                        System.Threading.Tasks.Task.Run(() =>
                        {
                            NAPI.Task.Run(() =>
                            {
                                iPlayer.SetPaintball(0);
                                iPlayer.SetDimension(0);
                                iPlayer.Dimension[0] = 0;
                                iPlayer.DimensionType[0] = DimensionType.World;
                                iPlayer.Player.SetPosition(PaintballMenuPosition);
                            }, delayTime: 3200); 
                        });

                    }
                    else
                    {

                        //GET/SET NEW SPAWN 
                        var spawn = PaintballSpawnModule.Instance.getSpawn(1);
                        iPlayer.SetDimension(pba.PaintBallDimension);
                        iPlayer.Dimension[0] = pba.PaintBallDimension;
                        iPlayer.DimensionType[0] = DimensionType.Paintball;
                        iPlayer.Player.SetPosition(new Vector3(spawn.x, spawn.y, spawn.z));

                        if (!colshapeSpawn)
                        {
                            iPlayer.SetHealth(100);
                            iPlayer.SetArmor(99, false);
                        }
                        //SPAWNPROTECTION IN MS
                        if (pba.SpawnProtection>0)
                        {
                            iPlayer.Player.TriggerEvent("spawnProtection", pba.SpawnProtection);
                        }

                        //REMOVE WEAPONS
                        iPlayer.RemoveAllServerWeapons();
                        iPlayer.Player.TriggerEvent("emptyWeaponAmmo");

                        //WEAPONS
                        foreach (var wpz in pba.Weapons)
                        {
                            iPlayer.GiveServerWeapon(NAPI.Util.WeaponNameToModel(wpz.name), wpz.ammo);
                        }


                        //REVIVE IF INJURED
                        if (iPlayer.Injury.Id != 0)
                        {
                            iPlayer.revive();
                        }

                        if (!colshapeSpawn)
                        {
                            iPlayer.SendNewNotification($"Du hast noch {pba.pbPlayers[iPlayer].life} Leben");
                        }

                        iPlayer.ResetData("paintball_death");

                        iPlayer.SetTied(false);
                        iPlayer.SetMedicCuffed(false);
                        iPlayer.SetCuffed(false);
                    }
                }
            }
        }

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            if (dbPlayer.Dimension[0] == 0 && key == Key.E)
            {
                if (dbPlayer.Player.Position.DistanceTo(PaintballMenuPosition) < 2.0f)
                {
                    if (Crime.CrimeModule.Instance.CalcJailTime(dbPlayer.Crimes) > 0)
                    {
                        dbPlayer.SendNewNotification("Zutritt verweigert: Ihr Steckbrief wurde von der Polizei bei uns ausgelegt.");
                        return true;
                    }

                    Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.PaintballEnterMenu, dbPlayer).Show(dbPlayer);
                    return true;
                }
            }
                return false;
        }

        public override void OnPlayerDisconnected(DbPlayer iPlayer, string reason)
        {
            if (iPlayer != null && iPlayer.HasData("paintball_map"))
            {
                PaintballArea pba = PaintballAreaModule.Instance.Get(iPlayer.GetData("paintball_map"));
                pba.pbPlayers.Remove(iPlayer);
            }
        }


        public override void OnPlayerDeath(DbPlayer iPlayer, NetHandle killer, uint hash)
        {
            try
            {
                DbPlayer iKiller = killer.ToPlayer().GetPlayer();
                if (iPlayer == null && !iPlayer.IsValid()) return;
                if (iKiller == null && !iKiller.IsValid()) return;
                if (!iPlayer.isAlive()) return; // Erneuter Tot verhindern


                if (iKiller.HasData("paintball_map") && iPlayer.HasData("paintball_map"))
                {
                    var playerMap = iPlayer.GetData("paintball_map");
                    PaintballArea pba = PaintballAreaModule.Instance.Get(playerMap);
                    if (pba.pbPlayers.ContainsKey(iPlayer) && pba.pbPlayers.ContainsKey(iKiller))
                    {
                        iPlayer.SetTied(true);

                        if (iPlayer != iKiller)
                        {
                            iPlayer.SetData("paintball_death", 1);
                            pba.pbPlayers[iPlayer] = new vars { life = pba.pbPlayers[iPlayer].life - 1, kills = pba.pbPlayers[iPlayer].kills, deaths = pba.pbPlayers[iPlayer].deaths + 1, killstreak = 0 };

                            if (pba.pbPlayers.ContainsKey(iKiller))
                            {
                                pba.pbPlayers[iKiller] = new vars { life = pba.pbPlayers[iKiller].life, kills = pba.pbPlayers[iKiller].kills + 1, deaths = pba.pbPlayers[iKiller].deaths, killstreak = pba.pbPlayers[iKiller].killstreak + 1 };

                                iPlayer.SendNewNotification($"Du wurdest umgebracht von {iKiller.Player.Name}");
                                iKiller.SendNewNotification($"Du hast {iPlayer.Player.Name} umgebracht");

                                //HP - ARMOR - BOOST
                                iKiller.SetHealth(Math.Min(100, NAPI.Player.GetPlayerHealth(iKiller.Player) + 25));
                                iKiller.SetArmor(Math.Min(99, NAPI.Player.GetPlayerArmor(iKiller.Player) + 25));

                                if (pba.pbPlayers[iKiller].killstreak == 3)
                                {
                                    foreach (var Players in pba.pbPlayers)
                                    {
                                        Players.Key.Player.TriggerEvent("sendGlobalNotification", $"Bei {iKiller.Player.Name} läuft!", 5000, "white", "glob");
                                    }
                                }
                                if (pba.pbPlayers[iKiller].killstreak == 6)
                                {
                                    foreach (var Players in pba.pbPlayers)
                                    {
                                        Players.Key.Player.TriggerEvent("sendGlobalNotification", $"{iKiller.Player.Name} scheppert richtig!", 5000, "white", "glob");
                                    }
                                }
                                if (pba.pbPlayers[iKiller].killstreak == 9)
                                {
                                    foreach (var Players in pba.pbPlayers)
                                    {
                                        Players.Key.Player.TriggerEvent("sendGlobalNotification", $"{iKiller.Player.Name} ist GODLIKE", 5000, "white", "glob");
                                    }
                                }
                                if (pba.pbPlayers[iKiller].killstreak == 12)
                                {
                                    foreach (var Players in pba.pbPlayers)
                                    {
                                        Players.Key.Player.TriggerEvent("sendGlobalNotification", $"{iKiller.Player.Name} - Savage, bist du es?", 5000, "white", "glob");
                                    }
                                }

                                iPlayer.Player.TriggerEvent("updatePaintballScore", pba.pbPlayers[iPlayer].kills, pba.pbPlayers[iPlayer].deaths, pba.pbPlayers[iPlayer].killstreak);
                                iKiller.Player.TriggerEvent("updatePaintballScore", pba.pbPlayers[iKiller].kills, pba.pbPlayers[iKiller].deaths, pba.pbPlayers[iKiller].killstreak);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Crash(ex);
                return;
            }
            return;
        }

        public override bool OnColShapeEvent(DbPlayer iPlayer, ColShape p_ColShape, ColShapeState p_ColShapeState)
        {
            if (p_ColShape.HasData("paintballId") && iPlayer.DimensionType[0] == DimensionType.Paintball)
            {
                if (iPlayer.isInjured()) return false;

                switch (p_ColShapeState)
                {
                    case ColShapeState.Exit:
                        if (iPlayer.HasData("paintball_map"))
                        {
                            Spawn(iPlayer, false, true);
                        }
                        return true;
                    default:
                        return true;
                }
            }

            return false;

        }


        [CommandPermission]
        [Command]
        public void Commandquit(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (!iPlayer.CanAccessMethod() || iPlayer.isInjured()) return;
            if (iPlayer.HasData("paintball_map"))
            {
                if (!iPlayer.HasData("paintball_death"))
                {
                    PaintballModule.Instance.Spawn(iPlayer, true);
                }
                else
                {
                    iPlayer.SendNewNotification($"/quit erst nach dem Spawn.");
                }
            }
        }
    }



    public class PaintballConfirm: Script
    {
        [RemoteEvent]
        public void PbaConfirm(Player p_Player, string pb_map, string none)
        {
            DbPlayer iPlayer = p_Player.GetPlayer();
            PaintballArea pba = PaintballAreaModule.Instance.Get(Convert.ToUInt32(pb_map));
            if (iPlayer == null || !iPlayer.IsValid() || pba == null)
            {
                return;
            }

            if (!iPlayer.TakeMoney(pba.LobbyEnterPrice))
            {
                iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(pba.LobbyEnterPrice));
                return;
            }

            PaintballModule.Instance.StartPaintball(iPlayer, pba.Id);
        }

        [RemoteEvent]
        public void PbaConfirmPassword(Player p_Player, string returnstring)
        {
            DbPlayer iPlayer = p_Player.GetPlayer();
            if (!iPlayer.HasData("pba_choose")) return;
            
            PaintballArea pba = PaintballAreaModule.Instance.Get(iPlayer.GetData("pba_choose"));
            if (pba.Password == returnstring)
            {
                PaintballModule.Instance.StartPaintball(iPlayer, pba.Id);
                iPlayer.ResetData("pba_choose");
            }
            else
            {
                iPlayer.SendNewNotification($"Passwort ist falsch!");
            }
            
        }

        [RemoteEvent]
        public void PbaSetPassword(Player p_Player, string returnstring)
        {
            DbPlayer iPlayer = p_Player.GetPlayer();
            if (returnstring.Length < 3)
            {
                p_Player.SendNotification("Das Passwort ist zu kurz!");
                return;
            }
            p_Player.SetData("PaintballCustomPW", returnstring);
            ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = $"Maxleben festlegen", Callback = "PbaSetMaxLife" });

        }

        [RemoteEvent]
        public void PbaSetMaxLife(Player p_Player, string returnstring)
        {
            DbPlayer iPlayer = p_Player.GetPlayer();
            if (returnstring.Length > 1)
            {
                p_Player.SendNotification("Zu hohe Leben");
                return;
            }
            p_Player.SetData("PaintballMaxLeben", int.Parse(returnstring));
            ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = $"Spawnprotection festlegen", Callback = "PbaSetSpawnProtect" });

        }

        [RemoteEvent]
        public void PbaSetSpawnProtect(Player p_Player, string returnstring)
        {
            DbPlayer iPlayer = p_Player.GetPlayer();
            if (returnstring.Length > 1)
            {
                p_Player.SendNotification("Zu hohe Spawnprotection!");
                return;
            }
            p_Player.SetData("PaintballSpawnProtect", int.Parse(returnstring));
            ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = $"Respawnzeit festlegen", Callback = "PbaSetRespawnTime" });

        }

        [RemoteEvent]
        public void PbaSetRespawnTime(Player p_Player, string returnstring)
        {
            DbPlayer iPlayer = p_Player.GetPlayer();
            if (returnstring.Length > 1)
            {
                p_Player.SendNotification("Zu hohe Respawnzeitprotection!");
                return;
            }
            p_Player.SetData("PaintballRespawnTime", int.Parse(returnstring));
            ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = $"Maximale Spielerzahl", Callback = "PbaSetMaxPlayer" });

        }

        [RemoteEvent]
        public void PbaSetMaxPlayer(Player p_Player, string returnstring)
        {
            DbPlayer iPlayer = p_Player.GetPlayer();

            if (iPlayer.money[0] < 15000)
            {

                return;
            }
            iPlayer.money[0] = iPlayer.money[0] - 15000;
            if (returnstring.Length > 2)
            {
                p_Player.SendNotification("Zu hohe Spielerzahl!");
                return;
            }
            int maxcount = int.Parse(returnstring);
            if (maxcount > 10)
            {
                p_Player.SendNotification("Dies ist nicht möglich!");

                return;
            }
            if (maxcount < 5)
            {
                p_Player.SendNotification("Dies ist nur als Donator möglich!");

                return;
            }
            p_Player.SendNotification("Paintballarena erstellt.");


            p_Player.SetData("PaintballMaxPlayerCount", int.Parse(returnstring));




            Task.Run(async () =>
            {
                string query = $"INSERT INTO `paintball` (`id`,`name`, `password`, `dimension`, `respawnTime`, `spawnprotection`, `lobbyenterprice`, `maxlobbyplayers`, `maxlife`, `active`, `ownerid`) VALUES ('1','{p_Player.Name}', '{p_Player.GetData<string>("PaintballCustomPW")}', '{p_Player.Dimension = p_Player.Dimension + 444}', '{p_Player.GetData<int>("PaintballRespawnTime")}', '{p_Player.GetData<int>("PaintballSpawnProtect")}', 1500, '{p_Player.GetData<int>("PaintballMaxPlayerCount")}', '{p_Player.GetData<int>("PaintballMaxLeben")}', 1, '{p_Player.GetPlayer().Id}');";

                MySQLHandler.ExecuteAsync(query);
                iPlayer.Player.TriggerEvent("freezePlayer", true);

                await Task.Delay(3000);
                iPlayer.Player.TriggerEvent("freezePlayer", false);


                using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                using (var cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandText = $"SELECT * FROM `paintball` where ownerid = '{p_Player.GetPlayer().Id}';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var arena = new PaintballArea(reader);
                                arena.Ownerid = p_Player.GetPlayer().Id;
                                arena.Id = reader.GetUInt32("sortid");

                                arena.Name = reader.GetString("name");
                                arena.Password = reader.GetString("password");
                                arena.PaintBallDimension = reader.GetUInt32("dimension");
                                arena.RespawnTime = reader.GetUInt32("respawnTime");
                                arena.SpawnProtection = reader.GetUInt32("spawnprotection");
                                arena.LobbyEnterPrice = reader.GetInt32("lobbyenterprice");
                                arena.MaxLobbyPlayers = reader.GetUInt32("maxlobbyplayers");
                                arena.MaxLife = reader.GetUInt32("maxlife");
                                arena.Weapons = JsonConvert.DeserializeObject<List<Weaponz>>(reader.GetString("weapons"));
                                arena.Area = JsonConvert.DeserializeObject<List<Area>>(reader.GetString("area"));

                                Console.WriteLine(arena.Name);

                                PaintballAreaModule.Instance.Add(arena.Id, arena);

                                p_Player.Dimension = 0;



                            }
                        }
                    }
                    await conn.CloseAsync();
                }
            });





        }
    }
}
