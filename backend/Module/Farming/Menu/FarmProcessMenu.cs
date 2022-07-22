﻿using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Handler;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.Business.Raffinery;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Farming;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Tattoo;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Business.Raffinery
{
    public class FarmProcessMenuBuilder : MenuBuilder
    {
        public FarmProcessMenuBuilder() : base(PlayerMenu.FarmProcessMenu)
        {
        }

        public override Menu.Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu.Menu(Menu, "Verarbeitung");

            menu.Add($"Schließen");

            List<SxVehicle> sxVehicles = VehicleHandler.Instance.GetClosestVehiclesPlayerCanControl(iPlayer, 20.0f);
            if(sxVehicles != null && sxVehicles.Count() > 0)
            {
                foreach(SxVehicle sxVehicle in sxVehicles)
                {
                    menu.Add($"{sxVehicle.GetName()} ({sxVehicle.databaseId}) verarbeiten");
                }
            }
            
            return menu;
        }

        public override IMenuEventHandler GetEventHandler()
        {
            return new EventHandler();
        }

        private class EventHandler : IMenuEventHandler
        {
            public bool OnSelect(int index, DbPlayer iPlayer)
            {
                if (index == 0)
                {
                    MenuManager.DismissCurrent(iPlayer);
                    return false;
                }

                var farmProcess = FarmProcessModule.Instance.GetByPosition(iPlayer.Player.Position);
                if (farmProcess == null) return false;

                if (iPlayer.Player.IsInVehicle) return false;

                List<SxVehicle> sxVehicles = VehicleHandler.Instance.GetClosestVehiclesPlayerCanControl(iPlayer, 20.0f);
                if (sxVehicles != null && sxVehicles.Count() > 0)
                {
                    int count = 1;
                    foreach (SxVehicle sxVehicle in sxVehicles)
                    {
                        if (index == count)
                        {
                            // Fahrzeug verarbeiten
                            if (sxVehicle == null || !sxVehicle.IsValid()) return false;
                            if (!sxVehicle.CanInteract) return false;

                            // Motor muss aus sein
                            if (sxVehicle.SyncExtension.EngineOn)
                            {
                                iPlayer.SendNewNotification("Motor muss ausgeschaltet sein!");
                                return false;
                            }
                            // zugeschlossen
                            if (!sxVehicle.SyncExtension.Locked)
                            {
                                iPlayer.SendNewNotification("Fahrzeug muss zugeschlossen sein!");
                                return false;
                            }
                            if (sxVehicle.HasData("Door_KRaum") && sxVehicle.GetData("Door_KRaum") == 1)
                            {
                                iPlayer.SendNewNotification("Der Kofferaum muss zugeschlossen sein!");
                                return false;
                            }
                            if (iPlayer.Player.VehicleSeat != -1)
                            {
                                iPlayer.SendNewNotification("Sie müssen Fahrer sein!");
                                return false;
                            }

                            FarmProcessModule.Instance.FarmProcessAction(farmProcess, iPlayer, sxVehicle.Container, sxVehicle);
                            MenuManager.DismissCurrent(iPlayer);
                            return true;
                        }
                        else count++;
                    }
                }

                MenuManager.DismissCurrent(iPlayer);
                return true;
            }
        }
    }
}