﻿using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Newtonsoft.Json;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Tattoo.Windows
{
    public class TattooLicenseWindow : Window<Func<DbPlayer, List<AssetsTattooZone>, bool>>
    {
        public class PlayerTattooLicense
        {
            public string Name;

            public int Price;

            public uint Id;
        }

        private class ShowEvent : Event
        {
            [JsonProperty(PropertyName = "zones")] private List<AssetsTattooZone> Zones { get; }

            public ShowEvent(DbPlayer dbPlayer, IEnumerable<AssetsTattooZone> zones) : base(dbPlayer)
            {
                Zones = zones.Where(t => t.Id != 7 && t.Id != 6)
                    .OrderBy(t => t.Name)
                    .ToList();
            }
        }

        public TattooLicenseWindow() : base("TattooLicenseShop")
        {
        }

        public override Func<DbPlayer, List<AssetsTattooZone>, bool> Show()
        {
            return (player, zones) => OnShow(new ShowEvent(player, zones));
        }

        [RemoteEvent]
        public void requestLicenseShopZoneLicenses(Player Player, int zoneId = 0)
        {
            try
            {
                DbPlayer dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid()) return;

                if (!dbPlayer.HasTattooShop())
                {
                    dbPlayer.SendNewNotification(
                        "Du besitzt keinen Tattoo-Shop und kannst entsprechend keine Lizenzen erwerben!",
                        PlayerNotification.NotificationType.ERROR);

                    return;
                }

                var licenses = TattooLicenseModule.Instance.GetAll().Values.ToList();

                if (licenses.Count == 0)
                {
                    dbPlayer.SendNewNotification("Es werden aktuell keine Tattoo-Lizenzen zum Kauf angeboten!");

                    return;
                }

                TattooShop tattooShop = dbPlayer.GetTattooShop();
                if (tattooShop == null)
                {
                    dbPlayer.SendNewNotification(
                        "Der Lizenzenshop konnte dich keinem Tattooladen zuordnen. Melde dies bitte im GVRP-Bugtracker!",
                        PlayerNotification.NotificationType.ERROR
                    );

                    return;
                }

                var tattoos = new List<PlayerTattooLicense>();

                foreach (TattooLicense lic in licenses)
                {
                    AssetsTattoo tattooData = lic.Tattoo;

                    if (tattooData == null)
                    {
                        tattooData = AssetsTattooModule.Instance.Get(lic.AssetsTattooId);

                        // TattooData not found.
                        if (tattooData == null) continue;
                    }

                    // Wrong zone id.
                    if (tattooData.ZoneId != zoneId) continue;

                    // License already bought.
                    if (tattooShop.tattooLicenses.Find(t => t.AssetsTattooId == lic.AssetsTattooId) != null) continue;

                    tattoos.Add(new PlayerTattooLicense()
                    {
                        Id = lic.Id,
                        Name = tattooData.Name,
                        Price = lic.Price
                    });
                }

                tattoos = tattoos.OrderBy(t => t.Name).ToList();

                dbPlayer.Player.TriggerEvent(
                    "componentServerEvent",
                    "TattooLicenseShop",
                    "responseLicenseShopZoneLicenses",
                    JsonConvert.SerializeObject(tattoos)
                );
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }

        [RemoteEvent]
        public void buyTattooLicenses(Player Player, string cart)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            try
            {
                if (!dbPlayer.HasTattooShop())
                {
                    dbPlayer.SendNewNotification(
                        "Du besitzt keinen Tattoo-Shop und kannst entsprechend keine Lizenzen erwerben!",
                        PlayerNotification.NotificationType.ERROR);

                    return;
                }

                var licenses = TattooLicenseModule.Instance.GetAll().Values.ToList();
                if (licenses.Count == 0)
                {
                    dbPlayer.SendNewNotification("Es werden aktuell keine Tattoo-Lizenzen zum Kauf angeboten!");

                    return;
                }

                TattooShop tattooShop = dbPlayer.GetTattooShop();
                if (tattooShop == null)
                {
                    dbPlayer.SendNewNotification(
                        "Der Lizenzenshop konnte dich keinem Tattooladen zuordnen. Melde dies bitte im GVRP-Bugtracker!",
                        PlayerNotification.NotificationType.ERROR
                    );

                    return;
                }

                var parsedCart = JsonConvert.DeserializeObject<List<PlayerTattooLicense>>(cart);

                // Get full data foreach cart item. To make sure that nobody do manipulate our prices.
                var relevantLicenses = (
                    from PlayerTattooLicense in parsedCart
                    where PlayerTattooLicense != null
                    select licenses.Find(l => l != null && l.Id == PlayerTattooLicense.Id)
                    into tattooLicense
                    where tattooLicense != null
                    select tattooLicense
                ).ToList();

                var price = relevantLicenses.Sum(l => l.Price);

                if (price <= 0) return;

                if (!dbPlayer.TakeMoney(price))
                {
                    dbPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(price));

                    return;
                }

                foreach (TattooLicense tattooLicense in relevantLicenses)
                {
                    tattooShop.AddLicense(tattooLicense);
                }

                dbPlayer.SendNewNotification($"Lizenzen im Wert von ${MSG.Money.fnumber(price)} erworben!");
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }
    }
}