using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Computer.Apps
{
    public class IpadDesktopApp : App<Func<DbPlayer, List<ComputerAppPlayerObject>, bool>>
    {
        private class ShowEvent : Event
        {
            [JsonProperty(PropertyName = "apps")] private List<ComputerAppPlayerObject> Apps { get; }

            public ShowEvent(DbPlayer dbPlayer, List<ComputerAppPlayerObject> computer) : base(dbPlayer)
            {
                Apps = Apps;
            }
        }

        public IpadDesktopApp() : base("IpadDesktopApp", "IpadDesktopApp") {}

        public override Func<DbPlayer, List<ComputerAppPlayerObject>, bool> Show()
        {
            return (dbPlayer, apps) => OnShow(new ShowEvent(dbPlayer, apps));
        }

        [RemoteEvent]
        public void requestIpadApp(Player player)
        {
            DbPlayer iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;
            if (player.IsReloading) return;
            if (!iPlayer.Rank.CanAccessFeature("adminTablet")) return;

            List<ComputerAppPlayerObject> computerAppPlayerObjects = new List<ComputerAppPlayerObject>();

            foreach (KeyValuePair<uint, ComputerApp> kvp in ComputerAppModule.Instance.GetAll())
            {
                if (iPlayer.CanAccessComputerApp(kvp.Value) && kvp.Value.Type == ComputerTypes.AdminTablet) computerAppPlayerObjects.Add(new ComputerAppPlayerObject(kvp.Value));
            }

            TriggerEvent(player, "responseIpadApps", NAPI.Util.ToJson(computerAppPlayerObjects));
        }
    }
}
