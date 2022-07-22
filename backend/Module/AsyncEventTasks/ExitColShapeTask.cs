using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Module.Armory;
using VMP_CNR.Module.Banks;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Vehicles.Garages;

namespace VMP_CNR.Module.AsyncEventTasks
{
    public static partial class AsyncEventTasks
    {
        public static void ExitColShapeTask(ColShape shape, Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null) return;
            if (!iPlayer.IsValid()) return;

            if (Modules.Instance.OnColShapeEvent(iPlayer, shape, ColShapeState.Exit)) return;

            if (shape.HasData("clothShopId"))
            {
                ClothModule.Instance.ResetClothes(iPlayer);
                
                if (iPlayer.HasData("clothShopId"))
                {
                    iPlayer.ResetData("clothShopId");
                }
            }

            if (shape.HasData("teamWardrobe"))
            {
                ClothModule.Instance.ResetClothes(iPlayer);
                
                if (iPlayer.HasData("teamWardrobe"))
                {
                    iPlayer.ResetData("teamWardrobe");
                }
            }

            if (shape.HasData("ammunationId"))
            {
                if (iPlayer.HasData("ammunationId"))
                {
                    iPlayer.ResetData("ammunationId");
                }
            }
            if (shape.HasData("name_change"))
            {
                if (iPlayer.HasData("name_change"))
                {
                    iPlayer.ResetData("name_change");
                }
            }

            if (shape.HasData("garageId"))
            {
                if (iPlayer.HasData("garageId"))
                {
                    iPlayer.ResetData("garageId");
                }
            }
            
            if (shape.HasData("bankId"))
            {
                if (iPlayer.HasData("bankId"))
                {
                    iPlayer.ResetData("bankId");
                }
            }

            if (shape.HasData("ArmoryId"))
            {
                if (iPlayer.HasData("ArmoryId"))
                {
                    iPlayer.ResetData("ArmoryId");
                }
            }
        }
    }
}
