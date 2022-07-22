using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Handler;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.Doors;
using VMP_CNR.Module.Events.CWS;
using VMP_CNR.Module.Farming;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.JumpPoints;
using static VMP_CNR.Module.Chat.Chats;

namespace VMP_CNR.Module.Events.Halloween
{
    public enum ZombieType
    {
        SUPERJUMP = 1,
        TANK = 2,
    }

    public class HalloweenModule : Module<HalloweenModule>
    {
        public static bool isActive = false;

        public static List<Vector3> spawnPositions = new List<Vector3>();

        public Dictionary<uint, ZombieType> activeZombies = new Dictionary<uint, ZombieType>();

        public List<uint> zombieSkins = new List<uint>();

        protected override bool OnLoad()
        {
            spawnPositions = new List<Vector3>();
            spawnPositions.Add(new Vector3(-1733.95, -233.64, 54.9494));
            spawnPositions.Add(new Vector3(-287.057, 2837.26, 55.137));
            spawnPositions.Add(new Vector3(-303.213, 6152.16, 32.2342));

            isActive = false;
            activeZombies = new Dictionary<uint, ZombieType>();

            zombieSkins = new List<uint>();

            zombieSkins.Add(EventSkins.Zombie1);
            zombieSkins.Add(EventSkins.Zombie2);
            zombieSkins.Add(EventSkins.Zombie3);
            zombieSkins.Add(EventSkins.Zombie4);
            zombieSkins.Add(EventSkins.Zombie5);

            return base.OnLoad();
        }

        public Vector3 GetClosestSpawn(Vector3 Position)
        {
            Vector3 returnPos = spawnPositions.First();
            foreach (Vector3 pos in spawnPositions)
            {
                if (Position.DistanceTo(pos) < Position.DistanceTo(returnPos))
                {
                    returnPos = pos;
                }
            }

            return returnPos;
        }

        public List<DbPlayer> GetActiveZombiePlayers()
        {
            return Players.Players.Instance.GetValidPlayers().Where(p => p != null && activeZombies.ContainsKey(p.Id) && p.IsValid()).ToList();
        }

        public List<DbPlayer> GetActiveZombiePlayersByCategorie(ZombieType zombieType)
        {
            return Players.Players.Instance.GetValidPlayers().Where(p => p != null && activeZombies.ContainsKey(p.Id) && activeZombies[p.Id] == zombieType && p.IsValid()).ToList();
        }

        public override void OnPlayerSpawn(DbPlayer dbPlayer)
        {
            if(HalloweenModule.isActive)
            {
                if(dbPlayer.IsZombie())
                {
                    dbPlayer.Player?.TriggerEvent("updatesuperjump", false);

                    Random rnd = new Random();

                    if (rnd.Next(1, 20) < 10)
                    {
                        HalloweenModule.Instance.activeZombies[dbPlayer.Id] = ZombieType.SUPERJUMP;
                    }
                    else
                    {
                        HalloweenModule.Instance.activeZombies[dbPlayer.Id] = ZombieType.TANK;
                    }

                    ZombieType playerZombieType = dbPlayer.GetZombieType();

                    if (playerZombieType == ZombieType.SUPERJUMP)
                    {
                        dbPlayer.Player?.TriggerEvent("updatesuperjump", true);
                        dbPlayer.Player?.TriggerEvent("updaterunspeed", true);
                    }
                    else
                    {
                        dbPlayer.Player?.TriggerEvent("updaterunspeed", true);
                        // tank
                    }

                    dbPlayer.RemoveWeapons();

                    dbPlayer.GiveWeapon(WeaponHash.Hatchet, 1);

                    Random random = new Random();

                    dbPlayer.Player.SetPosition(GetClosestSpawn(dbPlayer.Player.Position));
                    dbPlayer.SetSkin((PedHash)GetZombieSkinRandom());
                }
            }
        }

        public uint GetZombieSkinRandom()
        {
            var random = new Random();

            int index = random.Next(HalloweenModule.Instance.zombieSkins.Count);
            return HalloweenModule.Instance.zombieSkins[index];
        }

