using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using VMP_CNR.Handler;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.Business.FuelStations;
using VMP_CNR.Module.Business.Raffinery;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Business.NightClubs;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.RemoteEvents;
using VMP_CNR.Module.Storage;
using VMP_CNR.Module.Teams.Shelter;
using VMP_CNR.Module.Vehicles;
using VMP_CNR.Module.Teams.AmmoPackageOrder;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.DropItem;
using VMP_CNR.Module.Laboratories;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Armory;
using VMP_CNR.Module.Schwarzgeld;
using VMP_CNR.Module.Workstation;
using VMP_CNR.Module.Heist.Planning;
using VMP_CNR.Module.JobFactions.Mine;
using VMP_CNR.Module.FIB;
using VMP_CNR.Module.Bunker;

namespace VMP_CNR.Module.Items
{
    public sealed class ItemsModule : Module<ItemsModule>
    {

        public override void OnPlayerLoadData(DbPlayer dbPlayer, MySqlDataReader reader)
        {
            var id = reader.GetUInt32("id");
            dbPlayer.Container = ContainerManager.LoadContainer(id, ContainerTypes.PLAYER, 0);
            Console.WriteLine("ItemsModule");
            
        ///    if (TeamModule.Instance.IsGangsterTeamId(dbPlayer.TeamId))
        //    {
          //      dbPlayer.TeamFightContainer = ContainerManager.LoadContainer(id, ContainerTypes.TEAMFIGHT);
       //     }
                        
        }

