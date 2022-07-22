using System;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using Newtonsoft.Json;
using VMP_CNR.Handler;
using VMP_CNR.Module.Business.FuelStations;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Events.Halloween;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Node;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.RemoteEvents;
using VMP_CNR.Module.Vehicles.Windows;

namespace VMP_CNR.Module.Vehicles
{
    public class VehicleEventHandler : Script
    {
        private const int RepairkitId = 38;

        // Wenn ihr das mal in den Player einbinden wollt, müsst ihr nur [RemoteEvent] anfügen und dieses Event mit der Tür ID anpeilen
        public void ToggleDoorState(Player p_Player, Vehicle p_Vehicle, uint p_Door)
        {
            SxVehicle l_Vehicle = p_Vehicle.GetVehicle();
            if (l_Vehicle == null)
                return;

            if (!l_Vehicle.DoorStates.ContainsKey(p_Door))
                return;

            l_Vehicle.DoorStates[p_Door] = !l_Vehicle.DoorStates[p_Door];

            NAPI.Task.Run(() =>
            {
                var l_NearPlayers = NAPI.Player.GetPlayersInRadiusOfPosition(50.0f, p_Vehicle.Position);
                foreach (var l_Player in l_NearPlayers)
                {
                    l_Player.TriggerEvent("syncVehicleDoor", p_Vehicle, p_Door, l_Vehicle.DoorStates[p_Door]);
                }
            });
        }

        [RemoteEvent]
        public void VehicleSirenToggled(Player p_Player, Vehicle p_Vehicle, bool p_State)
        {
            SxVehicle l_Vehicle = p_Vehicle.GetVehicle();
            if (l_Vehicle == null)
                return;

            l_Vehicle.SirensActive = p_State;
        }

        [RemoteEvent]
        public void requestNormalSpeed(Player p_Player, Vehicle p_Vehicle)
        {
            SxVehicle l_Vehicle = p_Vehicle.GetVehicle();
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_Vehicle == null|| l_DbPlayer == null) return;
            p_Player.TriggerEvent("setNormalSpeed", p_Vehicle, l_Vehicle.Data.MaxSpeed);
        }

