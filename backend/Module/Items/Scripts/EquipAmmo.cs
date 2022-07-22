using System;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Weapons.Component;
using VMP_CNR.Module.Weapons.Data;
//Possible problem. Removed on use, but not possible to add without weapon. Readd item?
namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> EquipAmo(DbPlayer iPlayer, ItemModel ItemData, int Amount)
        {
            try
            {
                if (iPlayer.Player.IsInVehicle || !iPlayer.CanInteract()) return false;

                string[] parts = ItemData.Script.ToLower().Replace("ammo_", "").Split('_');
                string weaponstring = parts[0];

                WeaponData weaponData = WeaponDataModule.Instance.GetAll().Where(d => d.Value.Name.ToLower().Equals(weaponstring)).FirstOrDefault().Value;

                if (weaponData == null) return false;

                if (iPlayer.Weapons.Count == 0 || !iPlayer.Weapons.Exists(detail => detail.WeaponDataId == weaponData.Id))
                {
                    iPlayer.SendNewNotification(
                        "Sie müssen diese Waffe ausgerüstet haben!");
                    return false;
                }

                var l_Details = iPlayer.Weapons.FirstOrDefault(detail => detail.WeaponDataId == weaponData.Id);

                if (!Int32.TryParse(parts[1], out int magazineSize)) return false;

                if(l_Details.Ammo > magazineSize*35)
                {
                    iPlayer.SendNewNotification("Sie haben bereits die maximale Anzahl an Magazinen ausgerüstet!");
                    return false;
                }

                int addableAmmoAmount = Amount;

                int actualMags = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(l_Details.Ammo / Convert.ToInt32(parts[1]))));

                if ((actualMags + Amount) > 35)
                {
                    addableAmmoAmount = 35 - actualMags;
                }

                iPlayer.SetData("no-packgun", true);
                iPlayer.SetData("packgun-timestamp", DateTime.Now);
                iPlayer.SetCannotInteract(true);
                Chats.sendProgressBar(iPlayer, 800 * addableAmmoAmount);
                iPlayer.SetCannotInteract(false);
                iPlayer.Player.TriggerEvent("freezePlayer", true);

                iPlayer.Container.RemoveItem(ItemData, addableAmmoAmount);

                int l_AmmoToAdd = 0;
                for (int i = 0; i < addableAmmoAmount; i++)
                {
                    if (l_Details.Ammo > magazineSize * 35)
                        break;

                    iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@weapons@first_person@aim_rng@generic@pistol@minismg@str", "reload_aim", 8, true);

                    await Task.Delay(800);
                    iPlayer.StopAnimation();
                    l_AmmoToAdd += Convert.ToInt32(parts[1]);
                }

                l_Details.Ammo += l_AmmoToAdd;
                iPlayer.Player.TriggerEvent("updateWeaponAmmo", l_Details.WeaponDataId, l_Details.Ammo);
                iPlayer.SetWeaponAmmo((WeaponHash)weaponData.Hash, l_Details.Ammo);
                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.SetData("no-packgun", false);
                iPlayer.SendNewNotification("Sie haben ein Magazin fuer Ihre Waffe ausgeruestet!");
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }

            return false;
        }

        public static async Task<bool> EquipAmoCop(DbPlayer iPlayer, ItemModel ItemData, int Amount)
        {
            try
            {
                if (iPlayer.Player.IsInVehicle || !iPlayer.CanInteract()) return false;

                string[] parts = ItemData.Script.ToLower().Replace("bammo_", "").Split('_');
                string weaponstring = parts[0];

                if (!iPlayer.IsCopPackGun()) return false;

                WeaponData weaponData = WeaponDataModule.Instance.GetAll().Where(d => d.Value.Name.ToLower().Equals(weaponstring)).FirstOrDefault().Value;

                if (weaponData == null) return false;

                if (iPlayer.Weapons.Count == 0 || !iPlayer.Weapons.Exists(detail => detail.WeaponDataId == weaponData.Id) || (int)iPlayer.Player.CurrentWeapon != weaponData.Hash)
                {
                    iPlayer.SendNewNotification(
                        "Sie müssen diese Waffe ausgerüstet haben!");
                    return false;
                }

                var l_Details = iPlayer.Weapons.FirstOrDefault(detail => detail.WeaponDataId == weaponData.Id);

                if (!Int32.TryParse(parts[1], out int magazineSize)) return false;

                if (l_Details.Ammo > magazineSize * 35)
                {
                    iPlayer.SendNewNotification("Sie haben bereits die maximale Anzahl an Magazinen ausgerüstet!");
                    return false;
                }

                int addableAmmoAmount = Amount;

                int actualMags = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(l_Details.Ammo / Convert.ToInt32(parts[1]))));

                if((actualMags+Amount) > 35)
                {
                    addableAmmoAmount = 35 - actualMags;
                }

                iPlayer.SetData("no-packgun", true);
                iPlayer.SetData("packgun-timestamp", DateTime.Now);
                iPlayer.SetCannotInteract(true);
                Chats.sendProgressBar(iPlayer, 800 * addableAmmoAmount);
                iPlayer.SetCannotInteract(false);
                iPlayer.Player.TriggerEvent("freezePlayer", true);

                iPlayer.Container.RemoveItem(ItemData, addableAmmoAmount);

                int l_AmmoToAdd = 0;
                for (int i = 0; i < addableAmmoAmount; i++)
                {
                    if (l_Details.Ammo > magazineSize * 35)
                        break;

                    iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@weapons@first_person@aim_rng@generic@pistol@minismg@str", "reload_aim", 8, true);

                    await Task.Delay(800);
                    iPlayer.StopAnimation();
                    l_AmmoToAdd += Convert.ToInt32(parts[1]);
                }

                l_Details.Ammo += l_AmmoToAdd;
                iPlayer.Player.TriggerEvent("updateWeaponAmmo", l_Details.WeaponDataId, l_Details.Ammo);
                iPlayer.SetWeaponAmmo((WeaponHash)weaponData.Hash, l_Details.Ammo);
                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.SetData("no-packgun", false);
                iPlayer.SendNewNotification("Sie haben ein Magazin fuer Ihre Waffe ausgeruestet!");
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }

            return false;
        }

    }
}
