using System;
using System.Collections.Generic;
using GTANetworkMethods;
using VMP_CNR.Handler;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;
using VMP_CNR.Module.Vehicles.RegistrationOffice;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static bool VehicleRegister(DbPlayer iPlayer, ItemModel ItemData)
        {
            //check if worker is from DPOS
            if (!iPlayer.Team.CanRegisterVehicles())
            {
                iPlayer.SendNewNotification("Dieser Vorgang ist nur fuer geschultes Personal vom DPOS und DMV!");
                return false;
            }

            if (!iPlayer.IsInDuty())
            {
                iPlayer.SendNewNotification("Sie müssen im Dienst sein um Fahrzeuge anzumelden.");
                return false;
            }

            if (iPlayer.TeamRank < 3)
            {
                iPlayer.SendNewNotification("Sie müssen mindestens Rang 3 sein um Fahrzeuge anmelden zu können.");
                return false;
            }

            bool canUseEverywhere = bool.Parse(ItemData.Script.Split("_")[1]);
            if (!canUseEverywhere && iPlayer.Player.Position.DistanceTo(new GTANetworkAPI.Vector3(386.223, -1621.51, 29.292)) > RegistrationOfficeFunctions.RegistrationRadius)
            {
                iPlayer.SendNewNotification("Sie müssen am Zulassungsplatz sein.");
                return false;
            }

            SxVehicle sxVehicle = VehicleHandler.Instance.GetClosestVehicle(iPlayer.Player.Position);
            if (sxVehicle == null)
            {
                iPlayer.SendNewNotification("Kein Fahrzeug in der Nähe!");
                return false;
            }

            if (sxVehicle.databaseId == 0) return false;

            //check if vehicle has driver
            if (sxVehicle.GetOccupants().ContainsKey(-1))
            {
                //driver is available
                DbPlayer driver = sxVehicle.GetOccupants().GetValueOrDefault(-1);
                if (driver == null || !driver.IsValid()) return false;

                //check if driver is owner
                if (sxVehicle.ownerId == driver.Id || (sxVehicle.IsTeamVehicle() && sxVehicle.teamid == driver.TeamId))
                {
                    //yees driver is owner

                    if (sxVehicle.Team.IsStaatsfraktion())
                    {
                        if (driver.TeamRank < 9)
                        {
                            iPlayer.SendNewNotification("Der Bürger muss mindestens Rang 9 seiner Organisation zu sein um das Fahrzeug anzumelden.");
                            return false;
                        }
                    }

                    Item numberplate = iPlayer.Container.GetItemById(642);
                    if (numberplate == null)
                    {
                        numberplate = iPlayer.Container.GetItemById(596);
                        if (numberplate == null)
                        {
                            iPlayer.SendNewNotification("Sie benötigen ein Kennzeichen");
                            return false;
                        }
                    }

                    String plateString = "";
                    if (numberplate.Data.ContainsKey("Plate"))
                    {
                        plateString = numberplate.Data.GetValueOrDefault("Plate");
                    }
                    else
                    {
                        plateString = RegistrationOfficeFunctions.GetRandomPlate(sxVehicle.teamid == 0 ? true : false);
                    }

                    bool successfullyRegistered = RegistrationOfficeFunctions.registerVehicle(sxVehicle, driver, iPlayer, plateString, numberplate.Data.ContainsKey("Plate") ? true : false);

                    if (successfullyRegistered)
                    {
                        if (numberplate != null)
                        {
                            iPlayer.Container.RemoveItem(numberplate.Id);
                        }
                    }
                    
                    return successfullyRegistered;
                }
            }
            iPlayer.SendNewNotification("Der Besitzer des Fahrzeugs muss auf dem Fahrersitz sein");
            return false;
        }
    }
}