﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Handler;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Staatskasse;

namespace VMP_CNR.Module.Makler
{
    public enum MaklerSellType
    {
        HOUSE = 1,
        VEHICLE = 2,
        STORAGE = 3,
    }
    public class MaklerSellObject
    {
        public uint ObjectId { get; set; }
        public MaklerSellType MaklerSellType { get; set; }
        public uint BuyerId { get; set; }
        public uint SellerId { get; set; }
        public int Price { get; set; }

        public uint MaklerId { get; set; }
    }

    public class MaklerModule : Module<MaklerModule>
    {
        public static Vector3 MaklerPos = new Vector3(-531.676, -192.429, 38.2224);

        public static uint KaufVertragItemId = 1133;
    }

    public class MaklerEvents : Script
    {
        [RemoteEvent]
        public void maklerHouseApplyObjectId(Player player, string returnstring)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (!Int32.TryParse(returnstring, out int houseId))
            {
                dbPlayer.SendNewNotification("Ungültige Hausnummer!");
                return;
            }

            House house = HouseModule.Instance.GetAll().Values.ToList().Where(h => h.Id == houseId).FirstOrDefault();

            if (house == null || house.OwnerId == 0)
            {
                dbPlayer.SendNewNotification("Haus nicht gefunden!");
                return;
            }

            dbPlayer.SetData("mSellObject", new MaklerSellObject() { ObjectId = house.Id, SellerId = house.OwnerId, MaklerSellType = MaklerSellType.HOUSE });

            // Set Buyer Window
            ComponentManager.Get<TextInputBoxWindow>().Show()(dbPlayer, new TextInputBoxWindowObject() { Title = "Haus verkaufen", Callback = "maklerHouseApplyBuyer", Message = "Name des Käufers:" });

            return;
        }

