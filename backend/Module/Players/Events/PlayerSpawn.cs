using GTANetworkAPI;
using System;
using System.Linq;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Crime;
using VMP_CNR.Module.Gangwar;
using System.Threading.Tasks;
using VMP_CNR.Module.Einreiseamt;
using VMP_CNR.Module.AnimationMenu;
using VMP_CNR.Module.Swat;
using VMP_CNR.Module.Events.Halloween;
using VMP_CNR.Module.Paintball;
using Newtonsoft.Json;
using VMP_CNR.Handler;
using VMP_CNR.Module.Anticheat;
using Google.Protobuf.WellKnownTypes;
using VMP_CNR.Module.Teams;

namespace VMP_CNR.Module.Players.Events
{
    public class PlayerSpawn : Script
    {
        public static void InitPlayerSpawnData(Player player)
        {
            var iPlayer = player.GetPlayer();

            if (iPlayer == null || !iPlayer.IsValid())
            {
                return;
            }

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                if (iPlayer.isInjured())
                {
                    iPlayer.Player.TriggerEvent("startScreenEffect", "DeathFailMPIn", 5000, true);
                }
                else
                {
                    iPlayer.Player.TriggerEvent("stopScreenEffect", "DeathFailMPIn");
                }
            });

            iPlayer.Player.TriggerEvent("updateInjured", iPlayer.isInjured());
            iPlayer.Player.SetSharedData("deathStatus", iPlayer.isInjured());

            player.Transparency = 255;

            player.TriggerEvent("setPlayerHealthRechargeMultiplier");
            iPlayer.Player.SendNative(0xEBD76F2359F190AC, iPlayer.Player.Handle);

