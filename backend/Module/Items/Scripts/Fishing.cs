using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Module.Freiberuf.Fishing;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> Fishing(DbPlayer iPlayer)
        {
            if (!iPlayer.CanInteract() || iPlayer.Player.IsInVehicle) return false;



            if(iPlayer.HasData("fishing"))
            {
                if (iPlayer.Container.GetItemAmount(FishingModule.FishingRoItemId) <= 0) return false;

                iPlayer.StopFishing();
                iPlayer.ResetData("fishing");
            } 
            else
            {
                iPlayer.StartFishing();
                iPlayer.SetData("fishing", true);
            }

            return true;
        }

    }
}
