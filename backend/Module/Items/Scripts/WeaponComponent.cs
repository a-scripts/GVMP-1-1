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
        public static async Task<bool> EquipComponent(DbPlayer iPlayer, ItemModel ItemData)
        {
            try
            {
                if (iPlayer.Player.IsInVehicle || !iPlayer.CanInteract()) return false;

                string[] parts = ItemData.Script.ToLower().Replace("wc_", "").Split('_');

                if (!Int32.TryParse(parts[0], out int componentId)) return false;

                Weapons.Component.WeaponComponent component = WeaponComponentModule.Instance.Get(componentId);
                if (component == null) return false;

                WeaponData weaponData = WeaponDataModule.Instance.GetAll().Where(d => d.Value.Id == component.WeaponDataId).FirstOrDefault().Value;
                if (weaponData == null) return false;


                if (iPlayer.Weapons.Count == 0 || !iPlayer.Weapons.Exists(w => w.WeaponDataId == component.WeaponDataId) || (int)iPlayer.Player.CurrentWeapon != weaponData.Hash)
                {
                    iPlayer.SendNewNotification(
                        "Sie müssen diese Waffe ausgerüstet haben!");
                    return false;
                }

                if (iPlayer.HasWeaponComponent((uint)weaponData.Hash, component.Hash))
                {
                    iPlayer.SendNewNotification("Sie haben diese Modifikation bereits ausgerüstet!");
                    return false;
                }


                iPlayer.SetData("no-packgun", true);
                iPlayer.SetCannotInteract(true);
                Chats.sendProgressBar(iPlayer, 5000);

                iPlayer.SetCannotInteract(false);
                iPlayer.Player.TriggerEvent("freezePlayer", true);


                iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base", 8, true);

                iPlayer.Container.RemoveItem(ItemData, 1);

                await Task.Delay(5000);

                iPlayer.StopAnimation();
                iPlayer.GiveWeaponComponent((uint)weaponData.Hash, component.Hash);

                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.SetData("no-packgun", false);
                iPlayer.SendNewNotification($"Sie {component.Name} ausgerüstet!");
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