            // Workaround for freeze fails
            if (iPlayer.Freezed == false)
            {
                player.TriggerEvent("freezePlayer", false);
            }
        }


        [ServerEvent(Event.PlayerSpawn)]
        public static void OnPlayerSpawn(Player player)
        {
            player.SendNative(0xEBD76F2359F190AC, player.Handle);

            try
            {
                var firstName = "";
                var lastName = "";

                if (player == null) return;

                var iPlayer = player.GetPlayer();

                string l_EventKey = Helper.Helper.GenerateAuthKey();
                if (player.HasData("auth_key"))
                    player.ResetData("auth_key");

                player.SetData("auth_key", l_EventKey);

                if (iPlayer == null || !iPlayer.IsValid())
                {  
                    player.Health = 99;
                    // anti blocking player (online etc..)

                    // da isn anderer Spieler?
                    if (NAPI.Pools.GetAllPlayers().ToList().Where(p => p != null && p.Name == player.Name && p.HasData("connectedAt")).Count() > 0)
                    {
                        Player olderPlayer = NAPI.Pools.GetAllPlayers().ToList().Where(p => p != null && p.Name == player.Name && p.HasData("connectedAt")).First();

                        if (olderPlayer != null && olderPlayer != player && (!olderPlayer.HasData("Connected") || olderPlayer.GetData<bool>("Connected") != true))
                        {
                            olderPlayer.SendNotification("Duplicate Entry 1");
                            olderPlayer.ResetData("Duplicate Entry!");
                            olderPlayer.Kick("Duplicate Entry!");
                            DiscordHandler.SendMessage($"LOGIN FEHLER!", $"(PlayerSpawn.cs - 85) {olderPlayer.Name} - {DateTime.Now.ToString()}");
                            return;
                        }
                    }

                    player.SetData("connectedAt", DateTime.Now);
                    PlayerConnect.OnPlayerConnected(player);
                    return;
                }
                else
                {
                    iPlayer.SetAcPlayerSpawnDeath();
                    if (iPlayer.Firstspawn)
                    {
                        Modules.Instance.OnPlayerLoggedIn(iPlayer);
                    }
                }

                // Interrupt wrong Spawn saving
                iPlayer.ResetData("lastPosition");

                /*
                if (iPlayer.LastSpawnEvent.AddSeconds(2) > DateTime.Now) return;
                iPlayer.LastSpawnEvent = DateTime.Now;}
                */

                Modules.Instance.OnPlayerSpawn(iPlayer);

                iPlayer.Player.SendNative(0xEBD76F2359F190AC, iPlayer.Player.Handle);

                // init Spawn details
                var pos = new Vector3();
                float heading = 0.0f;

                uint dimension = 0;
                DimensionType dimensionType = DimensionType.World;

                // Default Data required for Spawn
                bool FreezedNoAnim = false;

                if (iPlayer.NeuEingereist())
                {
                    if (iPlayer.isInjured()) iPlayer.revive();

                    iPlayer.jailtime[0] = 0;
                    iPlayer.ApplyCharacter();

                    pos = new GTANetworkAPI.Vector3(-1144.26, -2792.27, 27.708);
                    heading = 237.428f;
                    dimension = 0;

                    //iPlayer.Player.Freeze(true, true, true);
                    iPlayer.Player.TriggerEvent("freezePlayer", true);
                    iPlayer.Player.SetPosition(pos);
                    iPlayer.SetDimension(dimension);

                    Task.Run(async () =>
                    {
                        await Task.Delay(20000);
                        //iPlayer.Player.Freeze(false, true, true);
                        iPlayer.Player.TriggerEvent("freezePlayer", false);
                        iPlayer.EinreiseSpawn();
                    });
                }
                else if (iPlayer.isInjured())
                {
                    pos.X = iPlayer.dead_x[0];
                    pos.Y = iPlayer.dead_y[0];
                    pos.Z = iPlayer.dead_z[0];
                    FreezedNoAnim = true;

                    if (iPlayer.HasData("tmpDeathDimension"))
                    {
                        dimension = iPlayer.GetData("tmpDeathDimension");
                        iPlayer.ResetData("tmpDeathDimension");
                    }

                    if (GangwarTownModule.Instance.IsTeamInGangwar(iPlayer.Team) && iPlayer.DimensionType[0] == DimensionType.Gangwar)
                    {
                        dimension = GangwarModule.Instance.DefaultDimension;
                    }

                    if(iPlayer.Injury.StabilizedInjuryId != 0 && iPlayer.Injury.Id != InjuryModule.Instance.InjuryKrankentransport)
                    {
                        VoiceListHandler.AddToDeath(iPlayer);
                    }
                    else VoiceListHandler.RemoveFromDeath(iPlayer);
                }
                else if (iPlayer.HasData("komaSpawn"))
                {
                    iPlayer.ResetData("komaSpawn");

                    Vector3 spawnPos = InjuryModule.Instance.GetClosestHospital(new Vector3(iPlayer.dead_x[0], iPlayer.dead_y[0], iPlayer.dead_z[0]));
                    iPlayer.SetPlayerKomaSpawn();

                    pos.X = spawnPos.X;
                    pos.Y = spawnPos.Y;
                    pos.Z = spawnPos.Z;
                    dimension = 0;
                }
                else if (iPlayer.HasData("SMGkilledPos") && iPlayer.HasData("SMGkilledDim"))
                {

                    pos = (Vector3)iPlayer.GetData("SMGkilledPos");
                    heading = 0.0f;
                    dimension = iPlayer.GetData("SMGkilledDim");

                    iPlayer.SetStunned(true);
                    FreezedNoAnim = true;
                }
                else if (HalloweenModule.isActive && iPlayer.IsZombie())
                {

                    pos = HalloweenModule.Instance.GetClosestSpawn(iPlayer.Player.Position);
                    heading = 0.0f;
                    dimension = 0;

                    iPlayer.SetStunned(false);
                    FreezedNoAnim = true;
                }
                else if (iPlayer.jailtime[0] > 1 && !iPlayer.Firstspawn)
                {
                    //Jail Spawn
                    if (iPlayer.jailtime[0] > 1)
                    {
                        pos.X = 1691.28f;
                        pos.Y = 2565.91f;
                        pos.Z = 45.5648f;
                        heading = 177.876f;
                    }
                }
                else
                {
                    if (iPlayer.spawnchange[0] == 1 && (iPlayer.ownHouse[0] > 0 || iPlayer.IsTenant())) //Haus
                    {
                        House iHouse;
                        if ((iHouse = HouseModule.Instance.Get(iPlayer.ownHouse[0])) != null)
                        {
                            pos = iHouse.Position;
                            heading = iHouse.Heading;
                        }
                        else if ((iHouse = HouseModule.Instance.Get(iPlayer.GetTenant().HouseId)) != null)
                        {
                            pos = iHouse.Position;
                            heading = iHouse.Heading;
                        }
                    }
                    else
                    {

                        if (iPlayer.Team.TeamSpawns.TryGetValue(iPlayer.fspawn[0], out var spawn))
                        {
                            pos = spawn.Position;
                            heading = spawn.Heading;
                        }
                        else
                        {
                            spawn = iPlayer.Team.TeamSpawns.FirstOrDefault().Value;
                            if (spawn != null)
                            {
                                pos = spawn.Position;
                                heading = spawn.Heading;
                            }
                        }
                    }
                }

                // Setting Pos
                if (iPlayer.Firstspawn)
                {
                    if (iPlayer.pos_x[0] != 0f)
                    {
                        iPlayer.spawnProtection = DateTime.Now;

                        pos = new GTANetworkAPI.Vector3(iPlayer.pos_x[0], iPlayer.pos_y[0], iPlayer.pos_z[0] + 0.1f);

                        if(iPlayer.HasData("cayoPerico"))
                        {
                            pos = new Vector3(pos.X, pos.Y, pos.Z + 3.0f);
                        }

                        heading = iPlayer.pos_heading[0];

                    }

                    Task.Run(async () =>
                    {
                        await Task.Delay(9000);
                        Modules.Instance.OnPlayerFirstSpawnAfterSync(iPlayer);
                    });
                }

                if (iPlayer.Firstspawn)
                {
                    Main.OnPlayerFirstSpawn(player);

                    // Fallback ...
                    if(iPlayer.DimensionType[0] == DimensionType.Gangwar)
                    {
                        iPlayer.Dimension[0] = 0;
                        iPlayer.DimensionType[0] = DimensionType.World;
                    }

                    // Load player Dimension from DB
                    dimension = iPlayer.Dimension[0];
                    dimensionType = iPlayer.DimensionType[0];

                    DialogMigrator.CloseUserDialog(player, Dialogs.menu_info);

                    // Connect to TS
                    Teamspeak.Connect(player, iPlayer.GetName());

                    var crumbs = player.Name.Split('_');
                    if (crumbs.Length > 1)
                    {
                        firstName = crumbs[0].ToString();
                        lastName = crumbs[1].ToString();
                        // Support multiple lastNames
                        for (int i = 2; i < crumbs.Length; i++)
                        {
                            lastName += "_" + crumbs[i];
                        }

                        string insurance = "keine";
                        if(iPlayer.InsuranceType == 1)
                        {
                            insurance = "vorhanden";
                        }
                        else if(iPlayer.InsuranceType == 2)
                        {
                            insurance = "privat";
                        }
                        player.TriggerEvent("hudReady");
                        //   player.TriggerEvent("SetOwnAnimData", JsonConvert.SerializeObject(new AnimationSyncItem(iPlayer)));

                        Console.WriteLine(iPlayer.GetJsonAnimationsShortcuts());
                       
                        player.TriggerEvent("onPlayerLoaded", firstName, lastName, iPlayer.Id, iPlayer.rp[0],
                            iPlayer.ActiveBusiness?.Id ?? 0, iPlayer.grade[0], iPlayer.money[0], 0,
                            iPlayer.ownHouse[0], iPlayer.TeamId, iPlayer.TeamRank, iPlayer.Level, iPlayer.isInjured(), iPlayer.IsInDuty(),
                            iPlayer.IsTied, iPlayer.IsCuffed, iPlayer.VoiceHash, iPlayer.funkStatus, iPlayer.handy[0], iPlayer.job[0], 
                            0, iPlayer.GetJsonAnimationsShortcuts(), iPlayer.RankId, 
                            Configurations.Configuration.Instance.WeaponDamageMultipier, Configurations.Configuration.Instance.MeeleDamageMultiplier, 
                            Configurations.Configuration.Instance.PlayerSync, Configurations.Configuration.Instance.VehicleSync, iPlayer.blackmoney[0], 1, insurance, iPlayer.zwd[0]);


                        iPlayer.Player.TriggerEvent("setPlayerInfoVoiceHash", iPlayer.VoiceHash);
                        iPlayer.Player.TriggerEvent("setPlayerInfoId", iPlayer.ForumId);


                    }
                    else iPlayer.Kick();

                    // Cuff & Tie
                    if (iPlayer.IsCuffed)
                    {
                        iPlayer.SetCuffed(true);
                        FreezedNoAnim = true;
                    }

                    if (iPlayer.IsTied)
                    {
                        iPlayer.SetTied(true);
                        FreezedNoAnim = true;
                    }
                }
                else
                {
                    iPlayer.SetHealth(100);
                }

                if (iPlayer.jailtime[0] > 0)
                {
                    iPlayer.ApplyCharacter();
                }

                InitPlayerSpawnData(player);

                iPlayer.LoadPlayerWeapons();

                if (!iPlayer.Firstspawn && iPlayer.Paintball == 1)
                {
                    player.TriggerEvent("freezePlayer", false);
                    PaintballModule.Instance.Spawn(iPlayer, false, false);
                    iPlayer.StopAnimation();

                    Task.Run(async () =>
                    {
                        await Task.Delay(1500);
                        player.TriggerEvent("freezePlayer", false);
                    });
                    return;
                }
                else if (iPlayer.NeuEingereist())
                {
                    if (iPlayer.isInjured()) iPlayer.revive();

                    iPlayer.jailtime[0] = 0;
                    iPlayer.ApplyCharacter(true);

                    // Start Customization
                        iPlayer.StartCustomization();
                }
                else
                {

                    iPlayer.Player.SetPosition(pos);
                    iPlayer.Player.SetRotation(heading);
                    iPlayer.SetDimension(dimension);

                    if (!FreezedNoAnim)
                    {
                        // uncuff....
                        iPlayer.SetTied(false);
                        iPlayer.SetMedicCuffed(false);
                        iPlayer.SetCuffed(false);
                        player.TriggerEvent("freezePlayer", false);

                        Task.Run(async () =>
                        {
                            NAPI.Task.Run(() =>
                            {
                                player.TriggerEvent("freezePlayer", true);
                                player.SetPosition(pos);
                                player.SetRotation(heading);
                            });
                            await Task.Delay(1000);
                            NAPI.Task.Run(() =>
                            {
                                player.SetPosition(pos);
                                player.SetRotation(heading);
                            });
                            await Task.Delay(1500);
                            NAPI.Task.Run(() =>
                            {
                                player.TriggerEvent("freezePlayer", false);
                            });
                        });
                    }

                    if (iPlayer.Firstspawn)
                    {
                        Task.Run(async () =>
                        {
                            NAPI.Task.Run(() =>
                            {
                                iPlayer.Player.TriggerEvent("moveSkyCamera", iPlayer.Player, "up", 1, false);
                            });
                            await Task.Delay(4000);
                            NAPI.Task.Run(() =>
                            {
                                iPlayer.Player.TriggerEvent("moveSkyCamera", iPlayer.Player, "down", 1, false);
                            });
                        });
                        NAPI.Task.Run(() =>
                        {
                            iPlayer.ApplyCharacter(true);
                            iPlayer.ApplyPlayerHealth();

                            iPlayer.Firstspawn = false;
                        });
                    }

                    if (iPlayer.isInjured())
                    {
                        iPlayer.ApplyDeathEffects();
                    }
                    else
                    {
                        iPlayer.Player.TriggerEvent("stopScreenEffect", "DeathFailMPIn");
                    }

                    if (iPlayer.HasData("SMGkilledPos") && iPlayer.HasData("SMGkilledDim"))
                    {
                       
                        Main.m_AsyncThread.AddToAsyncThread(new Task(async () =>
                        {

                            await Task.Delay(1500);
                            NAPI.Task.Run(() =>
                            {
                                iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "misstrevor3_beatup", "guard_beatup_kickidle_dockworker");
                            });
                            await Task.Delay(30000);
                            NAPI.Task.Run(() =>
                            {
                                iPlayer.SetStunned(false);
                                iPlayer.ResetData("SMGkilledPos");
                            });
                        }));
                    }
                    /*
                    if(HalloweenModule.isActive && iPlayer.IsZombie())
                    {
                        Main.m_AsyncThread.AddToAsyncThread(new Task(async () =>
                        {

                            await Task.Delay(3000);
                            iPlayer.StopAnimation();
                            iPlayer.Player.TriggerEvent("freezePlayer", false);
                            iPlayer.IsCuffed = false;
                            iPlayer.SetCuffed(false);
                            iPlayer.SetTied(false);

                            await Task.Delay(3000);
                            iPlayer.Player.TriggerEvent("freezePlayer", false);

                        }));
                    }*/
                }
            }
            catch (Exception e)
            {
                Logging.Logger.Crash(e);
            }
        }
    }
}
