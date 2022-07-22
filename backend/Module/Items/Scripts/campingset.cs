using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Camper;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> CampingSet(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;

            // Check near...
            if (CampingModule.Instance.CampingPlaces.Where(cp => cp.Position.DistanceTo(iPlayer.Player.Position) < 50.0f).Count() > 0)
            {
                iPlayer.SendNewNotification("Ein Camp ist bereits zu nahe!");
                return false;
            }

            if (CampingModule.Instance.CampingPlaces.Where(cp => cp.PlayerId == iPlayer.Id).Count() > 0)
            {
                iPlayer.SendNewNotification("Sie haben bereits ein Camp!");
                return false;
            }

            if (iPlayer.Player.Position.Z < 0 || iPlayer.Player.Dimension != 0) return false;

            // Disable Build on Island
            if (iPlayer.HasData("cayoPerico") || iPlayer.HasData("cayoPerico2")) return false;

            Vector3 targetPos = iPlayer.Player.Position.Add(new Vector3(-5.0f, 0, 0));

            iPlayer.SetData("cp_building_step", 1);
            iPlayer.SetData("cp_camppos", iPlayer.Player.Position);
            iPlayer.SetData("cp_markerpos", targetPos);
            iPlayer.Player.TriggerEvent("setCheckpoint", targetPos.X, targetPos.Y, targetPos.Z);
            iPlayer.SendNewNotification("Bitte fang mit dem Aufbau an (Markierungen)");
            return true;
        }
    }
}