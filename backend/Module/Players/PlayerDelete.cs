

using GTANetworkAPI;

namespace VMP_CNR.Module.Players
{
    public static class PlayerDelete
    {
        public static void DeleteEntity(this Player Player)
        {
            var iPlayer = Player.GetPlayer();
            if (iPlayer == null) return;
            var character = iPlayer.Character;
            if (character != null)
            {
                Player.RemoveEntityDataWhenExists("CustomCharacter");
                Player.RemoveEntityDataWhenExists("dataParentsMother");
                Player.RemoveEntityDataWhenExists("dataParentsFather");
                Player.RemoveEntityDataWhenExists("dataParentsSimilarity");
                Player.RemoveEntityDataWhenExists("dataParentsSkinSimilarity");
                Player.RemoveEntityDataWhenExists("dataHairColor");
                Player.RemoveEntityDataWhenExists("dataHairHighlightColor");
                Player.RemoveEntityDataWhenExists("dataBeardColor");
                Player.RemoveEntityDataWhenExists("dataEyebrowColor");
                Player.RemoveEntityDataWhenExists("dataBlushColor");
                Player.RemoveEntityDataWhenExists("dataLipstickColor");
                Player.RemoveEntityDataWhenExists("dataChestHairColor");
                Player.RemoveEntityDataWhenExists("dataEyeColor");
                Player.RemoveEntityDataWhenExists("dataFeaturesLength");
                Player.RemoveEntityDataWhenExists("dataAppearanceLength");
                if (iPlayer.Customization != null)
                {
                    for (int i = 0, length = iPlayer.Customization.Features.Length; i < length; i++)
                    {
                        Player.RemoveEntityDataWhenExists("dataFeatures-" + i);
                    }

                    for (int i = 0, length = iPlayer.Customization.Appearance.Length; i < length; i++)
                    {
                        Player.RemoveEntityDataWhenExists("dataAppearance-" + i);
                        Player.RemoveEntityDataWhenExists("dataAppearanceOpacity-" + i);
                    }
                }
            }

            Player.RemoveEntityDataWhenExists("phone_calling");
            Player.RemoveEntityDataWhenExists("phone_number");
            Player.RemoveEntityDataWhenExists("isInjured");
            Player.RemoveEntityDataWhenExists("VOICE_RANGE");
            Player.RemoveEntityDataWhenExists("death");
        }
    }
}