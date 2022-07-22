using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using GTANetworkMethods;
using Newtonsoft.Json;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Players
{
    public static class PlayerAnimation
    {
        public static async void PlayAnimation(this DbPlayer iPlayer, AnimationScenarioType Type, string Context1,
            string Context2 = "", int lifetime = 5, bool repeat = false,
            AnimationLevels AnimationLevel = AnimationLevels.User, int specialflag = 0, bool noFreeze = false)
        {
            
                if (((int)AnimationLevel < (int)iPlayer.AnimationScenario.AnimationLevel))
                {
                    return;
                }

                iPlayer.AnimationScenario.Context1 = Context1;
                iPlayer.AnimationScenario.Context2 = Context2;
                iPlayer.AnimationScenario.Lifetime = lifetime;
                iPlayer.AnimationScenario.AnimationLevel = AnimationLevel;
                iPlayer.AnimationScenario.StartTime = DateTime.Now;
                iPlayer.AnimationScenario.Repeat = repeat;
                iPlayer.AnimationScenario.SpecialFlag = specialflag;

                if (Type == AnimationScenarioType.Animation)
                {
                    // do animation
                    iPlayer.Player.PlayAnimation(Context1, Context2, specialflag);
                }
                else
                {
                    //do Scenario
                    iPlayer.Player.PlayScenario(Context1);
                }

                iPlayer.AnimationScenario.Active = true;
            
        }

        //public static async void StopAnimation(this DbPlayer iPlayer, AnimationLevels AnimationLevel = AnimationLevels.User)
        //{
            
        //        if (!iPlayer.AnimationScenario.Active)
        //        {
        //            if ((int)iPlayer.AnimationScenario.AnimationLevel > (int)AnimationLevel)
        //            {
        //                return;
        //            }
        //        }
        //        else iPlayer.AnimationScenario.Active = false;

        //        iPlayer.Player.StopAnimation();
        //        //iPlayer.Player.FreezePosition = false;
        //        iPlayer.AnimationScenario.Active = false;
        //        iPlayer.AnimationScenario.AnimationLevel = 0;
            
        //}

        public static void StopAnimation(this DbPlayer iPlayer, AnimationLevels AnimationLevel = AnimationLevels.User, bool dontRemoveAttachments = false)
        {
            iPlayer.PlayingAnimation = false;
            iPlayer.Player.TriggerEvent("SetOwnAnimData", JsonConvert.SerializeObject(new AnimationSyncItem(iPlayer)));

            // Sync für den Fall, dass man durch eine Tür geht. Damit die Anim für andere nicht wieder startet
            List<DbPlayer> nearPlayers = Players.Instance.GetPlayersListInRange(iPlayer.Player.Position);

            foreach (var dbPlayer in nearPlayers)
            {
                if (dbPlayer == null || !dbPlayer.IsValid()) continue;
                dbPlayer.Player.TriggerEvent("SetAnimDataNear", iPlayer.Player, JsonConvert.SerializeObject(new AnimationSyncItem(iPlayer)));
            }

            if(!dontRemoveAttachments) Attachments.AttachmentModule.Instance.ClearAllAttachments(iPlayer);

            iPlayer.SyncAttachmentOnlyItems();

            if (!iPlayer.AnimationScenario.Active)
            {
                if ((int)iPlayer.AnimationScenario.AnimationLevel > (int)AnimationLevel)
                {
                    return;
                }
            }
            else
                iPlayer.AnimationScenario.Active = false;

            if (!iPlayer.Player.IsInVehicle)
                iPlayer.Player.StopAnimation();

            iPlayer.AnimationScenario.Active = false;
            iPlayer.AnimationScenario.AnimationLevel = 0;
            iPlayer.Player.TriggerEvent("VisibleWindowBug");
        }

    public static bool IsInAnimation(this DbPlayer iPlayer)
        {
            return (iPlayer.AnimationScenario.Active &&
                    iPlayer.AnimationScenario.AnimationLevel > AnimationLevels.NonRelevant);
        }
    }
}