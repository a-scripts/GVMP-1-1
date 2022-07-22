using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Players.Crouch
{
    public class PlayerCrouchModule : Module<PlayerCrouchModule>
    {
        public override void OnPlayerWeaponSwitch(DbPlayer dbPlayer, WeaponHash oldgun, WeaponHash newgun)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (dbPlayer.HasData("isCrouched"))
            {
                dbPlayer.ResetData("isCrouched");

                Players.Instance.GetPlayersInRange(dbPlayer.Player.Position).TriggerEvent("changeCrouchingState", dbPlayer.Player, false);
            }
        }

        public override void OnPlayerEnterVehicle(DbPlayer dbPlayer, Vehicle vehicle, sbyte seat)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (dbPlayer.HasData("isCrouched"))
            {
                dbPlayer.ResetData("isCrouched");

                Players.Instance.GetPlayersInRange(dbPlayer.Player.Position).TriggerEvent("changeCrouchingState", dbPlayer.Player, false);
            }
        }
    }

    public class CrouchEvents : Script
    {
        [RemoteEvent]
        public void toggleCrouch(Player player)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || player.IsInVehicle || dbPlayer.isInjured())
                return;

            if (ServerFeatures.IsActive("sync-crouch"))
            {
                if (dbPlayer.HasData("isCrouched"))
                {
                    dbPlayer.ResetData("isCrouched");

                    Players.Instance.GetPlayersInRange(dbPlayer.Player.Position).TriggerEvent("changeCrouchingState", dbPlayer.Player, false);
                }
                else if(dbPlayer.Player.CurrentWeapon == WeaponHash.Unarmed)
                {
                    dbPlayer.SetData("isCrouched", true);
                    Players.Instance.GetPlayersInRange(dbPlayer.Player.Position).TriggerEvent("changeCrouchingState", dbPlayer.Player, true);
                }
            }
            else
            {
                // nur ausschalten
                if (dbPlayer.Player.HasSharedData("isCrouched"))
                {
                    dbPlayer.Player.ResetSharedData("isCrouched");

                    Players.Instance.GetPlayersInRange(dbPlayer.Player.Position).TriggerEvent("changeCrouchingState", dbPlayer.Player, false);
                }
            }
        }
    }
}
