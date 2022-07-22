using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Camper;
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
        public static async Task<bool> ZerlegtePistole(DbPlayer iPlayer, ItemModel ItemData)
        {

            if (iPlayer.Player.IsInVehicle) return false;
           
            string[] args = ItemData.Script.Split('_');
            if (!UInt32.TryParse(args[1], out uint newItemId)) return false;
            ItemModel newItem = ItemModelModule.Instance.Get(newItemId);
            if (newItem == null) return false;

            if(!iPlayer.Container.CanInventoryItemAdded(newItem, 1))
            {
                iPlayer.SendNewNotification("Du hast nicht genug platz im Inventar!");
                return false;
            }

            iPlayer.PlayAnimation(
                (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base");
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetData("userCannotInterrupt", true);

            Chats.sendProgressBar(iPlayer, 20000);
            await Task.Delay(20000);

            iPlayer.ResetData("userCannotInterrupt");
            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.StopAnimation();

            iPlayer.SendNewNotification($"Du hast {ItemData.Name} entpackt!");

            iPlayer.Container.AddItem(newItemId, 1);
            return true;
        }
    }
}