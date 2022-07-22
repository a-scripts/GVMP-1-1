using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Voice;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Business.NightClubs;
using System.Threading.Tasks;
using VMP_CNR.Module.Schwarzgeld;
using VMP_CNR.Module.NSA;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Events.CWS;
using VMP_CNR.Module.Teams.Shelter;

namespace VMP_CNR.Module.Workstation.Windows
{
    
    public class WorkStationSendPlayerItem
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
    }

    public class WorkstationWindow : Window<Func<DbPlayer, Workstation, bool>>
    {
        private class ShowEvent : Event
        {
            [JsonProperty(PropertyName = "workstation")] private Workstation workstationData { get; }
            [JsonProperty(PropertyName = "sourceItems")] private List<WorkStationSendPlayerItem> sourceItemsData { get; }
            [JsonProperty(PropertyName = "endItems")] private List<WorkStationSendPlayerItem> endItemsData { get; }
            [JsonProperty(PropertyName = "price")] private int price { get; }

            public ShowEvent(DbPlayer dbPlayer, Workstation Workstation) : base(dbPlayer)
            {
                workstationData = Workstation;

                List<WorkStationSendPlayerItem> sourceItems = new List<WorkStationSendPlayerItem>();
                List<WorkStationSendPlayerItem> endItems = new List<WorkStationSendPlayerItem>();

                foreach (KeyValuePair<uint, int> kvp in Workstation.SourceConvertItems)
                {
                    ItemModel xItem = ItemModelModule.Instance.Get(kvp.Key);
                    if(xItem != null)
                    {
                        sourceItems.Add(new WorkStationSendPlayerItem() { Id = kvp.Key, Name = xItem.Name, Amount = kvp.Value });
                    }
                }

                ItemModel xEndItem = ItemModelModule.Instance.Get(Workstation.EndItemId);
                if (xEndItem != null)
                {
                    endItems.Add(new WorkStationSendPlayerItem() { Id = Workstation.EndItemId, Name = xEndItem.Name, Amount = Workstation.End5MinAmount });
                }

                endItemsData = endItems;
                sourceItemsData = sourceItems;
                price = 2500;
            }
        }

        public WorkstationWindow() : base("Workstation")
        {
        }

        public override Func<DbPlayer, Workstation, bool> Show()
        {
            return (player, workstation) => OnShow(new ShowEvent(player, workstation));
        }

        [RemoteEvent]
        public void RentWorkstationEvent(Player Player, int workstationId)
        {
            DbPlayer iPlayer = Player.GetPlayer();

            if (iPlayer == null || !iPlayer.IsValid()) return;

            Workstation workstation = WorkstationModule.Instance.GetAll().Values.ToList().Where(w => w.Id == workstationId && w.NpcPosition.DistanceTo(iPlayer.Player.Position) < 2.0f).FirstOrDefault();

            if(workstation != null)
            {
                if (!workstation.LimitTeams.Contains(iPlayer.TeamId))
                {
                    iPlayer.SendNewNotification($"Du scheinst mir zu unseriös zu sein... Arbeitest du schon etwas anderes?");
                    return;
                }
                if (iPlayer.WorkstationId == workstation.Id)
                {
                    iPlayer.SendNewNotification($"Sie sind hier bereits eingemietet!");
                    return;
                }
                if (workstation.RequiredLevel > 0 && workstation.RequiredLevel > iPlayer.Level)
                {
                    iPlayer.SendNewNotification($"Für diese Workstation benötigen Sie mind Level {workstation.RequiredLevel}!");
                    return;
                }
                if (!iPlayer.TakeMoney(2500))
                {
                    iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(2500));
                    return;
                }

                iPlayer.WorkstationEndContainer.ClearInventory();
                iPlayer.WorkstationFuelContainer.ClearInventory();
                iPlayer.WorkstationSourceContainer.ClearInventory();

                iPlayer.SendNewNotification($"Sie haben sich in {workstation.Name} eingemietet und können diese nun benutzen!");
                iPlayer.WorkstationId = workstation.Id;
                iPlayer.SaveWorkstation();
            }
            
        }
    }
}
