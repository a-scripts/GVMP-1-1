using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Freiberuf;
using VMP_CNR.Module.Freiberuf.Mower;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.InteriorProp;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Storage;
using VMP_CNR.Module.Vehicles.Data;

namespace VMP_CNR
{
    public class StorageMenuBuilder : MenuBuilder
    {
        public StorageMenuBuilder() : base(PlayerMenu.StorageMenu)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            StorageRoom storageRoom = StorageRoomModule.Instance.GetClosest(iPlayer);
            if (storageRoom != null)
            {
                var menu = new Menu(Menu, $"Lagerraum ({storageRoom.Id})");
                if (storageRoom.IsBuyable())
                {
                    menu.Add("Lagerraum kaufen $" + storageRoom.Price);
                }
                else
                {
                    menu.Add("Lagerraum betreten");
                    menu.Add("Lagerraum ausbauen");
                    menu.Add("als Hauptlager setzen");
                    if (!storageRoom.CocainLabor) menu.Add("Kokainlabor ausbauen");
                }
                menu.Add(MSG.General.Close());
                return menu;
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
                StorageRoom storageRoom = StorageRoomModule.Instance.GetClosest(iPlayer);
                if(storageRoom != null)
                {
                    if(index == 0)
                    {
                        //Kaufen
                        if(storageRoom.IsBuyable())
                        {
                            if(iPlayer.GetStoragesOwned().Count >= StorageModule.Instance.LimitPlayerStorages)
                            {
                                iPlayer.SendNewNotification($"Sie haben die maximale Anzahl an Lager gekauft ({StorageModule.Instance.LimitPlayerStorages})!");
                                return true;
                            }
                            if(iPlayer.TakeBankMoney(storageRoom.Price))
                            {
                                storageRoom.SetOwnerTo(iPlayer);
                                iPlayer.SendNewNotification("Lager für $" + storageRoom.Price + " gekauft!");
                                return true;
                            }
                        }
                        else // betreten
                        {
                            if (!storageRoom.Locked)
                            {
                                // Player Into StorageRoom 
                                iPlayer.SetData("storageRoomId", storageRoom.Id);
                                iPlayer.Player.SetPosition(StorageModule.Instance.InteriorPosition);
                                iPlayer.Player.SetRotation(StorageModule.Instance.InteriorHeading);
                                iPlayer.SetDimension(storageRoom.Id);

                                if(storageRoom.CocainLabor)
                                {
                                    InteriorPropModule.Instance.LoadInteriorForPlayer(iPlayer, InteriorPropListsType.Kokainlabor);
                                }
                                else
                                {
                                    InteriorPropModule.Instance.LoadInteriorForPlayer(iPlayer, InteriorPropListsType.Lagerraum);
                                }
                                return true;
                            }
                            else
                            {
                                iPlayer.SendNewNotification("Lager ist abgeschlossen!", title: "Lager", notificationType: PlayerNotification.NotificationType.ERROR);
                                return true;
                            }
                        }
                    }
                    else if(index == 1)
                    {
                        if (iPlayer.Id != storageRoom.OwnerId) return true;
                        storageRoom.Upgrade(iPlayer);
                        return true;
                    }
                    else if (index == 2)
                    {
                        if (iPlayer.Id != storageRoom.OwnerId) return true;
                        storageRoom.SetMainFlagg(iPlayer);
                        return true;
                    }
                    else if (index == 3)
                    {
                        if (iPlayer.Id != storageRoom.OwnerId) return true;
                        if (storageRoom.CocainLabor) return true;
                        else
                            storageRoom.UpgradeCocain(iPlayer);
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}