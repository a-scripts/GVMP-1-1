using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Tattoo;

namespace VMP_CNR
{
    public class TattooBuyMenuBuilder : MenuBuilder
    {
        public TattooBuyMenuBuilder() : base(PlayerMenu.TattooBuyMenu)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            try { 
            if (!iPlayer.TryData("tattooShopId", out uint tattooShopId)) return null;
            var tattooShop = TattooShopModule.Instance.Get(tattooShopId);
            if (tattooShop == null || tattooShop.BusinessId != 0) return null;

            if(!iPlayer.GetActiveBusinessMember().Owner)
            {
                iPlayer.SendNewNotification("Sie muessen ein Business besitzen!");
            }

            var menu = new Menu(Menu, "TattooShop");

            menu.Add($"Schließen");
            menu.Add($"Shop erwerben {tattooShop.Price}$");

            menu.Add(MSG.General.Close());
                return menu;

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
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
                    return false;
                }
                if(index == 1)
                {
                    // Buy
                    if (!iPlayer.TryData("tattooShopId", out uint tattooShopId)) return false;
                    var tattooShop = TattooShopModule.Instance.Get(tattooShopId);
                    if (tattooShop == null || tattooShop.BusinessId != 0) return false;

                    if (!iPlayer.GetActiveBusinessMember().Owner)
                    {
                        iPlayer.SendNewNotification("Sie muessen ein Business besitzen!");
                    }

                    if (!iPlayer.TakeMoney(tattooShop.Price))
                    {
                        iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(tattooShop.Price));
                        return false;
                    }

                    tattooShop.SetBusiness((int)iPlayer.GetActiveBusinessMember().BusinessId);
                    iPlayer.SendNewNotification("Tattoshop erworben!");
                    return true;
                }
                MenuManager.DismissCurrent(iPlayer);
                return false;
            }
        }
    }
}