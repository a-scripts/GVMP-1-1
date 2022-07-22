using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Configurations;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.Business.FuelStations;
using static VMP_CNR.Module.Business.Apps.BusinessListApp;
using System.Linq;
using VMP_CNR.Module.Business.Apps;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Business.Raffinery;
using VMP_CNR.Module.Business.NightClubs;

namespace VMP_CNR.Module.Computer.Apps.BusinessDetailApp.Apps
{
  
    
    public class FuelstationObject
    {
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        [JsonProperty(PropertyName = "price")]
        public int Price { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }

        [JsonProperty(PropertyName = "Log")]
        public List<FuelstationLogObject> Log { get; set; }

    }

    public class RaffineryObject
    {

        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }
        [JsonProperty(PropertyName = "Log")]
        public List<RaffineryLogObject> Log { get; set; }

    }

    public class NightclubObject
    {

        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }
        [JsonProperty(PropertyName = "items")]
        public List<NightclubItemObject> Items { get; set; }

    }

    public class NightclubItemObject
    {
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }
        [JsonProperty(PropertyName = "price")]
        public int Price { get; set; }
        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }
    }

    public class BusinessDetailApp : SimpleApp
    {
        
        public BusinessDetailApp() : base("BusinessDetailApp") { }

            [RemoteEvent]
            public void requestBusinessDetailMembers(Player Player)
            {
                DbPlayer dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid())
                    return;

            var members = new List<BusinessMember>();
            var business = dbPlayer.ActiveBusiness;

            if (business.GetMembers().Count <= 0) return;
            if (business.GetMember(dbPlayer.Id) == null) return;

            foreach (var member in business.GetMembers().ToList())
            {
                if (member.Value == null) continue;
                var findplayer = Players.Players.Instance.FindPlayerById(member.Value.PlayerId);
                if (findplayer == null || !findplayer.IsValid() || findplayer.IsInAdminDuty() || findplayer.IsInGuideDuty() || findplayer.IsInGameDesignDuty()) continue;

                var businessMember = member.Value;
                var currentDbPlayer = Players.Players.Instance.GetByDbId(member.Key);
                if (currentDbPlayer == null || !currentDbPlayer.IsValid()) continue;

                members.Add(new BusinessMember(currentDbPlayer.Id, currentDbPlayer.GetName(), businessMember.Money, businessMember.Manage, businessMember.Owner, businessMember.Salary, (int)currentDbPlayer.handy[0], businessMember.Raffinery, businessMember.Fuelstation, businessMember.NightClub, businessMember.Tattoo));
            }

            int manage = 0;
            if (business.GetMember(dbPlayer.Id).Owner) manage = 2;
            else if (business.GetMember(dbPlayer.Id).Manage) manage = 1;

            var membersManageObject = new MembersManageObject { BusinessMemberList = members, ManagePermission = manage };
            var membersJson = JsonConvert.SerializeObject(membersManageObject);

            if (!string.IsNullOrEmpty(membersJson))
            {
                TriggerEvent(Player,"responseBusinessDetail", membersJson,business.Money);
            }
  
             }

            [RemoteEvent]
            public void requestBusinessDetailFuelstation(Player Player)
            {
                DbPlayer dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid())
                    return;

            if (dbPlayer.ActiveBusiness.BusinessBranch.hasFuelstation())
            {
                FuelStation fuelStation = FuelStationModule.Instance.Get(dbPlayer.ActiveBusiness.BusinessBranch.FuelstationId);

                FuelstationObject FuelstationObject = new FuelstationObject
                {
                    Name = fuelStation.Name,
                    Price = fuelStation.Price,
                    Amount = fuelStation.Container.GetItemAmount(537),
                    Log = fuelStation.GetLogFuelstationFilled(),
                };

                TriggerEvent(Player,"responseBusinessDetail", JsonConvert.SerializeObject(FuelstationObject));
            }


            }

            [RemoteEvent]
            public void requestBusinessDetailRaffinery(Player Player)
            {
                DbPlayer dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid())
                    return;

                if (dbPlayer.ActiveBusiness.BusinessBranch.hasRaffinerie())
                {
                    Raffinery raffinery = RaffineryModule.Instance.Get(dbPlayer.ActiveBusiness.BusinessBranch.RaffinerieId);

                    RaffineryObject raffineryObject = new RaffineryObject
                    {
                        Amount = raffinery.Container.GetItemAmount(536),
                        Log = raffinery.GetLogRaffinery(),
                    };

                    TriggerEvent(Player,"responseBusinessDetail", JsonConvert.SerializeObject(raffineryObject));
                }

            }

            [RemoteEvent]
            public void requestBusinessDetailNightclub(Player Player)
            {
            DbPlayer dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid())
                    return;
                if (dbPlayer.ActiveBusiness.BusinessBranch.hasNightClub())
                {
                    List<NightclubItemObject> nightclubItemObject = new List<NightclubItemObject>();
                    NightClub nightClub = NightClubModule.Instance.Get(dbPlayer.ActiveBusiness.BusinessBranch.NightClubId);
                    foreach (NightClubItem nightClubItem in nightClub.NightClubShopItems)
                    {
                        NightclubItemObject data = new NightclubItemObject
                        {
                            Name = nightClubItem.Name,
                            Price = nightClubItem.Price,
                            Amount = nightClub.Container.GetItemAmount(nightClubItem.ItemId),
                        };

                        nightclubItemObject.Add(data);
                    }

                    NightclubObject nightclubObject = new NightclubObject
                    {
                        Name = nightClub.Name,
                        Items = nightclubItemObject,
                    };

                    TriggerEvent(Player,"responseBusinessDetail", JsonConvert.SerializeObject(nightclubObject));
                }

            }
            [RemoteEvent]
            public void requestBusinessDetail(Player Player)
            {
                DbPlayer dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid())return;
                List<string> test = new List<string>();
                if (dbPlayer.ActiveBusiness == null) { TriggerEvent(Player, "responseBusinessDetailLinks", "[]"); return;}

                if (dbPlayer.ActiveBusiness.GetMember(dbPlayer.Id) != null && dbPlayer.ActiveBusiness.GetMembers().Count > 0)
                {
                    test.Add("Mitglieder");

                    if (dbPlayer.ActiveBusiness.BusinessBranch.hasFuelstation())
                    {
                        test.Add("Tankstelle");
                    }
                    if (dbPlayer.ActiveBusiness.BusinessBranch.hasRaffinerie())
                    {
                        test.Add("Oelpumpe");
                    }
                    if (dbPlayer.ActiveBusiness.BusinessBranch.hasNightClub())
                    {
                        test.Add("Club");
                    }

                    TriggerEvent(Player, "responseBusinessDetailLinks", JsonConvert.SerializeObject(test));
                }
                else
                {
                    TriggerEvent(Player, "responseBusinessDetailLinks", "[]");
                }
            }


        }
}
