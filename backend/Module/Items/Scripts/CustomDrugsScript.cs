using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Buffs;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> CustomDrugWeed(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle || !iPlayer.CanInteract()) return false;

            if (iPlayer.Buffs.LastDrugId == ItemData.Id)
            {
                if (iPlayer.Buffs.DrugBuff >= 120)
                {
                    iPlayer.SendNewNotification("Mehr solltest du davon echt nicht mehr nutzen...");
                    return false;
                }
            }
            Attachments.AttachmentModule.Instance.AddAttachment(iPlayer, (int)Attachments.Attachment.JOINT, true);

            Chats.sendProgressBar(iPlayer, 14000);

            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking@male@male_b@enter", "enter");
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetCannotInteract(true);

            await Task.Delay(14000);

            if (iPlayer.IsCuffed || iPlayer.IsTied || iPlayer.isInjured()) return false;

            iPlayer.SetCannotInteract(false);
            iPlayer.Player.TriggerEvent("freezePlayer", false);

            iPlayer.StopAnimation(AnimationLevels.User, true);

            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_smoking@male@male_b@base", "base");


            if(iPlayer.Buffs.LastDrugId == ItemData.Id)
            {
                if(iPlayer.Buffs.DrugBuildUsed < 10)
                {
                    iPlayer.Buffs.DrugBuildUsed++;
                }
                else
                {
                    iPlayer.Buffs.DrugBuff += 15;
                    CustomDrugModule.Instance.SetCustomDrugEffect(iPlayer);
                }
            }
            else
            {
                iPlayer.Buffs.LastDrugId = ItemData.Id;
                iPlayer.Buffs.DrugBuildUsed = 1;
                iPlayer.Buffs.DrugBuff = 0;
            }

            iPlayer.SaveBuffs();
            return true;
        }
        public static async Task<bool> CustomDrugMeth (DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle) return false;

            if (iPlayer.Buffs.LastDrugId == ItemData.Id)
            {
                if (iPlayer.Buffs.DrugBuff >= 120)
                {
                    iPlayer.SendNewNotification("Mehr solltest du davon echt nicht mehr nutzen...");
                    return false;
                }
            }

            Chats.sendProgressBar(iPlayer, 5000);

            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mp_suicide", "pill");
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetData("userCannotInterrupt", true);

            await Task.Delay(5000);
            iPlayer.ResetData("userCannotInterrupt");

            if (iPlayer.IsCuffed || iPlayer.IsTied || iPlayer.isInjured()) return false;

            iPlayer.Player.TriggerEvent("freezePlayer", false);

            iPlayer.StopAnimation();

            if (iPlayer.Buffs.LastDrugId == ItemData.Id)
            {
                if (iPlayer.Buffs.DrugBuildUsed < 5)
                {
                    iPlayer.Buffs.DrugBuildUsed++;
                }
                else
                {
                    iPlayer.Buffs.DrugBuff += 30;
                    CustomDrugModule.Instance.SetCustomDrugEffect(iPlayer);
                }
            }
            else
            {
                iPlayer.Buffs.LastDrugId = ItemData.Id;
                iPlayer.Buffs.DrugBuildUsed = 1;
                iPlayer.Buffs.DrugBuff = 0;
            }

            iPlayer.SaveBuffs();
            return true;
        }

        public static async Task<bool> CustomDrugJeff(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle) return false;

            Chats.sendProgressBar(iPlayer, 5000);

            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mp_suicide", "pill");
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetData("userCannotInterrupt", true);

            await Task.Delay(5000);
            iPlayer.ResetData("userCannotInterrupt");

            if (iPlayer.IsCuffed || iPlayer.IsTied || iPlayer.isInjured()) return false;

            iPlayer.Player.TriggerEvent("freezePlayer", false);

            iPlayer.StopAnimation();

            //CustomDrugModule.Instance.SetTrip(iPlayer, "s_m_y_clown_01", "clown");
            
            return true;
        }


        public static async Task<bool> CustomDrugTeflon(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.Player.IsInVehicle) return false;

            Chats.sendProgressBar(iPlayer, 5000);

            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["joint_start"].Split()[0], Main.AnimationList["joint_start"].Split()[1]);
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetData("userCannotInterrupt", true);

            await Task.Delay(4000);
            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["joint_end"].Split()[0], Main.AnimationList["joint_end"].Split()[1]);
            await Task.Delay(1000);
            iPlayer.ResetData("userCannotInterrupt");

            if (iPlayer.IsCuffed || iPlayer.IsTied || iPlayer.isInjured()) return false;

            iPlayer.Player.TriggerEvent("freezePlayer", false);

            iPlayer.StopAnimation();

            //CustomDrugModule.Instance.SetTrip(iPlayer, "u_m_y_staggrm_01", "gay");

            return true;
        }
    }
}