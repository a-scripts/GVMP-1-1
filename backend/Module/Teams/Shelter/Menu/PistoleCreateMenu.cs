using GTANetworkAPI;
using System.Data;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Teamfight;
using VMP_CNR.Module.Teamfight.Windows;

namespace VMP_CNR.Module.Teams.Shelter
{
    public class PistoleCreateMenuBuilder : MenuBuilder
    {
        public static uint PistolSetItemId = 1078;
        public static uint MuniPackItemId = 1;

        public PistoleCreateMenuBuilder() : base(PlayerMenu.PistoleCreateMenu)
        {
        }

        public override Menu.Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu.Menu(Menu, "Pistolen Herstellung", "Pistolen Herstellung");

            menu.Add($"Schließen");
            menu.Add($"Heavypistole ($5500)");
            menu.Add($"Pistole ($4000)");
            menu.Add($"Pistole 50 ($7000)");
            //menu.Add($"----Munition----");
            //menu.Add($"Heavypistol Munition");
            //menu.Add($"Pistole Munition");
            //menu.Add($"Pistole 50 Munition");


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
                else if (index == 1)
                {
                    uint weaponId = 1291;
                    int price = 5500;

                    if (iPlayer.Container.GetItemAmount(PistolSetItemId) < 1) return false;

                    if (!iPlayer.Container.CanInventoryItemAdded(weaponId))
                    {
                        iPlayer.SendNewNotification($"Nicht genug platz im Rucksack!");
                        return false;
                    }

                    if (!iPlayer.TakeBlackMoney(price))
                    {
                        iPlayer.SendNewNotification(MSG.Money.NotEnoughSWMoney(price));
                        return false;
                    }

                    iPlayer.Container.RemoveItem(PistolSetItemId);

                    iPlayer.Container.AddItem(weaponId, 1);

                    iPlayer.SendNewNotification($"Du hast eine Heavypistol für ${price} Schwarzgeld hergestellt!");
                    return true;
                }
                else if (index == 2)
                {
                    uint weaponId = 1292;
                    int price = 4000;

                    if (iPlayer.Container.GetItemAmount(PistolSetItemId) < 1) return false;

                    if (!iPlayer.Container.CanInventoryItemAdded(weaponId))
                    {
                        iPlayer.SendNewNotification($"Nicht genug platz im Rucksack!");
                        return false;
                    }

                    if (!iPlayer.TakeBlackMoney(price))
                    {
                        iPlayer.SendNewNotification(MSG.Money.NotEnoughSWMoney(price));
                        return false;
                    }

                    iPlayer.Container.RemoveItem(PistolSetItemId);

                    iPlayer.Container.AddItem(weaponId, 1);

                    iPlayer.SendNewNotification($"Du hast eine Pistol für ${price} Schwarzgeld hergestellt!");
                    return true;
                }
                else if (index == 3)
                {
                    uint weaponId = 1316;
                    int price = 7000;

                    if (iPlayer.Container.GetItemAmount(PistolSetItemId) < 1) return false;

                    if (!iPlayer.Container.CanInventoryItemAdded(weaponId))
                    {
                        iPlayer.SendNewNotification($"Nicht genug platz im Rucksack!");
                        return false;
                    }

                    if (!iPlayer.TakeBlackMoney(price))
                    {
                        iPlayer.SendNewNotification(MSG.Money.NotEnoughSWMoney(price));
                        return false;
                    }

                    iPlayer.Container.RemoveItem(PistolSetItemId);

                    iPlayer.Container.AddItem(weaponId, 1);

                    iPlayer.SendNewNotification($"Du hast eine Pistol für ${price} Schwarzgeld hergestellt!");
                    return true;
                }

                return false;
            }
        }
    }
}
