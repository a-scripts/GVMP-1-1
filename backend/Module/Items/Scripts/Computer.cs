using GTANetworkMethods;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.NSA;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static bool Computer(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (!iPlayer.IsValid()) return false;
            if (iPlayer.CanNSADuty())
            {
                Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSAComputerMenu, iPlayer).Show(iPlayer);
                return true;
            }
            if (iPlayer.IsInDuty() && iPlayer.TeamId == (uint)teams.TEAM_FIB)
            {
                Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSAComputerMenu, iPlayer).Show(iPlayer);
                return true;
            }
            if ((iPlayer.IsInDuty() && iPlayer.Team.Id == (int)teams.TEAM_GOV))
            {
                Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.GOVComputerMenu, iPlayer).Show(iPlayer);
                return true;
            }
            return false;
        }
    }
}