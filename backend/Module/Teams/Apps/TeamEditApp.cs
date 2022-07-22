using VMP_CNR.Module.PlayerUI.Apps;
using GTANetworkAPI;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Phone.Apps;

namespace VMP_CNR.Module.Teams.Apps
{
    public class TeamEditApp : SimpleApp
    {
        public TeamEditApp() : base("TeamEditApp")
        {
        }

        [RemoteEvent]
        public void leaveTeam(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            dbPlayer.SetTeam((uint) TeamList.Zivilist);
        }
    }
}