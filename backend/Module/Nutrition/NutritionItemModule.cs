using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.NutritionPlayer
{
    public class NutritionItemModule : SqlModule<NutritionItemModule, NutritionItem, uint>
    {
        protected override string GetQuery()
        {
            return "SELECT nutrition_items.* FROM `nutrition_items`,items_gd WHERE nutrition_items.items_gd_id = items_gd.id";
        }

        public NutritionItem GetItem(uint item)
        {
            var NutritionItem = Instance.GetAll().Where(p => p.Value.Items_gd_id == item);
            return NutritionItem.FirstOrDefault().Value ?? null;
        }

        public void UseItem(DbPlayer dbPlayer, uint item)
        {
            try { 
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (!dbPlayer.CanInteract() || dbPlayer.Player.IsInVehicle) return;

            NutritionItem Item = GetItem(item);
            if (Item != null)
            {
                dbPlayer.Nutrition.Wasser += Item.Wasser;
                dbPlayer.Nutrition.Kcal += Item.Kcal;
                dbPlayer.Nutrition.Fett += Item.Fett;
                dbPlayer.Nutrition.Zucker += Item.Zucker;
                NutritionModule.Instance.PushNutritionToPlayer(dbPlayer);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
