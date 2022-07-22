using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Tattoo;

namespace VMP_CNR
{
    public class TattooLicenseMenuBuilder : MenuBuilder
    {
        public TattooLicenseMenuBuilder() : base(PlayerMenu.TattooLicenseMenu)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            if (!iPlayer.HasTattooShop())
            {
                iPlayer.SendNewNotification("Du besitzt keinen Tattoo-Shop und kannst entsprechend keine Lizenzen erwerben!", PlayerNotification.NotificationType.ERROR);
                return null;
            }

            List<TattooLicense> licenses = TattooLicenseModule.Instance.GetAll().Values.ToList();
            if (licenses == null || licenses.Count == 0)
            {
                iPlayer.SendNewNotification("Es werden aktuell keine Tattoo-Lizenzen zum Kauf angeboten!");
                return null;
            }

            var tattooshop = TattooShopFunctions.GetTattooShop(iPlayer);
            if (tattooshop == null)
            {
                iPlayer.SendNewNotification("Der Lizenzenshop konnte dich keinem Tattooladen zuordnen. Melde dies bitte im GVMP-Bugtracker!", PlayerNotification.NotificationType.ERROR);
                return null;
            }

            var menu = new Menu(Menu, "Tattoo Licenses");

            var tattoos = new List<TattooLicenseList>();
            uint max_amount = uint.Parse(iPlayer.GetData("tattooLicensePage").ToString()) * 100;
            uint min_amount = max_amount <= 100 ? 0 : max_amount - 100;

            uint idx = 0;
            foreach (TattooLicense lic in licenses)
            {
                if (idx < min_amount)
                {
                    idx++;
                    continue;
                }

                if (idx >= max_amount)
                    break;
                
                AssetsTattoo assetsTattoo = AssetsTattooModule.Instance.Get(lic.AssetsTattooId);
                if (assetsTattoo == null) continue;
                if (tattooshop.tattooLicenses.Find(t => t.AssetsTattooId == lic.AssetsTattooId) != null) continue;
                TattooLicenseList tat = new TattooLicenseList()
                {
                    Name = assetsTattoo.Name,
                    Price = lic.Price
                };
                tattoos.Add(tat);
                idx++;
            }

            tattoos = tattoos.OrderBy(t => t.Name).ToList();
            
            foreach(TattooLicenseList tatlic in tattoos)
            {
                menu.Add($"{tatlic.Name} {tatlic.Price}$");
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
                List<TattooLicense> licenses = TattooLicenseModule.Instance.GetAll().Values.ToList();
                int idx = 0;

                var tattooshop = TattooShopFunctions.GetTattooShop(iPlayer);
                if (tattooshop == null)
                {
                    MenuManager.DismissCurrent(iPlayer);
                    return false;
                }

                var tattoos = new List<TattooLicenseList>();

                uint max_amount = uint.Parse(iPlayer.GetData("tattooLicensePage").ToString()) * 100;
                uint min_amount = max_amount <= 100 ? 0 : max_amount - 100;

                uint idx2 = 0;
                foreach (TattooLicense lic in licenses)
                {
                    if (idx2 < min_amount)
                    {
                        idx2++;
                        continue;
                    }

                    if (idx2 >= max_amount)
                        break;
                
                    AssetsTattoo assetsTattoo = AssetsTattooModule.Instance.Get(lic.AssetsTattooId);
                    if (assetsTattoo == null) continue;
                    if (tattooshop.tattooLicenses.Find(t => t.AssetsTattooId == lic.AssetsTattooId) != null) continue;

                    TattooLicenseList tat = new TattooLicenseList()
                    {
                        Id = lic.Id,
                        Name = assetsTattoo.Name,
                        Price = lic.Price
                    };

                    tattoos.Add(tat);
                    idx2++;
                }

                iPlayer.ResetData("tattooLicensePage"); // Reset Pagination

                tattoos = tattoos.OrderBy(t => t.Name).ToList();

                foreach(TattooLicenseList tatlic in tattoos)
                {
                    if (index == idx)
                    {
                        if (!iPlayer.TakeMoney(tatlic.Price))
                        {
                            iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(tatlic.Price));
                            return false;
                        }
                        TattooShop tattooShop = iPlayer.GetTattooShop();
                        TattooLicense lic = TattooLicenseModule.Instance.Get(tatlic.Id);
                        tattooShop.AddLicense(lic);

                        iPlayer.SendNewNotification($"Lizenz {tatlic.Name} erworben!");
                        return true;
                    }
                    idx++;
                }

                MenuManager.DismissCurrent(iPlayer);
                return false;
            }
        }
    }

    public class TattooLicenseList
    {
        public string Name;
        public int Price;
        public uint Id;
    }
}