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
using VMP_CNR.Module.Weapons.Component;

namespace VMP_CNR.Module.AsyncEventTasks
{
    public static partial class AsyncEventTasks
    {
        public static void PlayerWeaponSwitchTask(Player player, WeaponHash oldgun, WeaponHash newWeapon)
        {
            DbPlayer iPlayer = player.GetPlayer();

            if (!iPlayer.IsValid()) return;

            Modules.Instance.OnPlayerWeaponSwitch(iPlayer, oldgun, newWeapon);

            if (iPlayer.IsCuffed)
            {
                iPlayer.Player.PlayAnimation("mp_arresting", iPlayer.Player.IsInVehicle ? "sit" : "idle", 0);
            }

            if (iPlayer.IsTied)
            {
                if (iPlayer.Player.IsInVehicle) iPlayer.Player.PlayAnimation("mp_arresting", "sit", 0);
                else iPlayer.Player.PlayAnimation("anim@move_m@prisoner_cuffed_rc", "aim_low_loop", 0);
            }

            if ((iPlayer.Lic_Gun[0] <= 0 && iPlayer.Level < 3) || iPlayer.hasPerso[0] == 0)
            {
                iPlayer.RemoveWeapons();
                iPlayer.ResetAllWeaponComponents();
            }

            if (iPlayer.PlayingAnimation)
            {
                NAPI.Player.SetPlayerCurrentWeapon(player, WeaponHash.Unarmed);
            }
        }
    }
}
