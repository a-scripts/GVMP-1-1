﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using VMP_CNR.Handler;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Spawners;
using VMP_CNR.Module.Vehicles.Garages;

namespace VMP_CNR.Module.Houses
{
    public sealed class HouseModule : SqlModule<HouseModule, House, uint>
    {
        public static Vector3 ArmorPosition = new Vector3(1129.7, -3194.27, -40.3972);
        protected override string GetQuery()
        {
            return "SELECT * FROM `houses` WHERE posX != 0 AND posY != 0;";
        }

        public override Type[] RequiredModules()
        {
            return new[] { typeof(InteriorModule), typeof(GarageModule), typeof(HouseRentModule) };
        }

        public override void OnPlayerConnected(DbPlayer dbPlayer)
        {
            HouseKeyHandler.Instance.LoadHouseKeys(dbPlayer);
        }

        public override void OnPlayerDisconnected(DbPlayer dbPlayer, string reason)
        {
            if (dbPlayer.DimensionType[0] != DimensionType.House) return;
            var house = this[(uint)dbPlayer.Dimension[0]];
            house?.PlayersInHouse.Remove(dbPlayer);
        }
        
        protected override void OnItemLoaded(House house)
        {
            if (house.Disabled) return;

            if (house.ColShape == null)
            {
                house.ColShape = ColShapes.Create(house.ColShapePosition, 1.5f);
                house.ColShape.SetData("houseId", house.Id);
            }
            house.Interior = InteriorModule.Instance.Get(house.InteriorId);
        }

        public bool PlayerLockHouse(DbPlayer iPlayer)
        {

            House iHouse;
            if ((iPlayer.DimensionType[0] == DimensionType.House)
                || iPlayer.DimensionType[0] == DimensionType.Basement
                || iPlayer.DimensionType[0] == DimensionType.Labor
                || iPlayer.DimensionType[0] == DimensionType.MoneyKeller)
            {
                if (!iPlayer.HasData("inHouse")) return false;
    
                    if ((iHouse = this[iPlayer.GetData("inHouse")]) == null) return false;
                    if (iHouse.Id != iPlayer.ownHouse[0] && !iHouse.IsTenant(iPlayer) && !iPlayer.HouseKeys.Contains(iHouse.Id)) return false;
                    if (iHouse.LastBreak.AddMinutes(10) > DateTime.Now) return false;
                NAPI.Task.Run(() =>
                {
                    if (iHouse.Locked)
                    {
                        iHouse.Locked = false;
                        iPlayer.SendNewNotification("Haus aufgeschlossen!", title: "Haus", notificationType: PlayerNotification.NotificationType.SUCCESS);
                    }
                    else
                    {
                        iHouse.Locked = true;
                        iPlayer.SendNewNotification("Haus abgeschlossen!", title: "Haus", notificationType: PlayerNotification.NotificationType.ERROR);
                    }
                });
                return true;
            }

            if (iPlayer.HasData("houseId"))
            {

   
                    iHouse = this[iPlayer.GetData("houseId")];
                    if (iHouse == null) return false;
                    if (iHouse.Id != iPlayer.ownHouse[0] && !iHouse.IsTenant(iPlayer) && !iPlayer.HouseKeys.Contains(iHouse.Id)) return false;
                    if (iHouse.LastBreak.AddMinutes(10) > DateTime.Now) return false;
                NAPI.Task.Run(() =>
                {
                    if (iHouse.Locked)
                    {
                        iHouse.Locked = false;
                        iPlayer.SendNewNotification("Haus aufgeschlossen!", title: "Haus", notificationType: PlayerNotification.NotificationType.SUCCESS);
                    }
                    else
                    {
                        iHouse.Locked = true;
                        iPlayer.SendNewNotification("Haus abgeschlossen!", title: "Haus", notificationType: PlayerNotification.NotificationType.ERROR);
                    }
                });
                return true;
            }

            return false;
        }

