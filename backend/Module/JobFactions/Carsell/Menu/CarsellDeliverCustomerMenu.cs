﻿using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Handler;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.JobFactions.Carsell;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.NSA.Observation;
using VMP_CNR.Module.PlayerName;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Teams.Shelter;
using VMP_CNR.Module.Telefon.App;
using VMP_CNR.Module.Vehicles.Data;
using VMP_CNR.Module.Vehicles.RegistrationOffice;

namespace VMP_CNR.Module.Carsell.Menu
{
    public class CarsellDeliverCustomerMenuBuilder : MenuBuilder
    {
        public CarsellDeliverCustomerMenuBuilder() : base(PlayerMenu.CarsellDeliverCustomerMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            var l_Menu = new Module.Menu.Menu(Menu, "Bestellung abschliessen");
            l_Menu.Add($"Schließen");

            // Show finished orders from team
            foreach(DeliveryOrder deliveryOrder in JobCarsellFactionModule.Instance.DeliverableOrderList.ToList().Where(o => o.TeamId == p_DbPlayer.TeamId))
            {
                VehicleData vehData = VehicleDataModule.Instance.GetDataById(deliveryOrder.VehicleDataId);
                if (vehData == null) continue;

                l_Menu.Add($"{PlayerNameModule.Instance.Get(deliveryOrder.PlayerId).Name} - {(vehData.mod_car_name.Length <= 0 ? vehData.Model : vehData.mod_car_name)}");
            }

            return l_Menu;
        }

        public override IMenuEventHandler GetEventHandler()
        {
            return new EventHandler();
        }

        private class EventHandler : IMenuEventHandler
        {
            public bool OnSelect(int index, DbPlayer iPlayer)
            {
                if(index == 0)
                {
                    MenuManager.DismissCurrent(iPlayer);
                    return true;
                }

                int idx = 1;

                // Show finished orders from team
                foreach (DeliveryOrder deliveryOrder in JobCarsellFactionModule.Instance.DeliverableOrderList.ToList().Where(o => o.TeamId == iPlayer.TeamId))
                {
                    if (idx == index)
                    {
                        VehicleData vehData = VehicleDataModule.Instance.GetDataById(deliveryOrder.VehicleDataId);
                        if (vehData == null) return false;

                        if(!iPlayer.Container.CanInventoryItemAdded(641))
                        {
                            iPlayer.SendNewNotification("Sie haben keinen Platz für einen Kaufvertrag!");
                            return false;
                        }

                        uint GarageId = JobCarsellFactionModule.GarageTeam1;

                        if (iPlayer.TeamId == (int)teams.TEAM_CARSELL2) GarageId = JobCarsellFactionModule.GarageTeam2;
                        if (iPlayer.TeamId == (int)teams.TEAM_CARSELL3) GarageId = JobCarsellFactionModule.GarageTeam3;

                        // INSERT VEHICLE
                        MySQLHandler.Execute($"INSERT INTO `vehicles` (`team_id`, `owner`, `color1`, `color2`, `tuning`, `inGarage`, `garage_id`, `model`, `vehiclehash`) " +
                            $"VALUES ('0', '{deliveryOrder.PlayerId}', '{deliveryOrder.Color1}', '{deliveryOrder.Color2}', '23:{deliveryOrder.Wheel}', '1', '{GarageId}', '{vehData.Id}', '{(vehData.mod_car_name.Length <= 0 ? vehData.Model : vehData.mod_car_name)}');");

                        string query = string.Format($"SELECT * FROM `vehicles` WHERE `owner` = '{deliveryOrder.PlayerId}' AND `model` LIKE '{vehData.Id}' ORDER BY id DESC LIMIT 1;");

                        uint id = 0;

                        using (var conn =
                            new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                        using (var cmd = conn.CreateCommand())
                        {
                            conn.Open();
                            cmd.CommandText = @query;
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        id = reader.GetUInt32("id");
                                        break;
                                    }
                                }
                            }
                            conn.Close();
                        }

                        // Give Kaufvertrag zu Verkäufer
                        var info = $"Besitzer: {PlayerNameModule.Instance.Get(deliveryOrder.PlayerId).Name} Fahrzeug: {(vehData.mod_car_name.Length <= 0 ? vehData.Model : vehData.mod_car_name)} ({id}) am {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}. VK von {iPlayer.GetName()}";

                        iPlayer.Container.AddItem(641, 1, new Dictionary<string, dynamic>() { { "Info", info }, { "vehicleId", id } });

                        iPlayer.SendNewNotification($"Sie haben {(vehData.mod_car_name.Length <= 0 ? vehData.Model : vehData.mod_car_name)} von {PlayerNameModule.Instance.Get(deliveryOrder.PlayerId).Name} für die Liefergarage freigegeben");

                        // Set Vehicle to Status 2
                        MySQLHandler.ExecuteAsync($"UPDATE `jobfaction_carsell_orders` SET status = '2' WHERE id = '{deliveryOrder.Id}';");

                        // Remove From List
                        JobCarsellFactionModule.Instance.DeliverableOrderList.Remove(deliveryOrder);
                        return true;
                    }
                    idx++;
                }

                MenuManager.DismissCurrent(iPlayer);
                return true;
            }
        }
    }
}
