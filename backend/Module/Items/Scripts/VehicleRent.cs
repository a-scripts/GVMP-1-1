using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static bool vehiclerent(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (!iPlayer.CanInteract())
            {
                return false;
            }
            
            MenuManager.Instance.Build(PlayerMenu.VehicleRentMenu, iPlayer).Show(iPlayer);
            return false;
        }

        public static bool vehiclerentview(DbPlayer iPlayer, ItemModel ItemData, Item item)
        {
            if (!iPlayer.CanInteract())
            {
                return false;
            }

            if (item.Data != null && item.Data.ContainsKey("info"))
            {
                iPlayer.SendNewNotification($"KFZ-Mietvertrag: {item.Data["info"]}", PlayerNotification.NotificationType.STANDARD, "", 12000);
            }
            return false;
        }
    }
}