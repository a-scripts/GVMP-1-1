﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Teams.AmmoArmory;
using VMP_CNR.Module.Teams.Shelter;

namespace VMP_CNR.Module.Teams.AmmoPackageOrder
{
    class AmmoArmoryEvents : Script
    {


        [RemoteEvent]

        public void BuyAmmunition(Player player, String returnString)
        {
            DbPlayer iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;

            if (!iPlayer.HasData("ammo_armory")) return;
            int index = iPlayer.GetData("ammo_armory");

            if (!Int32.TryParse(returnString, out int amount))
            {
                iPlayer.SendNewNotification("Bitte gib eine Zahl ein!");
                return;
            }

            if (amount < 1) return;


            AmmoArmorie ammoArmorie = AmmoArmoryModule.Instance.GetByPosition(iPlayer.Player.Position);
            if (ammoArmorie == null || !iPlayer.Team.IsGangsters() || iPlayer.Team.Id != ammoArmorie.TeamId) return;
            int idx = 3;
            foreach (AmmoArmorieItem ammoArmorieItem in ammoArmorie.ArmorieItems)
            {
                if (idx == index)
                {
                    int RequiredPackets = ammoArmorieItem.GetRequiredPacketsForTeam(iPlayer.Team) * amount;

                    if (ammoArmorie.Packets < RequiredPackets)
                    {
                        iPlayer.SendNewNotification("Nicht genug Waffenpakete!");
                        return;
                    }
                    if (ammoArmorie.Powder < RequiredPackets)
                    {
                        iPlayer.SendNewNotification("Nicht genug Schwarzpulver!");
                        return;
                    }

                    ItemModel resultItem = ItemModelModule.Instance.Get(ammoArmorieItem.ItemId);
                    if (resultItem == null) return;

                    if (iPlayer.Container.CanInventoryItemAdded(resultItem, amount))
                    {
                        if (ammoArmorieItem.TeamPrice > 0)
                        {
                            if (!iPlayer.TakeBankMoney(ammoArmorieItem.TeamPrice * amount))
                            {
                                iPlayer.SendNewNotification("Nicht genug Geld (Bank)!");
                                return;
                            }

                            TeamShelter teamShelter = TeamShelterModule.Instance.GetByTeam(iPlayer.Team.Id);
                            if (teamShelter == null) return;

                            teamShelter.GiveMoney(ammoArmorieItem.TeamPrice * amount);
                        }

                        ammoArmorie.ChangePackets(-RequiredPackets);
                        ammoArmorie.ChangePowder(-RequiredPackets);
                        iPlayer.Container.AddItem(resultItem, amount);
                        iPlayer.SendNewNotification($"Sie haben {amount} {resultItem.Name} für ${ammoArmorieItem.TeamPrice * amount} (P: {RequiredPackets} SP: {RequiredPackets}) entnommen!");
                        return;
                    }
                    else
                    {
                        iPlayer.SendNewNotification("Zu wenig Platz!");
                        return;
                    }
                }
                else idx++;
            }
        }


        [RemoteEvent]
        public void ConfigAmmoArmoriePrice(Player player, string returnstring)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.HasData("configAmmoPrice")) return;

            AmmoArmorieItem item = AmmoArmoryItemModule.Instance.Get(dbPlayer.GetData("configAmmoPrice"));
            if (item == null) return;
            if (!dbPlayer.Team.IsGangsters() || dbPlayer.TeamRank <= 10) return;


            if (int.TryParse(returnstring, out int price))
            {
                if (price >= 0 && price < 10000)
                {
                    item.ChangeTeamPrice(price);
                    dbPlayer.SendNewNotification($"Sie haben den Preis für {ItemModelModule.Instance.Get(item.ItemId).Name} auf ${price} gesetzt!");
                    return;
                }
                return;
            }
            else
                dbPlayer.SendNewNotification("Falsche Anzahl!");
        }
    }
}
