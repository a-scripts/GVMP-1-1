using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Events.Halloween;
using VMP_CNR.Module.Players;

using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Weapons.Component;
using VMP_CNR.Module.Weapons.Data;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static bool WeaponUnpack(DbPlayer iPlayer, ItemModel ItemData, Item item)
        {
            string weaponstring = ItemData.Script.ToLower().Replace("w_", "");

            WeaponData weaponData = WeaponDataModule.Instance.GetAll().Where(d => d.Value.Name.ToLower().Equals(weaponstring)).FirstOrDefault().Value;

            if (weaponData == null) return false;

            WeaponHash weapon = (WeaponHash)weaponData.Hash;

            if (!iPlayer.CanWeaponAdded(weapon)) return false;

            if (!iPlayer.Team.CanWeaponEquipedForTeam(weapon))
            {
                iPlayer.SendNewNotification("Diese Waffe können Sie nicht ausrüsten!");
                return false;
            }

            List<uint> Components = new List<uint>();
            if(item.Data != null && item.Data.ContainsKey("components"))
            {
                Components = NAPI.Util.FromJson<List<uint>>(item.Data["components"]);
            }

            iPlayer.SendNewNotification("Sie haben Ihre Waffe ausgeruestet!");

            int defaultammo = 0;
            if (weapon == WeaponHash.Molotov || weapon == WeaponHash.Grenade ||
                weapon == WeaponHash.Flare)
            {
                defaultammo = 1;
            }

            if (weapon == WeaponHash.Snowball)
            {
                defaultammo = 10;
            }

            iPlayer.GiveWeapon(weapon, defaultammo);

            if(Components.Count > 0)
            {
                foreach(uint compId in Components)
                {
                    Weapons.Component.WeaponComponent comp = WeaponComponentModule.Instance.Get((int)compId);
                    if (comp != null) iPlayer.GiveWeaponComponent((uint)weapon, comp.Hash);
                }
            }

            return true;
        }

        public static bool WeaponUnpackCop(DbPlayer iPlayer, ItemModel ItemData, Item item)
        {
            string weaponstring = ItemData.Script.ToLower().Replace("bw_", "");

            if (!iPlayer.IsCopPackGun()) return false;

            WeaponData weaponData = WeaponDataModule.Instance.GetAll().Where(d => d.Value.Name.ToLower().Equals(weaponstring)).FirstOrDefault().Value;

            if (weaponData == null) return false;

            WeaponHash weapon = (WeaponHash)weaponData.Hash;

            if (!iPlayer.CanWeaponAdded(weapon)) return false;

            if (!iPlayer.Team.CanWeaponEquipedForTeam(weapon))
            {
                iPlayer.SendNewNotification("Diese Waffe können Sie nicht ausrüsten!");
                return false;
            }

            List<uint> Components = new List<uint>();
            if (item.Data != null && item.Data.ContainsKey("components"))
            {
                Components = NAPI.Util.FromJson<List<uint>>(item.Data["components"]);
            }

            iPlayer.SendNewNotification("Sie haben Ihre Waffe ausgeruestet!");

            int defaultammo = 0;
            if (weapon == WeaponHash.Molotov || weapon == WeaponHash.Grenade ||
                weapon == WeaponHash.Flare)
            {
                defaultammo = 1;
            }

            if (weapon == WeaponHash.Snowball)
            {
                defaultammo = 10;
            }

            iPlayer.GiveWeapon(weapon, defaultammo);

            if (Components.Count > 0)
            {
                foreach (uint compId in Components)
                {
                    Weapons.Component.WeaponComponent comp = WeaponComponentModule.Instance.Get((int)compId);
                    if (comp != null) iPlayer.GiveWeaponComponent((uint)weapon, comp.Hash);
                }
            }

            return true;
        }

        public static async Task<bool> ZerlegteWaffeUnpack(DbPlayer iPlayer, ItemModel ItemData, Item item)
        {
            string weaponstring = ItemData.Script.ToLower().Replace("zw_", "");

            if (!iPlayer.IsAGangster()) return false;

            if (weaponstring.Length <= 0 || !uint.TryParse(weaponstring, out uint WeaponItemId))
            {
                return false;
            }
            int time = 2500; // 2,5 sek
            Chats.sendProgressBar(iPlayer, time);
            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetData("userCannotInterrupt", true);

            iPlayer.PlayAnimation(
                (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base");

            await Task.Delay(time);

            iPlayer.Container.RemoveItem(ItemData, 1);
            iPlayer.Container.AddItem(WeaponItemId, 1, new Dictionary<string, dynamic>() { { "fingerprint" , iPlayer.GetName() } });

            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.SetData("userCannotInterrupt", false);
            iPlayer.StopAnimation();
            return false; // wird ja durch den Remove schon gemacht.
        }
    }
}
