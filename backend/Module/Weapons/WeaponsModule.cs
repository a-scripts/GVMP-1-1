using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;
using VMP_CNR.Module.Vehicles.Data;
using VMP_CNR.Module.Weapons.Data;

namespace VMP_CNR.Module.Weapons
{
    public sealed class WeaponsModule : Module<WeaponsModule>
    {
        public override void OnPlayerEnterVehicle(DbPlayer dbPlayer, Vehicle vehicle, sbyte seat)
        {
            if (IsRestrictedForDriveBy(dbPlayer.Player.CurrentWeapon))
                NAPI.Player.SetPlayerCurrentWeapon(dbPlayer.Player, WeaponHash.Unarmed);
        }

        public override void OnPlayerWeaponSwitch(DbPlayer dbPlayer, WeaponHash oldgun, WeaponHash newgun)
        {
            // WeaponDisabling Driveby
            if (dbPlayer.Player.IsInVehicle && IsRestrictedForDriveBy(newgun))
            {
                NAPI.Player.SetPlayerCurrentWeapon(dbPlayer.Player, WeaponHash.Unarmed);
            }

            if (dbPlayer.Player.Dimension != (uint)9999)
            {
                VMP_CNR.Anticheat.Anticheat.CheckForbiddenWeapons(dbPlayer);
                dbPlayer.ResyncWeaponAmmo(false);
                var l_WeaponDatas = WeaponDataModule.Instance.GetAll();

                int l_OldGun = 0;
                int l_NewGun = 0;
                if (dbPlayer.DimensionType[0] != DimensionType.Paintball)
                {
                    foreach (var l_Data in l_WeaponDatas)
                    {
                        if (l_OldGun != 0 && l_NewGun != 0)
                            break;

                        if (l_Data.Value.Hash != (int)oldgun && l_Data.Value.Hash != (int)newgun)
                            continue;

                        if (l_Data.Value.Hash == (int)oldgun)
                        {
                            if (dbPlayer.Weapons.Exists(detail => detail.WeaponDataId == l_Data.Key))
                            {
                                l_OldGun = l_Data.Value.Id;
                            }
                            else
                            {
                                if (oldgun != WeaponHash.Unarmed)
                                    dbPlayer.RemoveServerWeapon(oldgun); // Interessant

                                continue;
                            }
                        }
                        else if (l_Data.Value.Hash == (int)newgun)
                        {
                            if (!dbPlayer.Weapons.Exists(detail => detail.WeaponDataId == l_Data.Value.Id))
                            {
                                if (newgun != WeaponHash.Unarmed)
                                    dbPlayer.RemoveServerWeapon(newgun); // Interessant

                                continue;
                            }

                            l_NewGun = l_Data.Value.Id;
                            dbPlayer.Player.TriggerEvent("setCurrentWeapon", l_NewGun);
                        }
                    }
                }
            }
        }

        public bool IsRestrictedForDriveBy(WeaponHash p_Weapon)
        {
            return (p_Weapon == WeaponHash.Microsmg ||
                    p_Weapon == WeaponHash.Minismg ||
                    p_Weapon == WeaponHash.Machinepistol ||
                    p_Weapon == WeaponHash.Revolver);
        }
    }
}
