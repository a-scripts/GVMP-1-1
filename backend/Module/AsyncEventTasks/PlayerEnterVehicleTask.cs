using GTANetworkAPI;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMP_CNR.Handler;
using VMP_CNR.Module.Einreiseamt;
using VMP_CNR.Module.Events.Halloween;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.AsyncEventTasks
{
    public static partial class AsyncEventTasks
    {
        public static async Task PlayerEnterVehicleTask(Player player, Vehicle vehicle, sbyte seat)
        {
            await Task.Delay(2000);
            //Todo: maybe save vehicle and player position here
            DbPlayer iPlayer = player.GetPlayer();

            if (iPlayer == null || !iPlayer.IsValid()|| vehicle == null)
            {
                return;
            }

            iPlayer.SetData("Teleport", 3);

            if (iPlayer.Dimension[0] == 0)
            {
                iPlayer.MetaData.Dimension = iPlayer.Player.Dimension;
                iPlayer.MetaData.Heading = iPlayer.Player.Heading;
                iPlayer.MetaData.Position = iPlayer.Player.Position;
            }

            if((iPlayer.hasPerso[0] == 0 || iPlayer.Level < 3) && (vehicle.Model == (uint)VehicleHash.Hydra || vehicle.Model == (uint)VehicleHash.Lazer || vehicle.Model == (uint)VehicleHash.Rhino ||
                vehicle.Model == (uint)VehicleHash.Hunter || vehicle.Model == (uint)VehicleHash.Savage || vehicle.Model == (uint)VehicleHash.Buzzard))
            {
                Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"DRINGENDER-Anticheat-Verdacht: {iPlayer.Player.Name} (ARMY VEHICLE ENTERED)");
                Logging.Logger.LogToAcDetections(iPlayer.Id, Logging.ACTypes.VehicleControlAbuse, $"ARMY VEHICLE ENTER");

                Anticheat.AntiCheatModule.Instance.ACBanPlayer(iPlayer, "Army Vehicle Enter");
                return;
            }

            Modules.Instance.OnPlayerEnterVehicle(iPlayer, vehicle, seat);

            if (!vehicle.HasData("serverhash") || (string)vehicle.GetData<string>("serverhash") != "1312asdbncawssd1ccbSh1")
            {
                Players.Players.Instance.SendMessageToAuthorizedUsers("anticheat", $"ANTI CARHACK " + player.Name);
                vehicle.Delete();
                return;
            }

            if (iPlayer.Player == null)
                return;

            //ac stuff
            if (iPlayer.Player.Vehicle == null)
                return;

            iPlayer.SetData("ac_lastPos", iPlayer.Player.Vehicle.Position);

            if (iPlayer.NeuEingereist())
            { 
                Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"DRINGENDER-Anticheat-Verdacht: {iPlayer.Player.Name} (ohne Einreiseamt - Fahrzeug betreten)");
                Logging.Logger.LogToAcDetections(iPlayer.Id, Logging.ACTypes.EinreseVehicleEnter, $"");
                iPlayer.WarpOutOfVehicle(true);
            }

            SxVehicle sxVeh = vehicle.GetVehicle();
            if (sxVeh == null || !sxVeh.IsValid())
            {
                return;
            }

            if (sxVeh != null && sxVeh.IsValid() && seat == -1 && sxVeh.Data != null && sxVeh.Data.MaxSpeed > 0)
            {
                iPlayer.Player.TriggerEvent("setNormalSpeed", sxVeh.entity, sxVeh.Data.MaxSpeed);
            }

            // Respawnstate
            sxVeh.respawnInteractionState = true;

            if (sxVeh.jobid > 0)
            {
                if (player.VehicleSeat == -1 && sxVeh.jobid != iPlayer.job[0] && sxVeh.jobid != 99 &&
                    sxVeh.jobid != 999 && sxVeh.jobid != -1)
                {
                    if (sxVeh.jobid == 999 && (iPlayer.RankId == 0))
                    {
                        iPlayer.WarpOutOfVehicle();
                    }
                }
            }

            VehicleHandler.Instance.AddPlayerToVehicleOccupants(sxVeh, player.GetPlayer(), seat);

            NAPI.Task.Run(() =>
            {
                float newVehicleHealth = NAPI.Vehicle.GetVehicleEngineHealth(vehicle) + NAPI.Vehicle.GetVehicleBodyHealth(vehicle);
                player.TriggerEvent("initialVehicleData", sxVeh.fuel.ToString().Replace(",", "."), sxVeh.Data.Fuel.ToString().Replace(",", "."), newVehicleHealth.ToString().Replace(",", "."),
                VehicleHandler.MaxVehicleHealth.ToString().Replace(",", "."), sxVeh.entity.MaxSpeed.ToString().Replace(",", "."), sxVeh.entity.Locked ? "true" : "false", string.Format("{0:0.00}", sxVeh.Distance).Replace(",", "."), sxVeh.entity.EngineStatus ? "true" : "false");
            });

            await Task.Delay(1000);// Workaround for locked vehs

            // Resync Entity Lock & Engine Status
            if (sxVeh.SyncExtension != null)
            {
                NAPI.Task.Run(() =>
                {
                    NAPI.Vehicle.SetVehicleEngineStatus(sxVeh.entity, sxVeh.SyncExtension.EngineOn);
                    NAPI.Vehicle.SetVehicleLocked(sxVeh.entity, sxVeh.SyncExtension.Locked);
                    if (seat == -1)
                    {
                        iPlayer.Player.TriggerEvent("setPlayerVehicleMultiplier", sxVeh.DynamicMotorMultiplier);
                        sxVeh.LastDriver = iPlayer.GetName();
                    }


                    if (sxVeh != null && sxVeh.IsValid() && seat == -1 && sxVeh.Data != null && sxVeh.Data.MaxSpeed > 0)
                    {
                        iPlayer.Player.TriggerEvent("setNormalSpeed", sxVeh.entity, sxVeh.Data.MaxSpeed);
                    }
                });
            }

            if (sxVeh.entity.Locked || sxVeh.SyncExtension.Locked || iPlayer.IsTied || iPlayer.IsCuffed)
            {
                if (iPlayer.HasData("vehicleData"))
                {
                    iPlayer.WarpOutOfVehicle();
                }
            }

            if (sxVeh.entity.Locked || sxVeh.SyncExtension.Locked)
            {
                if (iPlayer.HasData("vehicleData"))
                {
                    iPlayer.WarpOutOfVehicle();
                }
            }
        }
    }
}
