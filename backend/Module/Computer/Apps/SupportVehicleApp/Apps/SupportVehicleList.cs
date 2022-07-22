using GTANetworkAPI;
using System.Threading.Tasks;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using static VMP_CNR.Module.Computer.Apps.SupportVehicleApp.SupportVehicleFunctions;

namespace VMP_CNR.Module.Computer.Apps.SupportVehicleApp.Apps
{
    class SupportVehicleList : SimpleApp
    {
        public SupportVehicleList() : base("SupportVehicleList") { }

        [RemoteEvent]
        public async void requestSupportVehicleList(Player Player, int owner)
        {
            
                DbPlayer iPlayer = Player.GetPlayer();
                if (iPlayer == null || !iPlayer.IsValid()) return;

                VehicleCategory category = VehicleCategory.ALL;
                var vehicleData = SupportVehicleFunctions.GetVehicleData(category, owner);

                var vehicleDataJson = NAPI.Util.ToJson(vehicleData);
                TriggerEvent(Player, "responseVehicleList", vehicleDataJson);
            
        }
    }
}
