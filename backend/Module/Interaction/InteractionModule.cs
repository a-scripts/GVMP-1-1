using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Handler;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.NutritionPlayer;
using VMP_CNR.Module.Outfits;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.RemoteEvents;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Interaction
{
    public class InteractionModule : SqlModule<InteractionModule, InteractionItem, uint>
    {
        protected override string GetQuery()
        {
            return "SELECT * FROM `interactions`;";
        }

        public Dictionary<InteractionItem, DbPlayer> InteractionUsed = new Dictionary<InteractionItem, DbPlayer>();

        protected override void OnItemLoaded(InteractionItem u)
        {
            if (InteractionUsed == null)
            {
                InteractionUsed = new Dictionary<InteractionItem, DbPlayer>();
            }

            InteractionUsed.Add(u, null);
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandsaveint(Player player, string commandParams)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null) return;

            if (!Configurations.Configuration.Instance.DevMode) return;

            string[] cmd = commandParams.Split(' ');

            if(cmd.Length < 2) 
            {
                iPlayer.SendNewNotification($"/saveint groupID Comment");
                return;
            }

            string comment = String.Join(' ', cmd);

            if(!Int32.TryParse(cmd[0], out int group))
            {
                return;
            }

            string x = player.Position.X.ToString().Replace(",", ".");
            string y = player.Position.Y.ToString().Replace(",", ".");
            string z = player.Position.Z.ToString().Replace(",", ".");
            string heading = player.Rotation.Z.ToString().Replace(",", ".");


            MySQLHandler.ExecuteAsync($"INSERT INTO interactions (`pos_x`, `pos_y`, `pos_z`, `heading`, anim_group, comment) VALUES ('{x}', '{y}', '{z}', '{heading}', '{group}', '{comment}');");
            iPlayer.SendNewNotification($"Interaciton saved, {commandParams}");

            return;
        }
        //sex module imagine im rp geschlechtsverkehr haben hahaha cringe
        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandsex(Player player, string commandParams)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid() || !iPlayer.CanInteract() || !iPlayer.Player.IsInVehicle) return;

            DbPlayer target = Players.Players.Instance.FindPlayer(commandParams);

            if (target == null || !target.IsValid() || !target.CanInteract() || !target.Player.IsInVehicle) return;

            SxVehicle sxVehicle = iPlayer.Player.Vehicle.GetVehicle();
            if (sxVehicle == null || !sxVehicle.IsValid()) return;

            SxVehicle sxTargetVehicle = target.Player.Vehicle.GetVehicle();
            if (sxTargetVehicle == null || !sxTargetVehicle.IsValid()) return;

            if(sxVehicle != sxTargetVehicle)
            {
                iPlayer.SendNewNotification("Sie müssen im selben Fahrzeug sein!");
                return;
            }

            if (iPlayer.Player.VehicleSeat != 0 && iPlayer.Player.VehicleSeat == 2)
            {
                iPlayer.SendNewNotification("Falscher Sitz");
                return;
            }

            if (iPlayer.Player.VehicleSeat == 0 && target.Player.VehicleSeat != -1)
            {
                iPlayer.SendNewNotification("Sie müssen nebeneinander sitzen!");
                return;
            }

            if (iPlayer.Player.VehicleSeat == 2 && target.Player.VehicleSeat != 1)
            {
                iPlayer.SendNewNotification("Sie müssen nebeneinander sitzen!");
                return;
            }

            target.SetData("sexAsk", iPlayer.Id);
            iPlayer.SetData("sexAskQ", target.Id);
            target.SendNewNotification("Sie haben eine Anfrage bekommen! (/acceptsex zum annehmen)");
            iPlayer.SendNewNotification("Anfrage wurde gestellt!");
            return;
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandacceptsex(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid() || !iPlayer.CanInteract() || !iPlayer.Player.IsInVehicle) return;

            if (!iPlayer.HasData("sexAsk")) return;

            DbPlayer target = Players.Players.Instance.FindPlayerById(iPlayer.GetData("sexAsk"));

            if (target == null || !target.IsValid() || !target.CanInteract() || !target.Player.IsInVehicle) return;

            if(!target.HasData("sexAskQ"))
            {
                iPlayer.SendNewNotification("Es wurde keine Anfrage gestellt!");
                return;
            }

            SxVehicle sxVehicle = iPlayer.Player.Vehicle.GetVehicle();
            if (sxVehicle == null || !sxVehicle.IsValid()) return;

            SxVehicle sxTargetVehicle = target.Player.Vehicle.GetVehicle();
            if (sxTargetVehicle == null || !sxTargetVehicle.IsValid()) return;

            if (sxVehicle != sxTargetVehicle)
            {
                iPlayer.SendNewNotification("Sie müssen im selben Fahrzeug sein!");
                return;
            }

            if(iPlayer.Player.VehicleSeat != -1 && iPlayer.Player.VehicleSeat == 1)
            {
                iPlayer.SendNewNotification("Sie müssen nebeneinander sitzen!");
                return;
            }

            if (iPlayer.Player.VehicleSeat == -1 && target.Player.VehicleSeat != 0)
            {
                iPlayer.SendNewNotification("Sie müssen nebeneinander sitzen!");
                return;
            }

            if (iPlayer.Player.VehicleSeat == 1 && target.Player.VehicleSeat != 2)
            {
                iPlayer.SendNewNotification("Sie müssen nebeneinander sitzen!");
                return;
            }

            iPlayer.Player.TriggerEvent("freezePlayer", true);
            iPlayer.SetData("userCannotInterrupt", true);
            target.Player.TriggerEvent("freezePlayer", true);
            target.SetData("userCannotInterrupt", true);

            iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@prostitutes@sexlow_veh", "low_car_sex_loop_player");
            target.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@prostitutes@sexlow_veh", "low_car_sex_loop_female");

            iPlayer.SendNewNotification("Gestartet...... (/stopsex zum beenden)");
            target.SendNewNotification("Gestartet...... (/stopsex zum beenden)");

            iPlayer.ResetData("sexAsk");
            target.ResetData("sexAskQ");

            iPlayer.SetData("inSex", target.Id);
            target.SetData("inSex", iPlayer.Id);
            return;
        }


        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandstopsex(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid() || !iPlayer.Player.IsInVehicle) return;

            if (!iPlayer.HasData("inSex")) return;

            int partnerId = iPlayer.GetData("inSex");

            iPlayer.Player.TriggerEvent("freezePlayer", false);
            iPlayer.SetData("userCannotInterrupt", false);
            iPlayer.StopAnimation();
          
            iPlayer.ResetData("inSex");

            DbPlayer target = Players.Players.Instance.FindPlayerById(partnerId);

            if (target == null || !target.IsValid()|| !target.Player.IsInVehicle) return;

            target.Player.TriggerEvent("freezePlayer", false);
            target.SetData("userCannotInterrupt", false);
            target.StopAnimation();

            target.ResetData("inSex");
            return;
        }

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            if (dbPlayer.Player.IsInVehicle || key != Key.E) return false;

            if (dbPlayer.HasData("InjuryMovePointID"))
                return false;

            // First search for player in Interaction
            InteractionItem interactionItem = InteractionUsed.ToList().Where(i => i.Value == dbPlayer).FirstOrDefault().Key;
            if (interactionItem != null)
            {
                if (InteractionUsed[interactionItem] != null && InteractionUsed[interactionItem].IsValid() && InteractionUsed[interactionItem] == dbPlayer)
                {
                    Instance.InteractionUsed[interactionItem] = null;

                    dbPlayer.StopAnimation();
                    dbPlayer.Player.TriggerEvent("freezePlayer", false);
                    dbPlayer.ResetData("userCannotInterrupt");

                    dbPlayer.StopAnimation();
                    return true;
                }
            }


            // InteractionItem
            interactionItem = InteractionModule.Instance.GetAll().Values.Where(ts => ts.Position.DistanceTo(dbPlayer.Player.Position) < 0.5f &&
            (!InteractionUsed.ContainsKey(ts) || Instance.InteractionUsed[ts] == null)).FirstOrDefault();

            if (interactionItem != null)
            {
                if (!InteractionUsed.ContainsKey(interactionItem)) return false;

                if (!dbPlayer.CanInteract()) return false;

                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                dbPlayer.SetData("userCannotInterrupt", true);

                Instance.InteractionUsed[interactionItem] = dbPlayer;

                Task.Run(async () =>
                {

                    dbPlayer.Player.SetPosition(interactionItem.Position);
                    dbPlayer.Player.SetRotation(interactionItem.Heading);

                    await Task.Delay(300);


                    dbPlayer.Player.SetPosition(interactionItem.Position);
                    dbPlayer.Player.SetRotation(interactionItem.Heading);

                    await Task.Delay(300);

                    dbPlayer.StopAnimation();

                    await Task.Delay(500);

                    dbPlayer.Player.SetPosition(interactionItem.Position);
                    dbPlayer.Player.SetRotation(interactionItem.Heading);

                    List<InteractionGroupItem> items = InteractionGroupModule.Instance.GetByGroup(interactionItem.InteractionGroupType);
                    if (items.Count() > 0)
                    {

                        InteractionGroupItem item = items[0];

                        // first
                        dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), item.Anim1, item.Anim2);

                        dbPlayer.SetData("interactionVal", 0);
                    }

                    if (interactionItem.InteractionGroupType == 4 && Instance.InteractionUsed[interactionItem] == dbPlayer)
                    {
                        //Kacken
                        // Hose Ausziehen

                        if (dbPlayer.IsMale())
                        {
                            dbPlayer.SetClothes(4, 61, 0);
                        }
                        else
                        {
                            dbPlayer.SetClothes(4, 15, 0);
                        }

                        Instance.InteractionUsed[interactionItem] = null;
                        int time = 125000;
                        Chats.sendProgressBar(dbPlayer, time);
                        await Task.Delay(time);
                        NutritionModule.Instance.TakeAShit(dbPlayer);
                        NutritionModule.Instance.PushNutritionToPlayer(dbPlayer);

                        dbPlayer.Player.TriggerEvent("freezePlayer", false);
                        dbPlayer.ResetData("userCannotInterrupt");
                        dbPlayer.StopAnimation();
                        try
                        {
                            if (dbPlayer.HasData("outfitactive"))
                            {
                                int variation = OutfitsModule.Instance.GetOutfitComponentVariation(dbPlayer, dbPlayer.GetData("outfitactive"), 4);
                                int texture = OutfitsModule.Instance.GetOutfitComponentTexture(dbPlayer, dbPlayer.GetData("outfitactive"), 4);
                                dbPlayer.SetClothes(4, variation, texture);
                            }
                            else
                            {
                                uint clothId = dbPlayer.Character.Clothes[4];
                                Cloth cloth = ClothModule.Instance[clothId];
                                if (cloth == null) return;
                                dbPlayer.SetClothes(4, cloth.Variation, cloth.Texture);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Crash(e);
                        }
                    }
                });

                return true;
            }
            return false;
        }
    }

    public class InteractionItem : Loadable<uint>
    {
        public uint Id { get; set; }
        public Vector3 Position { get; set; }
        public float Heading { get; set; }

        public int InteractionGroupType { get; set; }

        public InteractionItem(MySqlDataReader reader) : base(reader)
        {
            Id = reader.GetUInt32("id");
            Position = new Vector3(reader.GetFloat("pos_x"), reader.GetFloat("pos_y"), reader.GetFloat("pos_z"));
            Heading = reader.GetFloat("heading");
            InteractionGroupType = reader.GetInt32("anim_group");


            if (Configurations.Configuration.Instance.DevMode) Spawners.Markers.Create(1, Position, new Vector3(), new Vector3(), 0.7f, 255, 255, 0, 0);
        }

        public override uint GetIdentifier()
        {
            return Id;
        }
    }

    public class InteractionEventHandler : Script
    {
        [RemoteEvent]
        public static void nextInteractionAnim(Player Player, int state)
        {
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.IsValid() || !dbPlayer.HasData("interactionVal")) return;

            InteractionItem interactionItem = InteractionModule.Instance.InteractionUsed.ToList().Where(i => i.Value == dbPlayer).FirstOrDefault().Key;
            if (interactionItem != null)
            {
                if (InteractionModule.Instance.InteractionUsed[interactionItem] != null && InteractionModule.Instance.InteractionUsed[interactionItem].IsValid() && InteractionModule.Instance.InteractionUsed[interactionItem] == dbPlayer)
                {
                    dbPlayer.StopAnimation();

                    int animIndex = dbPlayer.GetData("interactionVal");
                    animIndex++;

                    List<InteractionGroupItem> items = InteractionGroupModule.Instance.GetByGroup(interactionItem.InteractionGroupType);

                    if(animIndex > items.Count()-1)
                    {
                        animIndex = 0;
                    }
                    dbPlayer.SetData("interactionVal", animIndex);

                    InteractionGroupItem item = items[animIndex];

                    dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), item.Anim1, item.Anim2);

                    return;
                }
            }

        }

        [RemoteEvent]
        public static void prevInteractionAnim(Player Player, int state)
        {
            var dbPlayer = Player.GetPlayer();
            if (!dbPlayer.IsValid() || !dbPlayer.HasData("interactionVal")) return;

            InteractionItem interactionItem = InteractionModule.Instance.InteractionUsed.ToList().Where(i => i.Value == dbPlayer).FirstOrDefault().Key;
            if (interactionItem != null)
            {
                if (InteractionModule.Instance.InteractionUsed[interactionItem] != null && InteractionModule.Instance.InteractionUsed[interactionItem].IsValid() && InteractionModule.Instance.InteractionUsed[interactionItem] == dbPlayer)
                {
                    dbPlayer.StopAnimation();

                    List<InteractionGroupItem> items = InteractionGroupModule.Instance.GetByGroup(interactionItem.InteractionGroupType);
                    int animIndex = dbPlayer.GetData("interactionVal");
                    animIndex--;
                    if (animIndex < 0)
                    {
                        animIndex = items.Count()-1;
                    }
                    dbPlayer.SetData("interactionVal", animIndex);

                    InteractionGroupItem item = items[animIndex];

                    dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), item.Anim1, item.Anim2);

                    return;
                }
            }
        }
    }
}
