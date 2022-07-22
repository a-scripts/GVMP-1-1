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

namespace VMP_CNR.Module.Warehouse
{
    public class WarehouseMenuBuilder : MenuBuilder
    {
        public WarehouseMenuBuilder() : base(PlayerMenu.WarehouseMenu)
        {
        }

        public override Menu.Menu Build(DbPlayer iPlayer)
        {
            Warehouse warehouse = WarehouseModule.Instance.GetThis(iPlayer.Player.Position);
            if (warehouse == null) return null;

            var menu = new Menu.Menu(Menu, "Warenlager");

            menu.Add($"Schließen");
            menu.Add($"Waren Ankauf");
            menu.Add($"Waren verkauf");
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
                Warehouse warehouse = WarehouseModule.Instance.GetThis(iPlayer.Player.Position);
                if (warehouse == null) return false;
                if (index == 0)
                {
                    MenuManager.DismissCurrent(iPlayer);
                    return false;
                }
                else if(index == 1) // ankauf Menu
                {
                    MenuManager.Instance.Build(PlayerMenu.WarehouseBuyMenu, iPlayer).Show(iPlayer);
                    return false;
                }
                else // VK Menu
                {
                    MenuManager.Instance.Build(PlayerMenu.WarehouseSellMenu, iPlayer).Show(iPlayer);
                    return false;
                }
            }
        }
    }
}