        [RemoteEvent]
        public void maklerHouseApplyBuyer(Player player, string returnstring)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.HasData("mSellObject")) return;

            DbPlayer buyer = Players.Players.Instance.FindPlayer(returnstring);

            if (buyer == null || !buyer.IsValid())
            {
                dbPlayer.SendNewNotification("Käufer nicht gefunden!");
                return;
            }

            if (buyer.ownHouse[0] > 0)
            {
                dbPlayer.SendNewNotification("Der Kunde besitzt bereits ein Haus!");
                return;
            }

            MaklerSellObject maklerSellObject = dbPlayer.GetData("mSellObject");
            if (maklerSellObject == null) return;

            maklerSellObject.BuyerId = buyer.Id;

            dbPlayer.SetData("mSellObject", maklerSellObject);

            // Set Price Window
            ComponentManager.Get<TextInputBoxWindow>().Show()(dbPlayer, new TextInputBoxWindowObject() { Title = "Haus verkaufen", Callback = "maklerHouseApplyPrice", Message = "Preis der Immobilie:" });

            return;
        }

        [RemoteEvent]
        public void maklerHouseApplyPrice(Player player, string returnstring)
        {
            try
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.HasData("mSellObject")) return;


                MaklerSellObject maklerSellObject = dbPlayer.GetData("mSellObject");
                if (maklerSellObject == null) return;

                House iHouse = HouseModule.Instance.Get(maklerSellObject.ObjectId);
                if (iHouse == null) return;

                if (!Int32.TryParse(returnstring, out int price))
                {
                    return;
                }

                if (price > 100000000 || price < iHouse.Price / 2)
                {
                    dbPlayer.SendNewNotification(
                        "Ungueltiger Preis, dieser muss mindestens die Hälfte ($" +
                        (iHouse.Price / 2) + ") des Hauspreises sein.");
                    return;
                }

                if (price > 500000 && dbPlayer.jobskill[0] < 1000)
                {
                    dbPlayer.SendNewNotification(
                        "Sie benötigen mindestens 1000 Skillpunkte bei einem Wert ueber $500.000");
                    return;
                }

                if (price > 1000000 && dbPlayer.jobskill[0] < 2500)
                {
                    dbPlayer.SendNewNotification(
                        "Sie benötigen mindestens 2500 Skillpunkte bei einem Wert ueber $1.000.000");
                    return;
                }

                if (price > 5000000 && dbPlayer.jobskill[0] < 5000)
                {
                    dbPlayer.SendNewNotification(
                        "Sie benötigen mindestens 5000 Skillpunkte bei einem Wert ueber $5.000.000");
                    return;
                }

                maklerSellObject.Price = price;

                maklerSellObject.MaklerId = dbPlayer.Id;

                dbPlayer.ResetData("mSellObject");

                DbPlayer govBeamter = Players.Players.Instance.GetValidPlayers().Where(p => p.TeamId == (uint)teams.TEAM_GOV && p.TeamRank >= 5 && p.Player.Position.DistanceTo(dbPlayer.Player.Position) < 3.0f).FirstOrDefault();

                if (govBeamter == null || !govBeamter.IsValid() || (govBeamter.Player.Position.DistanceTo(Government.GovernmentModule.ComputerBuero1Pos) > 4.0f && govBeamter.Player.Position.DistanceTo(Government.GovernmentModule.ComputerBuero2Pos) > 4.0f) || !govBeamter.IsInDuty())
                {
                    dbPlayer.SendNewNotification("Es ist niemand vom Grundbuchamt in der Nähe!");
                    return;
                }

                govBeamter.SetData("mSellObject", maklerSellObject);

                // Send Contract to GOV 
                ComponentManager.Get<ConfirmationWindow>().Show()(govBeamter, new ConfirmationObject($"Grundbuch Änderung", $"" +
                    $"Immobilie ({maklerSellObject.ObjectId}) " +
                    $"Aktueller Besitzer: {PlayerName.PlayerNameModule.Instance.Get(maklerSellObject.SellerId).Name}" +
                    $"Käufer: {PlayerName.PlayerNameModule.Instance.Get(maklerSellObject.BuyerId).Name}" +
                    $"Preis: ${maklerSellObject.Price}" +
                    $"Makler: {dbPlayer.GetName()}",
                    "govHouseConfirm", "", ""));

                return;
            }
            catch(Exception e)
            {
                Logging.Logger.Crash(e);
            }
        }

        [RemoteEvent]
        public void govHouseConfirm(Player p_Player, string pb_map, string none)
        {

            DbPlayer iPlayer = p_Player.GetPlayer();

            if (iPlayer == null || !iPlayer.IsValid())
            {
                return;
            }
            if (!iPlayer.HasData("mSellObject"))
            {
                return;
            }

            MaklerSellObject maklerSellObject = iPlayer.GetData("mSellObject");
            if (maklerSellObject == null) return;

            DbPlayer makler = Players.Players.Instance.GetByDbId(maklerSellObject.MaklerId);
            DbPlayer customer = Players.Players.Instance.GetByDbId(maklerSellObject.BuyerId);
            DbPlayer owner = Players.Players.Instance.GetByDbId(maklerSellObject.SellerId);

            int price = maklerSellObject.Price;

            House iHouse = HouseModule.Instance.Get(maklerSellObject.ObjectId);


            if (iHouse == null || price < 0 || makler == null || customer == null || owner == null || !makler.IsValid() || !customer.IsValid() || !owner.IsValid())
            {
                iPlayer.SendNewNotification("Nicht alle Vertragspartner vorhanden!");
                return;
            }

            if(makler.Container.GetItemAmount(MaklerModule.KaufVertragItemId) <= 0)
            {
                return;
            }

            if (!customer.TakeBankMoney(price, $"Makler-Hauskauf - Haus {iHouse.Id}"))
            {
                iPlayer.SendNewNotification(
                    "Der Kunde hat nicht genug Geld!");
                customer.SendNewNotification(MSG.Money.NotEnoughMoney(price));
                return;
            }

            // Haus switch Process
            owner.ownHouse[0] = 0;
            if (owner.IsTenant()) owner.RemoveTenant();
            customer.ownHouse[0] = iHouse.Id;

            HouseKeyHandler.Instance.DeleteAllHouseKeys(iHouse);
            iHouse.OwnerId = customer.Id;
            iHouse.OwnerName = customer.GetName();
            iHouse.SaveOwner();

            var provision = price / 10;

            owner.GiveBankMoney(price - provision * 2, $"Makler-Hausverkauf - Haus {iHouse.Id}");
            makler.GiveBankMoney(provision, $"Makler-Provision - Haus {iHouse.Id}");

            KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, provision * 2);

            makler.SendNewNotification(
                "Sie haben das Haus erfolgreich verkauft! Ihre Provision $" + provision);
            customer.SendNewNotification(
                "Sie haben das Haus erfolgreich fuer $" +
                price +
                " erworben!");
            owner.SendNewNotification(
                "Ihr Haus wurde an " + customer.GetName() +
                " fuer $" +
                (price - provision * 2) + " verkauft!");
            makler.JobSkillsIncrease(7);
            KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, price - provision * 2);

            iPlayer.SendNewNotification("Der Grundbucheintrag war erfolgreich!");
            owner.SendNewNotification("Der Grundbucheintrag war erfolgreich!");
            makler.SendNewNotification("Der Grundbucheintrag war erfolgreich!");
            customer.SendNewNotification("Der Grundbucheintrag war erfolgreich!");
            iPlayer.ResetData("mSellObject");

            // Logging
            Logging.Logger.SaveToGovSellLog(iHouse.Id, owner.Id, customer.Id, iPlayer.Id, price);

            makler.Container.RemoveItem(MaklerModule.KaufVertragItemId, 1);
        }
    }
}
