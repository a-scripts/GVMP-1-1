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
        public static async Task<bool> Fire(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;


            CampingPlace campingPlace = CampingModule.Instance.CampingPlaces.ToList().Where(cp => cp.Position.DistanceTo(iPlayer.Player.Position) < 10.0f).FirstOrDefault();
            if (campingPlace != null)
            {
                // Fire Bed
                if(iPlayer.Player.Position.DistanceTo(campingPlace.Position.Add(CampingModule.AdjustmentBed)) < 1.5f)
                {
                    if(campingPlace.FireStateBed == 0)
                    {
                        iPlayer.SetCannotInteract(true);
                        iPlayer.Player.TriggerEvent("freezePlayer", true);
                        iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base");

                        Chats.sendProgressBar(iPlayer, 5000);
                        await Task.Delay(5000);

                        iPlayer.Player.TriggerEvent("freezePlayer", false);
                        iPlayer.StopAnimation();
                        iPlayer.SetCannotInteract(false);

                        campingPlace.FireStateBed = 5;

                        campingPlace.RefreshObjectsForPlayerInRange();
                        return true;
                    }
                }

                // Fire Tent
                if (iPlayer.Player.Position.DistanceTo(campingPlace.Position.Add(CampingModule.AdjustmentTent)) < 1.5f)
                {
                    if (campingPlace.FireStateTent == 0)
                    {
                        iPlayer.SetCannotInteract(true);
                        iPlayer.Player.TriggerEvent("freezePlayer", true);
                        iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base");

                        Chats.sendProgressBar(iPlayer, 5000);
                        await Task.Delay(5000);

                        iPlayer.Player.TriggerEvent("freezePlayer", false);
                        iPlayer.StopAnimation();
                        iPlayer.SetCannotInteract(false);

                        campingPlace.FireStateTent = 5;

                        campingPlace.RefreshObjectsForPlayerInRange();
                        return true;
                    }
                }

                // Fire Table
                if (campingPlace.IsCocain)
                {
                    if (iPlayer.Player.Position.DistanceTo(campingPlace.Position.Add(CampingModule.AdjustmentTable)) < 1.5f)
                    {
                        if (campingPlace.FireStateTable == 0)
                        {
                            iPlayer.SetCannotInteract(true);
                            iPlayer.Player.TriggerEvent("freezePlayer", true);
                            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base");

                            Chats.sendProgressBar(iPlayer, 5000);
                            await Task.Delay(5000);

                            iPlayer.Player.TriggerEvent("freezePlayer", false);
                            iPlayer.StopAnimation();
                            iPlayer.SetCannotInteract(false);

                            campingPlace.FireStateTable = 5;

                            campingPlace.RefreshObjectsForPlayerInRange();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}