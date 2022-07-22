using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> VehicleRepair(DbPlayer iPlayer, ItemModel ItemData)
        {

            if (iPlayer.Player.IsInVehicle) return false;

            //Items.Instance.UseItem(ItemData.id, iPlayer);
            var closestVehicle = VehicleHandler.Instance.GetClosestVehicle(iPlayer.Player.Position, 3);
            if (closestVehicle != null)
            {
             
                if(closestVehicle.SyncExtension.EngineOn || closestVehicle.entity.EngineStatus)
                {
                    iPlayer.SendNewNotification("Der Motor muss zum reparieren ausgeschaltet sein!");
                    return false;
                }

                iPlayer.PlayAnimation(
                    (int) (AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@repair", "fixing_a_ped");
                iPlayer.Player.TriggerEvent("freezePlayer", true);
                iPlayer.SetData("userCannotInterrupt", true);

                Chats.sendProgressBar(iPlayer, 20000);
                await Task.Delay(20000);

                iPlayer.ResetData("userCannotInterrupt");
                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.StopAnimation();

                if (closestVehicle.SyncExtension.EngineOn || closestVehicle.entity.EngineStatus)
                {
                    iPlayer.SendNewNotification("Der Motor muss zum reparieren ausgeschaltet sein!");
                    return false;
                }

                if (closestVehicle.entity.Position.DistanceTo(iPlayer.Player.Position) > 10) return false;

                closestVehicle.Repair();
                return true;
            }

            return false;
        }
    }
}