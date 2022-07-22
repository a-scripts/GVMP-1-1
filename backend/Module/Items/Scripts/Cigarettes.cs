using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> SmokeCigarrette(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;

            Attachments.AttachmentModule.Instance.AddAttachment(iPlayer, (int)Attachments.Attachment.CIGARRETES, true);

            await Task.Delay(500);

            iPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_smoking@male@male_b@enter", "enter");
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetCannotInteract(true);

            await Task.Delay(14000);
            iPlayer.StopAnimation(AnimationLevels.User, true);
            iPlayer.SetCannotInteract(false);
            iPlayer.Player.TriggerEvent("freezePlayer", false);

            iPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_smoking@male@male_a@base", "base");

            return true;
        }
    }
}