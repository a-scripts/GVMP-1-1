using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Tattoo;

namespace VMP_CNR.Module.Tattoo.Windows
{
    public class TattooShopWindow : Window<Func<DbPlayer, List<PlayerTattoo>, bool>>
    {
        private class ShowEvent : Event
        {
            [JsonProperty(PropertyName = "tattoos")] private List<PlayerTattoo> Tattoos { get; }

            public ShowEvent(DbPlayer dbPlayer, List<PlayerTattoo> tattoos) : base(dbPlayer)
            {
                tattoos = tattoos.OrderBy(t => t.Name).ToList();
                Tattoos = tattoos;
            }
        }

        public TattooShopWindow() : base("TattooShop")
        {
        }

        public override Func<DbPlayer, List<PlayerTattoo>, bool> Show()
        {
            return (player, tattoos) => OnShow(new ShowEvent(player, tattoos));
        }

        [RemoteEvent]
        public void syncTattoo(Player Player, string hash)
        {
            var dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            dbPlayer.ClearDecorations();

            AssetsTattoo assetsTattoo = AssetsTattooModule.Instance.GetAll().Values.ToList().Find(t => t.HashFemale == hash || t.HashMale == hash);
            if (assetsTattoo != null)
            {

                Decoration decoration = new Decoration();
                decoration.Collection = NAPI.Util.GetHashKey(assetsTattoo.Collection);
                decoration.Overlay = dbPlayer.Customization.Gender == 0 ? NAPI.Util.GetHashKey(assetsTattoo.HashMale) : NAPI.Util.GetHashKey(assetsTattoo.HashFemale);

                NAPI.Player.SetPlayerDecoration(Player, decoration);
            }
        }

        [RemoteEvent]
        public void requestTattooShopCategoryTattoos(Player Player, int CategoryId=0)
        {
            try
            {
                var dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid()) return;
                if (!dbPlayer.TryData("tattooShopId", out uint tattooShopId)) return;
                var tattooShop = TattooShopModule.Instance.Get(tattooShopId);
                if (tattooShop == null) return;

                List<PlayerTattoo> cTattooList = new List<PlayerTattoo>();

                foreach (TattooAddedItem tattooAddedItem in tattooShop.tattooLicenses)
                {
                    AssetsTattoo assetsTattoo = AssetsTattooModule.Instance.Get((uint)tattooAddedItem.AssetsTattooId);
                    if (assetsTattoo.GetHashForPlayer(dbPlayer) != "" && assetsTattoo.ZoneId == CategoryId)
                    {
                        cTattooList.Add(new PlayerTattoo(assetsTattoo.GetHashForPlayer(dbPlayer), assetsTattoo.ZoneId, assetsTattoo.Price, assetsTattoo.Name));
                    }
                }
                IEnumerable<PlayerTattoo> cTattooList_o = cTattooList.OrderBy(t => t.Name);
                string json = JsonConvert.SerializeObject(cTattooList_o);
                dbPlayer.Player.TriggerEvent("componentServerEvent", "TattooShop", "responseTattooShopCategory", json);
            }
            catch (Exception ex)
            {
                Logger.Crash(ex);
            }
        }

        [RemoteEvent]
        public void tattooShopBuy(Player Player, string hash)
        {
            try
            {
                var dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid()) return;
                if (!dbPlayer.TryData("tattooShopId", out uint tattooShopId)) return;
                var tattooShop = TattooShopModule.Instance.Get(tattooShopId);
                if (tattooShop == null) return;


                Business.Business business = BusinessModule.Instance.GetById((uint)tattooShop.BusinessId);

                if (business == null) return;

                if (business.GetMembers().Count > 0)
                {
                    foreach (var member in business.GetMembers().Values)
                    {
                        DbPlayer memberPlayer = Players.Players.Instance.GetByDbId(member.PlayerId);
                        if (memberPlayer == null || !memberPlayer.IsValid()) continue;

                        if ((member.Manage || member.Owner || member.Tattoo) && memberPlayer.Player.Position.DistanceTo(dbPlayer.Player.Position) < 10)
                        {

                            AssetsTattoo assetsTattoo = AssetsTattooModule.Instance.GetAll().Values.ToList().Find(t => t.HashFemale == hash || t.HashMale == hash);
                            if (assetsTattoo != null)
                            {
                                TattooAddedItem tattooAddedItem = tattooShop.tattooLicenses.Find(l => l.AssetsTattooId == assetsTattoo.Id);
                                TattooLicense tattooLicense = TattooLicenseModule.Instance.Get((uint) tattooAddedItem.TattooLicenseId);
                                if (tattooLicense == null) return;

                                if (!dbPlayer.TakeMoney(assetsTattoo.Price))
                                {
                                    dbPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(assetsTattoo.Price));
                                    return;
                                }
                                Decoration decoration = new Decoration();
                                decoration.Collection = NAPI.Util.GetHashKey(assetsTattoo.Collection);
                                decoration.Overlay = dbPlayer.Customization.Gender == 0 ? NAPI.Util.GetHashKey(assetsTattoo.HashMale) : NAPI.Util.GetHashKey(assetsTattoo.HashFemale);

                                NAPI.Player.SetPlayerDecoration(Player, decoration);
                                dbPlayer.AddTattoo(assetsTattoo.Id);

                                dbPlayer.SendNewNotification($"Tattoo {assetsTattoo.Name} fuer ${assetsTattoo.Price} gekauft!");

                                tattooShop.AddBank((int) (assetsTattoo.Price * 0.5));
                                Logger.AddTattoShopLog(tattooShop.Id, dbPlayer.Id, (int) (assetsTattoo.Price * 0.5), true);


                                dbPlayer.ApplyDecorations();
                                return;
                            }
                        }
                    }
                }


                dbPlayer.SendNewNotification("Um dir ein Tatto stechen zu lassen muss ein Tätowierer anwesend sein.");
                return;



            }
            catch (Exception ex)
            {
                Logger.Crash(ex);
            }
        }
    }
}
