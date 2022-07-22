﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Handler;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Einreiseamt;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Sync;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Anticheat
{
    public class ACTeleportReportObject
    {
        public Vector3 SourcePos { get; set; }
        public Vector3 DestinationPos { get; set; }

        public DateTime ReportDateTime { get; set; }

        public float Distance { get; set; }
        public bool OnFoot { get; set; }
        public string VehicleReportString { get; set; }
    }

    public class AntiCheatModule : Module<AntiCheatModule>
    {
        public Dictionary<uint, List<ACTeleportReportObject>> ACTeleportReports = new Dictionary<uint, List<ACTeleportReportObject>>();

        protected override bool OnLoad()
        {

            MenuManager.Instance.AddBuilder(new Anticheat.Menu.AntiCheatTeleportDetailMenu());
            MenuManager.Instance.AddBuilder(new Anticheat.Menu.AntiCheatTeleportMenu());

            return base.OnLoad();
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandcheckactp(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null) return;

            if (!iPlayer.IsValid() || !iPlayer.CanAccessMethod())
            {
                iPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.AntiCheatTeleportMenu, iPlayer).Show(iPlayer);
            return;
        }

        public override void OnPlayerWeaponSwitch(DbPlayer dbPlayer, WeaponHash oldgun, WeaponHash newgun)
        {
            if (ServerFeatures.IsActive("ac-weaponcheck"))
            {
                if (dbPlayer.DimensionType[0] == DimensionType.Gangwar || dbPlayer.HasData("paintball_map")) return;

                if (dbPlayer.HasData("ac-ignorews")) return;

                Dictionary<WeaponHash, int> weapons = new Dictionary<WeaponHash, int>();

                if (dbPlayer.HasData("ac-compareweaponobject"))
                {
                    weapons = dbPlayer.GetData("ac-compareweaponobject");
                }

                if (!weapons.ContainsKey(newgun) && newgun != WeaponHash.Unarmed && (int)newgun != 0)
                {
                    if (UInt32.TryParse(newgun.ToString(), out uint testInt)) return;

                    Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Anticheat-Verdacht: {dbPlayer.Player.Name} (Weaponcheat {newgun.ToString()} spawned).");
                    Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.WeaponCheat, $"(Weaponcheat {newgun.ToString()} spawned)");
                    dbPlayer.Player.RemoveWeapon(newgun);
                }
            }
        }

        public override void OnPlayerEnterVehicle(DbPlayer dbPlayer, Vehicle vehicle, sbyte seat)
        {
            if (ServerFeatures.IsActive("ac-checkvehicletp"))
            {
                if (vehicle != null && seat == -1)
                {
                    CheckVehicleGotTeleported(dbPlayer, vehicle.GetVehicle());
                }
            }
        }

        public override void OnPlayerExitVehicle(DbPlayer dbPlayer, Vehicle vehicle)
        {
            if (ServerFeatures.IsActive("ac-checkvehicletp"))
            {
                if (vehicle != null)
                {
                    SxVehicle sxVehicle = vehicle.GetVehicle();
                    if (sxVehicle != null && sxVehicle.IsValid())
                    {
                        UpdatePosition(sxVehicle, dbPlayer);
                    }
                }
            }
            if (ServerFeatures.IsActive("ac-maxspeed"))
            {
                if (dbPlayer.HasData("speedCheckFirst"))
                {
                    dbPlayer.ResetData("speedCheckFirst");
                }
            }
        }

        public void ACBanPlayer(DbPlayer iPlayer, string reason)
        {
            Logging.Logger.LogToAcDetections(iPlayer.Id, Logging.ACTypes.AntiCheatBan, reason);
            iPlayer.warns[0] = 3;
            SocialBanHandler.Instance.AddEntry(iPlayer.Player);
            PlayerLoginDataValidationModule.SyncUserBanToForum(iPlayer.ForumId);
            iPlayer.Player.Kick();
        }

        public override void OnFiveSecUpdate()
        {
            if (!AnticheatThread.Instance.IsActive())
                return;

            AnticheatThread.Instance.ScheduleTask(new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    foreach (DbPlayer dbPlayer in Players.Players.Instance.GetValidPlayers())
                    {
                        if (dbPlayer == null || !dbPlayer.IsValid()) continue;

                        if (dbPlayer.HasData("ac-ignorews"))
                        {
                            if ((int)dbPlayer.GetData("ac-ignorews") > 1)
                            {
                                int tp = (int)dbPlayer.GetData("ac-ignorews") - 1;
                                dbPlayer.SetData("ac-ignorews", tp);
                            }
                            else
                            {
                                dbPlayer.ResetData("ac-ignorews");
                            }
                        }

                        if (ServerFeatures.IsActive("ac-dimensioncheck"))
                        {
                            dbPlayer.AcDimensionCheck();
                        }

                        if (ServerFeatures.IsActive("ac-exiteacheck") && !dbPlayer.Firstspawn)
                        {
                            dbPlayer.AcEinreiseAmtCheck();
                        }

                        if (ServerFeatures.IsActive("ac-vehcheck"))
                        {
                            dbPlayer.AcVehicleDrivingKeyCheck();
                        }

                        // Armorstuff AC
                        if (ServerFeatures.IsActive("ac-armor") && !dbPlayer.Firstspawn)
                        {
                            dbPlayer.AcArmorCheck();
                        }
                        
                        // Healthstuff AC
                        if (ServerFeatures.IsActive("ac-health") && !dbPlayer.Firstspawn)
                        {
                            dbPlayer.AcHealthCheck();
                        }
                        if (ServerFeatures.IsActive("ac-teleport"))
                        {
                            dbPlayer.AcTeleportCheck();
                        }
                        if (ServerFeatures.IsActive("ac-maxspeed"))
                        {
                            dbPlayer.AcMaxSpeedValidator();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.Logger.Crash(e);
                }
            }));
        }


        public static void UpdatePosition(SxVehicle sxVehicle, DbPlayer xPlayer)
        {
            sxVehicle.SetData("position", sxVehicle.entity.Position);
            sxVehicle.SetData("lastExitPlayer", xPlayer.Id);
        }

        public static void CheckVehicleGotTeleported(DbPlayer dbPlayer, SxVehicle sxVehicle)
        {
            
            if (sxVehicle != null && sxVehicle.IsValid() && dbPlayer != null && dbPlayer.IsValid() && !dbPlayer.CanControl(sxVehicle))
            {
                if (sxVehicle.databaseId == 0 || (!sxVehicle.IsPlayerVehicle() && !sxVehicle.IsTeamVehicle())) return;

                if (!sxVehicle.HasData("position") || !sxVehicle.HasData("lastExitPlayer")) return;

                if (sxVehicle.Data.ClassificationId == 7 || sxVehicle.Data.ClassificationId == 8 || sxVehicle.Data.ClassificationId == 9 || sxVehicle.Data.ClassificationId == 3) return;

                if (dbPlayer.TeamRank > 0) return; // Teammitglieder ausgeschlossen

                // Spieler der grade erst ausgestiegen ist löst es aus zb beim rausfallen kp
                if (sxVehicle.GetData("lastExitPlayer") == dbPlayer.Id) return;

                Vector3 lastPos = sxVehicle.GetData("position");

                int distance = Convert.ToInt32(lastPos.DistanceTo2D(dbPlayer.Player.Position));

                if (distance > 20.0f)
                {
                    Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Anticheat-Verdacht: Fahrzeug Teleport (wurde von {dbPlayer.GetName()} über eine Distance von {distance} teleportiert).");
                    Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.VehicleTeleport, $"{sxVehicle.databaseId} {distance}m");
                    sxVehicle.SetData("position", sxVehicle.entity.Position);
                }
            }
        }
    }

    public static class AntiCheatPlayerExtension
    {
        public static void AcEinreiseAmtCheck(this DbPlayer dbPlayer)
        {
            if (dbPlayer.hasPerso[0] == 0)
            {
                if (dbPlayer.Player.Position.DistanceTo(new Vector3(-1144.26, -2792.27, 27.7081)) > 150
                    && dbPlayer.Player.Position.DistanceTo(EinreiseamtModule.PositionPC1) > 150
                    && dbPlayer.Player.Position.DistanceTo(EinreiseamtModule.PositionPC2) > 150
                    && dbPlayer.Player.Position.DistanceTo(EinreiseamtModule.PositionPC3) > 150
                    && dbPlayer.Player.Position.DistanceTo(EinreiseamtModule.PositionPC4) > 150)
                {
                    Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"DRINGENDER-Anticheat-Verdacht: {dbPlayer.Player.Name} (Einreiseamt ohne Perso verlassen)");
                    Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.EinreiseAmtVerlassen, $"");
                    dbPlayer.Player.Freeze(true, true, true);
                }
            }
        }
        public static void AcVehicleDrivingKeyCheck(this DbPlayer dbPlayer)
        {
            if (dbPlayer.Player.IsInVehicle)
            {
                SxVehicle sxVeh = dbPlayer.Player.Vehicle.GetVehicle();
                if (sxVeh != null && sxVeh.IsValid())
                {
                    if (dbPlayer.Player.VehicleSeat == -1)
                    {
                        if (!dbPlayer.CanControl(sxVeh) && sxVeh.SyncExtension != null && sxVeh.GetSpeed() > 20 && sxVeh.Data != null && sxVeh.Data.ClassificationId != 2 && sxVeh.fuel > 0)
                        {
                            if (!sxVeh.SyncExtension.EngineOn || (!sxVeh.entity.EngineStatus && !sxVeh.SyncExtension.EngineOn))
                            {
                                Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"DRINGENDER-Anticheat-Verdacht: {dbPlayer.Player.Name} (Vehicle Control without Key (Motoraus wird bewegt))");
                                Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.VehicleControlAbuse, $"{sxVeh.databaseId}");
                                dbPlayer.WarpOutOfVehicle();
                            }
                        }
                    }
                }
            }
        }

        public static void AcMaxSpeedValidator(this DbPlayer dbPlayer)
        {
            if (dbPlayer.Player.IsInVehicle)
            {
                SxVehicle sxVeh = dbPlayer.Player.Vehicle.GetVehicle();
                if (sxVeh != null && sxVeh.IsValid())
                {
                    if (dbPlayer.Player.VehicleSeat == -1)
                    {
                        int Speed = sxVeh.GetSpeed();
                        if (sxVeh.Data == null) return;

                        if(sxVeh.Data.MaxSpeed > 0 && sxVeh.Data.MaxSpeed+10 < Speed)
                        {
                            if (dbPlayer.HasData("speedCheckFirst"))
                            {
                                Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Anticheat-Verdacht: {dbPlayer.Player.Name} (FahrzeugSpeed von {sxVeh.GetName()} überschritten {Speed} km/h  (LIMIT {sxVeh.Data.MaxSpeed})).");
                                Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.Speedhack, $"{dbPlayer.Player.Name} (FahrzeugSpeed überschritten {Speed} km/h  (LIMIT {sxVeh.Data.MaxSpeed}))");

                                // Melde & Resette
                                dbPlayer.ResetData("speedCheckFirst");
                            }
                            else
                            {
                                // Counte +1 and resync
                                dbPlayer.SetData("speedCheckFirst", true);

                                if (sxVeh != null && sxVeh.IsValid() && sxVeh.Data != null && sxVeh.Data.MaxSpeed > 0)
                                {
                                    dbPlayer.Player.TriggerEvent("setNormalSpeed", sxVeh.entity, sxVeh.Data.MaxSpeed);
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void AcDimensionCheck(this DbPlayer dbPlayer)
        {
            if (dbPlayer.HasData("ac_lastDimension"))
            {
                if (dbPlayer.HasData("serverDimensionChange"))
                {
                    if (Int32.TryParse(dbPlayer.GetData("serverDimensionChange").ToString(), out int serverDimensionChange) && serverDimensionChange > 1)
                    {
                        int tp = serverDimensionChange - 1;
                        dbPlayer.SetData("ac_lastDimension", dbPlayer.Player.Dimension);
                        dbPlayer.SetData("serverDimensionChange", tp);
                    }
                    else
                    {
                        dbPlayer.SetData("ac_lastDimension", dbPlayer.Player.Dimension);
                        dbPlayer.ResetData("serverDimensionChange");
                        return;
                    }
                }
                else
                {
                    if (dbPlayer.Player.Dimension != dbPlayer.GetData("ac_lastDimension"))
                    {
                        Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Anticheat-Verdacht: {dbPlayer.Player.Name} (Dimension Change {dbPlayer.GetData("ac_lastDimension")} zu {dbPlayer.Player.Dimension}).");
                        Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.DimensionChange, $"SD {dbPlayer.GetData("ac_lastDimension")} DD {dbPlayer.Player.Dimension}");
                    }

                    dbPlayer.SetData("ac_lastDimension", dbPlayer.Player.Dimension);
                    return;
                }
            }
            else dbPlayer.SetData("ac_lastDimension", dbPlayer.Player.Dimension);
        }

        public static void AcArmorCheck(this DbPlayer dbPlayer)
        {
            if (dbPlayer.HasData("ac_lastArmor"))
            {
                if (dbPlayer.HasData("serverArmorChanged"))
                {
                    if (dbPlayer.HasData("blockArmorCheat")) dbPlayer.ResetData("blockArmorCheat");

                    if (Int32.TryParse(dbPlayer.GetData("serverArmorChanged").ToString(), out int serverArmorChanged) && serverArmorChanged > 1)
                    {
                        int tp = serverArmorChanged - 1;
                        dbPlayer.SetData("ac_lastArmor", dbPlayer.Player.Armor);
                        dbPlayer.SetData("serverArmorChanged", tp);
                        return;
                    }
                    else
                    {
                        dbPlayer.SetData("ac_lastArmor", dbPlayer.Player.Armor);
                        dbPlayer.ResetData("serverArmorChanged");
                        return;
                    }
                }
                else
                {

                    int armor = dbPlayer.Player.Armor;
                    if (armor > dbPlayer.GetData("ac_lastArmor") && (int)dbPlayer.GetData("ac_lastArmor") >= 0)
                    {
                        Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Anticheat-Verdacht: {dbPlayer.Player.Name} (Armor Hack von {dbPlayer.GetData("ac_lastArmor")} zu {armor}).");
                        Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.ArmorCheat, $"SV {dbPlayer.GetData("ac_lastArmor")} DV {armor}");
                        dbPlayer.SetData("blockArmorCheat", true);
                    }

                    dbPlayer.SetData("ac_lastArmor", dbPlayer.Player.Armor);
                    return;
                }
            }
            else dbPlayer.SetData("ac_lastArmor", dbPlayer.Player.Armor);
        }
        public static void AcHealthCheck(this DbPlayer dbPlayer)
        {
            if (dbPlayer.HasData("ac_lastHealth"))
            {
                if (dbPlayer.HasData("ac-healthchange") || dbPlayer.IsInAdminDuty())
                {
                    if (Int32.TryParse(dbPlayer.GetData("ac-healthchange").ToString(), out int achealthchange) && achealthchange > 1)
                    {
                        int tp = achealthchange - 1;
                        dbPlayer.SetData("ac_lastHealth", dbPlayer.Player.Health);
                        dbPlayer.SetData("ac-healthchange", tp);
                    }
                    else
                    {
                        dbPlayer.SetData("ac_lastHealth", dbPlayer.Player.Health);
                        dbPlayer.ResetData("ac-healthchange");
                    }
                }
                else
                {

                    int health = dbPlayer.Player.Health;
                    if (health > dbPlayer.GetData("ac_lastHealth"))
                    {
                        if (((dbPlayer.GetData("ac_lastHealth") - health) > 10 || health > 51) && dbPlayer.GetData("ac_lastHealth") > 0 && !dbPlayer.Player.IsInVehicle && dbPlayer.GetData("ac_lastHealth") < 99)
                        {

                            Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Anticheat-Verdacht: {dbPlayer.Player.Name} (Health Hack von {dbPlayer.GetData("ac_lastHealth")} zu {health}).");
                            Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.HealthCheat, $"SV {dbPlayer.GetData("ac_lastHealth")} DV {health}");
                        }
                    }

                    dbPlayer.SetData("ac_lastHealth", dbPlayer.Player.Health);
                }
            }
            else dbPlayer.SetData("ac_lastHealth", dbPlayer.Player.Armor);
        }

        public static void AcTeleportCheck(this DbPlayer dbPlayer)
        {
            if (dbPlayer.HasData("ac_lastPos"))
            {
                if (dbPlayer.HasData("Teleport") || dbPlayer.Player.IsInVehicle || dbPlayer.Firstspawn)
                {
                    if (dbPlayer.Firstspawn)
                    {
                        if (dbPlayer.GetData("Teleport") < 5)
                        {
                            dbPlayer.SetData("Teleport", 5);
                        }
                        dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Position);
                        return;
                    }
                    else if (dbPlayer.Player.IsInVehicle)
                    {
                        if (dbPlayer.HasData("Teleport"))
                        {
                            if (dbPlayer.GetData("Teleport") < 3)
                            {
                                dbPlayer.SetData("Teleport", 3);
                                dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Vehicle.Position);
                                return;
                            }
                        }
                        else
                        {
                            dbPlayer.SetData("Teleport", 3);
                            dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Vehicle.Position);
                            return;
                        }
                    }
                    else
                    {
                        if (dbPlayer.GetData("Teleport") > 1)
                        {
                            int tp = dbPlayer.GetData("Teleport") - 1;
                            dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Position);
                            dbPlayer.SetData("Teleport", tp);
                            return;
                        }
                        else
                        {
                            dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Position);
                            dbPlayer.ResetData("Teleport");
                            return;
                        }
                    }
                }
                else
                {
                    Vector3 lastPos = dbPlayer.GetData("ac_lastPos");

                    // why lastpos = spawnpos kp...
                    if (lastPos.DistanceTo(new Vector3(17.4809, 637.872, 210.595)) < 15.0f)
                    {
                        dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Position);
                        return;
                    }

                    int distance = Convert.ToInt32(lastPos.DistanceTo2D(dbPlayer.Player.Position));
                    if (distance > 140.0f)
                    {
                        if (dbPlayer.Level < 3)
                        {
                            Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"DRINGEND Anticheat-Verdacht: {dbPlayer.Player.Name} (Teleporthack Distance {distance}m | unter Level 3).");
                        }
                        else
                        {
                            Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"Anticheat-Verdacht: {dbPlayer.Player.Name} (Teleporthack Distance {distance}m).");
                        }
                        Logging.Logger.LogToAcDetections(dbPlayer.Id, Logging.ACTypes.Teleport, $"Dist {distance}");

                        if (AntiCheatModule.Instance.ACTeleportReports.ContainsKey(dbPlayer.Id))
                        {
                            AntiCheatModule.Instance.ACTeleportReports[dbPlayer.Id].Add(new ACTeleportReportObject() { OnFoot = false, SourcePos = lastPos, DestinationPos = dbPlayer.Player.Position, ReportDateTime = DateTime.Now, VehicleReportString = "", Distance = distance });
                        }
                        else
                        {
                            List<ACTeleportReportObject> list = new List<ACTeleportReportObject>();
                            list.Add(new ACTeleportReportObject() { OnFoot = false, SourcePos = lastPos, DestinationPos = dbPlayer.Player.Position, ReportDateTime = DateTime.Now, VehicleReportString = "", Distance = distance });
                            AntiCheatModule.Instance.ACTeleportReports.Add(dbPlayer.Id, list);
                        }
                    }
                    dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Position);
                }
            }
            else dbPlayer.SetData("ac_lastPos", dbPlayer.Player.Position);
        }

        public static void SetACLogin(this DbPlayer iPlayer)
        {
            // Disable Anticheat for 70s
            iPlayer.SetData("ac-healthchange", 12);
            iPlayer.SetData("serverArmorChanged", 12);
            iPlayer.SetData("ignoreGodmode", 12);
            iPlayer.SetData("Teleport", 12);
            iPlayer.SetData("serverDimensionChange", 12);
            iPlayer.SetData("ac-ignorews", 12);
        }

        public static void SetAcPlayerSpawnDeath(this DbPlayer iPlayer)
        {
            // Disable Anticheat for 30s
            if (!iPlayer.HasData("ac-healthchange") || (int)iPlayer.GetData("ac-healthchange") < 5) iPlayer.SetData("ac-healthchange", 5);
            if (!iPlayer.HasData("serverArmorChanged") || (int)iPlayer.GetData("serverArmorChanged") < 5) iPlayer.SetData("serverArmorChanged", 5);
            if (!iPlayer.HasData("ignoreGodmode") || (int)iPlayer.GetData("ignoreGodmode") < 5) iPlayer.SetData("ignoreGodmode", 5);
            if (!iPlayer.HasData("Teleport") || (int)iPlayer.GetData("Teleport") < 5) iPlayer.SetData("Teleport", 5);
            if (!iPlayer.HasData("serverDimensionChange") || (int)iPlayer.GetData("serverDimensionChange") < 5) iPlayer.SetData("serverDimensionChange", 5);
            if (!iPlayer.HasData("ac-ignorews") || (int)iPlayer.GetData("ac-ignorews") < 4) iPlayer.SetData("ac-ignorews", 4);
        }
    }
}
