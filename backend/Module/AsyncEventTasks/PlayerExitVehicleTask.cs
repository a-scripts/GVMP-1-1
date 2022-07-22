using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Handler;
using VMP_CNR.Module.Armory;
using VMP_CNR.Module.Banks;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Vehicles;
using static VMP_CNR.Module.Sync.SyncThread;

namespace VMP_CNR.Module.AsyncEventTasks
{
    public static partial class AsyncEventTasks
    {
        public static void PlayerExitVehicleTask(Player player, Vehicle handle)
        {
            
            var vehicle = NAPI.Entity.GetEntityFromHandle<Vehicle>(handle);
            //Todo: maybe save vehicle and player position here
            DbPlayer iPlayer = player.GetPlayer();

            if (iPlayer == null || !iPlayer.IsValid()) return;
            
            // AC
            iPlayer.SetData("Teleport", 2);

            if (vehicle != null)
            {
                //ac stuff
                iPlayer.SetData("ac_lastPos", vehicle.Position);
            }

            if (iPlayer.Dimension[0] == 0)
            {
                iPlayer.MetaData.Dimension = iPlayer.Player.Dimension;
                iPlayer.MetaData.Heading = iPlayer.Player.Heading;
                iPlayer.MetaData.Position = iPlayer.Player.Position;
            }

            if (iPlayer.PlayingAnimation)
                iPlayer.PlayingAnimation = false;
            
                
            if (iPlayer.HasData("paintCar"))
            {
                if (vehicle.HasData("color1") && vehicle.HasData("color2"))
                {
                    int color1 = vehicle.GetData<int>("color1");
                    int color2 = vehicle.GetData<int>("color2");
                    vehicle.PrimaryColor = color1;
                    vehicle.SecondaryColor = color2;
                    vehicle.ResetData("color1");
                    vehicle.ResetData("color2");
                    iPlayer.ResetData("p_color1");
                    iPlayer.ResetData("p_color2");
                }

                iPlayer.ResetData("paintCar");
            }

            if (vehicle != null)
            {
                SxVehicle sxVeh = vehicle.GetVehicle();
                if (sxVeh != null)
                {
                    // Respawnstate
                    sxVeh.respawnInteractionState = true;
                    sxVeh.DynamicMotorMultiplier = sxVeh.Data.Multiplier;

                    if (iPlayer != null && iPlayer.IsValid())
                    {
                        if (iPlayer.HasData("neonCar"))
                        {
                            if (sxVeh.neon != "")
                            {
                                sxVeh.LoadNeon();
                                iPlayer.ResetData("neonCar");
                            }
                        }

                        // ResetMods
                        if (iPlayer.HasData("hornCar")) iPlayer.ResetData("hornCar");
                        if(iPlayer.HasData("perlCar")) iPlayer.ResetData("perlCar");

                        // ResetMods
                        if (iPlayer.HasData("tuneIndex")) iPlayer.ResetData("tuneIndex");
                        if (iPlayer.HasData("tuneSlot")) iPlayer.ResetData("tuneSlot");
                        if (iPlayer.HasData("tuneVeh")) iPlayer.ResetData("tuneVeh");
                    }


                    if (sxVeh.Occupants.ContainsValue(iPlayer))
                    {
                        sxVeh.Occupants.Remove(sxVeh.Occupants.First(x => x.Value == iPlayer).Key);
                    }
                }
            }
            
        }
    }
}
