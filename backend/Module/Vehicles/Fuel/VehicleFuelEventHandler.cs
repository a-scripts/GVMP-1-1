using System;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Vehicles.Fuel
{
    public class VehicleFuelEventHandler : Script
    {
        [RemoteEvent]
        public void updateVehicleDistance(Player Player, Vehicle vehicle, double distance, double fuelDistance)
        {
            if (Player == null || vehicle == null || Player.Vehicle == null) return;
            var dbVehicle = vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.Player.IsInVehicle)
                return;

            SxVehicle dbPlayerVehicle = dbPlayer.Player.Vehicle.GetVehicle();
            if (dbPlayerVehicle == null || !dbPlayerVehicle.IsValid() || (dbPlayerVehicle.databaseId != dbVehicle.databaseId))
                return;

            dbVehicle.Distance += distance;
            var consumedFuel = dbVehicle.Data.FuelConsumption * fuelDistance;
            dbVehicle.fuel -= consumedFuel;
            if (dbVehicle.fuel < 0) dbVehicle.fuel = 0;

            var newFuel = dbVehicle.fuel.ToString().Replace(",", ".");
            var newDistance = String.Format("{0:0.00}", dbVehicle.Distance).Replace(",", ".");
            var newVehicleHealth = NAPI.Vehicle.GetVehicleEngineHealth(vehicle) + NAPI.Vehicle.GetVehicleBodyHealth(vehicle);
            var newHealth = newVehicleHealth.ToString().Replace(",", ".");
            var newLockState = dbVehicle.entity.Locked?"true":"false";

            //ToDo: Workaround bis 0.4 alle Insassen haben ggf. eine kleine Differenz in der Anzeige, da occupants nicht die Insassen zurückliefert.
            Player.TriggerEvent("updateVehicleData", newFuel, newDistance, newHealth, newLockState, dbVehicle.entity.EngineStatus ? "true":"false");
            Player.TriggerEvent("setPlayerVehicleMultiplier", dbVehicle.DynamicMotorMultiplier);

            if (dbVehicle.fuel > 0.0) return;

            dbVehicle.SyncExtension.SetEngineStatus(false);
        }
    }
}