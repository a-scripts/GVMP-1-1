using GTANetworkMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Handler;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.Business.Raffinery;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Tattoo;

namespace VMP_CNR.Module.Business.Raffinery
{
    public class RaffineryMenuBuilder : MenuBuilder
    {
        public RaffineryMenuBuilder() : base(PlayerMenu.RaffineryMenu)
        {
        }

        public override Menu.Menu Build(DbPlayer iPlayer)
        {
            if (!iPlayer.TryData("raffineryId", out uint raffineryId)) return null;
            var raffinery = RaffineryModule.Instance.Get(raffineryId);
            if (raffinery == null) return null;
            
            var menu = new Menu.Menu(Menu, "Oelfoerderpumpe");

            menu.Add($"Schließen");

            if (raffinery.IsOwnedByBusines())
            {
                try
                {
                    Console.WriteLine($"---RAFFINERY {raffinery.Id}---");
                    Console.WriteLine($"RaffineryBusiness: {raffinery.GetOwnedBusiness().Id}");
                    Console.WriteLine($"Name: {iPlayer.GetName()}");
                    Console.WriteLine($"BusinessId: {iPlayer.ActiveBusiness.Id}");
                    Console.WriteLine($"IsMemberOfABusiness: {iPlayer.IsMemberOfBusiness()}");
                    Console.WriteLine($"Rights?: {iPlayer.GetActiveBusinessMember().Raffinery}");
                }
                catch (Exception ex)
                {
                    Logger.Crash(ex);
                }

                if (raffinery.GetOwnedBusiness() == iPlayer.ActiveBusiness && iPlayer.IsMemberOfBusiness() && iPlayer.GetActiveBusinessMember().Raffinery) // Member of business and has rights
                {
                    SxVehicle sxVehicle = VehicleHandler.Instance.GetClosestVehicle(iPlayer.Player.Position, 12.0f);
                    if(sxVehicle != null)
                    {
                        menu.Add($"{sxVehicle.GetName()} ({sxVehicle.databaseId}) beladen");
                    }
                }
            }
            else if (iPlayer.IsMemberOfBusiness() && iPlayer.GetActiveBusinessMember().Owner && iPlayer.ActiveBusiness.BusinessBranch.RaffinerieId == 0 && iPlayer.ActiveBusiness.BusinessBranch.CanBuyBranch())
            {
                menu.Add($"Oelfoerderpumpe kaufen {raffinery.BuyPrice}$");
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
                if (index == 1)
                {
                    if (!iPlayer.TryData("raffineryId", out uint raffineryId)) return false;
                    var raffinery = RaffineryModule.Instance.Get(raffineryId);
                    if (raffinery == null) return false;

                    if (raffinery.IsOwnedByBusines())
                    {
                        if (raffinery.GetOwnedBusiness() == iPlayer.ActiveBusiness && iPlayer.IsMemberOfBusiness() && iPlayer.GetActiveBusinessMember().Raffinery) // Member of business and has rights     
                        {
                            SxVehicle sxVehicle = VehicleHandler.Instance.GetClosestVehicle(iPlayer.Player.Position, 6.0f);
                            if (sxVehicle != null)
                            {
                                // Fahrzeug beladen
                                if(sxVehicle.SyncExtension.Locked)
                                {
                                    iPlayer.SendNewNotification("Fahrzeug muss aufgeschlossen sein!");
                                    return true;
                                }
                                else
                                {
                                    ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = sxVehicle.GetName() + " beladen" , Callback = "LoadIntoVehicle", Message = "Geben Sie die Lademenge an" });

                                }
                            }
                        }
                    }
                    else if (iPlayer.IsMemberOfBusiness() && iPlayer.GetActiveBusinessMember().Owner && iPlayer.ActiveBusiness.BusinessBranch.RaffinerieId == 0 && iPlayer.ActiveBusiness.BusinessBranch.CanBuyBranch())
                    {
                        // Kaufen
                        if (iPlayer.ActiveBusiness.TakeMoney(raffinery.BuyPrice))
                        {
                            iPlayer.ActiveBusiness.BusinessBranch.SetRaffinerie(raffinery.Id);
                            iPlayer.SendNewNotification($"Oelfoerderpumpe erfolgreich fuer ${raffinery.BuyPrice} erworben!");
                            raffinery.OwnerBusiness = iPlayer.ActiveBusiness;
                        }
                        else {
                            iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(raffinery.BuyPrice));
                        }
                    }
                    return true;
                }
                MenuManager.DismissCurrent(iPlayer);
                return false;
            }
        }
    }
}