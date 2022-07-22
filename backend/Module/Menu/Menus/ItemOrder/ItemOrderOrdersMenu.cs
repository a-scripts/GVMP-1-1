using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR
{
    public class ItemOrderOrdersMenuBuilder : MenuBuilder
    {
        public ItemOrderOrdersMenuBuilder() : base(PlayerMenu.ItemOrderOrdersMenu)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            ItemOrderNpc itemOrderNpc = ItemOrderNpcModule.Instance.GetByPlayerPosition(iPlayer);
            if (itemOrderNpc == null) return null;
            
            var menu = new Menu(Menu, "Fertiggestellt");
            
            foreach(ItemOrder itemOrder in ItemOrderModule.Instance.GetPlayerFinishedListByNpc(iPlayer, itemOrderNpc))
            {
                menu.Add($"{itemOrder.ItemAmount} {itemOrder.Item.Name}");
            }

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

                ItemOrderNpc itemOrderNpc = ItemOrderNpcModule.Instance.GetByPlayerPosition(iPlayer);
                if (itemOrderNpc == null) return false;

                int idx = 0;
                foreach (ItemOrder itemOrder in ItemOrderModule.Instance.GetPlayerFinishedListByNpc(iPlayer, itemOrderNpc))
                {
                    if (idx == index)
                    {
                        // Entnehme Item
                        if (!iPlayer.Container.CanInventoryItemAdded(itemOrder.Item, itemOrder.ItemAmount))
                        {
                            iPlayer.SendNewNotification($"Sie koennen so viel nicht tragen!");
                            return false;
                        }

                        if(ItemOrderModule.Instance.DeleteOrder(itemOrder))
                        {
                            iPlayer.Container.AddItem(itemOrder.Item, itemOrder.ItemAmount);

                            Task.Run(async () =>
                            {
                                iPlayer.SetData("Itemorderflood", true);
                                Chats.sendProgressBar(iPlayer, 2000);
                                iPlayer.Player.TriggerEvent("freezePlayer", true);
                                await Task.Delay(2000);
                                iPlayer.Player.TriggerEvent("freezePlayer", false);
                                if (iPlayer.HasData("Itemorderflood"))
                                {
                                    iPlayer.ResetData("Itemorderflood");
                                }

                                iPlayer.SendNewNotification($"Sie haben {itemOrder.ItemAmount} {itemOrder.Item.Name} entnommen!");
                            });

                        }

                        return true;
                    }
                    idx++;
                }
                MenuManager.DismissCurrent(iPlayer);
                return false;
            }
        }
    }
}