        public Container findInventory(DbPlayer dbPlayer)
        {
 
                // Haus
                if (dbPlayer.DimensionType[0] == DimensionType.House && dbPlayer.HasData("inHouse"))
                {
                    House xHouse = HouseModule.Instance.Get((uint)dbPlayer.GetData("inHouse"));
                    if (xHouse != null && (dbPlayer.HouseKeys.Contains(xHouse.Id) || dbPlayer.ownHouse[0] == xHouse.Id/* || (xHouse.LastBreak.AddMinutes(10) > DateTime.Now)*/ ))
                    {
                        if (xHouse.Interior.InventoryPosition.DistanceTo(dbPlayer.Player.Position) < 3.0f) return xHouse.Container;
                    }
                }

                // Meetr Labor
                if (dbPlayer.DimensionType[0] == DimensionType.Labor)
                {
                    House xHouse = HouseModule.Instance.Get((uint)dbPlayer.Player.Dimension);
                    if (xHouse != null && (dbPlayer.HouseKeys.Contains(xHouse.Id) || dbPlayer.ownHouse[0] == xHouse.Id))
                    {
                        if (dbPlayer.Player.Position.DistanceTo(new Vector3(1118.81, -3193.38, -40.3918)) < 2.0f)
                        {
                            return xHouse.LaborContainer;
                        }
                    }
                }

                // Planningroom wardrobe
                if (dbPlayer.HasData("inPlanningRoom"))
                {
                    PlanningRoom room = PlanningModule.Instance.GetPlanningRoomByTeamId(dbPlayer.Team.Id);

                    if (room != null && dbPlayer.Player.Position.DistanceTo(PlanningModule.Instance.PlanningroomWadrobeLocation) < 2.0f)
                    {
                        return room.PlanningroomWardrobeContainer;
                    }
                }

                // Blackmoney (Geldwäsche)
                if (dbPlayer.DimensionType[0] == DimensionType.MoneyKeller)
                {
                    House xHouse = HouseModule.Instance.Get((uint)dbPlayer.Player.Dimension);
                    if (xHouse != null && (dbPlayer.HouseKeys.Contains(xHouse.Id) || dbPlayer.ownHouse[0] == xHouse.Id || (dbPlayer.TeamId == (int)teams.TEAM_FIB && dbPlayer.IsInDuty() && dbPlayer.TeamRank >= 5)))
                    {
                        if (dbPlayer.Player.Position.DistanceTo(SchwarzgeldModule.BlackMoneyInvPosition) < 1.5f)
                        {
                            return xHouse.BlackMoneyInvContainer;
                        }
                        else if (dbPlayer.Player.Position.DistanceTo(SchwarzgeldModule.BlackMoneyBatteriePoint) < 1.5f)
                        {
                            return xHouse.BlackMoneyBatterieContainer;
                        }
                        else if (dbPlayer.Player.Position.DistanceTo(SchwarzgeldModule.BlackMoneyCodeContainer) < 1.5f)
                        {
                            return xHouse.BlackMoneyCodeContainer;
                        }
                    }
                }

                TeamShelter teamShelter = TeamShelterModule.Instance.GetByInventoryPosition(dbPlayer.Player.Position, dbPlayer.Player.Dimension);
                if (teamShelter != null)
                {
                    if (teamShelter.Team.Id == (int)teams.TEAM_FIB)
                    {
                        if (dbPlayer.IsUndercover() || dbPlayer.IsNSADuty)
                        {
                            return teamShelter.Container;
                        }
                    }
                    else if (teamShelter.Team.Id == dbPlayer.TeamId) return teamShelter.Container;
                }

                // FSaves GW
                if (dbPlayer.HasData("teamShelterMenuId"))
                {
                    teamShelter = TeamShelterModule.Instance.Get(dbPlayer.GetData("teamShelterMenuId"));
                    if (teamShelter != null && teamShelter.Team.Id == dbPlayer.TeamId)
                    {
                        return dbPlayer.TeamFightContainer;
                    }
                }

                // Prison Locker
                if (dbPlayer.DimensionType[0] == DimensionType.World)
                {
                    if (dbPlayer.Player.Position.DistanceTo(Coordinates.PrisonLockerPutIn) < 2.0f ||
                        dbPlayer.Player.Position.DistanceTo(Coordinates.PrisonLockerTakeOut) < 2.0f ||
                        dbPlayer.Player.Position.DistanceTo(Coordinates.LSPDLockerPutOut) < 2.0f ||
                        dbPlayer.Player.Position.DistanceTo(Coordinates.LSPDPDLockerPutIn) < 2.0f ||
                        dbPlayer.Player.Position.DistanceTo(Coordinates.SandyPDLockerPutInOut) < 2.0f ||
                        dbPlayer.Player.Position.DistanceTo(Coordinates.PaletoPDLockerPutInOut) < 2.0f)
                    {
                        return ContainerManager.LoadContainer(dbPlayer.Id, ContainerTypes.PRISONLOCKER, 0);
                    }
                }

                // Laboratory Containers
                if (dbPlayer.DimensionType[0] == DimensionType.Methlaboratory)
                {
                    Methlaboratory methlaboratory = MethlaboratoryModule.Instance.GetAll().Values.Where(laboratory => laboratory.TeamId == dbPlayer.Player.Dimension).FirstOrDefault();
                    if (methlaboratory != null && methlaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.MethlaboratoryInvInputPosition) < 1.0f)
                        return dbPlayer.MethlaboratoryInputContainer;
                    if (methlaboratory != null && methlaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.MethlaboratoryInvOutputPosition) < 1.0f)
                        return dbPlayer.MethlaboratoryOutputContainer;
                    if (methlaboratory != null && methlaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.MethlaboratoryInvFuelPosition) < 1.0f)
                        return methlaboratory.FuelContainer;
                }

                // Laboratory Containers
                if (dbPlayer.DimensionType[0] == DimensionType.Weaponlaboratory)
                {
                    Weaponlaboratory weaponlaboratory = WeaponlaboratoryModule.Instance.GetAll().Values.Where(laboratory => laboratory.TeamId == dbPlayer.Player.Dimension).FirstOrDefault();
                    if (weaponlaboratory != null && weaponlaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.WeaponlaboratoryInvInputPosition) < 1.0f)
                        return dbPlayer.WeaponlaboratoryInputContainer;
                    if (weaponlaboratory != null && weaponlaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.WeaponlaboratoryInvOutputPosition) < 1.0f)
                        return dbPlayer.WeaponlaboratoryOutputContainer;
                    if (weaponlaboratory != null && weaponlaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.WeaponlaboratoryInvFuelPosition) < 1.0f)
                        return weaponlaboratory.FuelContainer;
                }

                // Laboratory Containers
                if (dbPlayer.DimensionType[0] == DimensionType.Cannabislaboratory)
                {
                    Cannabislaboratory cannabislaboratory = CannabislaboratoryModule.Instance.GetAll().Values.Where(laboratory => laboratory.TeamId == dbPlayer.Player.Dimension).FirstOrDefault();
                    if (cannabislaboratory != null && cannabislaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.CannabislaboratoryInvInputPosition) < 1.0f)
                        return dbPlayer.CannabislaboratoryInputContainer;
                    if (cannabislaboratory != null && cannabislaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.CannabislaboratoryInvOutputPosition) < 1.0f)
                        return dbPlayer.CannabislaboratoryOutputContainer;
                    if (cannabislaboratory != null && cannabislaboratory.TeamId == dbPlayer.TeamId && dbPlayer.Player.Position.DistanceTo(Coordinates.CannabislaboratoryInvFuelPosition) < 1.0f)
                        return cannabislaboratory.FuelContainer;
                }

                // Armory Copinv
                if (dbPlayer.DimensionType[0] == DimensionType.World)
                {
                    if (dbPlayer.HasData("ArmoryId"))
                    {
                        var ArmoryId = dbPlayer.GetData("ArmoryId");
                        Armory.Armory Armory = ArmoryModule.Instance.Get(ArmoryId);
                        if (Armory != null)
                        {
                            if (Armory.AccessableTeams.Contains(dbPlayer.Team))
                            {
                                return ContainerManager.LoadContainer(dbPlayer.Id, ContainerTypes.Copinv, 0);
                            }
                        }
                    }
                }

                // Vehicles near me
                if (!dbPlayer.Player.IsInVehicle)
                {
                    var delVeh = VehicleHandler.Instance.GetClosestVehicle(dbPlayer.Player.Position);
                    if (delVeh != null && delVeh.IsValid() && !delVeh.SyncExtension.Locked && dbPlayer.Player.Position.DistanceTo(delVeh.entity.Position) < 15.0f && dbPlayer.Player.Dimension == delVeh.entity.Dimension)
                    {
                        if (delVeh.HasData("Door_KRaum") && delVeh.GetData("Door_KRaum") == 1)
                        {
                            return delVeh.Container;
                        }
                    }
                }
                else // self in Vehicle
                {
                    var delVeh = dbPlayer.Player.Vehicle.GetVehicle();
                    if (delVeh != null && delVeh.IsValid())
                    {
                        if (!delVeh.SyncExtension.Locked)
                        {
                            // Nur "Autos" haben ein Handschuhfach
                            if (delVeh.Data != null && delVeh.Data.ClassificationId != 2 && delVeh.Data.ClassificationId != 5 && delVeh.Data.ClassificationId != 7
                                && delVeh.Data.ClassificationId != 8 && delVeh.Data.ClassificationId != 9 && delVeh.Data.ClassificationId != 10 && delVeh.Data.ClassificationId != 3)
                            {

                                // Handschuhfach nur für Fahrer & Beifahrer
                                if (dbPlayer.Player.VehicleSeat == -1 || dbPlayer.Player.VehicleSeat == 0)
                                {
                                    return delVeh.Container2;
                                }
                            }

                            // Boote, Helikopter, Flugzeuge inventar von innen wegen größe dies das
                            if (delVeh.Data != null && delVeh.Data.ClassificationId == 3 && delVeh.Data.ClassificationId == 8 && delVeh.Data.ClassificationId == 9)
                            {
                                if (delVeh.HasData("Door_KRaum") && delVeh.GetData("Door_KRaum") == 1)
                                {
                                    return delVeh.Container;
                                }
                            }
                        }
                    }
                }

                // Storages
                if (dbPlayer.HasData("storageRoomId"))
                {
                    StorageRoom storageRoom = StorageRoomModule.Instance.Get((uint)dbPlayer.GetData("storageRoomId"));
                    if (storageRoom != null && dbPlayer.Player.Position.DistanceTo(StorageModule.Instance.InventoryPosition) < 2.0f && dbPlayer.Player.Dimension == storageRoom.Id)
                    {
                        return storageRoom.Container;
                    }
                }

                // Nightclubs
                NightClub nightClub = NightClubModule.Instance.Get(dbPlayer.Player.Dimension);
                if (nightClub != null && dbPlayer.Player.Position.DistanceTo(NightClubsModule.InventoryPosition) < 2.0f && dbPlayer.Player.Dimension == nightClub.Id)
                {
                    if (dbPlayer.IsMemberOfBusiness() && dbPlayer.ActiveBusiness.BusinessBranch.NightClubId == nightClub.Id && dbPlayer.GetActiveBusinessMember().NightClub)
                    {
                        return nightClub.Container;
                    }
                }

                // Raffineries 
                Raffinery raffinery = RaffineryModule.Instance.GetThis(dbPlayer.Player.Position);
                if (raffinery != null && raffinery.IsOwnedByBusines())
                {
                    Business.Business business = raffinery.GetOwnedBusiness();

                    if (business != null && dbPlayer.IsMemberOfBusiness() && dbPlayer.ActiveBusiness != null)
                    {
                        if (business == dbPlayer.ActiveBusiness && dbPlayer.GetActiveBusinessMember().Raffinery) // Member of business and has rights
                        {
                            return raffinery.Container;
                        }
                    }
                }

                // FuelStation 
                FuelStation fuelstation = FuelStationModule.Instance.GetThis(dbPlayer.Player.Position);
                if (fuelstation != null && fuelstation.IsOwnedByBusines())
                {
                    Business.Business business = fuelstation.GetOwnedBusiness();

                    if (business != null && dbPlayer.IsMemberOfBusiness() && dbPlayer.ActiveBusiness != null)
                    {
                        if (business == dbPlayer.ActiveBusiness && dbPlayer.GetActiveBusinessMember().Fuelstation) // Member of business and has rights
                        {
                            return fuelstation.Container;
                        }
                    }
                }

                // TeamMetaData AmmoArmoryIsland
                if (dbPlayer.Team.IsGangsters() && dbPlayer.Team.TeamMetaData != null && dbPlayer.Player.Position.DistanceTo(AmmoPackageOrderModule.ContainerPosition) < 2.0)
                {
                    return dbPlayer.Team.TeamMetaData.Container;
                }

                // statics
                foreach (KeyValuePair<uint, StaticContainer> kvp in StaticContainerModule.Instance.GetAll().ToList())
                {
                    if (dbPlayer.Player.Position.DistanceTo(kvp.Value.Position) < 3.0f && !kvp.Value.Locked && dbPlayer.Dimension[0] == kvp.Value.Dimension)
                    {
                        if (kvp.Value.TeamId == 0 || kvp.Value.TeamId == dbPlayer.TeamId)
                            return kvp.Value.Container;
                    }
                }

                if (dbPlayer.HasData("container_refund"))
                {
                    return ContainerManager.LoadContainer(dbPlayer.Id, ContainerTypes.REFUND, 0, 0);

                }

                if (dbPlayer.HasData("dropItemHeap"))
                {
                    if (DropItemModule.Instance.ItemHeapDictionary.TryGetValue(dbPlayer.GetData("dropItemHeap"), out ItemHeap itemHeap))
                    {
                        itemHeap.CreateDateTime = DateTime.Now;
                        return itemHeap.Container;
                    }
                }

                if (dbPlayer.HasData("aser_lspd"))
                {
                    StaticContainer staticContainer = StaticContainerModule.Instance.Get((int)StaticContainerTypes.ASERLSPD);
                    if (staticContainer != null)
                        return staticContainer.Container;
                }

                if (dbPlayer.HasWorkstation())
                {
                    Workstation.Workstation workstation = dbPlayer.GetWorkstation();

                    if (workstation != null && workstation.LimitTeams.Contains(dbPlayer.TeamId))
                    {
                        if (dbPlayer.Player.Position.DistanceTo(workstation.EndPosition) < 2.0f && workstation.Dimension == dbPlayer.Player.Dimension) return dbPlayer.WorkstationEndContainer;
                        if (dbPlayer.Player.Position.DistanceTo(workstation.SourcePosition) < 2.0f && workstation.Dimension == dbPlayer.Player.Dimension) return dbPlayer.WorkstationSourceContainer;

                        if (workstation.FuelItemId != 0 && dbPlayer.Player.Position.DistanceTo(workstation.FuelPosition) < 2.0f && workstation.Dimension == dbPlayer.Player.Dimension) return dbPlayer.WorkstationFuelContainer;
                    }
                }

                if (dbPlayer.TeamId == (int)teams.TEAM_MINE1 || dbPlayer.TeamId == (int)teams.TEAM_MINE2)
                {
                    // Alu
                    if (dbPlayer.Player.Position.DistanceTo(JobMineFactionModule.Instance.ContainerAluPosition) < JobMineFactionModule.Instance.ContainerAliRange
                        && dbPlayer.Team.MineAluContainer != null)
                    {
                        return dbPlayer.Team.MineAluContainer;
                    }

                    // Eisen
                    if (dbPlayer.Player.Position.DistanceTo(JobMineFactionModule.Instance.ContainerIronPosition) < JobMineFactionModule.Instance.ContainerIronRange
                        && dbPlayer.Team.MineIronContainer != null)
                    {
                        return dbPlayer.Team.MineIronContainer;
                    }

                    // Zink
                    if (dbPlayer.Player.Position.DistanceTo(JobMineFactionModule.Instance.ContainerZinkPositon) < JobMineFactionModule.Instance.ContainerZinkRange
                        && dbPlayer.Team.MineZinkContainer != null)
                    {
                        return dbPlayer.Team.MineZinkContainer;
                    }

                    // Bronce
                    if (dbPlayer.Player.Position.DistanceTo(JobMineFactionModule.Instance.ContainerBroncePositon) < JobMineFactionModule.Instance.ContainerBronceRange
                        && dbPlayer.Team.MineBronceContainer != null)
                    {
                        return dbPlayer.Team.MineBronceContainer;
                    }

                    // Schmelzofen
                    if (dbPlayer.Player.Position.DistanceTo(JobMineFactionModule.Instance.ContainerSchmelzofen) <= 2
                        && dbPlayer.Team.MineContainerSchmelze != null)
                    {
                        return dbPlayer.Team.MineContainerSchmelze;
                    }

                    // Schmelzcoal
                    if (dbPlayer.Player.Position.DistanceTo(JobMineFactionModule.Instance.ContainerSchmelzcoal) <= 2
                        && dbPlayer.Team.MineContainerSchmelzCoal != null)
                    {
                        return dbPlayer.Team.MineContainerSchmelzCoal;
                    }

                    // Storagecontainer
                    if (dbPlayer.Player.Position.DistanceTo(JobMineFactionModule.Instance.MineStoragePosition) <= 2
                        && dbPlayer.Team.MineContainerStorage != null)
                    {
                        return dbPlayer.Team.MineContainerStorage;
                    }
                }

            return null;

        }

    }
}
