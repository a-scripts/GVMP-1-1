using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Injury.InjuryMove;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> NutritionTest(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;

            if (!iPlayer.IsAMedic() || !iPlayer.IsInDuty()) return false;

            DbPlayer target = Players.Players.Instance.GetClosestPlayerForPlayer(iPlayer);
            if (target == null || !target.IsValid()) return false;

            iPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@prop_human_parking_meter@male@base", "base");
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetCannotInteract(true);
            Chats.sendProgressBar(iPlayer, 10000);
            await Task.Delay(10000);
            iPlayer.StopAnimation(AnimationLevels.User, true);
            iPlayer.SetCannotInteract(false);
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            if (target == null || !target.IsValid() || target.Player.Position.DistanceTo(iPlayer.Player.Position) > 4.0) return false;

            iPlayer.SendNewNotification($"Test durchgeführt: Fett {target.Nutrition.Fett}, Zucker {target.Nutrition.Zucker}, Wasser {target.Nutrition.Wasser}, KCal {target.Nutrition.Kcal}", PlayerNotification.NotificationType.STANDARD, "", 10000);
            return true;
        }
    }

    public static partial class ItemScript
    {
        public static async Task<bool> MedicStationaer(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;

            if (!iPlayer.IsAMedic() || !iPlayer.IsInDuty()) return false;

            DbPlayer target = Players.Players.Instance.GetClosestInjuredForPlayer(iPlayer, 2.5f);
            if (target == null || !target.IsValid()) return false;

            if (!target.isInjured()) return false;

            InjuryMovePoint injuryMovePoint = InjuryMoveModule.Instance.GetAll().Values.Where(ip => ip.Position.DistanceTo(target.Player.Position) < 2.0f).FirstOrDefault();

            if (injuryMovePoint == null) return false;

            if (!target.HasData("injuredName")) return false;

            string note = target.GetData("injuredName");

            iPlayer.SendNewNotification($"Person wurde mit {note} eingeliefert!", PlayerNotification.NotificationType.STANDARD, "", 10000);
            return true;
        }
    }
}