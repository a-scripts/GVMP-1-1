using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Events.Halloween;
using VMP_CNR.Module.Gangwar;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> UnderArmor(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle) return false;
            iPlayer.SetCannotInteract(true);

            Chats.sendProgressBar(iPlayer, 4000);
            iPlayer.PlayAnimation(
                    (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);

            iPlayer.SetData("armorusing", true);

            await Task.Delay(4000);

            iPlayer.ResetData("armorusing");
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.SetCannotInteract(false);
            iPlayer.StopAnimation();

            int type = -1;
            if (iPlayer.VisibleArmorType != type)
                iPlayer.SaveArmorType(type);
            iPlayer.SetArmor(99, false);

            return true;
        }

        public static async Task<bool> Armor(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle) return false;
            iPlayer.SetCannotInteract(true);

            Chats.sendProgressBar(iPlayer, 4000);
            iPlayer.PlayAnimation(
                    (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);

            iPlayer.SetData("armorusing", true);

            await Task.Delay(4000);

            iPlayer.ResetData("armorusing");
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.SetCannotInteract(false);
            iPlayer.StopAnimation();
            iPlayer.SetArmor(100, true);

            return true;
        }

        public static async Task<bool> BArmor(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle || !iPlayer.IsCopPackGun() || !iPlayer.IsInDuty()) return false;
            iPlayer.SetCannotInteract(true);

            Chats.sendProgressBar(iPlayer, 4000);
            iPlayer.PlayAnimation(
                (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);

            iPlayer.SetData("armorusing", true);

            await Task.Delay(4000);

            iPlayer.ResetData("armorusing");
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.SetCannotInteract(false);
            iPlayer.StopAnimation();
            iPlayer.SetArmor(100, true);
            
            return true;
        }

        public static async Task<bool> BUnderArmor(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle || !iPlayer.IsCopPackGun() || !iPlayer.IsInDuty()) return false;
            iPlayer.SetCannotInteract(true);

            Chats.sendProgressBar(iPlayer, 4000);
            iPlayer.PlayAnimation(
                    (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);

            iPlayer.SetData("armorusing", true);

            await Task.Delay(4000);

            iPlayer.ResetData("armorusing");
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.SetCannotInteract(false);
            iPlayer.StopAnimation();

            int type = 30;
            if (iPlayer.VisibleArmorType != type)
                iPlayer.SaveArmorType(type);
            iPlayer.SetArmor(99, false);

            return true;
        }

        public static async Task<bool> FArmor(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle) return false;
            //if (!iPlayer.Team.IsInTeamfight()) return false;
            if (!GangwarTownModule.Instance.IsTeamInGangwar(iPlayer.Team)) return false;
            iPlayer.SetCannotInteract(true);

            Chats.sendProgressBar(iPlayer, 4000);
            iPlayer.PlayAnimation(
                (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);

            iPlayer.SetData("armorusing", true);

            await Task.Delay(4000);

            iPlayer.ResetData("armorusing");
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.SetCannotInteract(false);
            iPlayer.StopAnimation();
            if (iPlayer.VisibleArmorType != 0)
                iPlayer.SaveArmorType(0);
            iPlayer.VisibleArmorType = 0;
            iPlayer.SetArmor(120, true);

            return true;
        }
    }
}