        [RemoteEvent]
        public void requestVehicleSyncData(Player p_Player, Vehicle p_RequestedVehicle)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null)
                return;

            SxVehicle l_SxVehicle = p_RequestedVehicle.GetVehicle();
            if (l_SxVehicle == null || !l_SxVehicle.IsValid() || l_SxVehicle.databaseId == 0)
                return;

            var l_Tuning        = l_SxVehicle.Mods;
            var l_Sirens        = p_RequestedVehicle.Siren;
            var l_DoorStates    = l_SxVehicle.DoorStates;

            try
            {
                string l_SerializedTuning = JsonConvert.SerializeObject(l_Tuning);
                string l_SerializedDoor = JsonConvert.SerializeObject(l_DoorStates);

                bool AnkerState = false;
                if (l_SxVehicle.HasData("anker") && l_SxVehicle.GetData("anker")) AnkerState = true;
                p_Player.TriggerEvent("responseVehicleSyncData", p_RequestedVehicle, JsonConvert.SerializeObject(l_Tuning), l_Sirens,
                    JsonConvert.SerializeObject(l_DoorStates), AnkerState, l_SxVehicle.Data.LiveryIndex);
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }

        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_INFORMATION(Player Player, Vehicle vehicle)
        {
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent()) return;
            var dbVehicle = vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            // Respawnstate
            dbVehicle.respawnInteractionState = true;

            var msg = "";

            //vehicle information

            // number plate
            msg += "Nummernschild: " + dbVehicle.entity.NumberPlate;
            
            // vehicle model name
            if (dbVehicle.Data.modded_car == 1)
                msg += " Modell: " + dbVehicle.Data.mod_car_name;
            else
                msg += " Modell: " + dbVehicle.Data.Model;
            
            // vehicle serial number
            if (dbVehicle.Undercover)
            {
                msg += " Seriennummer: " + dbVehicle.entity.GetData<int>("nsa_veh_id");

                if (dbVehicle.teamid == (uint)teams.TEAM_FIB && dbPlayer.TeamId == (uint)teams.TEAM_FIB && dbPlayer.TeamRank >= 11)
                {
                    dbPlayer.SendNewNotification($"Interne Nummer: {dbVehicle.databaseId.ToString()}");
                }
                else if (dbPlayer.TeamId == dbVehicle.teamid)
                {
                    msg += $" Interne Nummer: {dbVehicle.databaseId.ToString()}";
                }
            }
            else
            {
                msg += " Seriennummer: " + dbVehicle.databaseId;
            }

            if(dbVehicle.CarsellPrice > 0)
            {
                msg += " VB $" + string.Format("{0:0,0}", dbVehicle.CarsellPrice);
            }

            dbPlayer.SendNewNotification(msg, PlayerNotification.NotificationType.INFO, "KFZ", 10000);
        }
                
        //[RemoteEventPermission]
        //[RemoteEvent]
        //public void REQUEST_VEHICLE_FlATBED_LOAD(Player Player, Vehicle vehicle)
        //{
        //    return;
            /*var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent()) return;
            if (!dbPlayer.IsInDuty() || dbPlayer.TeamId != (int) teams.TEAM_DPOS) return;
            var dbVehicle = vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;
            
            var offsetFlatbed = vehicle.GetModel().GetFlatbedVehicleOffset();
            if (offsetFlatbed == null)
                return;
            
            if (offsetFlatbed == null) return;

            // Respawnstate
            dbVehicle.respawnInteractionState = true;
            
            foreach (var dposVehicle in VehicleHandler.Instance.GetAllVehicles())
            {
                if (dposVehicle == null || dposVehicle.entity == null) continue;
                Vector3 offset = new Vector3(0,0,0);
                if (dposVehicle.entity.GetModel() == VehicleHash.Flatbed && offsetFlatbed != null
                                                                         && vehicle.Position.DistanceTo(
                                                                             dposVehicle.entity.Position) <=
                                                                         12.0f)
                {
                    offset = offsetFlatbed;
                }
                else
                {
                    continue;
                }
                
                if (dposVehicle.entity.HasData("loadedVehicle")) continue;
                
                var call = new NodeCallBuilder("attachTo").AddVehicle(dposVehicle.entity).AddInt(0).AddFloat(offset.X).AddFloat(offset.Y).AddFloat(offset.Z).AddFloat(0).AddFloat(0).AddFloat(0).AddBool(true).AddBool(false).AddBool(false).AddBool(false).AddInt(0).AddBool(false).Build();
                vehicle.Call(call);

                dposVehicle.entity.SetData("loadedVehicle", vehicle);
                vehicle.SetData("isLoaded", true);
                return;
            }*/
        //}

        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_FlATBED_UNLOAD(Player Player)
        {
            return;
            /*var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent() || dbPlayer.isInjured() || !Player.IsInVehicle) return;
            if (!dbPlayer.IsInDuty() || dbPlayer.TeamId != (int) teams.TEAM_DPOS) return;
            if ((VehicleHash)Player.Vehicle.Model != VehicleHash.Flatbed &&
                (VehicleHash)Player.Vehicle.Model != VehicleHash.Wastelander) return;
            var dbVehicle = Player.Vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            if (!Player.Vehicle.HasData("loadedVehicle")) return;
            Vehicle loadedVehicle = Player.Vehicle.GetData("loadedVehicle");

            var call = new NodeCallBuilder("detach").Build();
            loadedVehicle.Call(call);

            Player.Vehicle.ResetData("loadedVehicle");
            loadedVehicle.ResetData("isLoaded");*/
        }


        [RemoteEventPermission]
        [RemoteEvent]
        public async Task REQUEST_VEHICLE_FRISK(Player player)
        {
            try { 
            var iPlayer = player.GetPlayer();
            if (!iPlayer.IsValid())
                return;

            if (iPlayer.Player.IsInVehicle) return;

            if (!iPlayer.IsACop())
                return;

            if (!iPlayer.CanAccessMethod())
                return;

            if (iPlayer.TeamRank < 2)
                return;

            ItemsModuleEvents.resetFriskInventoryFlags(iPlayer);
            ItemsModuleEvents.resetDisabledInventoryFlag(iPlayer);

            var delVeh = VehicleHandler.Instance.GetClosestVehicle(player.Position);
            if (delVeh == null || !delVeh.IsValid()) return;

            if (iPlayer.Player.Position.DistanceTo(delVeh.entity.Position) > 10f) return;

            if (!iPlayer.HasData("lastfriskveh") || iPlayer.GetData("lastfriskveh") != delVeh.databaseId)
            {
                iPlayer.Player.TriggerEvent("freezePlayer", true);
                iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base");

                Chat.Chats.sendProgressBar(iPlayer, 8000);
                await Task.Delay(8000);

                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.StopAnimation();
            }

            iPlayer.SetData("lastfriskveh", delVeh.databaseId);

            if (iPlayer.Player.Position.DistanceTo(delVeh.entity.Position) > 10f) return;

            delVeh.Container.ShowVehFriskInventory(iPlayer, delVeh.Data.Model);

            Logger.SaveToFriskVehLog(iPlayer.Id, (int)delVeh.databaseId, iPlayer.GetName());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_TOGGLE_ENGINE(Player Player)
        {
            try { 
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent() || !Player.IsInVehicle) return;
            var dbVehicle = Player.Vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            // player is not in driver seat
            if (Player.VehicleSeat != 0) return;
            if (!dbVehicle.CanInteract) return;
            if (!dbPlayer.CanControl(dbVehicle)) return;
            
            // Respawnstate
            dbVehicle.respawnInteractionState = true;

            // EMP
            if(dbVehicle.IsInAntiFlight())
            {
                Player.Vehicle.GetVehicle().SyncExtension.SetEngineStatus(false);
                dbVehicle.SyncExtension.SetEngineStatus(false);
                return;
            }
            
            if (dbVehicle.fuel == 0 && dbVehicle.SyncExtension.EngineOn == false)
            {
                dbPlayer.SendNewNotification("Dieses Fahrzeug hat kein Benzin mehr!", notificationType:PlayerNotification.NotificationType.ERROR);
                return;
            }

            if (dbVehicle.WheelClamp > 0)
            {
                dbPlayer.SendNewNotification("Dein Fahrzeug wurde mit einer Parkkralle festgesetzt und rührt sich keinen Meter mehr vom Fleck...", notificationType: PlayerNotification.NotificationType.ERROR);
                return;
            }

            if (dbVehicle.Data != null && dbVehicle.Data.MaxSpeed > 0)
            {
                Player.TriggerEvent("setNormalSpeed", dbVehicle.entity, dbVehicle.Data.MaxSpeed);
            }

            if (dbVehicle.SyncExtension.EngineOn == false)
            {
                if (HalloweenModule.isActive) return;

                dbPlayer.SendNewNotification("Motor eingeschaltet!", notificationType:PlayerNotification.NotificationType.SUCCESS);
                Player.Vehicle.GetVehicle().SyncExtension.SetEngineStatus(true);

                if (dbVehicle.entity.HasData("paintCar"))
                {
                    if (dbVehicle.entity.HasData("origin_color1") && dbVehicle.entity.HasData("origin_color2"))
                    {
                        int color1 = dbVehicle.entity.GetData<int>("origin_color1");
                        int color2 = dbVehicle.entity.GetData<int>("origin_color2");
                        dbVehicle.entity.PrimaryColor = color1;
                        dbVehicle.entity.SecondaryColor = color2;
                        dbVehicle.entity.ResetData("color1");
                        dbVehicle.entity.ResetData("color2");
                        dbVehicle.entity.ResetData("p_color1");
                        dbVehicle.entity.ResetData("p_color2");
                    }

                    dbVehicle.entity.ResetData("paintCar");
                }
            }
            else
            {
                dbPlayer.SendNewNotification("Motor ausgeschaltet!", notificationType: PlayerNotification.NotificationType.ERROR);
                Player.Vehicle.GetVehicle().SyncExtension.SetEngineStatus(false);
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_TOGGLE_INDICATORS(Player Player)
        {
            try { 
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent() || !Player.IsInVehicle || Player.VehicleSeat != 0) return;
            var dbVehicle = Player.Vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            if (!Player.Vehicle.HasSharedData("INDICATOR_0"))
            {
                Player.Vehicle.SetSharedData("INDICATOR_0", true);
            }
            else
            {
                Player.Vehicle.ResetSharedData("INDICATOR_0");
            }

            if (!Player.Vehicle.HasSharedData("INDICATOR_1"))
            {
                Player.Vehicle.SetSharedData("INDICATOR_1", true);
            }
            else
            {
                Player.Vehicle.ResetSharedData("INDICATOR_1");
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void handleVehicleLockInside(Player Player)
        {
            try { 

            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent() || !Player.IsInVehicle) return;
            var dbVehicle = Player.Vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            if (!dbVehicle.CanInteract) return;

            if (!dbPlayer.CanControl(dbVehicle)) return;
            var l_Handler = new VehicleEventHandler();
            if (Player.Vehicle.GetVehicle().SyncExtension.Locked)
            {
                // closed to open
                Player.Vehicle.GetVehicle().SyncExtension.SetLocked(false);
                dbPlayer.SendNewNotification("Fahrzeug aufgeschlossen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                if (dbVehicle.isDoorOpen[5])
                {
                    l_Handler.ToggleDoorState(Player, dbVehicle.entity, (uint) 5);
                }
            }
            else
            {
                // open to closed
                Player.Vehicle.GetVehicle().SyncExtension.SetLocked(true);
                dbPlayer.SendNewNotification("Fahrzeug zugeschlossen!", notificationType: PlayerNotification.NotificationType.ERROR);
                if (dbVehicle.isDoorOpen[5])
                {
                    l_Handler.ToggleDoorState(Player, dbVehicle.entity, (uint) 5);
                }
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_TOGGLE_LOCK(Player Player)
        {
            try { 
            handleVehicleLockInside(Player);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }



        public void handleVehicleLockOutside(Player Player, Vehicle vehicle)
        {
            try { 
            var dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessRemoteEvent()) return;
            var dbVehicle = vehicle.GetVehicle();
            if (dbVehicle == null || !dbVehicle.IsValid()) return;
            if (dbPlayer.Player.Position.DistanceTo(vehicle.Position) > 20f) return;

            if (!dbVehicle.CanInteract) return;

            // check Users rights to toogle Locked state
            if (!dbPlayer.CanControl(dbVehicle)) return;
            var l_Handler = new VehicleEventHandler();

            if (dbVehicle.SyncExtension.Locked)
            {
                // closed to open
                dbVehicle.SyncExtension.SetLocked(false);
                dbPlayer.SendNewNotification("Fahrzeug aufgeschlossen!", notificationType: PlayerNotification.NotificationType.SUCCESS);

                if (dbVehicle.isDoorOpen[5])
                {
                    l_Handler.ToggleDoorState(Player, dbVehicle.entity, (uint) 5);
                }
            }
            else
            {
                // open to closed
                dbVehicle.SyncExtension.SetLocked(true);
                dbPlayer.SendNewNotification("Fahrzeug zugeschlossen!", notificationType: PlayerNotification.NotificationType.ERROR);

                if (dbVehicle.isDoorOpen[5])
                {
                    l_Handler.ToggleDoorState(Player, dbVehicle.entity, (uint) 5);
                }
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        
        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_TOGGLE_LOCK_OUTSIDE(Player Player, Vehicle vehicle)
        {
            try { 
            handleVehicleLockOutside(Player, vehicle);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        public void handleVehicleDoorInside(Player Player, int door)
        {
            try { 
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent() || !Player.IsInVehicle) return;
            var dbVehicle = Player.Vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            if (!dbVehicle.CanInteract) return;
            // validate player opens a doors with permission
            var userseat = Player.VehicleSeat;

            // validate player can open right doors
            if (userseat != -1 && userseat != door)
            {
                return;
            }
            // trunk handling
            if (door == 5)
            {
                // Locked vehicle can only close open doors
                if (dbVehicle.SyncExtension.Locked)
                {
                    dbPlayer.SendNewNotification("Fahrzeug zugeschlossen!", notificationType: PlayerNotification.NotificationType.ERROR);
                    return;
                }

                if (dbVehicle.isDoorOpen[door])
                {
                    // trunk was opened    
                    dbPlayer.SendNewNotification("Kofferraum zugeschlossen!", notificationType: PlayerNotification.NotificationType.ERROR);
                    dbVehicle.SetData("Door_KRaum", 0);
                }
                else
                {
                    // trunk was closed
                    dbPlayer.SendNewNotification("Kofferraum aufgeschlossen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                    dbVehicle.SetData("Door_KRaum", 1);
                }
                var l_Handler = new VehicleEventHandler();
                l_Handler.ToggleDoorState(Player, dbVehicle.entity, (uint) door);
            }
            var newstate = !dbVehicle.isDoorOpen[door];

            dbVehicle.isDoorOpen[door] = newstate;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        [RemoteEventPermission]
        [RemoteEvent]
        public  void REQUEST_VEHICLE_TOGGLE_DOOR(Player Player, int door)
        {
            try { 
            handleVehicleDoorInside(Player, door);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_EJECT(Player player)
        {

            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            try
            {
                if (dbPlayer.Player.VehicleSeat != 0)
                {
                    dbPlayer.SendNewNotification(
                        "Sie muessen Fahrer des Fahrzeuges sein!");
                    return;
                }

                var sxVeh = dbPlayer.Player.Vehicle.GetVehicle();
                if (sxVeh == null || !sxVeh.IsValid() || sxVeh.GetOccupants() == null || sxVeh.GetOccupants().Count <= 0) return;

                ComponentManager.Get<EjectWindow>().Show()(dbPlayer, sxVeh);
            }
            catch(Exception e)
            {
                Logger.Crash(e);
            }
        }

        public void handleVehicleDoorOutside(Player Player, Vehicle vehicle, int door)
        {
            try { 
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent()) return;
            var dbVehicle = vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;
            if (dbPlayer.Player.Position.DistanceTo(vehicle.Position) > 20f) return;

            if (!dbVehicle.CanInteract) return;
            // bikes not supported
            if (dbVehicle.Data.ClassificationId == 2)
            {
                return;
            }

            if (dbVehicle.SyncExtension.Locked)
            {
                dbPlayer.SendNewNotification("Fahrzeug zugeschlossen!", notificationType: PlayerNotification.NotificationType.ERROR);
                return;
            }

            // trunk handling
            if (door == 5)
            {
                if (dbVehicle.isDoorOpen[door])
                {
                    // trunk was opened
                    dbVehicle.SetData("Door_KRaum", 0);
                    dbPlayer.SendNewNotification("Kofferraum zugeschlossen!", notificationType: PlayerNotification.NotificationType.ERROR);
                }
                else
                {
                    // trunk was closed
                    dbVehicle.SetData("Door_KRaum", 1);
                    dbPlayer.SendNewNotification("Kofferraum aufgeschlossen!", notificationType: PlayerNotification.NotificationType.SUCCESS);
                }
                var l_Handler = new VehicleEventHandler();
                l_Handler.ToggleDoorState(Player, dbVehicle.entity, (uint) door);
            }

            // faction vehicle
            if (dbVehicle.teamid > 0)
            {
                if (dbPlayer.TeamId != dbVehicle.teamid)
                {
                    return;
                }
            }

            bool newstate = !dbVehicle.isDoorOpen[door];

            dbVehicle.isDoorOpen[door] = newstate;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_TOGGLE_DOOR_OUTSIDE(Player Player, Vehicle vehicle, int door)
        {
            try { 
            handleVehicleDoorOutside(Player, vehicle, door);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        
        [RemoteEventPermission]
        [RemoteEvent]
        public  async void REQUEST_VEHICLE_REPAIR(Player Player, Vehicle vehicle)
        {
            try { 
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent() || Player.IsInVehicle) return;
            var dbVehicle = vehicle.GetVehicle();
            if (!dbVehicle.IsValid()) return;

            if (dbVehicle.entity.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f) return;

            uint repairKitItem = RepairkitId;

            // verify player has required item
            if (dbPlayer.Container.GetItemAmount(repairKitItem) < 1)
            {
                return;
            }

            var x = new ItemsModuleEvents();
            await x.useInventoryItem(Player, dbPlayer.Container.GetSlotOfSimilairSingleItems(repairKitItem));

            // verfiy player can interact
            if (dbPlayer.isInjured() || dbPlayer.IsCuffed)
            {
                dbPlayer.SendNewNotification(
                    "Sie koennen diese Funktion derzeit nicht benutzen.");
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [RemoteEventPermission]
        [RemoteEvent]
        public void REQUEST_VEHICLE_TOGGLE_SEATBELT(Player Player)
        {
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.CanAccessRemoteEvent() || !Player.IsInVehicle) return;

        }

        [RemoteEvent]
        public void syncSirens(Player p_Player, Vehicle p_Vehicle)
        {
            if (p_Player == null || p_Vehicle == null)
                return;

            var l_Vehicle = p_Vehicle.GetVehicle();
            if (l_Vehicle == null || !l_Vehicle.IsValid() || !l_Vehicle.Team.IsCops())
                return;
            

            l_Vehicle.SilentSiren = !l_Vehicle.SilentSiren;
            var l_SurroundingPlayers = NAPI.Player.GetPlayersInRadiusOfPlayer(50.0f, p_Player);
            foreach (var l_User in l_SurroundingPlayers)
            {
                if (l_User.Dimension == p_Player.Dimension)
                {
                    l_User.TriggerEvent("syncSirenState", p_Vehicle, l_Vehicle.SilentSiren);
                }
            }
        }
    }
}