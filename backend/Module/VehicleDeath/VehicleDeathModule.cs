using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Handler;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Meth;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.VehicleDeath
{

    public class VehicleDeathModule : Module<VehicleDeathModule>
    {
        private string VehicleBackupDB = "container_vehicle_backups";

        public int GetVehiclesRepairPrice(SxVehicle sxVehicle)
        {
            int price = sxVehicle.Data.Price / 1000;
            if (price <= 500) price = 500; // min
            if (price >= 50000) price = 50000; // max
            return price;
        }

        public void CreateVehicleBackupInventory(SxVehicle sxVehicle)
        {
            if (sxVehicle == null || (!sxVehicle.IsPlayerVehicle() && !sxVehicle.IsTeamVehicle()) || sxVehicle.databaseId == 0) return;

            string saveQuery = GetContainerInsertionQuery(sxVehicle.Container);
            if (saveQuery != "")
                MySQLHandler.ExecuteAsync(saveQuery, Sync.MySqlSyncThread.MysqlQueueTypes.Inventory);

            Logging.Logger.Debug(saveQuery);

            if (sxVehicle.Container != null)
                sxVehicle.Container.ClearInventory(); 

            if (sxVehicle.Container2 != null)
                sxVehicle.Container2.ClearInventory();
        }

        public void RemoveOccupantsOnDeath(SxVehicle xVeh)
        {
            try
            {
                if (xVeh.Visitors.Count > 0)
                {
                    foreach (DbPlayer iPlayer in xVeh.Visitors)
                    {
                        if (iPlayer.DimensionType[0] == DimensionType.Camper && iPlayer.Player.Dimension != 0)
                        {
                            try
                            {
                                if (xVeh.Visitors.Contains(iPlayer)) xVeh.Visitors.Remove(iPlayer);
                                iPlayer.Player.SetPosition(new Vector3(xVeh.entity.Position.X + 3.0f,
                                    xVeh.entity.Position.Y,
                                    xVeh.entity.Position.Z));
                            }
                            catch (Exception e)
                            {
                                Logging.Logger.Crash(e);
                            }
                            finally
                            {
                                // Reset Cooking on Exit
                                if (iPlayer.HasData("cooking"))
                                {
                                    iPlayer.ResetData("cooking");
                                }
                                if (MethModule.CookingPlayers.Contains(iPlayer)) MethModule.CookingPlayers.Remove(iPlayer);

                                iPlayer.DimensionType[0] = DimensionType.World;
                                iPlayer.Dimension[0] = 0;
                                iPlayer.SetDimension(0);
                                iPlayer.Player.SetPosition((Vector3)iPlayer.GetData("CamperEnterPos"));
                                iPlayer.ResetData("CamperEnterPos");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Logger.Crash(e);
            }
        }

        private string GetContainerInsertionQuery(Container container)
        {
            string slotsValuesQuery = "";

            for (int i = 0; i < container.MaxSlots; i++)
            {
                slotsValuesQuery += $"'{NAPI.Util.ToJson(container.ConvertToSaving()[i])}',";
            }

            return $"INSERT INTO `{VehicleBackupDB}` VALUES ('', '{container.Id}', '', '{(int)container.Type}', '{container.MaxWeight}', '{container.MaxSlots}', {slotsValuesQuery.Substring(0, slotsValuesQuery.Length - 1)});";
        }
    }
}