        public void PlayerEnterHouse(DbPlayer iPlayer, House iHouse)
        {
            if (iHouse.Locked == true)
            {
                iPlayer.SendNewNotification("Bitte schließe dein Haus zunaechst auf 'L'", title: "Haus", notificationType: PlayerNotification.NotificationType.HOUSE);
                return;
            }

            if (iHouse.IsDimensionNullHouse)
            {
                iPlayer.SetDimension(0);
            }
            else
            {
                iPlayer.SetDimension(iHouse.Id);
            }

            // Go into house
            iPlayer.ResetData("houseId");
            iPlayer.DimensionType[0] = DimensionType.House;
            iPlayer.Player.SetPosition(iHouse.Interior.Position);
            iPlayer.Player.SetRotation(iHouse.Interior.Heading);
            iPlayer.UnloadInteriorIPLs(iHouse.Interior.Id);
            iHouse.PlayersInHouse.Add(iPlayer);

            iPlayer.SetData("inHouse", (uint)iHouse.Id);

            return;
        }
        
        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            if (key == Key.E && !dbPlayer.Player.IsInVehicle)
            {
                if (dbPlayer.DimensionType[0] == DimensionType.House && dbPlayer.HasData("inHouse"))
                {
                    House iHouse;
                    if ((iHouse = HouseModule.Instance.Get(dbPlayer.GetData("inHouse"))) != null)
                    {
                        if (dbPlayer.Player.Position.DistanceTo(iHouse.Interior.Position) < 2.5)
                        {
                            if (iHouse.Type == 9) return true;
                            if (iHouse.Locked == true)
                            {
                                return true;
                            }
                            NAPI.Task.Run(() =>
                            {
                                dbPlayer.DimensionType[0] = DimensionType.World;
                                dbPlayer.Player.SetPosition(iHouse.Position);
                                dbPlayer.Player.SetRotation(iHouse.Heading);
                                dbPlayer.SetDimension(0);
                                dbPlayer.LoadUnloadedInteriorIPLs(iHouse.Interior.Id);
                                dbPlayer.ResetData("inHouse");
                            });

                            try
                            {
                                iHouse.PlayersInHouse.Remove(dbPlayer);
                            }
                            catch (Exception e)
                            {
                                Logger.Crash(e);
                            }
                            return true;
                        }
                    }
                }
                else if (dbPlayer.DimensionType[0] == DimensionType.Basement ||
                    dbPlayer.DimensionType[0] == DimensionType.Labor ||
                    dbPlayer.DimensionType[0] == DimensionType.MoneyKeller)
                {
                    House xHouse = HouseModule.Instance.Get((uint)dbPlayer.Player.Dimension);
                    if (xHouse != null)
                    {
                        // Wenn Ausgang
                        if (
                            ((dbPlayer.DimensionType[0] == DimensionType.Basement || dbPlayer.DimensionType[0] == DimensionType.Labor) && dbPlayer.Player.Position.DistanceTo(new Vector3(1138.25, -3198.88, -39.6657)) <= 2.0f)
                             || (dbPlayer.DimensionType[0] == DimensionType.MoneyKeller && dbPlayer.Player.Position.DistanceTo(new Vector3(1138.25f, -3198.88f, -39.6657f)) <= 2.0))
                        {
                            if (dbPlayer.DimensionType[0] == DimensionType.MoneyKeller)
                                dbPlayer.Player.TriggerEvent("unloadblackmoneyInterior");
                            NAPI.Task.Run(() =>
                            {
                                dbPlayer.Player.SetPosition(xHouse.Position);
                                dbPlayer.SetDimension(0);
                                dbPlayer.DimensionType[0] = DimensionType.World;
                                dbPlayer.LoadUnloadedInteriorIPLs(xHouse.Interior.Id);
                                xHouse.PlayersInHouse.Remove(dbPlayer);
                                if (dbPlayer.HasData("inHouse")) dbPlayer.ResetData("inHouse");
                            });
                            return true;
                        }

                        // Schutzwesten
                        if (dbPlayer.DimensionType[0] == DimensionType.Labor)
                        {
                            if (dbPlayer.Player.Position.DistanceTo(new Vector3(1129.7, -3194.27, -40.3972)) <= 2.0f)
                            {
                                NAPI.Task.Run(() =>
                                {
                                    MenuManager.Instance.Build(PlayerMenu.LaborArmorMenu, dbPlayer).Show(dbPlayer);
                                });
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override bool OnColShapeEvent(DbPlayer dbPlayer, ColShape colShape, ColShapeState colShapeState)
        {
            if(colShape.HasData("houseId"))
            {
                if (colShapeState == ColShapeState.Exit)
                {
                    if (dbPlayer.HasData("houseId"))
                    {
                        dbPlayer.ResetData("houseId");
                    }
                    return true;
                }
                else
                {
                    string housestring = Convert.ToString(colShape.GetData<uint>("houseId"));
                    if (!UInt32.TryParse(housestring, out uint houseid))
                    {
                        return false;
                    }
                    dbPlayer.SetData("houseId", houseid);

                    var house = HouseModule.Instance.Get(houseid);
                    if (house == null) return false;

                    if (house.Type == 9)
                    {
                        dbPlayer.SendNewNotification($"Preis: {house.Price} $", title: $"({house.Id}) Immobilie", notificationType: PlayerNotification.NotificationType.HOUSE);
                    }
                    else if (house.OwnerId == 0)
                    {
                        dbPlayer.SendNewNotification($"Preis: {house.Price} $ Mieterraeume: {house.Maxrents} Lagerraum: {house.GetInventorySize()}kg", title: $"({house.Id}) Immobilie", notificationType: PlayerNotification.NotificationType.HOUSE);
                    }
                    else
                    {
                       dbPlayer.SendNewNotification($"Besitzer: {house.OwnerName} Freie Mietplätze: {house.GetFreeRents()} {(house.ShowPhoneNumber.Length > 0 ? "Tel: " + house.ShowPhoneNumber : "")}", title: $"({house.Id}) Immobilie", notificationType: (!house.Locked ? PlayerNotification.NotificationType.SUCCESS : PlayerNotification.NotificationType.ERROR));
                    }
                    return true;
                }
            }
            return false;
        }

        public House GetThisHouse(Player player, float range = 3.0f)
        {
            return GetAll().FirstOrDefault(house => house.Value.Position.DistanceTo(player.Position) <= range).Value;
        }

        public House GetByOwner(uint ownerId)
        {
            return GetAll().FirstOrDefault(house => house.Value.OwnerId == ownerId).Value;
        }

        public House GetThisHouseFromPos(Vector3 position, bool forceload = false, float range = 3.0f)
        {
            var iHouse = GetAll().FirstOrDefault(house => house.Value.Position.DistanceTo(position) <= range).Value;
            return iHouse;
        }

        protected override bool OnLoad()
        {
            MenuManager.Instance.AddBuilder(new LaborArmorMenuBuilder());
            return base.OnLoad();
        }
    }
}