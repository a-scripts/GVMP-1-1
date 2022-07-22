﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Handler;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Players;

namespace VMP_CNR.Module.Business.Raffinery
{
    class RaffineryEvents : Script
    {
        [RemoteEvent]
        public void LoadIntoVehicle(Player player, string returnstring)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(async () =>
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null) return;

                int amount = 0;
                if (!Int32.TryParse(returnstring, out amount))
                {
                    dbPlayer.SendNewNotification("Diese Menge kann nicht geladen werden!");
                    return;
                }

                // check realistic value
                if (amount < 1)
                {
                    dbPlayer.SendNewNotification("Diese Menge kann nicht geladen werden!");
                    return;
                }

                if (!dbPlayer.TryData("raffineryId", out uint raffineryId)) return;
                var rafinnery = RaffineryModule.Instance.Get(raffineryId);
                if (rafinnery == null) return;

                if (rafinnery.IsOwnedByBusines())
                {
                    if (rafinnery.GetOwnedBusiness() == dbPlayer.ActiveBusiness && dbPlayer.GetActiveBusinessMember() != null && dbPlayer.GetActiveBusinessMember().Raffinery) // Member of business and has rights
                    {
                        if (rafinnery.Container.GetItemAmount(RaffineryModule.RohölItemModelId) < amount)
                        {
                            dbPlayer.SendNewNotification("Diese Menge kann nicht geladen werden!");
                            return;
                        }
                        SxVehicle sxVehicle = VehicleHandler.Instance.GetClosestVehicle(dbPlayer.Player.Position, 6.0f);
                        if (sxVehicle != null)
                        {
                            // Fahrzeug beladen
                            if (sxVehicle.SyncExtension.Locked)
                            {
                                dbPlayer.SendNewNotification("Fahrzeug muss aufgeschlossen sein!");
                                return;
                            }

                            if (!sxVehicle.Container.CanInventoryItemAdded(RaffineryModule.RohölItemModelId, amount))
                            {
                                dbPlayer.SendNewNotification("Diese Menge kann nicht geladen werden!");
                                return;
                            }

                            if (sxVehicle.Container.IsItemRestrictedForContainer(RaffineryModule.RohölItemModelId))
                            {
                                dbPlayer.SendNewNotification("Dieses Item koennen sie hier nicht einlagern!");
                                return;
                            }

                            Chats.sendProgressBar(dbPlayer, (100 * amount));

                            dbPlayer.Player.TriggerEvent("freezePlayer", true);
                            await Task.Delay(100 * amount);
                            dbPlayer.Player.TriggerEvent("freezePlayer", false);

                            Logging.Logger.AddToRaffineryLog(raffineryId, dbPlayer.Id, amount);

                            rafinnery.Container.LoadIntoVehicle(sxVehicle, RaffineryModule.RohölItemModelId, amount);
                        }
                    }
                }
            }));
        }
    }
}
