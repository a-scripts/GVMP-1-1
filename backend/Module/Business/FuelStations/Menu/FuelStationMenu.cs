using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Tattoo;

namespace VMP_CNR.Module.Business.FuelStations
{
    public class FuelStationMenuBuilder : MenuBuilder
    {
        public FuelStationMenuBuilder() : base(PlayerMenu.FuelStationMenu)
        {
        }

        public override Menu.Menu Build(DbPlayer iPlayer)
        {
            if (!iPlayer.TryData("fuelstationId", out uint fuelStationId)) return null;
            var fuelstation = FuelStationModule.Instance.Get(fuelStationId);
            if (fuelstation == null) return null;
            
            var menu = new Menu.Menu(Menu, fuelstation.Name);

            menu.Add($"Schließen");

            if(fuelstation.IsOwnedByBusines())
            {
                if(fuelstation.GetOwnedBusiness() == iPlayer.ActiveBusiness && iPlayer.IsMemberOfBusiness() && iPlayer.GetActiveBusinessMember().Fuelstation) // Member of business and has rights
                {
                    // Preis einstellen...
                    menu.Add($"Literpreis einstellen");
                    menu.Add($"Namen einstellen");
                }
            }
            else if(iPlayer.IsMemberOfBusiness() && iPlayer.GetActiveBusinessMember().Owner && iPlayer.ActiveBusiness.BusinessBranch.FuelstationId == 0 && iPlayer.ActiveBusiness.BusinessBranch.CanBuyBranch())
            {
                menu.Add($"Tankstelle kaufen {fuelstation.BuyPrice}$");
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
                else
                {
                    if (!iPlayer.TryData("fuelstationId", out uint fuelStationId)) return false;
                    var fuelstation = FuelStationModule.Instance.Get(fuelStationId);
                    if (fuelstation == null) return false;
                    
                    if (fuelstation.IsOwnedByBusines())
                    {
                        if (fuelstation.GetOwnedBusiness() == iPlayer.ActiveBusiness && iPlayer.GetActiveBusinessMember().Fuelstation) // Member of business and has rights
                        {
                            MenuManager.DismissCurrent(iPlayer);

                            if (index == 1)
                            {
                                // Preis einstellen...
                                ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = "Preis pro Liter", Callback = "SetFuelPrice", Message = "Stelle den Preis pro Liter ein" });
                                return true;
                            }
                            else if(index == 2)
                            {
                                // Name
                                ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = "Tankstellen Name", Callback = "SetFuelName", Message = "Gib einen neuen Namen ein (max 32 Stellen)." });
                                return true;
                            }
                        }
                    }
                    else if (iPlayer.IsMemberOfBusiness() && iPlayer.GetActiveBusinessMember().Owner && iPlayer.ActiveBusiness.BusinessBranch.FuelstationId == 0 && iPlayer.ActiveBusiness.BusinessBranch.CanBuyBranch())
                    {
                        // Kaufen
                        if(iPlayer.ActiveBusiness.TakeMoney(fuelstation.BuyPrice))
                        {
                            iPlayer.ActiveBusiness.BusinessBranch.SetFuelstation(fuelstation.Id);
                            iPlayer.SendNewNotification($"{fuelstation.Name} erfolgreich fuer ${fuelstation.BuyPrice} erworben!");
                            fuelstation.OwnerBusiness = iPlayer.ActiveBusiness;
                        }
                        else {
                            iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(fuelstation.BuyPrice));
                        }
                    }
                    return true;
                }
            }
        }
    }
}