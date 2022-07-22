using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Gangwar;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> medikit(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.Health > 98)
            {
                return false;
            }

            if (iPlayer.Player.IsInVehicle)
            {
                iPlayer.SendNewNotification("Du kannst waehrend der Fahrt keinen Verbandskasten benutzen");
                return false;
            }

            Chats.sendProgressBar(iPlayer, 4000);
            iPlayer.SetCannotInteract(true);
            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["revive"].Split()[0], Main.AnimationList["revive"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);

            iPlayer.SetData("mediusing", true);

            await Task.Delay(4000);

            iPlayer.ResetData("mediusing");
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.StopAnimation();
            iPlayer.SetHealth(100);
            iPlayer.SetCannotInteract(false);

            return true;
        }

        public static async Task<bool> FMedikit(DbPlayer iPlayer, ItemModel ItemData)

        {

            if (iPlayer.Player.IsInVehicle || iPlayer.Player.Health > 99) return false;
            //if (!iPlayer.Team.IsInTeamfight()) return false;
            if (!GangwarTownModule.Instance.IsTeamInGangwar(iPlayer.Team)) return false;

            Chats.sendProgressBar(iPlayer, 4000);
            iPlayer.SetCannotInteract(true);
            iPlayer.PlayAnimation(
                (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["revive"].Split()[0], Main.AnimationList["revive"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);

            iPlayer.SetData("mediusing", true);

            await Task.Delay(4000);

            iPlayer.ResetData("mediusing");

            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.StopAnimation();
            iPlayer.SetHealth(100);
            iPlayer.SetCannotInteract(false);

            return true;

        }

    }
}