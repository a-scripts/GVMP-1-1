using GTANetworkAPI;
using System;
using System.Linq;
using System.Threading.Tasks;
using VMP_CNR.Handler;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Doors;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Laboratories;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.JumpPoints;
using VMP_CNR.Module.Players.PlayerAnimations;
using VMP_CNR.Module.Teams;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> BatterieRam(DbPlayer iPlayer, ItemModel ItemData)
        {
            // Only Cops
            if (iPlayer.TeamId != (int)teams.TEAM_FIB && iPlayer.TeamId != (int)teams.TEAM_SWAT && iPlayer.TeamId != (int)teams.TEAM_FIB) return false;

            // Check Door
            if (iPlayer.TryData("doorId", out uint doorId))
            {
                var door = DoorModule.Instance.Get(doorId);
                if (door != null)
                {
                    if (door.AdminUnbreakable) return false;
                    if (!door.Locked)
                    {
                        iPlayer.SendNewNotification("Tuer ist bereits aufgeschlossen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                        return false;
                    }
                    if (door.LastBreak.AddMinutes(5) > DateTime.Now) return false; // Bei einem Break, kann 5 min nicht interagiert werden

                    Chats.sendProgressBar(iPlayer, 30000);

                    iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "missheistdockssetup1ig_3@talk", "oh_hey_vin_dockworker");
                    iPlayer.Player.TriggerEvent("freezePlayer", true);
                    iPlayer.SetData("userCannotInterrupt", true);

                    await Task.Delay(30000);
                    iPlayer.ResetData("userCannotInterrupt");

                    if (iPlayer.IsCuffed || iPlayer.IsTied || iPlayer.isInjured()) return true;

                    iPlayer.Player.TriggerEvent("freezePlayer", false);

                    door.Break();

                    iPlayer.SendNewNotification("Tuer aufgebrochen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                    iPlayer.StopAnimation();
                    return true;
                }
            }

            if(iPlayer.TryData("houseId", out uint houseid))
            {
                var house = HouseModule.Instance[houseid];
                if (house != null)
                {
                    if(!house.Locked)
                    {
                        iPlayer.SendNewNotification("Eingang ist bereits aufgeschlossen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                        return false;
                    }
                    if (house.LastBreak.AddMinutes(10) > DateTime.Now) return false; // Bei einem Break, kann 10 min nicht interagiert werden

                    Chats.sendProgressBar(iPlayer, 30000);
                    
                    iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "missheistdockssetup1ig_3@talk", "oh_hey_vin_dockworker");
                    iPlayer.Player.TriggerEvent("freezePlayer", true);

                    await Task.Delay(30000);

                    iPlayer.Player.TriggerEvent("freezePlayer", false);
                    house.Locked = false;
                    house.LastBreak = DateTime.Now;

                    iPlayer.SendNewNotification("Eingang aufgebrochen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                    iPlayer.StopAnimation();
                    return true;
                }
            }

            // Check Jumppoint
            if (iPlayer.TryData("jumpPointId", out int jumpPointId))
            {
                var jumpPoint = JumpPointModule.Instance.Get(jumpPointId);
                if (jumpPoint != null)
                {
                    if (jumpPoint.AdminUnbreakable) return false;
                    if (!jumpPoint.Locked)
                    {
                        iPlayer.SendNewNotification("Eingang ist bereits aufgeschlossen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                        return false;
                    }
                    if (jumpPoint.LastBreak.AddMinutes(5) > DateTime.Now) return false; // Bei einem Break, kann 5 min nicht interagiert werden

                    int time = 30000;
                    Methlaboratory methlaboratory = MethlaboratoryModule.Instance.GetLaboratoryByJumppointId(jumpPointId);
                    Weaponlaboratory weaponlaboratory = WeaponlaboratoryModule.Instance.GetLaboratoryByJumppointId(jumpPointId);
                    Cannabislaboratory cannabislaboratory = CannabislaboratoryModule.Instance.GetLaboratoryByJumppointId(jumpPointId);

                    if (methlaboratory != null)
                    {
                        time = LaboratoryModule.TimeToBreakDoor;
                        TeamModule.Instance.Get(methlaboratory.TeamId).SendNotification("Das Sicherheitssystem des Methlabors meldet einen Alarm...", time: 30000);
                    }
                    if (weaponlaboratory != null)
                    {
                        time = LaboratoryModule.TimeToBreakDoor;
                        TeamModule.Instance.Get(weaponlaboratory.TeamId).SendNotification("Das Sicherheitssystem des Waffenlabors meldet einen Alarm...", time: 30000);
                    }
                    if (cannabislaboratory != null)
                    {
                        time = LaboratoryModule.TimeToBreakDoor;
                        TeamModule.Instance.Get(cannabislaboratory.TeamId).SendNotification("Das Sicherheitssystem des Cannabislabors meldet einen Alarm...", time: 30000);
                    }

                    Chats.sendProgressBar(iPlayer, time);


                    iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "missheistdockssetup1ig_3@talk", "oh_hey_vin_dockworker");
                    iPlayer.Player.TriggerEvent("freezePlayer", true);

                    await Task.Delay(time);

                    iPlayer.Player.TriggerEvent("freezePlayer", false);
                    jumpPoint.Locked = false;
                    jumpPoint.LastBreak = DateTime.Now;
                    jumpPoint.Destination.Locked = false;
                    jumpPoint.Destination.LastBreak = DateTime.Now;

                    iPlayer.SendNewNotification("Eingang aufgebrochen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                    iPlayer.StopAnimation();
                    return true;
                }
            }

            var l_Vehicle = VehicleHandler.Instance.GetClosestVehicle(iPlayer.Player.Position, 3.0f);
            if (l_Vehicle == null)
                return false;

            if (l_Vehicle.entity.Model != (uint)VehicleHash.Journey && l_Vehicle.entity.Model != (uint)VehicleHash.Camper)
                return false;

            if (l_Vehicle.SyncExtension.Locked)
            {
                Chats.sendProgressBar(iPlayer, 30000);

                Random rnd = new Random();

                if(rnd.Next(0, 100) <= 10) // 10%
                {
                    foreach (DbPlayer insidePlayer in l_Vehicle.Visitors.ToList())
                    {
                        if (insidePlayer == null || !insidePlayer.IsValid() || insidePlayer.Dimension[0] == 0 || insidePlayer.DimensionType[0] != DimensionType.Camper) continue;
                        insidePlayer.SendNewNotification($"Irgendetwas rappelt an der Tür...");
                    }
                }

                iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "missheistdockssetup1ig_3@talk", "oh_hey_vin_dockworker");
                iPlayer.Player.TriggerEvent("freezePlayer", true);

                await Task.Delay(30000);

                iPlayer.Player.TriggerEvent("freezePlayer", false);

                l_Vehicle.SyncExtension.SetLocked(false);

                iPlayer.SendNewNotification("Fahrzeug aufgebrochen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                iPlayer.StopAnimation();
                return true;

            }
            else
            {
                iPlayer.SendNewNotification("Fahrzeug ist offen!");
                return false;
            }
        }
    }
}