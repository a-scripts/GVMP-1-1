﻿using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Items.Scripts.Presents;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles.Data;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static bool PresentKFZBrief(DbPlayer iPlayer, ItemModel ItemData)
        {
            string itemScript = ItemData.Script;

            if (!uint.TryParse(itemScript.Split('_')[1], out uint VehicleModelId))
            {
                return false;
            }
            VehicleData vehModel = VehicleDataModule.Instance.GetDataById(VehicleModelId);

            if (vehModel == null) return false;

            var query = $"INSERT INTO `vehicles` (`owner`, `garage_id`, `inGarage`, `plate`, `model`, `vehiclehash`) VALUES ('{iPlayer.Id}', '1', '1', '', '{vehModel.Id}', '{vehModel.Model}');";

            MySQLHandler.ExecuteAsync(query);

            iPlayer.SendNewNotification("Du hast " + ItemData.Name + " eingelöst und ein " + vehModel.Model + " erhalten!");

            Logging.Logger.AddToItemPresentLog(iPlayer.Id, ItemData.Id);

            // RefreshInventory
            return true;
        }

        public static bool PresentMoney(DbPlayer iPlayer, ItemModel ItemData)
        {
            string itemScript = ItemData.Script;

            if (!Int32.TryParse(itemScript.Split('_')[1], out int Amount))
            {
                return false;
            }

            iPlayer.GiveMoney(Amount);
            iPlayer.SendNewNotification("Du hast " + ItemData.Name + " eingelöst und $" + Amount + " erhalten!");

            // RefreshInventory
            return true;
        }
    }
}
