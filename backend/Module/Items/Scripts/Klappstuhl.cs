using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> Klappstuhl(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;

            Attachments.AttachmentModule.Instance.AddAttachment(iPlayer, (int)Attachments.Attachment.KLAPPSTUHL, true);

            await Task.Delay(500);

            iPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop), "switch@michael@sitting", "idle");
          
            return true;
        }

        public static async Task<bool> Klappstuhlb(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;

            Attachments.AttachmentModule.Instance.AddAttachment(iPlayer, (int)Attachments.Attachment.KLAPPSTUHLBLAU, true);

            await Task.Delay(500);

            iPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop), "switch@michael@sitting", "idle");

            return true;
        }
    }
}
