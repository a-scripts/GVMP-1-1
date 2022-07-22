using System;
using VMP_CNR.Module.Players.Db;
using Newtonsoft.Json;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Vehicles.Garages;
using VMP_CNR.Handler;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Tasks;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.JobFactions.Carsell;
using VMP_CNR.Module.Items;

namespace VMP_CNR.Module.Vehicles.Windows
{
    public class GarageWindow : Window<Func<DbPlayer, Garage, bool>>
    {
        private class ShowEvent : Event
        {
            [JsonProperty(PropertyName = "id")] private uint GarageId { get; }
            [JsonProperty(PropertyName = "name")] private string GarageName { get; }

            public ShowEvent(DbPlayer dbPlayer, Garage garage) : base(dbPlayer)
            {
                GarageId = garage.Id;
                GarageName = garage.Name;
            }
        }

        public GarageWindow() : base("Garage")
        {
        }

        public override Func<DbPlayer, Garage, bool> Show()
        {
            return (player, garage) => OnShow(new ShowEvent(player, garage));
        }

        [RemoteEvent]
        public void requestVehicleList(Player Player, uint garageId, string state)
        {
            var iPlayer = Player.GetPlayer();
            if (!iPlayer.IsValid()) return;

            if (!iPlayer.TryData("garageId", out uint playerGarageId)) return;
            if (playerGarageId != garageId) return;

            var garage = GarageModule.Instance[garageId];
            if (garage == null) return;
            Console.WriteLine("Garage-ID: " + garageId + " | " + state);
            switch (state)
            {
                
                case "takeout":
                    SynchronizedTaskManager.Instance.Add(new GarageVehiclesTask(iPlayer, garage));
                    break;
                case "takein":
                    if (garage.Id == 0) return;
                    if (garage.Type == GarageType.VehicleCollection) return;

                    var vehicles = garage.GetAvailableVehicles(iPlayer, garage.Radius);

                    var vehicleJson = JsonConvert.SerializeObject(vehicles);

                    iPlayer.Player.TriggerEvent("componentServerEvent", "Garage", "responseVehicleList", vehicleJson);
                    break;
            }
        }

