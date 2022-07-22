﻿using GTANetworkAPI;
using System.Linq;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.PlayerName;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Computer.Apps.HouseApp.Apps
{
    public class HouseList : SimpleApp
    {

        public HouseList() : base("HouseList")
        {

        }

        [RemoteEvent]
        public void requestTenants(Player Player)
        {

            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid())
                return;

            House house = HouseModule.Instance.GetByOwner(dbPlayer.Id);
            if (house == null)
            {
                dbPlayer.SendNewNotification("Du besitzt kein Haus.");
                return;
            }

            TriggerEvent(Player, "responseTenants", NAPI.Util.ToJson(HouseAppFunctions.GetTenantsForHouseByPlayer(dbPlayer)), house.Maxrents);
        }


        [RemoteEvent]
        public void saverentprice(Player Player, int price, int slotid)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid())
                return;

            if (slotid == 0) return; // Gibts nicht

            if (dbPlayer.ownHouse[0] != 0)
            {
                if (price < 0 || price > 15000) return;

                HouseRent houseRent = HouseRentModule.Instance.houseRents.ToList().Where(hr => hr.HouseId == dbPlayer.ownHouse[0] && hr.SlotId == slotid).FirstOrDefault();
                if (houseRent == null) return;

                houseRent.RentPrice = price;
                houseRent.Save();
                dbPlayer.SendNewNotification($"Sie haben den Mietpreis des Mietslots {houseRent.SlotId} auf ${price} geändert!");

                var findPlayer = Players.Players.Instance.FindPlayer(houseRent.PlayerId);
                
                if (findPlayer != null && findPlayer.IsValid())
                {
                    findPlayer.SendNewNotification($"Dein Mietvertrag wurde geändert, neuer Mietpreis ${price}!");
                }
            }
        }

        [RemoteEvent]
        public void unrentTenant(Player Player, int slotid)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid())
                return;

            if (slotid == 0) return; // Gibts nicht

            if (dbPlayer.ownHouse[0] != 0)
            {

                HouseRent houseRent = HouseRentModule.Instance.houseRents.ToList().Where(hr => hr.HouseId == dbPlayer.ownHouse[0] && hr.SlotId == slotid).FirstOrDefault();
                if (houseRent == null || houseRent.PlayerId == 0) return;

                var findPlayer = Players.Players.Instance.FindPlayer(houseRent.PlayerId);

                string PlayerName = PlayerNameModule.Instance.Get(houseRent.PlayerId).Name;

                if (findPlayer != null && findPlayer.IsValid())
                {
                    findPlayer.SendNewNotification($"Dein Mietvertrag für das Haus von {dbPlayer.Player.Name} wurde gekündigt.");
                }

                houseRent.Clear();

                dbPlayer.SendNewNotification($"Du hast den Mietvertrag von {PlayerName} gekündigt.");
            }
        }
    }
}
