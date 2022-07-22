using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Computer.Apps
{
    public class DesktopApp : App<Func<DbPlayer, List<ComputerAppPlayerObject>, bool>>
    {
        private class ShowEvent : Event
        {
            [JsonProperty(PropertyName = "apps")] private List<ComputerAppPlayerObject> Apps { get; }

            public ShowEvent(DbPlayer dbPlayer, List<ComputerAppPlayerObject> computer) : base(dbPlayer)
            {
                Apps = Apps;
            }
        }

        public DesktopApp() : base("DesktopApp", "DesktopApp")
        {
        }

        public override Func<DbPlayer, List<ComputerAppPlayerObject>, bool> Show()
        {
            return (dbPlayer, apps) => OnShow(new ShowEvent(dbPlayer, apps));
        }

        [RemoteEvent]
        public void requestComputerApps(Player player)
        {
            DbPlayer iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;
            if (player.IsReloading) return;
            if (!iPlayer.CanInteract()) return;

            List<ComputerAppPlayerObject> computerAppPlayerObjects = new List<ComputerAppPlayerObject>();

            foreach (KeyValuePair<uint, ComputerApp> kvp in ComputerAppModule.Instance.GetAll())
            {
                if (iPlayer.CanAccessComputerApp(kvp.Value) && kvp.Value.Type == ComputerTypes.Computer) computerAppPlayerObjects.Add(new ComputerAppPlayerObject(kvp.Value));
            }

            TriggerEvent(player, "responseComputerApps", NAPI.Util.ToJson(computerAppPlayerObjects));
        }
    }
}
