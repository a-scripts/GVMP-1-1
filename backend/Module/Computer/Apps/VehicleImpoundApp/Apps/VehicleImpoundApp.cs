using System;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Computer.Apps.VehicleImpoundApp.Apps
{
    public class VehicleImpoundApp : SimpleApp
    {
        public VehicleImpoundApp() : base ("VehicleImpoundApp") { }


        [RemoteEvent]
        public void requestVehicleConfiscationById(Player Player, uint vehicleId)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            var overview = VehicleImpoundFunctions.GetVehicleImpoundOverviews(dbPlayer, vehicleId);
            TriggerEvent(Player, "responseVehicleImpound", NAPI.Util.ToJson(overview));
        }

        [RemoteEvent]
        public void requestVehicleImpoundMember(Player player, string member)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, member)) return;
            if (member == null) return;

            var overview = VehicleImpoundFunctions.GetVehicleImpoundOverviewsByMember(dbPlayer, member);
            TriggerEvent(player, "responseVehicleImpound", NAPI.Util.ToJson(overview));
        }
    }
}