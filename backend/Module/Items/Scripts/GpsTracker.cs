using GTANetworkMethods;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static bool GpsTracker(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (!iPlayer.Player.IsInVehicle) return false;
            {
                if (iPlayer.job[0] != (int) jobs.JOB_MECH)
                {
                    iPlayer.SendNewNotification( MSG.Error.NoPermissions());
                    return false;
                }
                /*
                if (iPlayer.jobskill[0] < 2500)
                {
                        iPlayer.SendNewNotification(
                        
                        "Ihnen fehlt noch die notwendige Erfahrung! (2500)");
                        return false;
                }
                */
                var vehicle = iPlayer.Player.Vehicle.GetVehicle();
                if (vehicle.databaseId == 0) return false;
                if (!vehicle.GpsTracker)
                {
                    //Vehicle has no gps tracker
                    var table = vehicle.IsTeamVehicle() ? "fvehicles" : "vehicles";
                    MySQLHandler.ExecuteAsync($"UPDATE {table} SET gps_tracker = 1 WHERE id = {vehicle.databaseId}");
                    vehicle.GpsTracker = true;
                    iPlayer.SendNewNotification("Der GPS-Tracker wurde eingebaut.");
                    iPlayer.JobSkillsIncrease(2);
                }
                else
                {
                    //Vehicle already has gps tracker
                    iPlayer.SendNewNotification("Dieses Fahrzeug ist bereits mit einem GpsTracker ausgestattet.");
                    return false;
                }
                // RefreshInventory
                return true;
            }
        }
    }
}