using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Outfits;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Clothes.Mobile
{
    public class MobileClothModule : Module<MobileClothModule>
    {

        public async Task PlayerSwitchMaskState(DbPlayer iPlayer)
        {
            try
            {
                int choice = 1; // Maskierung

                if (iPlayer.Character == null || iPlayer.Character.Clothes == null || iPlayer.Character.ActiveClothes == null || iPlayer.Character.Clothes.Count <= 0) return;
                if (!iPlayer.CanInteract()) return;

                if (iPlayer.HasData("lastmaskestate"))
                {
                    DateTime latest = iPlayer.GetData("lastmaskestate");
                    if (latest.AddSeconds(2) > DateTime.Now) return;
                }

                iPlayer.SetData("lastmaskestate", DateTime.Now);
                if (!iPlayer.Character.Clothes.ContainsKey(choice))
                    return;

                uint clothId = iPlayer.Character.Clothes[choice];
                Cloth cloth = ClothModule.Instance[clothId];
                if (cloth == null) return;

                if (!iPlayer.Freezed && iPlayer.CanInteract() && !iPlayer.Player.IsInVehicle)
                {
                    iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "missfbi4", "takeoff_mask");
                }

                await Task.Delay(1300);
                if (iPlayer.Character.ActiveClothes.ContainsKey(choice) && iPlayer.Character.ActiveClothes[choice]) //Spieler hat das Kleidungsstück an
                {
                    iPlayer.SetClothes(1, 0, 0);
                    iPlayer.Character.ActiveClothes[choice] = false;
                }
                else //Spieler hat das Kleidungsstück nicht an
                {
                    if (iPlayer.HasData("outfitactive"))
                    {
                        int variation = OutfitsModule.Instance.GetOutfitComponentVariation(iPlayer, iPlayer.GetData("outfitactive"), 1);
                        int texture = OutfitsModule.Instance.GetOutfitComponentTexture(iPlayer, iPlayer.GetData("outfitactive"), 1);
                        iPlayer.SetClothes(1, variation, texture);
                    }
                    else iPlayer.SetClothes(1, cloth.Variation, cloth.Texture);

                    if (!iPlayer.Character.ActiveClothes.ContainsKey(choice))
                    {
                        iPlayer.Character.ActiveClothes.TryAdd(choice, true);
                    }
                    else iPlayer.Character.ActiveClothes[choice] = true;
                }

                // remove anim if still nothing occured
                if (!iPlayer.Freezed && iPlayer.CanInteract() && !iPlayer.Player.IsInVehicle)
                    iPlayer.StopAnimation();
            }
            catch(Exception e)
            {
                Logging.Logger.Crash(e);
            }
        }
    }
}
