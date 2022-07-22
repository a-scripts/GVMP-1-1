﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Handler;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.JobFactions.Carsell;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.NSA.Observation;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Teams.Shelter;
using VMP_CNR.Module.Telefon.App;
using VMP_CNR.Module.Vehicles.Data;

namespace VMP_CNR.Module.Carsell.Menu
{
    public class CarsellBuycarSubMenuBuilder : MenuBuilder
    {
        public CarsellBuycarSubMenuBuilder() : base(PlayerMenu.CarsellBuySubMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            if (!p_DbPlayer.HasData("carsellCat")) return null;

            var l_Menu = new Module.Menu.Menu(Menu, "Fahrzeug bestellen");
            l_Menu.Add($"Schließen");

            foreach (VehicleData vehData in VehicleDataModule.Instance.data.Values.ToList().Where(vd => vd.IsShopVehicle && vd.CarsellCategory.Id == p_DbPlayer.GetData("carsellCat")))
            {
                l_Menu.Add($"{(vehData.mod_car_name.Length <=0 ? vehData.Model : vehData.mod_car_name)}");
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
                foreach (VehicleData vehData in VehicleDataModule.Instance.data.Values.ToList().Where(vd => vd.IsShopVehicle && vd.CarsellCategory.Id == iPlayer.GetData("carsellCat")))
                {
                    if(idx == index)
                    {
                        // Fahrzeug zum Bestellen

                        if(!JobCarsellFactionModule.Instance.CanAddVehicle(iPlayer.TeamId))
                        {
                            iPlayer.SendNewNotification("Sie haben bereits die maximale Anzahl an Vorführfahrzeugen erreicht!");
                            return true;
                        }

                        if (vehData.CarsellCategory.Limit != 0)
                        {
                            if (JobCarsellFactionModule.Instance.GetCategoryAmount(vehData.CarsellCategory.Id, iPlayer.TeamId) >= vehData.CarsellCategory.Limit)
                            {
                                iPlayer.SendNewNotification($"Sie können nur {vehData.CarsellCategory.Limit} dieses Fahrzeugtypes besitzen!");
                                return true;
                            }
                        }


                        TeamShelter teamShelter = TeamShelterModule.Instance.GetByTeam(iPlayer.TeamId);
                        if (teamShelter == null) return false;
                        if (teamShelter.Money < JobCarsellFactionModule.FVehicleBuyPrice)
                        {
                            iPlayer.SendNewNotification("Eine Bestellung kostet 10.000$ (Fraktionsbank)!");
                            return true;
                        }

                        // Remove Money
                        teamShelter.TakeMoney(JobCarsellFactionModule.FVehicleBuyPrice);

                        // Insert Into Orders
                        MySQLHandler.ExecuteAsync($"INSERT INTO `jobfaction_carsell_fvorder` (`team_id`, `player_id`, `vehicle_data_id`) VALUES ('{iPlayer.TeamId}', '{iPlayer.Id}', '{vehData.Id}');");
                        iPlayer.SendNewNotification($"Fahrzeug {(vehData.mod_car_name.Length <= 0 ? vehData.Model : vehData.mod_car_name)} wurde bestellt! (Nächste Sonnenwende)");
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
