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
using VMP_CNR.Module.Teams.AmmoPackageOrder;
using VMP_CNR.Module.Teams.Shelter;

namespace VMP_CNR.Module.Teams.AmmoArmory
{
    public class AmmoArmoriePriceMenuBuilder : MenuBuilder
    {
        public AmmoArmoriePriceMenuBuilder() : base(PlayerMenu.AmmoArmoriePriceMenu)
        {
        }

        public override Menu.Menu Build(DbPlayer iPlayer)
        {
            AmmoArmorie ammoArmorie = AmmoArmoryModule.Instance.GetByPosition(iPlayer.Player.Position);
            if (ammoArmorie == null || !iPlayer.Team.IsGangsters() || iPlayer.Team.Id != ammoArmorie.TeamId || iPlayer.TeamRank <= 10) return null;

            var menu = new Menu.Menu(Menu, "Munitionskammer");

            menu.Add($"Schließen");
            
            foreach (AmmoArmorieItem ammoArmorieItem in ammoArmorie.ArmorieItems)
            {
                menu.Add(ItemModelModule.Instance.Get(ammoArmorieItem.ItemId).Name + " $" + ammoArmorieItem.TeamPrice + " (P:" + ammoArmorieItem.GetRequiredPacketsForTeam(iPlayer.Team) + ")");
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
                    AmmoArmorie ammoArmorie = AmmoArmoryModule.Instance.GetByPosition(iPlayer.Player.Position);
                    if (ammoArmorie == null || !iPlayer.Team.IsGangsters() || iPlayer.Team.Id != ammoArmorie.TeamId) return false;

                    int idx = 1;
                    foreach (AmmoArmorieItem ammoArmorieItem in ammoArmorie.ArmorieItems)
                    {
                        if (idx == index)
                        {
                            iPlayer.SetData("configAmmoPrice", ammoArmorieItem.Id);
                            ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = "Preis anpassen", Callback = "ConfigAmmoArmoriePrice", Message = "Geben Sie die Kosten für " + ItemModelModule.Instance.Get(ammoArmorieItem.ItemId).Name + " an:"});
                            return true;
                        }
                        else idx++;
                    }
                    return true;
                }
            }
        }
    }
}