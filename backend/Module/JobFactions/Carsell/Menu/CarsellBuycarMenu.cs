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
    public class CarsellBuycarMenuBuilder : MenuBuilder
    {
        public CarsellBuycarMenuBuilder() : base(PlayerMenu.CarsellBuyMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            var l_Menu = new Module.Menu.Menu(Menu, "Fahrzeug bestellen");
            l_Menu.Add($"Schließen");

            foreach(VehicleCarsellCategory vehicleCarsellCategory in VehicleCarsellCategoryModule.Instance.GetAll().Values)
            {
                l_Menu.Add($"{vehicleCarsellCategory.Name}");
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

                foreach (VehicleCarsellCategory vehicleCarsellCategory in VehicleCarsellCategoryModule.Instance.GetAll().Values)
                {
                    if(idx == index)
                    {
                        iPlayer.SetData("carsellCat", vehicleCarsellCategory.Id);
                        Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.CarsellBuySubMenu, iPlayer).Show(iPlayer);
                        return false;
                    }
                    idx++;
                }

                MenuManager.DismissCurrent(iPlayer);
                return true;
            }
        }
    }
}
