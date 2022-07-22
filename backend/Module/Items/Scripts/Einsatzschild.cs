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
        public static async Task<bool> Combatshield(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;

            Attachments.AttachmentModule.Instance.AddAttachment(iPlayer, (int)Attachments.Attachment.COMBATSHIELD, true);

            await Task.Delay(500);

            iPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_aa_coffee@base", "base");

            return true;
        }
    }
}