        [RemoteEvent]
        public void requestVehicle(Player Player, string state, uint garageId, uint vehicleId)
        {
            var iPlayer = Player.GetPlayer();
            if (!iPlayer.IsValid()) return;

            if (!iPlayer.TryData("garageId", out uint playerGarageId)) return;
            if (playerGarageId != garageId) return;

            var garage = GarageModule.Instance[garageId];
            if (garage == null) return;

            uint currTeam = iPlayer.TeamId;

            // Wenn NSA Duty und IAA Garage ist...
            if (iPlayer.IsNSADuty && garage.Teams.Contains((uint)teams.TEAM_IAA)) currTeam = (uint)teams.TEAM_IAA;

            switch (state)
            {
                case "takeout":
                    var spawn = garage.GetFreeSpawnPosition();

                    if (spawn == null)
                    {
                        iPlayer.SendNewNotification("Kein freier Ausparkpunkt.");
                        return;
                    }

                    if (garage.IsTeamGarage())
                    {
                        if (garage.Rang > 0 && iPlayer.TeamRank < garage.Rang)
                        {
                            iPlayer.SendNewNotification("Sie haben nicht den benötigten Rang!");
                            return;
                        }

                        SynchronizedTaskManager.Instance.Add(new GaragePlayerTeamVehicleTakeOutTask(garage, vehicleId, iPlayer, spawn));
                    }
                    else
                    {
                        if (garage.Type == GarageType.VehicleCollection)
                        {
                            if (!iPlayer.TakeMoney(2500))
                            {
                                iPlayer.SendNewNotification(
                                    
                                    "Um ein Fahrzeug freizukaufen benötigst du mindestens $2500 fuer eine Kaution!");
                                return;
                            }
                            else
                            {
                                iPlayer.SendNewNotification(
                                    "Fahrzeug fuer 2500$ Freigekauft.");
                            }
                        }

                        if (garage.Type == GarageType.VehicleAdminGarage)
                        {
                            if (!iPlayer.TakeMoney(25000))
                            {
                                iPlayer.SendNewNotification(

                                    "Um ein Fahrzeug freizukaufen benötigst du mindestens $25000 fuer eine Kaution!");
                                return;
                            }
                            else
                            {
                                iPlayer.SendNewNotification(
                                    "Fahrzeug fuer 25000$ Freigekauft.");
                            }
                        }

                        if (garage.Id == 493)
                        {
                            if (!iPlayer.TakeMoney(1000))
                            {
                                iPlayer.SendNewNotification(

                                    "Dein Fahrzeug wurde zerstört um es zu reparieren benötigst du mindestens 1000$!");
                                return;
                            }
                            else
                            {
                                iPlayer.SendNewNotification(
                                    "Fahrzeug fuer 1000$ repariert.");
                            }
                        }


                        if (garage.Id == JobCarsellFactionModule.GarageTeam1 || garage.Id == JobCarsellFactionModule.GarageTeam2 || garage.Id == JobCarsellFactionModule.GarageTeam3)
                        {
                            // Check Kaufvertrag
                            if (iPlayer.Container.GetItemAmount(641) <= 0)
                            {
                                iPlayer.SendNewNotification($"Sie benötigen Ihren Kaufvertrag um das Fahrzeug zu entnehmen!");
                                return;
                            }
                            Item kaufVertrag = iPlayer.Container.GetItemById(641);
                            if(kaufVertrag == null || kaufVertrag.Data == null || !kaufVertrag.Data.ContainsKey("vehicleId") || kaufVertrag.Data["vehicleId"] != vehicleId)
                            {
                                iPlayer.SendNewNotification($"Sie benötigen Ihren Kaufvertrag um das Fahrzeug zu entnehmen!");
                                return;
                            } 
                        }

                        SynchronizedTaskManager.Instance.Add(
                            new GaragePlayerVehicleTakeOutTask(garage, vehicleId, iPlayer, spawn));
                    }

                    break;
                case "takein":
                 
                    if (garage.IsTeamGarage() && garage.Teams.Contains(currTeam))
                    {
                        var vehicle = VehicleHandler.Instance.GetByVehicleDatabaseId(vehicleId, currTeam);


                        if (vehicle == null || vehicle.teamid != currTeam) return;
                        if (vehicle.Visitors.Count != 0) return;
                        if (vehicle.entity.Position.DistanceTo(garage.Position) > garage.Radius) return;

                        if (vehicle.CreatedDate.AddSeconds(10) > DateTime.Now)
                        {
                            iPlayer.SendNewNotification("Bitte warte kurz bevor du dieses Fahrzeug erneut einparkst!");
                            return;
                        }
                        vehicle.SetTeamCarGarage(true, (int)garage.Id);
                    }
                    else
                    {
                        if (garage.HouseId > 0 && !garage.CanVehiclePutIntoHouseGarage())
                        {
                            iPlayer.SendNewNotification("Hausgarage ist voll!");
                            return;
                        }
                        if (garage.Type == GarageType.VehicleCollection) return;
                        if (garage.Type == GarageType.Import) return;
                        if (garage.Id == 493) return;
                        // Carsell Garagen NUR ausparken
                        Console.WriteLine("SEXY BEXY");
                        if (garage.Id == JobCarsellFactionModule.GarageTeam1 || garage.Id == JobCarsellFactionModule.GarageTeam2 || garage.Id == JobCarsellFactionModule.GarageTeam3) return;
                        var vehicle = VehicleHandler.Instance.GetByVehicleDatabaseId(vehicleId);
                        if (vehicle == null || vehicle.databaseId == 0) return;

                        if(vehicle.CreatedDate.AddSeconds(10) > DateTime.Now)
                        {
                            iPlayer.SendNewNotification("Bitte warte kurz bevor du dieses Fahrzeug erneut einparkst!");
                            return;
                        }

                        if (vehicle.entity.Position.DistanceTo(garage.Position) > garage.Radius) return;
                        vehicle.SetPrivateCarGarage(1, garageId);
                    }
                    break;
            }
        }
    }
}