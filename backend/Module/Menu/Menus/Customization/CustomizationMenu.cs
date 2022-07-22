using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Tattoo;

namespace VMP_CNR
{
    public class CustomizationMenuBuilder : MenuBuilder
    {
        public CustomizationMenuBuilder() : base(PlayerMenu.CustomizationMenu)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu(Menu, "Schönheitsklinik");

            menu.Add($"Schönheitschirugie");
            menu.Add($"Tattoos lasern");

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
                        iPlayer.StartCustomization();
                        MenuManager.DismissCurrent(iPlayer);
                        return true;
                    case 1:
                        MenuManager.Instance.Build(PlayerMenu.TattooLaseringMenu, iPlayer).Show(iPlayer);
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