        public override void OnTenSecUpdate()
        {
            if (!HalloweenModule.isActive) return;
            // investplayer in vehicle & range
            foreach(DbPlayer iPlayer in GetActiveZombiePlayers())
            {
                if (iPlayer == null || !iPlayer.IsValid()) continue;
                
                foreach(DbPlayer closestPlayers in Players.Players.Instance.GetValidPlayers().ToList().Where(p => p.Player.Position.DistanceTo(iPlayer.Player.Position) < 4.0f && p.Player.IsInVehicle))
                {
                    closestPlayers.InfestPlayer();
                }

                if(iPlayer.GetZombieType() == ZombieType.TANK)
                {
                    iPlayer.SetHealth(100);
                    iPlayer.SetArmorPlayer(iPlayer.Player.Armor += 50);
                }
            }

        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandstarthalloween(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid() || (iPlayer.Rank.Id != 6 && iPlayer.Rank.Id != 5)) return;
                       
            // Set Active
            if(!HalloweenModule.isActive)
            {
                HalloweenModule.isActive = true;

                NAPI.World.SetWeather(GTANetworkAPI.Weather.THUNDER);
                Main.m_CurrentWeather = GTANetworkAPI.Weather.THUNDER;
                Main.WeatherOverride = true;

                Weather.WeatherModule.Instance.SetBlackout(true);

                Configurations.Configuration.Instance.WeaponDamageMultipier = 0.3f;
                Configurations.Configuration.Instance.MeeleDamageMultiplier = 2.0f;

                // Alle türen auf
                foreach (Door door in DoorModule.Instance.GetAll().Values)
                {
                    door.SetLocked(false);
                }

                // Alle Jumppoints auf
                foreach (JumpPoint jp in JumpPointModule.Instance.jumpPoints.Values)
                {
                    jp.Locked = false;
                }

                // alle fahrzeuge ausschalten
                foreach(SxVehicle sxVeh in VehicleHandler.Instance.GetAllVehicles())
                {
                    sxVeh.SyncExtension.SetEngineStatus(false);
                }

                foreach(DbPlayer dbPlayer in FarmingModule.FarmingList.ToList())
                {
                    dbPlayer.Player.TriggerEvent("freezePlayer", false);
                    dbPlayer.StopAnimation();
                    dbPlayer.ResetData("pressedEOnFarm");
                    if (FarmingModule.FarmingList.Contains(dbPlayer)) FarmingModule.FarmingList.Remove(dbPlayer);
                }

                foreach(DbPlayer dbPlayer in Players.Players.Instance.GetValidPlayers())
                {
                    dbPlayer.SendNewNotification($"1337Allahuakbar$halloween", duration: 104000);
                    dbPlayer.GiveCWS(CWSTypes.Halloween, 50);
                    dbPlayer.Player.TriggerEvent("setPlayerDamageMultiplier", Configurations.Configuration.Instance.WeaponDamageMultipier, Configurations.Configuration.Instance.MeeleDamageMultiplier);
                }

                Chats.SendGlobalMessage($"ACHTUNG, durch eine Mutation des Z1 Virus kommt es zu tollwutähnlichen Symptomen! Alle Waffen sind zum Schutz vor infizierten Personen erlaubt! Gott beschütze Sie! #Halloween2020", COLOR.LIGHTBLUE, ICON.GOV, 20000);
            }
            else
            {
                HalloweenModule.isActive = false;

                foreach(DbPlayer dbPlayer in GetActiveZombiePlayers())
                {
                    if(dbPlayer != null && dbPlayer.IsValid())
                    {
                        dbPlayer.ApplyCharacter();
                        if(dbPlayer.HasData("zombie"))
                        {
                            iPlayer.Player?.TriggerEvent("updatesuperjump", false);
                            iPlayer.Player?.TriggerEvent("updaterunspeed", false);
                            dbPlayer.ResetData("zombie");
                        }
                    }
                }
                HalloweenModule.Instance.activeZombies.Clear();
            }
            return;
        }
        
       [CommandPermission(PlayerRankPermission = true)]
       [Command]
       public void Commandsetzombie(Player player, string returnstring)
       {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid() || (iPlayer.Rank.Id != 6 && iPlayer.Rank.Id != 5 && iPlayer.Rank.Id != 8)) return;

            if (returnstring.Length < 2) return;

            DbPlayer target = Players.Players.Instance.FindPlayer(returnstring);
            if (target != null && target.IsValid())
            {
                target.InfestPlayer();
                iPlayer.SendNewNotification($"Du hast {target.Player.Name} zum Zombie gemacht!");
            }

            return;
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandremovezombie(Player player, string returnstring)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid() || (iPlayer.Rank.Id != 6 && iPlayer.Rank.Id != 5 && iPlayer.Rank.Id != 8)) return;

            if (returnstring.Length < 2) return;

            DbPlayer target = Players.Players.Instance.FindPlayer(returnstring);
            if (target != null && target.IsValid())
            {
                if(HalloweenModule.Instance.activeZombies.ContainsKey(target.Id))
                {
                    HalloweenModule.Instance.activeZombies.Remove(target.Id);
                    iPlayer.SendNewNotification($"Du hast {target.Player.Name} normal gemacht!");
                }
            }

            return;
        }

        public override void OnPlayerWeaponSwitch(DbPlayer dbPlayer, WeaponHash oldgun, WeaponHash newgun)
        {
            if (HalloweenModule.isActive)
            {
                if (dbPlayer.IsZombie())
                {
                    NAPI.Player.SetPlayerCurrentWeapon(dbPlayer.Player, WeaponHash.Hatchet);
                }
            }
        }
    }

    public static class HalloweenPlayerExtension
    {
        public static void InfestPlayer(this DbPlayer iPlayer)
        {
            //if (!HalloweenModule.isActive || iPlayer.IsZombie()) return;

            if (iPlayer.IsZombie()) return;

            Random rnd = new Random();

            if(rnd.Next(1, 20) < 10)
            {
                HalloweenModule.Instance.activeZombies.Add(iPlayer.Id, ZombieType.SUPERJUMP);
            }
            else
            {
                HalloweenModule.Instance.activeZombies.Add(iPlayer.Id, ZombieType.TANK);
            }

            iPlayer.SetSkin(HalloweenModule.Instance.GetZombieSkinRandom());
            iPlayer.SendNewNotification("Du wurdest infisziert!");

            iPlayer.RemoveWeapons();

            NAPI.Player.SetPlayerCurrentWeapon(iPlayer.Player, WeaponHash.Unarmed);

            iPlayer.GiveWeapon(WeaponHash.Hatchet, 1);

        }

        public static ZombieType GetZombieType(this DbPlayer dbPlayer)
        {
            if (!dbPlayer.IsZombie()) return ZombieType.SUPERJUMP;
            return HalloweenModule.Instance.activeZombies[dbPlayer.Id];
        }

        public static bool IsZombie(this DbPlayer iPlayer)
        {
            return HalloweenModule.isActive && HalloweenModule.Instance.activeZombies.ContainsKey(iPlayer.Id);
        }
    }
}
