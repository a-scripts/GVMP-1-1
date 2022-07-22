﻿using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR
{
    public class ItemOrderMenuBuilder : MenuBuilder
    {
        public ItemOrderMenuBuilder() : base(PlayerMenu.ItemOrderMenu)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu(Menu, "Verarbeitung");
            menu.Add("Waren verarbeiten");
            menu.Add("Waren entnehmen");
            menu.Add(MSG.General.Close());
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
                switch (index)
                {
                    case 0:
                        MenuManager.Instance.Build(PlayerMenu.ItemOrderItemsMenu, iPlayer).Show(iPlayer);
                        break;
                    case 1:
                        MenuManager.Instance.Build(PlayerMenu.ItemOrderOrdersMenu, iPlayer).Show(iPlayer);
                        break;
                    default:
                        MenuManager.DismissCurrent(iPlayer);
                        break;
                }

                return false;
            }
        }
    }
}