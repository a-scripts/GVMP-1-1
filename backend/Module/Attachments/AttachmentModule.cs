using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMP_CNR.Handler;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Attachments
{
    public class AttachmentModule : Module<AttachmentModule>
    {
        public static float Attachmentrange = 30;

        // Handle attachment
        public void HandleAttachment(DbPlayer dbPlayer, int uid, bool remove)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (!AttachmentItemModule.Instance.Contains((uint)uid)) return;

            if (dbPlayer.Attachments.Count == 0)
            {
                if (!remove)
                {
                    dbPlayer.Attachments.TryAdd(uid, AttachmentItemModule.Instance.Get((uint)uid));
                }
            }
            else
            {
                if (remove)
                {
                    if (dbPlayer.Attachments.ContainsKey(uid))
                    {
                        dbPlayer.Attachments.Remove(uid);
                    }
                }
                else
                {
                    dbPlayer.Attachments.TryAdd(uid, AttachmentItemModule.Instance.Get((uint)uid));
                }
            }

            if (ServerFeatures.IsActive("sync-attachments"))
            {
                Players.Players.Instance.GetPlayersInRange(dbPlayer.Player.Position, Attachmentrange).TriggerEvent("setAttachments", dbPlayer.Player, SerializeAttachments(dbPlayer));
            }
            else
            {
                Players.Players.Instance.GetPlayersInRange(dbPlayer.Player.Position, Attachmentrange).TriggerEvent("setAttachments", dbPlayer.Player, NAPI.Util.ToJson(new List<AttachmentItem>()));
            }
        }

        /*
        public void HandleVehicleAttachment(SxVehicle Vehicle, int uid, bool remove)
        {
            if (Vehicle == null || !Vehicle.IsValid()) return;

            int id = 0;

            if (Vehicle.Attachments.Count == 0)
            {
                if (!remove)
                {
                    Vehicle.Attachments.Add(Vehicle.Attachments.Count + 1, uid);
                }
            }
            else
            {
                id = Vehicle.Attachments.Where(st => st.Value == uid || st.Key != 0).FirstOrDefault().Key;

                if (remove)
                {
                    Vehicle.Attachments.Remove(id);
                }
                else
                {
                    Vehicle.Attachments.Add(Vehicle.Attachments.Count + 1, uid);
                }
            }

            Players.Players.Instance.GetPlayersInRange(Vehicle.entity.Position, Attachmentrange).TriggerEvent("setAttachments", Vehicle.entity, SerializeVehicleAttachments(Vehicle));
        }*/

        public void ClearAllAttachments(DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (dbPlayer.Attachments.Count > 0)
            {
                if (dbPlayer.Attachments.ContainsKey(80) && dbPlayer.IsNSADuty) dbPlayer.Player.TriggerEvent("setTM", false);
                dbPlayer.Attachments.Clear();
                Players.Players.Instance.GetPlayersInRange(dbPlayer.Player.Position, Attachmentrange).TriggerEvent("setAttachments", dbPlayer.Player, SerializeAttachments(dbPlayer));
            }
        }

        // Add attachment
        public void AddAttachment(DbPlayer dbPlayer, int type, bool removeAllOthers = false)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (dbPlayer.Attachments.ContainsKey(type)) return; // bereits vorhanden

            if (removeAllOthers) RemoveAllAttachments(dbPlayer);

            HandleAttachment(dbPlayer, type, false);

            if (type == 80 && dbPlayer.IsNSADuty) dbPlayer.Player.TriggerEvent("setTM", true);
        }
        /*
        public void AddAttachmentVehicle(SxVehicle Vehicle, Attachment type)
        {
            HandleVehicleAttachment(Vehicle, (int)type, false);
        }

        public void RemoveVehicleAttachment(SxVehicle Vehicle, Attachment type)
        {
            HandleVehicleAttachment(Vehicle, (int)type, true);
        }
        */
        // Remove attachment
        public void RemoveAttachment(DbPlayer dbPlayer, int type)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            HandleAttachment(dbPlayer, type, true);


            if (type == 80) dbPlayer.Player.TriggerEvent("setTM", false);
        }

        public void RemoveAllAttachments(DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (dbPlayer.Attachments.Count > 0)
            {
                // nsa stuff
                if (dbPlayer.Attachments.ContainsKey(80) && dbPlayer.IsNSADuty) dbPlayer.Player.TriggerEvent("setTM", false);

                dbPlayer.Attachments.Clear();
            }

            Players.Players.Instance.GetPlayersInRange(dbPlayer.Player.Position, Attachmentrange).TriggerEvent("removeAllAttachments", dbPlayer.Player, SerializeAttachments(dbPlayer));
        }

        public void ResyncAllAttachments(DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (dbPlayer.Attachments.Count > 0)
            {
                Players.Players.Instance.GetPlayersInRange(dbPlayer.Player.Position, Attachmentrange).TriggerEvent("resyncAttachments", dbPlayer.Player);
            }
        }

        // Serialize attachments
        public string SerializeAttachments(DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return "";

            return NAPI.Util.ToJson(dbPlayer.Attachments.Values.ToList());
        }

       /* public string SerializeVehicleAttachments(SxVehicle Vehicle)
        {
            return NAPI.Util.ToJson(Vehicle.Attachments.Values.ToList());
        }*/

        public override void OnPlayerEnterVehicle(DbPlayer dbPlayer, Vehicle vehicle, sbyte seat)
        {
            RemoveAllAttachments(dbPlayer);
        }

        public override void OnPlayerExitVehicle(DbPlayer dbPlayer, Vehicle vehicle)
        {
            Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(1000);
                dbPlayer.SyncAttachmentOnlyItems();
            });
        }
    }

    public class AttachmentsSync : Script
    {
        [RemoteEvent]
        public void requestAttachmentsPlayer(Player player, Player destinationPlayer)
        {
           try { 
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (destinationPlayer == null) return;
            var destinationDbPlayer = destinationPlayer.GetPlayer();
            if (destinationDbPlayer == null || !destinationDbPlayer.IsValid() || destinationDbPlayer.Player.IsInVehicle) return;


            if (destinationDbPlayer.Attachments.Count > 0)
            {
                // send sync
                dbPlayer.Player.TriggerEvent("setAttachments", destinationPlayer, AttachmentModule.Instance.SerializeAttachments(destinationDbPlayer));
            }
            return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public enum Attachment
    {
        BOX = 1,
        BEER = 2,
        TRASH = 3,
        FISHINGROD = 4,
        HANDY = 5,
        DRILL = 6,
        CIGARRETES = 7,
        CIGAR = 8,
        JOINT = 9,
        WELDING = 10,
        GUITAR = 11,
        DRINKBOTTLE = 12,
        BONGOS = 13,
        DRINKCAN = 14,
        COMBATSHIELD = 21,
        TABLET = 23,
        KLAPPSTUHL = 27,
        MEDICBAG = 49,
        KLAPPSTUHLBLAU = 57,
    }
}