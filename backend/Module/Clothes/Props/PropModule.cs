using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Clothes.Props
{
    public class PropModule : SqlModule<PropModule, Prop, uint>
    {
        protected override string GetQuery()
        {
            return
                "SELECT `props`.*, IFNULL(`props_subitem`.`subcat_id`, -1) subcat_id " +
                "FROM `props` " +
                "LEFT JOIN `props_subitem` ON `props`.`id` = `props_subitem`.`prop_id` " +
                "ORDER BY `props`.`default` DESC, `props`.`slot` ASC , `props`.`variation` ASC , `props`.`texture` ASC;";
        }

        protected override void OnItemLoaded(Prop prop)
        {
            prop.Tuple = new Tuple<int, uint, int>(prop.Slot, prop.TeamId, prop.Gender);
        }

        public List<Prop> GetPropsForShop(uint shopId)
        {
            return (
                from prop in GetAll()
                where prop.Value != null && (prop.Value.StoreId == shopId || prop.Value.IsDefault)
                select prop.Value
            ).ToList();
        }

        public List<Prop> GetBySlot(int slot)
        {
            return GetAll().Values.Where<Prop>(p => p.Slot == slot).ToList();
        }

        public List<Prop> GetWardrobeBySlot(DbPlayer iPlayer, int slot)
        {
            var wardrobeClothes = new List<Prop>();

            if (iPlayer.Customization == null) return wardrobeClothes;

            foreach (var prop in GetAll().Values)
            {
                if (prop.IsDefault && prop.Slot == slot &&
                    (prop.Gender == 3 || prop.Gender == iPlayer.Customization.Gender))
                {
                    wardrobeClothes.Add(prop);
                }
            }

            foreach (var propId in iPlayer.Character.Props)
            {
                var cloth = this[propId];
                if (cloth?.Slot != slot ||
                    cloth.TeamId != (int) teams.TEAM_CIVILIAN && cloth.TeamId != iPlayer.TeamId ||
                    cloth.Gender != 3 && cloth.Gender != iPlayer.Customization.Gender) continue;
                if (!wardrobeClothes.Contains(cloth))
                {
                    wardrobeClothes.Add(cloth);
                }
            }

            return wardrobeClothes;
        }

        public List<Prop> GetTeamWarerobe(DbPlayer iPlayer, int slot)
        {
            var wardrobeClothes = new List<Prop>();
            var character = iPlayer.Character;
            if (character == null) return wardrobeClothes;
            if (iPlayer.Customization == null) return wardrobeClothes;

            foreach (var prop in GetAll().Values)
            {
                if (prop.Slot != slot) continue;
                if (!prop.IsDefault && prop.TeamId != iPlayer.TeamId) continue;
                if (prop.Gender != 3 && prop.Gender != iPlayer.Customization.Gender) continue;
                wardrobeClothes.Add(prop);
            }

            if (character.Props != null && character.Props.Count > 0)
            {
                foreach (var propId in character.Props)
                {
                    var cloth = this[propId];
                    if (cloth?.Slot != slot ||
                        cloth.TeamId != (int) teams.TEAM_CIVILIAN && cloth.TeamId != iPlayer.TeamId ||
                        cloth.Gender != 3 && cloth.Gender != iPlayer.Customization.Gender) continue;
                    if (!wardrobeClothes.Contains(cloth))
                    {
                        wardrobeClothes.Add(cloth);
                    }
                }
            }

            return wardrobeClothes;
        }
    }
}