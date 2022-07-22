using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Camper
{
    public class CampingModule : SqlModule<CampingModule, CampingPlace, uint>
    {

        public static int TentObjectId = 1777271576;
        public static int BedObjectId = -1673752417;
        public static int TableObjectId = -515655246;

        public static float StreamingCampingRange = 200;

        public static Vector3 AdjustmentTent = new Vector3(0, -1.89, -0.4);
        public static Vector3 AdjustmentTable = new Vector3(2.0, -0.9, -1.0);
        public static Vector3 AdjustmentBed = new Vector3(-0.9, 2.0, -1.0);

        public static Vector3 AdjustmentRotTent = new Vector3(0, 0, -164.854f);
        public static Vector3 AdjustmentRotTable = new Vector3(0, 0, 196.854f);
        public static Vector3 AdjustmentRotBed = new Vector3(0, 0, -364.854f);

        public List<CampingPlace> CampingPlaces = new List<CampingPlace>();

        public static uint WaterBarrelItemId = 1271;
        public static uint FuelBarrelItemId = 1270;
        public static uint GrillItemId = 1284;

        public static uint CocainLeafItemId = 553;
        public static uint CocainPacketItemId = 557;

        public uint CampingSetDeluxeId = 1269;
        public uint CampingSetId = 1268;

        public int WaterDefault = 100;
        public int FuelDefault = 100;


        protected override string GetQuery()
        {
            return "SELECT * FROM `camping_places` WHERE lastused > NOW() - INTERVAL 1 DAY;";
        }

        protected override void OnLoaded()
        {
            if (CampingPlaces == null)
                CampingPlaces = new List<CampingPlace>();

            foreach(CampingPlace cp in CampingPlaces)
            {
                cp.Shape = Spawners.ColShapes.Create(cp.Position, CampingModule.StreamingCampingRange, 0);
                cp.Shape.SetData("campingPlace", cp.PlayerId);
            }

            // delete old ones...
            MySQLHandler.ExecuteAsync("DELETE FROM `camping_places` WHERE lastused < NOW() - INTERVAL 1 DAY;");
        }

        protected override void OnItemLoaded(CampingPlace loadable)
        {
            if(CampingPlaces == null)
                CampingPlaces = new List<CampingPlace>();
            CampingPlaces.Add(loadable);

            base.OnItemLoaded(loadable);
        }

        public override bool OnColShapeEvent(DbPlayer dbPlayer, ColShape colShape, ColShapeState colShapeState)
        {
            if(dbPlayer != null && dbPlayer.IsValid() && colShape.HasData("campingPlace"))
            {

                int PlayerID = colShape.GetData<int>("campingPlace");

                CampingPlace campingPlace = CampingPlaces.ToList().Where(cp => cp.PlayerId == PlayerID).FirstOrDefault();

                if (campingPlace == null) return false;

                if (colShapeState == ColShapeState.Enter)
                {
                    dbPlayer.Player.TriggerEvent("createCustomObjects", "cp_" + PlayerID, NAPI.Util.ToJson(campingPlace.GetCampingSpotObjects(dbPlayer.Player.Position.DistanceTo(campingPlace.Position))));
                }
                else
                {
                    dbPlayer.Player.TriggerEvent("removeCustomObjects", "cp_" + PlayerID);
                }
            }
            return false;
        }

        public override void OnMinuteUpdate()
        {
            foreach(CampingPlace campingPlace in CampingPlaces.ToList())
            {
                // Fire minus--
                if(campingPlace.FireStateBed > 1) 
                {
                    campingPlace.FireStateBed--;
                }
                if (campingPlace.FireStateTable > 1)
                {
                    campingPlace.FireStateTable--;
                }
                if (campingPlace.FireStateTent > 1)
                {
                    campingPlace.FireStateTent--;
                }

                // Wenn alles brennt
                if(campingPlace.FireStateTent == 1 || campingPlace.FireStateBed == 1 || campingPlace.FireStateTable == 1)
                {
                    if(campingPlace.FireStateTent <= 0 || campingPlace.FireStateBed <= 0 || ((campingPlace.FireStateTable <= 0 && campingPlace.IsCocain)))
                    {
                        // Es muss alles brennen...
                        continue;
                    }

                    // Remove camper / debug later
                    campingPlace.RemoveAllObjectsForPlayerInRange();
                    CampingPlaces.Remove(campingPlace);
                    MySQLHandler.ExecuteAsync($"DELETE FROM `camping_places` WHERE `player_id` = '{campingPlace.PlayerId}';");
                    continue;
                }
                else
                {
                    // Reset Vars
                    if (campingPlace.FireStateBed == 1) campingPlace.FireStateBed = 0;
                    if (campingPlace.FireStateTent == 1) campingPlace.FireStateTent = 0;
                    if (campingPlace.FireStateTable == 1) campingPlace.FireStateTable = 0;

                    campingPlace.RefreshObjectsForPlayerInRange();
                }

                if (!campingPlace.IsCocain) continue;
                bool cockedhere = false;

                // Cooking Action
                foreach(DbPlayer dbPlayer in campingPlace.CookingPlayers.ToList())
                {
                    if (!PlayerCooked(dbPlayer, campingPlace))
                    {
                        campingPlace.CookingPlayers.Remove(dbPlayer);
                        dbPlayer.SendNewNotification("Kokainveraebeitung abgebrochen!");
                    }
                    else cockedhere = true;
                }

                if (cockedhere)
                {
                    // Update Water&Fuel To DB
                    campingPlace.UpdateWaterAndFuel();

                    // Calc State Down
                    if (campingPlace.SmokingState > 0)
                    {
                        campingPlace.SmokingState -= 1;

                        // Remove State
                        if(campingPlace.SmokingState <= 0)
                        {
                            campingPlace.RefreshObjectsForPlayerInRange();
                            campingPlace.SmokingState = 0;
                        }
                    }
                    else
                    {
                        // Set State
                        if (new Random().Next(1, 50) == 5)
                        {
                            campingPlace.SmokingState = 5;
                            campingPlace.RefreshObjectsForPlayerInRange();
                        }
                    }
                }
            }
        }

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            if(dbPlayer != null && dbPlayer.IsValid() && key == Key.E)
            {
                CampingPlace campingPlace = CampingPlaces.ToList().Where(cp => cp.Position.DistanceTo(dbPlayer.Player.Position) < 7.0f).FirstOrDefault();
                if (campingPlace != null)
                {
                    if (dbPlayer.HasAttachmentOnlyItem())
                    {
                    
                        // First check on item 
                        Item xItem = dbPlayer.Container.GetAttachmentOnlyItem();
                        if (xItem != null)
                        {
                            if(xItem.Id == FuelBarrelItemId && campingPlace.Fuel <= 0)
                            {
                                dbPlayer.Container.RemoveItem(xItem.Id, 1);
                                dbPlayer.SyncAttachmentOnlyItems();

                                // Update Pos
                                campingPlace.FuelBarrelPosition = dbPlayer.Player.Position;
                                campingPlace.FuelBarrelPosition.Z -= 0.5f;
                                campingPlace.Fuel = FuelDefault;
                                campingPlace.SaveFuelBarrelPosition();
                                campingPlace.RefreshObjectsForPlayerInRange();
                                campingPlace.UpdateWaterAndFuel();

                                Attachments.AttachmentModule.Instance.RemoveAllAttachments(dbPlayer);
                                return true;
                            }
                            if(xItem.Id == WaterBarrelItemId && campingPlace.Water <= 0)
                            {

                                dbPlayer.Container.RemoveItem(xItem.Id, 1);

                                // Update Pos
                                campingPlace.WaterBarrelPosition = dbPlayer.Player.Position;
                                campingPlace.WaterBarrelPosition.Z -= 0.5f;
                                campingPlace.Water = WaterDefault;
                                campingPlace.SaveWaterBarrelPosition();
                                campingPlace.RefreshObjectsForPlayerInRange();
                                campingPlace.UpdateWaterAndFuel();

                                Attachments.AttachmentModule.Instance.RemoveAllAttachments(dbPlayer);
                                return true;
                            }
                            if (xItem.Id == GrillItemId && campingPlace.GrillPosition == new Vector3(0,0,0))
                            {
                                dbPlayer.Container.RemoveItem(xItem.Id, 1);
                                dbPlayer.SyncAttachmentOnlyItems();

                                // Update Pos
                                campingPlace.GrillPosition = dbPlayer.Player.Position;
                                campingPlace.GrillPosition.Z -= 1.0f;
                                campingPlace.SaveGrillPosition();
                                campingPlace.RefreshObjectsForPlayerInRange();

                                Attachments.AttachmentModule.Instance.RemoveAllAttachments(dbPlayer);
                                return true;
                            }
                        }
                    }
                    else if (dbPlayer.Player.Position.DistanceTo(campingPlace.Position.Add(CampingModule.AdjustmentTable)) < 1.5f && Math.Abs(CampingModule.AdjustmentRotTable.Z - dbPlayer.Player.Heading) < 45)
                    {
                        if (!campingPlace.IsCocain) return false;

                        if (campingPlace.CookingPlayers.Contains(dbPlayer))
                        {
                            dbPlayer.SendNewNotification("Kokainveraebeitung beendet!");
                            campingPlace.CookingPlayers.Remove(dbPlayer);
                            return true;
                        }
                        else
                        {
                            if(campingPlace.Water <= 0 || campingPlace.Fuel <= 0)
                            {
                                dbPlayer.SendNewNotification("Es fehlt Wasser oder Benzin um Kokain zu verarbeiten!");
                                return true;
                            }

                            Task.Run(async () =>
                            {
                                dbPlayer.SetCannotInteract(true);
                                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                                dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_parking_meter@male@base", "base");

                                Chats.sendProgressBar(dbPlayer, 5000);
                                await Task.Delay(5000);

                                dbPlayer.Player.TriggerEvent("freezePlayer", false);
                                dbPlayer.StopAnimation();
                                dbPlayer.SetCannotInteract(false);

                                dbPlayer.SendNewNotification("Kokainveraebeitung gestartet!");
                                campingPlace.CookingPlayers.Add(dbPlayer);
                            });
                            return true;
                        }
                    }
                }
                else if (dbPlayer.HasData("cp_building_step") && dbPlayer.HasData("cp_camppos") && dbPlayer.HasData("cp_markerpos"))
                {
                    Vector3 campPos = dbPlayer.GetData("cp_camppos");
                    // Wenn an nem Marker zum bauen
                    if(dbPlayer.Player.Position.DistanceTo(dbPlayer.GetData("cp_markerpos")) < 1.0f)
                    {
                        int marker = dbPlayer.GetData("cp_building_step");

                        if(marker == 1)
                        {
                            // 2 = X +
                            Vector3 targetPos = campPos.Add(new Vector3(5.0f, 0, 0));

                            if (Math.Abs(campPos.Z - dbPlayer.Player.Position.Z) > 1.0)
                            {
                                dbPlayer.SendNewNotification("Hier können Sie kein Camp errichten! (Boden zu uneben)");

                                dbPlayer.ResetData("cp_markerpos");
                                dbPlayer.ResetData("cp_building_step");
                                dbPlayer.ResetData("cp_camppos");
                                return true;
                            }

                            Task.Run(async () =>
                            {
                                dbPlayer.SetCannotInteract(true);
                                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                                dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@cpr@char_a@cpr_str", "cpr_kol_idle");

                                Chats.sendProgressBar(dbPlayer, 3000);
                                await Task.Delay(3000);

                                dbPlayer.Player.TriggerEvent("freezePlayer", false);
                                dbPlayer.StopAnimation();
                                dbPlayer.SetCannotInteract(false);

                                dbPlayer.SetData("cp_building_step", marker + 1);
                                dbPlayer.SetData("cp_markerpos", targetPos);
                                dbPlayer.Player.TriggerEvent("setCheckpoint", targetPos.X, targetPos.Y, targetPos.Z);
                            });
                            return true;
                        }
                        if (marker == 2)
                        {
                            // 3 = Y +
                            Vector3 targetPos = campPos.Add(new Vector3(0, 5.0f, 0));

                            if (Math.Abs(campPos.Z - dbPlayer.Player.Position.Z) > 1.0)
                            {
                                dbPlayer.SendNewNotification("Hier können Sie kein Camp errichten! (Boden zu uneben)");

                                dbPlayer.ResetData("cp_markerpos");
                                dbPlayer.ResetData("cp_building_step");
                                dbPlayer.ResetData("cp_camppos");
                                return true;
                            }

                            Task.Run(async () =>
                            {
                                dbPlayer.SetCannotInteract(true);
                                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                                dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@cpr@char_a@cpr_str", "cpr_kol_idle");

                                Chats.sendProgressBar(dbPlayer, 3000);
                                await Task.Delay(3000);

                                dbPlayer.Player.TriggerEvent("freezePlayer", false);
                                dbPlayer.StopAnimation();
                                dbPlayer.SetCannotInteract(false);

                                dbPlayer.SetData("cp_building_step", marker + 1);
                                dbPlayer.SetData("cp_markerpos", targetPos);
                                dbPlayer.Player.TriggerEvent("setCheckpoint", targetPos.X, targetPos.Y, targetPos.Z);
                            });
                            return true;
                        }
                        if (marker == 3)
                        {
                            // 4 = Y -
                            Vector3 targetPos = campPos.Add(new Vector3(0f, -5.0f, 0));

                            if (Math.Abs(campPos.Z - dbPlayer.Player.Position.Z) > 1.0)
                            {
                                dbPlayer.SendNewNotification("Hier können Sie kein Camp errichten! (Boden zu uneben)");

                                dbPlayer.ResetData("cp_markerpos");
                                dbPlayer.ResetData("cp_building_step");
                                dbPlayer.ResetData("cp_camppos");
                                return true;
                            }

                            Task.Run(async () =>
                            {
                                dbPlayer.SetCannotInteract(true);
                                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                                dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@cpr@char_a@cpr_str", "cpr_kol_idle");

                                Chats.sendProgressBar(dbPlayer, 3000);
                                await Task.Delay(3000);

                                dbPlayer.Player.TriggerEvent("freezePlayer", false);
                                dbPlayer.StopAnimation();
                                dbPlayer.SetCannotInteract(false);

                                dbPlayer.SetData("cp_building_step", marker + 1);
                                dbPlayer.SetData("cp_markerpos", targetPos);
                                dbPlayer.Player.TriggerEvent("setCheckpoint", targetPos.X, targetPos.Y, targetPos.Z);
                            });
                            return true;
                        }
                        if (marker == 4)
                        {
                            // 5 = normal
                            Vector3 targetPos = campPos;

                            if (Math.Abs(campPos.Z - dbPlayer.Player.Position.Z) > 1.0)
                            {
                                dbPlayer.SendNewNotification("Hier können Sie kein Camp errichten! (Boden zu uneben)");

                                dbPlayer.ResetData("cp_markerpos");
                                dbPlayer.ResetData("cp_building_step");
                                dbPlayer.ResetData("cp_camppos");
                                return true;
                            }
                            
                            Task.Run(async () =>
                            {
                                dbPlayer.SetCannotInteract(true);
                                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                                dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@cpr@char_a@cpr_str", "cpr_kol_idle");

                                Chats.sendProgressBar(dbPlayer, 3000);
                                await Task.Delay(3000);

                                dbPlayer.Player.TriggerEvent("freezePlayer", false);
                                dbPlayer.StopAnimation();
                                dbPlayer.SetCannotInteract(false);

                                dbPlayer.SetData("cp_building_step", marker + 1);
                                dbPlayer.SetData("cp_markerpos", campPos);
                                dbPlayer.Player.TriggerEvent("setCheckpoint", targetPos.X, targetPos.Y, targetPos.Z);
                            });
                            return true;
                        }
                        if (marker == 5)
                        {
                            // 5 = normal
                            dbPlayer.Player.TriggerEvent("clearCheckpoint");
                            dbPlayer.SendNewNotification("Zeltlager wird errichtet..");

                            dbPlayer.ResetData("cp_markerpos");
                            dbPlayer.ResetData("cp_building_step");
                            dbPlayer.ResetData("cp_camppos");

                            Task.Run(async () =>
                            {
                                if (!dbPlayer.IsValid() || !dbPlayer.CanInteract()) return;

                                // Disable Build on Island
                                if (dbPlayer.HasData("cayoPerico") || dbPlayer.HasData("cayoPerico2")) return;

                                bool cocain = false;

                                if (dbPlayer.Container.GetItemAmount(CampingSetDeluxeId) > 0)
                                {
                                    cocain = true;
                                    dbPlayer.Container.RemoveItem(CampingSetDeluxeId);
                                }
                                else if (dbPlayer.Container.GetItemAmount(CampingSetId) > 0)
                                {
                                    cocain = false;
                                    dbPlayer.Container.RemoveItem(CampingSetId);
                                }
                                else return;

                                dbPlayer.SetCannotInteract(true);
                                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                                dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@cpr@char_a@cpr_str", "cpr_kol_idle");

                                Chats.sendProgressBar(dbPlayer, 10000);
                                await Task.Delay(10000);

                                dbPlayer.Player.TriggerEvent("freezePlayer", false);
                                dbPlayer.StopAnimation();
                                dbPlayer.SetCannotInteract(false);

                                CampingPlace campingPlaceNew = new CampingPlace((int)dbPlayer.Id, dbPlayer.Player.Position, cocain);
                                if (campingPlaceNew == null) return;
                                campingPlaceNew.Create();

                                CampingPlaces.Add(campingPlaceNew);

                                await Task.Delay(1000);

                                campingPlaceNew.RefreshObjectsForPlayerInRange();

                                MySQLHandler.ExecuteAsync($"INSERT INTO camping_places (`player_id`, `pos_x`, `pos_y`, `pos_z`, `iscocain`) VALUES " +
                                    $"('{dbPlayer.Id}', '{dbPlayer.Player.Position.X.ToString().Replace(",", ".")}', '{dbPlayer.Player.Position.Y.ToString().Replace(",", ".")}', '{dbPlayer.Player.Position.Z.ToString().Replace(",", ".")}'," +
                                    $"'{(campingPlaceNew.IsCocain ? '1' : '0')}');");
                            });
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandcpinfo(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;

            if (!iPlayer.CanAccessMethod())
            {
                iPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            CampingPlace campingPlace = CampingPlaces.ToList().Where(cp => cp.Position.DistanceTo(iPlayer.Player.Position) < 7.0f).FirstOrDefault();
            if (campingPlace != null)
            {
                iPlayer.SendNewNotification($"Campingplace-Besitzer: {PlayerName.PlayerNameModule.Instance.Get((uint)campingPlace.PlayerId).Name}({campingPlace.PlayerId})");
            }
            return;
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandtestsmoke(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null) return;

            if (!iPlayer.IsValid() || !Configurations.Configuration.Instance.DevMode) return;

            CampingPlace campingPlace = CampingPlaces.ToList().Where(cp => cp.Position.DistanceTo(iPlayer.Player.Position) < 7.0f).FirstOrDefault();
            if (campingPlace != null)
            {
                campingPlace.SmokingState = 1;
                campingPlace.RefreshObjectsForPlayerInRange();
            }
            return;
        }

        public bool PlayerCooked(DbPlayer dbPlayer, CampingPlace campingPlace)
        {
            if (campingPlace == null || !campingPlace.IsCocain) return false;

            if(dbPlayer == null || !dbPlayer.IsValid() || dbPlayer.Player.Position.DistanceTo(campingPlace.Position) > 5.0f || !dbPlayer.CanInteract())
            {
                return false;
            }

            if(campingPlace.Water <= 0)
            {
                dbPlayer.SendNewNotification("Es ist kein Wasser mehr da!");
                return false;
            }

            if (campingPlace.Fuel <= 0)
            {
                dbPlayer.SendNewNotification("Es ist kein Benzin mehr da!");
                return false;
            }

            if(dbPlayer.Container.GetItemAmount(CocainLeafItemId) < 3)
            {
                dbPlayer.SendNewNotification("Du hast nicht mehr genug Kokainblätter!");
                return false;
            }

            if(!dbPlayer.Container.CanInventoryItemAdded(CocainPacketItemId))
            {
                dbPlayer.SendNewNotification("Dein Inventar reicht nicht aus um das fertige Kokain zu tragen!");
                return false;
            }

            dbPlayer.Container.RemoveItem(CocainLeafItemId, 3);
            dbPlayer.Container.AddItem(CocainPacketItemId);

            Random rnd = new Random();
            campingPlace.Fuel -= rnd.Next(1, 3);
            campingPlace.Water -= rnd.Next(1,4);

            if(campingPlace.Fuel <= 0 || campingPlace.Water <= 0)
            {
                campingPlace.RefreshObjectsForPlayerInRange();
            }
            return true;
        }
    }
}
