﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Jobs;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Buffs;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Vehicles;
using VMP_CNR.Module.Robbery;
using VMP_CNR.Module.Teams.Shelter;
using VMP_CNR.Module.Vehicles.Shops;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Shops;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Weapons.Data;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Items.Scripts;
using VMP_CNR.Module.Banks.Windows;
using VMP_CNR.Module.Crime;
using VMP_CNR.Module.Armory;
using VMP_CNR.Module.News.App;
using static VMP_CNR.Module.Chat.Chats;
using VMP_CNR.Module.ReversePhone;
using VMP_CNR.Module.Swat;
using VMP_CNR.Module.MapParser;
using VMP_CNR.Module.Storage;
using VMP_CNR.Module.Spawners;
using VMP_CNR.Module.Staatskasse;
using VMP_CNR.Module.Players.Commands;
using VMP_CNR.Module.Support;
using static VMP_CNR.Module.Computer.Apps.SupportApp.SupportKonversation;
using VMP_CNR.Module.Zone;
using VMP_CNR.Module.Computer.Apps.SupportApp;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.DropItem;
using VMP_CNR.Module.Gamescom;
using VMP_CNR.Module.Meth;
using VMP_CNR.Module.LeitstellenPhone;
using VMP_CNR.Module.Players.Events;
using VMP_CNR.Module.Paintball;
using VMP_CNR.Module.UHaft;
using VMP_CNR.Module.Tuning;
using VMP_CNR.Module.Vehicles.Garages;
using VMP_CNR.Module.Vehicles.RegistrationOffice;
using VMP_CNR.Module.Gangwar;
using VMP_CNR.Module.LSCustoms;
using VMP_CNR.Module.FIB;
using VMP_CNR.Module.NSA;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.VehicleRent;
using VMP_CNR.Module.Events.Halloween;
using VMP_CNR.Module.MAZ;
using VMP_CNR.Module.Weapons.Component;
using VMP_CNR.Module.Jobs.Taxi.App;

namespace VMP_CNR.Module.Players
{
    public class PlayerCommands : Script
    {
        private readonly List<DbPlayer> _users = Players.Instance.GetValidPlayers();

        #region Los Santos Customs

        [Command]
        public void lscpaint(Player p_Player)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            LSCustoms.LSCustoms l_LSC = LSCustomsModule.Instance.GetAll().Where(x => l_DbPlayer.Player.Position.DistanceTo(x.Value.position) <= 5.0f && x.Value.type == 0).FirstOrDefault().Value;
            if (l_LSC == null)
            {
                l_DbPlayer.SendNewNotification("Du bist nicht in einer Lackierkabine!");
                return;
            }

            l_DbPlayer.SetData("lsc_type", 0);
            MenuManager.Instance.Build(PlayerMenu.LSCVehicleListMenu, l_DbPlayer).Show(l_DbPlayer);
        }

        [Command]
        public void lsctune(Player p_Player)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            LSCustoms.LSCustoms l_LSC = LSCustomsModule.Instance.GetAll().Where(x => l_DbPlayer.Player.Position.DistanceTo(x.Value.position) <= 5.0f && x.Value.type == 1).FirstOrDefault().Value;
            if (l_LSC == null)
            {
                l_DbPlayer.SendNewNotification("Du bist nicht in einer Tuningwerkstatt!");
                return;
            }

            l_DbPlayer.SetData("lsc_type", 1);
            MenuManager.Instance.Build(PlayerMenu.LSCVehicleListMenu, l_DbPlayer).Show(l_DbPlayer);
        }

        [Command]
        public void lscpearl(Player p_Player)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            LSCustoms.LSCustoms l_LSC = LSCustomsModule.Instance.GetAll().Where(x => l_DbPlayer.Player.Position.DistanceTo(x.Value.position) <= 5.0f && x.Value.type == 0).FirstOrDefault().Value;
            if (l_LSC == null)
            {
                l_DbPlayer.SendNewNotification("Du bist nicht in einer Lackierkabine!");
                return;
            }

            l_DbPlayer.SetData("lsc_type", 2);
            MenuManager.Instance.Build(PlayerMenu.LSCVehicleListMenu, l_DbPlayer).Show(l_DbPlayer);
        }

        [Command]
        public void lscrim(Player p_Player)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            LSCustoms.LSCustoms l_LSC = LSCustomsModule.Instance.GetAll().Where(x => l_DbPlayer.Player.Position.DistanceTo(x.Value.position) <= 5.0f && x.Value.type == 0).FirstOrDefault().Value;
            if (l_LSC == null)
            {
                l_DbPlayer.SendNewNotification("Du bist nicht in einer Lackierkabine!");
                return;
            }

            l_DbPlayer.SetData("lsc_type", 3);
            MenuManager.Instance.Build(PlayerMenu.LSCVehicleListMenu, l_DbPlayer).Show(l_DbPlayer);
        }

        [Command]
        public void lscsmoke(Player p_Player)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            LSCustoms.LSCustoms l_LSC = LSCustomsModule.Instance.GetAll().Where(x => l_DbPlayer.Player.Position.DistanceTo(x.Value.position) <= 5.0f && x.Value.type == 1).FirstOrDefault().Value;
            if (l_LSC == null)
            {
                l_DbPlayer.SendNewNotification("Du bist nicht in einer Tuningwerkstatt!");
                return;
            }

            l_DbPlayer.SetData("lsc_type", 4);
            MenuManager.Instance.Build(PlayerMenu.LSCVehicleListMenu, l_DbPlayer).Show(l_DbPlayer);
        }

        [Command(GreedyArg = true)]
        public void lscveh(Player p_Player, string p_VehicleID)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if (!uint.TryParse(p_VehicleID, out uint l_VehicleID))
                return;

            SxVehicle l_Vehicle = VehicleHandler.Instance.GetByVehicleDatabaseId(l_VehicleID);
            if (l_Vehicle == null)
                return;

            DbPlayer l_VehOwner = Players.Instance.FindPlayer(l_Vehicle.ownerId);
            if (l_VehOwner == null || !l_VehOwner.IsValid())
                return;

            if (l_VehOwner.Player.Position.DistanceTo(l_DbPlayer.Player.Position) > 5.0f)
            {
                l_DbPlayer.SendNewNotification("Der Fahrzeugbesitzer muss in deiner Nähe stehen!");
                return;
            }

            ComponentManager.Get<ConfirmationWindow>().Show()(l_VehOwner, new ConfirmationObject(
                $"Schlüsselübergabe an LSC",
                $"Der Tuner {l_DbPlayer.Player.Name} bittet um die Schlüssel deines Fahrzeugs (ID: {l_VehicleID.ToString()}) während des Umbaus." +
                $"ACHTUNG! Das Fahrzeug steht dir während des Werkstattaufenthalts nicht zur Verfügung!"
               , "LscConfirmVehRequest", "", ""));

            l_VehOwner.SetData("lsc_vehkey_request", l_VehicleID);
            l_DbPlayer.SendNewNotification($"Du hast eine Schlüssel Anfrage zum Tunen des Fahrzeugs {l_VehicleID.ToString()} abgesendet.");
        }
        
        [Command]
        public void acceptlsc(Player p_Player)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (!l_DbPlayer.HasData("lsc_vehkey_request"))
            {
                l_DbPlayer.SendNewNotification("Derzeit ist keine Tuner-Anfrage offen!");
                return;
            }

            uint l_VehicleID = l_DbPlayer.GetData("lsc_vehkey_request");
            SxVehicle l_Vehicle = VehicleHandler.Instance.GetByVehicleDatabaseId(l_VehicleID);
            if (l_Vehicle == null)
            {
                l_DbPlayer.SendNewNotification($"Das Fahrzeug mit der ID {l_VehicleID.ToString()} ist nicht ausgeparkt!");
                return;
            }

            l_DbPlayer.ResetData("lsc_vehkey_request");
            l_Vehicle.SetTuningState(true);
            l_DbPlayer.SendNewNotification("Du hast das Fahrzeug an die Tuningwerkstatt übergeben!");
            Teams.TeamModule.Instance.Get((uint)teams.TEAM_LSC).SendNotification($"{l_DbPlayer.Player.Name} hat sein Fahrzeug {l_Vehicle.Data.Model} an LSC übergeben!");
        }

        [Command(GreedyArg = true)]
        public void lscrechnung(Player p_Player, string p_Args)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            var l_Args = p_Args.Split(new[] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
            if (l_Args.Length < 4)
            {
                l_DbPlayer.SendNewNotification("Syntax: /lscrechnung SPIELER FAHRZEUG_ID RECHNUNG_ID BETRAG");
                return;
            }

            if (!uint.TryParse(l_Args[1], out uint l_VehicleID))
                return;

            if (!uint.TryParse(l_Args[2], out uint l_BillID))
                return;

            if (!uint.TryParse(l_Args[3], out uint l_Amount))
                return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                l_DbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            SxVehicle l_Vehicle = VehicleHandler.Instance.GetByVehicleDatabaseId(l_VehicleID);
            if (l_Vehicle == null)
            {
                l_DbPlayer.SendNewNotification($"Das Fahrzeug {l_VehicleID.ToString()} wurde nicht gefunden.");
                return;
            }

            DbPlayer l_Payer = Players.Instance.FindPlayer(l_Args[0]);
            if (l_Payer == null || !l_Payer.IsValid())
            {
                l_DbPlayer.SendNewNotification("Der angegebene Spieler wurde nicht gefunden.");
                return;
            }

            ComponentManager.Get<ConfirmationWindow>().Show()(l_Payer, new ConfirmationObject(
               $"Rechnung von LSC",
               $"Los Santos Customs hat dir eine Rechnung (Nr: {l_BillID.ToString()}) in Höhe von {l_Amount:N0}$ ausgestellt." +
               $"Das Geld wird von deinem Konto abgebucht."
              , "LscConfirmPayRequest", "", ""));

            l_Payer.SetData("lsc_bill_amount", l_Amount);
            l_Payer.SetData("lsc_bill_tuner", l_DbPlayer.Id);
            l_Payer.SetData("lsc_bill_vehicle", l_VehicleID);
            l_Payer.SetData("lsc_bill_id", l_BillID);
            Teams.TeamModule.Instance.Get((uint)teams.TEAM_LSC).SendNotification($"{l_Payer.Player.Name} wurde eine Rechnung für das Fahrzeug {l_Vehicle.Data.Model} in Höhe von {l_Amount.ToString()} ausgestellt!");
        }

        [Command]
        public void lscpay(Player p_Player)
        {
            DbPlayer l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.IsValid())
                return;

            if (!l_DbPlayer.HasData("lsc_bill_amount") || !l_DbPlayer.HasData("lsc_bill_tuner") || !l_DbPlayer.HasData("lsc_bill_vehicle") || !l_DbPlayer.HasData("lsc_bill_id"))
            {
                l_DbPlayer.SendNewNotification("Es ist keine LS-Customs Rechnung offen.");
                return;
            }

            uint l_VehicleID    = l_DbPlayer.GetData("lsc_bill_vehicle");
            uint l_Amount       = l_DbPlayer.GetData("lsc_bill_amount");
            uint l_TunerID      = l_DbPlayer.GetData("lsc_bill_tuner");
            uint l_BillID       = l_DbPlayer.GetData("lsc_bill_id");

            SxVehicle l_Vehicle = VehicleHandler.Instance.GetByVehicleDatabaseId(l_VehicleID);
            if (l_Vehicle == null)
                return;

            if (!l_DbPlayer.TakeBankMoney((int)l_Amount, $"LS-Customs: Rechnung {l_BillID.ToString()}"))
            {
                l_DbPlayer.SendNewNotification("Deine Konto ist nicht gedeckt um die LSC Rechnung zu bezahlen.");
                return;
            }

            l_DbPlayer.ResetData("lsc_bill_vehicle");
            l_DbPlayer.ResetData("lsc_bill_amount");
            l_DbPlayer.ResetData("lsc_bill_tuner");
            l_DbPlayer.ResetData("lsc_bill_id");
            l_Vehicle.SetTuningState(false);

            var l_Rechnung = new LSCustoms.LSCRechnung(l_BillID, l_TunerID, l_Amount, l_DbPlayer.Id, l_VehicleID);
            l_Rechnung.Save();

            l_DbPlayer.SendNewNotification($"Du hast die LSC-Rechnung (ID: {l_BillID.ToString()}) bezahlt!");
            Teams.TeamModule.Instance.Get((uint)teams.TEAM_LSC).SendNotification($"{l_DbPlayer.Player.Name} hat die Rechnung {l_BillID.ToString()} bezahlt!");

            var l_Shelter = TeamShelterModule.Instance.GetAll().FirstOrDefault(s => s.Value.Team.Id == (uint)teams.TEAM_LSC).Value;
            if (l_Shelter == null || l_Shelter.Team == null) return;

            // 50% vom Tuning verschwinden, 10% Staatskasse, 40% in Fbank
            l_Shelter.GiveMoney((int)Math.Round(l_Amount * 0.4, 0));
            KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, (int)Math.Round(l_Amount * 0.1, 0));

            Logger.SaveToCustomLog("customs_verdienst_wegmiddeviecher", (int)(l_Amount * 0.5), (int)(l_Amount * 0.5));
        }

        #endregion

        [CommandPermission]
        [Command]
        public void acceptservice(Player player, string asCommand = " ")
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

                try
                {
                    if (string.IsNullOrWhiteSpace(asCommand))
                    {
                        dbPlayer.SendNewNotification(
                            MSG.General.Usage("/acceptservice", "[Name]"));
                        return;
                    }

                    DbPlayer findPlayer = Players.Instance.FindPlayer(asCommand);

                    if (dbPlayer.HasData("taxi") &&
                        dbPlayer.GetData("taxi") > 0)
                    {
                        if (findPlayer == null || !findPlayer.IsValid()
                                               || findPlayer.Dimension[0] != dbPlayer.Dimension[0]
                                               || !findPlayer.HasData("taxi_request")
                                               || dbPlayer.GetName() != findPlayer.GetData("taxi_request")
                                               || dbPlayer.Container.GetItemAmount(174)<1
                                               || dbPlayer.phoneSetting.flugmodus)
                        {
                            PlayerNotFoundOrNoService(dbPlayer);
                            return;
                        }

                        if (findPlayer.GetName() == dbPlayer.GetName()) return;

                        var taxiprice = dbPlayer.GetData("taxi");
                        var agreedOnPrice = findPlayer.GetData("taxi_request_price");

                        if (taxiprice != agreedOnPrice)
                        {
                            dbPlayer.SendNewNotification("Der Taxifahrer hat seinen Preis geaendert. Bitte neu beauftragen!");
                            findPlayer.SendNewNotification("Der Taxifahrer hat seinen Preis geaendert. Bitte neu beauftragen!");
                            findPlayer.ResetData("taxi_request");
                            findPlayer.ResetData("taxi_request_price");
                            findPlayer.Player.ResetSharedData("taxi_request");
                            findPlayer.Player.ResetSharedData("taxi_request_price");
                            return;
                        }

                        if (!PlayerMoney.TakeMoney(findPlayer, taxiprice))
                        {
                            dbPlayer.SendNewNotification("Spieler kann sich die Anfahrtskosten nicht leisten!");
                            String awdawd = MSG.Money.NotEnoughMoney(taxiprice);
                            findPlayer.SendNewNotification(awdawd);
                            return;
                        }

                        PlayerMoney.GiveMoney(dbPlayer, taxiprice);
                        findPlayer.ResetData("taxi_request");
                        findPlayer.ResetData("taxi_request_message");
                        findPlayer.ResetData("taxi_request_price");
                        findPlayer.Player.ResetSharedData("taxi_request");
                        findPlayer.Player.ResetSharedData("taxi_request_price");
                        GetPlayerpositionAndInformService(findPlayer, dbPlayer);

                        var tsl = new TaxiServiceListApp();
                        tsl.requestTaxiServiceList(player);

                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Crash(ex);
                }
            }));
        }

        [Command(GreedyArg = true)]
        public void alktest(Player p_Player, string p_Name)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(async () =>
            {
                var l_DbPlayer = p_Player.GetPlayer();
                if (l_DbPlayer == null)
                    return;

                if (l_DbPlayer.TeamId != (uint)teams.TEAM_FIB && l_DbPlayer.TeamId != (uint)teams.TEAM_POLICE && l_DbPlayer.TeamId != (uint)teams.TEAM_ARMY && l_DbPlayer.TeamId != (uint)teams.TEAM_MEDIC && l_DbPlayer.TeamId != (uint) teams.TEAM_GOV)
                    return;

                DbPlayer l_FindPlayer = Players.Instance.FindPlayer(p_Name);
                if (l_FindPlayer == null)
                    return;

                if (p_Player.Position.DistanceTo(l_FindPlayer.Player.Position) > 3.0f)
                    return;

                l_DbPlayer.SendNewNotification("Alkoholtest gestartet.");
                l_FindPlayer.SendNewNotification("Ein Beamter fuehrt einen Alkoholtest durch.");
                await Task.Delay(10000);
                if (!l_FindPlayer.HasData("alkLevel"))
                {
                    l_DbPlayer.SendNewNotification("0.00 Promille");
                    return;
                }

                decimal l_AlkLevel = (decimal)l_FindPlayer.GetData("alkLevel") / 18;
                Decimal.Round(l_AlkLevel, 2);
                l_DbPlayer.SendNewNotification($"Der Alkoholtest ergab {l_AlkLevel.ToString()} Promille");
                l_FindPlayer.SendNewNotification($"Ihr Alkoholtest ergab {l_AlkLevel.ToString()} Promille");
                return;
            }));
        }
        
        [CommandPermission]
        [Command]
        public void gamescom(Player player)
        {
            return; // deactivated actually
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;
            if (dbPlayer.Container.GetInventoryFreeSpace() > 1000 && dbPlayer.Container.MaxSlots-dbPlayer.Container.GetUsedSlots() >= 1)
            {
                GTANetworkAPI.NAPI.Task.Run(() => ComponentManager.Get<TextInputBoxWindow>().Show()(
                    dbPlayer, new TextInputBoxWindowObject() { Title = "Gamescom Code einlösen", Callback = "UseGamescomCode", Message = "Gib den Gutscheincode ein." }));
            }
            else
            {
                dbPlayer.SendNewNotification("Du hast nicht genug Platz in deinem Rucksack!");
                return;
            }

        }


        [CommandPermission(AllowedDeath = true)]
        [Command(GreedyArg = true)]
        public void chat(Player player, string message)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (!TicketModule.Instance.getCurrentChatStatus(dbPlayer))
            {
                dbPlayer.SendNewNotification("Sie haben keine Befugnis diesen Befehl zu nutzen!");
                return;
            }

            string name = TicketModule.Instance.getCurrentTicketSupporter(dbPlayer);

            var findplayer = Players.Instance.FindPlayer(name);
            if (findplayer == null) return;

            Konversation konversationMessage = new Konversation(dbPlayer, false, message);
            bool response = KonversationModule.Instance.Add(dbPlayer, konversationMessage);

            var konvMessage = new konversationObject() { id = (int)konversationMessage.Player.Id, sender = konversationMessage.Player.GetName(), receiver = konversationMessage.Receiver, message = konversationMessage.Message, date = konversationMessage.Created_at };

            var messageJson = NAPI.Util.ToJson(konvMessage);
            new SupportKonversation().sendMessage(findplayer.Player, messageJson);

            findplayer.SendNewNotification($"Antwort von {dbPlayer.GetName()} erhalten!");
            dbPlayer.SendNewNotification($"Die Antwort wurde an {findplayer.GetName()} gesendet!");
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void givefalic(Player player, string name)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;
                if (dbPlayer.TeamId != (int)teams.TEAM_MEDIC) return;
                if (dbPlayer.TeamRank < 3) return;

                var findPlayer = Players.Instance.FindPlayer(name);
                if (findPlayer == null) return;

                if (!(dbPlayer.Player.Position.DistanceTo(findPlayer.Player.Position) < 5.0f)) return;
                if (findPlayer.Lic_FirstAID[0] == 1)
                {
                    dbPlayer.SendNewNotification("Buerger hat bereits erste Hilfe gelernt!");
                    return;
                }

                if (!findPlayer.TakeMoney(6000))
                {
                    dbPlayer.SendNewNotification(MSG.Money.PlayerNotEnoughMoney(6000));
                    return;
                }

                dbPlayer.Team.SendNotification($"{dbPlayer.GetName()} hat {findPlayer.GetName()} in erste Hilfe ausgebildet.");

                dbPlayer.GiveMoney(3000);
                findPlayer.Lic_FirstAID[0] = 1;
                dbPlayer.SendNewNotification("Sie haben " + findPlayer.GetName() +
                                          " in erste Hilfe ausgebildet.");
                findPlayer.SendNewNotification(player.Name + " hat Sie in erste Hilfe ausgebildet.");
            }));
        }

        [CommandPermission]
        [Command]
        public void jailtime(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.CanAccessMethod()) return;
            if (dbPlayer.jailtime[0] <= 0) return;
            dbPlayer.SendNewNotification("Sie befinden sich noch fuer " + (dbPlayer.jailtime[0]) +
                                      " Minuten im Staatsgefaengis!");
        }

        [CommandPermission]
        [Command]
        public void erstattung(Player player)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (!Configuration.Instance.InventoryActivated)
            {
                dbPlayer.SendNewNotification("Das Inventarsystem ist aus Performance-Gründen deaktiviert.");
                dbPlayer.SendNewNotification("Es ist in wenigen Minuten wieder erreichbar!");
                return;
            }

            //if (dbPlayer.Rank.Id != 11 && dbPlayer.Rank.Id != 8 && dbPlayer.Rank.Id != 4 && dbPlayer.Rank.Id != 5) return;

            if (!dbPlayer.TryData("garageId", out uint playerGarageId)) return;

            var garage = GarageModule.Instance[playerGarageId];
            if (garage == null || garage.HouseId != 0) return;

            Container container = ContainerManager.LoadContainer(dbPlayer.Id, ContainerTypes.REFUND, 0, 0);

            if (Main.RefundPlayers.ContainsKey(dbPlayer))
            {
                if (DateTime.Now > Main.RefundPlayers.GetValueOrDefault(dbPlayer).AddMinutes(15))
                {
                    dbPlayer.SendNewNotification("Deine 15 Minuten sind rum, der Erstattungsinhalt wird gelöscht.");
                    container.ClearInventory();
                    Main.RefundPlayers.Remove(dbPlayer);
                    return;
                }
            }
            List<PlayerContainerObject> containerList = new List<PlayerContainerObject>();
            containerList.Add(dbPlayer.Container.ConvertForPlayer(1, "", dbPlayer.money[0]));
            

            // Find Now The Inventory
            string Playersending = "[";
            Playersending += NAPI.Util.ToJson(dbPlayer.Container.ConvertForPlayer(1));
            containerList.Add(container.ConvertForPlayer(2));
            Playersending += "," + NAPI.Util.ToJson(container.ConvertForPlayer(2));
            Playersending += "]";

            dbPlayer.Player.TriggerEvent("responseInventory", Playersending);

            ComponentManager.Get<InventoryWindow>().Show()(dbPlayer, containerList);
            dbPlayer.SetData("container_refund", true);

            if (!Main.RefundPlayers.ContainsKey(dbPlayer))
            {
                Main.RefundPlayers.Add(dbPlayer, DateTime.Now);
                dbPlayer.SendNewNotification("Du hast nun 15 Minuten Zeit deine Gegenstände aus dem Erstattungsinventar zu entnehmen. Nach den 15 Minuten wird der Inhalt gelöscht!", PlayerNotification.NotificationType.ADMIN, "ADMIN", duration: 20000);
            }

        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void setgwd(Player player, string command)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var commandSplitted = command.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
                if (commandSplitted.Length < 1) return;
                if (!int.TryParse(commandSplitted[1], out var noteId)) return;
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;
                if (dbPlayer.TeamId != (int)teams.TEAM_ARMY || dbPlayer.TeamRank < 4) return;

                var findPlayer = Players.Instance.FindPlayer(commandSplitted[0]);
                if (findPlayer == null) return;

                if (dbPlayer.Id == findPlayer.Id) return;

                if (noteId < 1 || noteId > 9)
                {
                    dbPlayer.SendNewNotification("Falsche Benotung!");
                    return;
                }

                if (dbPlayer.Player.Position.DistanceTo(findPlayer.Player.Position) < 5.0f)
                {
                    dbPlayer.SendNewNotification("Sie haben " + findPlayer.GetName() +
                                              " die GWD Note " + commandSplitted[1] + " gegeben.");
                    findPlayer.SendNewNotification(dbPlayer.GetName() + " hat ihnen die GWD Note " +
                                                commandSplitted[1] + " gegeben.");
                    findPlayer.grade[0] = noteId;
                }
                else
                {
                    dbPlayer.SendNewNotification(MSG.General.notInRange);
                }
            }));
        }


        [CommandPermission]
        [Command(GreedyArg = true)]
        public void setzwd(Player player, string command)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var commandSplitted = command.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
                if (commandSplitted.Length < 1) return;
                if (!int.TryParse(commandSplitted[1], out var noteId)) return;
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;
                if (dbPlayer.TeamId != (int)teams.TEAM_MEDIC || dbPlayer.TeamRank < 9) return;

                var findPlayer = Players.Instance.FindPlayer(commandSplitted[0]);
                if (findPlayer == null) return;

                if (dbPlayer.Id == findPlayer.Id) return;

                if (noteId < 1 || noteId > 9)
                {
                    dbPlayer.SendNewNotification("Falsche Benotung!");
                    return;
                }

                if (dbPlayer.Player.Position.DistanceTo(findPlayer.Player.Position) < 5.0f)
                {
                    dbPlayer.SendNewNotification("Sie haben " + findPlayer.GetName() +
                                              " die ZWD Note " + commandSplitted[1] + " gegeben.");
                    findPlayer.SendNewNotification(dbPlayer.GetName() + " hat ihnen die ZWD Note " +
                                                commandSplitted[1] + " gegeben.");
                    findPlayer.zwd[0] = noteId;

                    dbPlayer.Team.SendNotification($"{dbPlayer.GetName()} hat an {findPlayer.GetName()} die Zivildienst-Note {noteId} vergeben.");
                }
                else
                {
                    dbPlayer.SendNewNotification(MSG.General.notInRange);
                }
            }));
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void editjailtime(Player player, string command)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var commandSplitted = command.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
                if (commandSplitted.Length < 1) return;
                if (!int.TryParse(commandSplitted[1], out var plusTime)) return;
                if (plusTime <= 0) return;
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;
                if (!dbPlayer.IsACop() || dbPlayer.TeamRank <= 6) return;

                var findPlayer = Players.Instance.FindPlayer(commandSplitted[0]);
                if (findPlayer == null) return;

                if (dbPlayer.Player.Position.DistanceTo(new Vector3(1846.450, 2585.875, 45.672)) < 5.0f)
                {
                    findPlayer.jailtime[0] += plusTime;

                    if (findPlayer.jailtime[0] > 120)
                    {
                        findPlayer.jailtime[0] = 120;
                    }

                    dbPlayer.SendNewNotification("Sie haben die Gefaengniszeit von Spieler: " +
                                                findPlayer.GetName() + " auf: " + findPlayer.jailtime[0] + " gesetzt.");
                    findPlayer.SendNewNotification("Beamter: " + dbPlayer.GetName() +
                                                " hat deine Gefaengniszeit auf: " + findPlayer.jailtime[0] + " gesetzt.");
                }
            }));
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void getfree(Player player, string commandParams)
        {
            /*var command = commandParams.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
            if (command.Length <= 1) return;
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;
            if (dbPlayer.job[0] != (int)jobs.JOB_ANWALT) return;

            if (command.Length < 1) return;

            var findPlayer = Players.Instance.FindPlayer(command[0]);
            if (findPlayer == null) return;

            var findBeamter = Players.Instance.FindPlayer(command[1]);
            if (findBeamter == null || !findBeamter.IsValid()) return;

            if (dbPlayer.Id == findPlayer.Id || dbPlayer.Id == findBeamter.Id || findPlayer.Id == findBeamter.Id)
            {
                dbPlayer.SendNewNotification("Sie können sich nicht selbst vertreten!");
                return;
            }

            if (dbPlayer.Player.Position.DistanceTo(findBeamter.Player.Position) < 5.0f && findBeamter.IsACop())
            {
                if (Main.canPlayerFreed(dbPlayer, findPlayer))
                {
                    findBeamter.SendNewNotification("Anwalt " + findPlayer.GetName() +
                                                    " moechte Gefangenen " + findPlayer.GetName() +
                                                    " aus dem Gefaengnis freilassen.");
                    findBeamter.SendNewNotification(
                        "Benutze /acceptfree [Anwaltname] um das Free zu bestaetigen.");
                    dbPlayer.SendNewNotification("Sie haben eine Freilassung fuer " +
                                                findPlayer.GetName() + " beantragt.");
                    dbPlayer.SetData("getFree", findPlayer);
                    dbPlayer.SetData("acceptFree", 0);
                }
                else
                {
                    dbPlayer.SendNewNotification(
                        "Spieler kann nicht aus dem Gefaengnis geholt werden! (Zu Wenig Skill oder Gefaengniszeit zu lang)");
                }
            }
            else
            {
                dbPlayer.SendNewNotification(MSG.General.notInRange);
            }*/
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void acceptfree(Player player, string anwalt)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;
                if (!dbPlayer.IsACop()) return;

                var findPlayer = Players.Instance.FindPlayer(anwalt);
                if (findPlayer == null) return;

                if (dbPlayer.Player.Position.DistanceTo(findPlayer.Player.Position) < 5.0f)
                {
                    if (findPlayer.GetData("getFree"))
                    {
                        findPlayer.SetData("acceptFree", 1);
                        findPlayer.SendNewNotification("Beamter " + dbPlayer.GetName() +
                                                    " hat Ihren Antrage auf Freilassung genehmigt!");
                        findPlayer.SendNewNotification("Der Kautionspreis betraegt: $" +
                                                    Main.getFreePrice(findPlayer) + ", zu zahlen am Staatsgefaengnis!");
                        dbPlayer.SendNewNotification("Sie haben eine Freilassung genehmigt.");
                    }
                }
                else
                {
                    dbPlayer.SendNewNotification(MSG.General.notInRange);
                }
            }));
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void free(Player player, string mandant)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;
                if (dbPlayer.job[0] != (int)jobs.JOB_ANWALT) return;

                var findPlayer = Players.Instance.FindPlayer(mandant);
                if (findPlayer == null) return;

                if (dbPlayer.Player.Position.DistanceTo(new Vector3(1846.949, 2584.07, 45.672)) < 5.0f &&
                    Main.canPlayerFreed(dbPlayer, findPlayer))
                {
                    if (dbPlayer.HasData("getFree") && dbPlayer.GetData("getFree") == findPlayer)
                    {
                        if (dbPlayer.HasData("acceptFree") && dbPlayer.GetData("acceptFree") == 1)
                        {
                            if (!dbPlayer.TakeMoney(Main.getFreePrice(findPlayer)))
                            {
                                dbPlayer.SendNewNotification("Die Kaution betraegt: $" +
                                                          Main.getFreePrice(findPlayer));
                            }

                            dbPlayer.ResetData("acceptFree");
                            dbPlayer.ResetData("getFree");
                            dbPlayer.JobSkillsIncrease(5);
                            Main.freePlayer(dbPlayer, findPlayer);
                            dbPlayer.SendNewNotification("Sie haben die Kaution in hoehe von $" +
                                                      Main.getFreePrice(findPlayer) + " bezahlt!");
                        }
                    }
                }
                else
                {
                    dbPlayer.SendNewNotification("Sie muessen am Gefaengnis sein!");
                }
            }));
        }

        [CommandPermission]
        [Command]
        public async void givemarrylic(Player player, string playerName = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (string.IsNullOrWhiteSpace(playerName))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/givemarrylic", "[Vorname_Nachname]"), notificationType: PlayerNotification.NotificationType.SERVER);
                return;
            }

            await AsyncCommands.Instance.HandleGiveMarryLic(player, playerName);
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void marry(Player player, string commandParams = "")
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod() || dbPlayer.Player == null) return;
                if (dbPlayer.marryLic != 1)
                {
                    dbPlayer.SendNewNotification(MSG.Error.NoMarry(), title: "SERVER" ,notificationType: PlayerNotification.NotificationType.SERVER);
                    return;
                }

                var command = commandParams.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

                if (command.Length != 2)
                {
                    dbPlayer.SendNewNotification(

                        MSG.General.Usage("/marry", "Name der ersten Person", "& Name der zweiten Person"), title: "SERVER", notificationType: PlayerNotification.NotificationType.SERVER);
                    return;
                }


                var player1 = Players.Instance.FindPlayer(command[0]);
                if (player1 == null) return;
                var player2 = Players.Instance.FindPlayer(command[1]);
                if (player2 == null) return;
                if (player1 == player2) return;
                

                if ((dbPlayer.Player.Position.DistanceTo(new Vector3(-538.46, -176.525, 38.22)) < 5.0f
                    || dbPlayer.Player.Position.DistanceTo(new Vector3(-530.09, -191.157, 38.22)) < 5.0f)
                    &&
                    dbPlayer.Player.Position.DistanceTo(player1.Player.Position) < 5.0f && dbPlayer.Player.Position.DistanceTo(player2.Player.Position) < 5.0f)
                {
                    if (player1.married[0] > 0 || player2.married[0] > 0)
                    {
                        dbPlayer.SendNewNotification(
                            "Einer der beiden Eheleute ist bereits verheiratet!");
                        return;
                    }

                    dbPlayer.SendNewNotification("Hochzeitsanfrage wurde an " + player1.GetName() + " & " +  player2.GetName() + " gesendet!");

                    player1.SendNewNotification("Standesamt-Beamter "+player.Name+" möchte Sie mit " + player2.GetName() + " trauen. (/acceptmarry um anzunehmen)!");
                    player2.SendNewNotification("Standesamt-Beamter " + player.Name + " möchte Sie mit " + player1.GetName() + " trauen. (/acceptmarry um anzunehmen)!");


                    player1.SetData("marry", player2.GetName());
                    player2.SetData("marry", player1.GetName());                    

                    player1.SetData("beamter_Player1", dbPlayer.Player.Name);
                    player2.SetData("beamter_Player2", dbPlayer.Player.Name);
                    dbPlayer.SetData("beamter_Player1Name", player1.Player.Name);
                    dbPlayer.SetData("beamter_Player2Name", player2.Player.Name);
                    dbPlayer.SetData("bPlayer1Accepted", 0);
                    dbPlayer.SetData("bPlayer2Accepted", 0);                    
                    
                    
                }
                else
                {
                    dbPlayer.SendNewNotification("Das Brautpaar muss sich im Standesamt Büro befinden!");
                }
            }));
        }        

        [CommandPermission]
        [Command]
        public async void acceptmarry(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (!dbPlayer.HasData("marry")) return;
            if (!dbPlayer.HasData("beamter_Player1") && !dbPlayer.HasData("beamter_Player2")) return; 
            DbPlayer beamter = null;          
            
           if (dbPlayer.married[0] > 0)
            {
                dbPlayer.SendNewNotification(
                    "Sie sind bereits verheiratet!");
                return;
            }            
            
            if (dbPlayer.HasData("beamter_Player1"))
            {
                string beamterName = dbPlayer.GetData("beamter_Player1");

                beamter = Players.Instance.FindPlayer(beamterName);

                if (beamter == null)
                {
                    dbPlayer.SendNewNotification(
                        "Standesamt-Beamter nicht gefunden!");
                    return;
                }

                if (beamter.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f)
                {
                    dbPlayer.SendNewNotification(
                        "Standesamt-Beamter nicht in Reichweite!");
                    return;
                }

                dbPlayer.SendNewNotification(
                    "Sie haben die Anfrage angenommen.");

                beamter.SetData("bPlayer1Accepted", 1);
                beamter.SendNewNotification(
                    "Hochzeitsanfrage wurde von " + dbPlayer.GetName() + " bestätigt!");               
            }
            
            else if (dbPlayer.HasData("beamter_Player2"))
            {
                string beamterName = dbPlayer.GetData("beamter_Player2");

                beamter = Players.Instance.FindPlayer(beamterName);

                if (beamter == null)
                {
                    dbPlayer.SendNewNotification(
                        "Standesamt-Beamter nicht gefunden, oder nicht in Reichweite!");
                    return;
                }

                if (beamter.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f)
                {
                    dbPlayer.SendNewNotification(
                        "Standesamt-Beamter nicht in Reichweite!");
                    return;
                }

                dbPlayer.SendNewNotification(
                    "Sie haben die Anfrage angenommen.");

                beamter.SetData("bPlayer2Accepted", 1);
                beamter.SendNewNotification(
                    "Hochzeitsanfrage wurde von " + dbPlayer.GetName() + " bestätigt!");                
            }
            else
            {                
                return;
            }

            
            
            int ownerAccept = beamter.GetData("bPlayer1Accepted");            
            int customerAccept = beamter.GetData("bPlayer2Accepted");            
            
            // Wenn Beide bestaetigt haben
            if (ownerAccept != 1) return;
            if (customerAccept != 1) return;

            DbPlayer owner = null;
            DbPlayer customer = null;                      

            if (!beamter.HasData("beamter_Player1Name")) return;
            if (!beamter.HasData("beamter_Player2Name")) return;            

            string ownername = beamter.GetData("beamter_Player1Name");
            string customername = beamter.GetData("beamter_Player2Name");
            
            owner = Players.Instance.GetByName(ownername);
            customer = Players.Instance.GetByName(customername);
            
            if (owner == null || customer == null)
            {
                dbPlayer.SendNewNotification(
                    "Eine der Ehepartner wurde nicht gefunden oder ist nicht in Reichweite!");
                return;
            }

            if (owner.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f)
            {
                beamter.SendNewNotification(
                    "Eine der Ehepartner wurde nicht gefunden oder ist nicht in Reichweite!");
                return;
            }
            if (customer.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f)
            {
                beamter.SendNewNotification(
                    "Eine der Ehepartner wurde nicht gefunden oder ist nicht in Reichweite!");
                return;
            }
           

            if (!beamter.Container.CanInventoryItemAdded(670, 1))
            {
                beamter.SendNewNotification(
                    "Sie haben kein Platz im Inventar für die Eheurkunde!");
                return;
            }

            if (!beamter.Container.CanInventoryItemAdded(1065, 1))
            {
                beamter.SendNewNotification(
                    "Sie haben kein Platz im Inventar für die Eheverkuendung!");
                return;
            }

            owner.married[0] = customer.Id;
            customer.married[0] = owner.Id;

            var info = $"{owner.Player.Name} & {customer.Player.Name} gaben sich am {DateTime.Now.ToString("dd.MM.yyyy HH:mm")} das Ja-Wort. Bei Vorlage der Eheurkunde beim Rathaus kann die Namensänderung durchgeführt werden";
            beamter.Container.AddItem(670, 1, new Dictionary<string, dynamic>() { { "Info", info } });
            beamter.SendNewNotification("Du hast die Eheurkunde erhalten. Gebe sie dem Brautpaar!");

            info = $"Hochzeitsnews: Wir haben ein neues Ehepaar, " + owner.GetName() + " und " + customer.GetName() + " haben den Bund zur Ehe geschlossen!";
            beamter.Container.AddItem(1065, 1, new Dictionary<string, dynamic>() { { "Info", info } });
            beamter.SendNewNotification("Du hast die Eheverkuendung erhalten. Gebe sie dem Brautpaar!");

            dbPlayer.ResetData("marry");
            owner.ResetData("beamter_Player1");
            customer.ResetData("beamter_Player2");
            beamter.ResetData("beamter_Player1Name");
            beamter.ResetData("beamter_Player2Name");
            beamter.ResetData("bPlayer1Accepted");
            beamter.ResetData("bPlayer2Accepted");
            
        }

        [CommandPermission]
        [Command]
        public void wanteds(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.TeamId != (int) teams.TEAM_POLICE && dbPlayer.TeamId != (int) teams.TEAM_FIB &&
                dbPlayer.TeamId != (int) teams.TEAM_ARMY && dbPlayer.TeamId != (int)teams.TEAM_GOV) return;

            DialogMigrator.CreateMenu(player, Dialogs.menu_show_wanteds, "Gesuchte Personen", "");
            DialogMigrator.AddMenuItem(player, Dialogs.menu_show_wanteds, MSG.General.Close(), "");

            foreach (DbPlayer xPlayer in Players.Instance.GetValidPlayers())
            {
                if (CrimeModule.Instance.CalcJailTime(xPlayer.Crimes) > 0)
                {
                    int jailtime = CrimeModule.Instance.CalcJailTime(xPlayer.Crimes);
                    if (xPlayer.jailtime[0] > 0)
                    {
                        DialogMigrator.AddMenuItem(player, Dialogs.menu_show_wanteds,
                       "[INHAFTIERT]" + jailtime + "M - " + xPlayer.GetName(), "");
                    }
                    else
                    {
                        DialogMigrator.AddMenuItem(player, Dialogs.menu_show_wanteds,
                           "" + jailtime + "M - " + xPlayer.GetName(), "");
                    }
                }
            }

            DialogMigrator.OpenUserMenu(dbPlayer, Dialogs.menu_show_wanteds);
        }

        [CommandPermission]
        [Command]
        public void vehpark(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;
            if (dbPlayer.TeamId != (int) teams.TEAM_DPOS || !dbPlayer.IsInDuty()) return;

            if (dbPlayer.Player.Position.DistanceTo(new Vector3(401.34, -1631.674, 29.29195)) < 10.0f ||
                (dbPlayer.Player.Position.DistanceTo(new Vector3(533.1231, -179.434, 54.38259)) < 10.0f) ||
                 dbPlayer.Player.Position.DistanceTo(new Vector3(-3157.27, 1130.49, 20.8484)) < 10.0f ||
                 dbPlayer.Player.Position.DistanceTo(new Vector3(-803.674, -1505.27, 0.856)) < 10.0f ||
                 dbPlayer.Player.Position.DistanceTo(new Vector3(2124.04, 4800.71, 41.5033)) < 10.0f ||
                 dbPlayer.Player.Position.DistanceTo(new Vector3(-441.188, 6143.38, 31.4783)) < 10.0f ||
                 dbPlayer.Player.Position.DistanceTo(new Vector3(1669.1, 3826.99, 34.889)) < 10.0f ||
                 dbPlayer.Player.Position.DistanceTo(new Vector3(716.099, -1384.25, 26.404)) < 10.0f )
            {
                foreach (var Vehicle in VehicleHandler.Instance.GetAllVehicles())
                {
                    if (Vehicle == null || Vehicle.teamid == (int) teams.TEAM_DPOS) continue;
                    if (dbPlayer.Player.Position.DistanceTo(Vehicle.entity.Position) < 5.0f)
                    {
                        if (Vehicle.IsPlayerVehicle() && Vehicle.databaseId > 0)
                        {
                            Vehicle.SetPrivateCarGarage(1, 31);
                            dbPlayer.SendNewNotification(
                                "Fahrzeug wurde verwahrt! (Provision 1000$)");
                            dbPlayer.GiveMoney(1000);
                        }
                        else if (Vehicle.IsTeamVehicle())
                        {
                            Vehicle.SetTeamCarGarage(true);

                            if (Vehicle.teamid != (int) teams.TEAM_DPOS)
                            {
                                dbPlayer.SendNewNotification(
                                    "Fahrzeug wurde verwahrt! (Provision 1000$)");
                                dbPlayer.GiveMoney(1000);
                            }
                        }
                        else
                        {
                            VehicleHandler.Instance.DeleteVehicleByEntity(Vehicle.entity);
                            dbPlayer.SendNewNotification("Fahrzeug wurde verwahrt! (Provision 500$)");
                            dbPlayer.GiveMoney(500);
                        }
                    }
                }
            }
        }

        [CommandPermission]
        [Command]
        public async void seat(Player player, string commandParams)
        {
            
                try
                {
                    var dbPlayer = player.GetPlayer();
                    if (!dbPlayer.CanAccessMethod()) return;
                    if (dbPlayer.IsCuffed || dbPlayer.IsTied) return;

                    var command = commandParams.Split(new[] { ' ' }, 1, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
                    if (command.Length <= 0) return;
                    if (!int.TryParse(command[0], out int seatLocation)) return;

                    if (seatLocation == -1) return;
                    if (dbPlayer.Player.IsInVehicle && seatLocation > -2 && seatLocation < 15)
                    {
                        if (dbPlayer.Player.Vehicle.IsSeatFree(seatLocation))
                        {
                            dbPlayer.ChangeSeat(seatLocation);
                        }
                    }
                }
                catch(Exception e)
                {
                    Logger.Crash(e);
                }
            
        }

        [CommandPermission]
        [Command]
        public async void checkwanted(Player player, string name)
        {
            
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;
                if (!dbPlayer.IsACop()) return;

                var findPlayer = Players.Instance.FindPlayer(name);
                if (findPlayer == null) return;

                dbPlayer.SendNewNotification(dbPlayer.GetName() + " Wanteds:" + dbPlayer.wanteds[0]);
            
        }
        
        [CommandPermission]
        [Command]
        public void buyhouse(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.hasPerso[0] == 0)
            {
                dbPlayer.SendNewNotification(
                    "Um ein Haus zu kaufen, benoetigen Sie einen Personalausweis!");
                return;
            }
            
            if (dbPlayer.ownHouse[0] != 0)
            {
                dbPlayer.SendNewNotification("Sie besitzten bereits ein Haus!");
                return;
            }

            if (!dbPlayer.HasData("houseId")) return;
            uint houseId = dbPlayer.GetData("houseId");
            var house = HouseModule.Instance.Get(houseId);

            if (house == null) return;
            if (house.OwnerId == 0)
            {
                if (house.Type == 9) return;
                var price = house.Price;

                if (price < 0) return;

                if (!dbPlayer.TakeMoney(price))
                {
                    dbPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(house.Price));
                    return;
                }
                
                if(dbPlayer.IsTenant()) dbPlayer.RemoveTenant();
                HouseKeyHandler.Instance.DeleteAllHouseKeys(house);

                dbPlayer.SendNewNotification("Sie haben diese Immobilie fuer " + price + "$ erworben.", title: "", notificationType: PlayerNotification.NotificationType.SUCCESS);
                house.OwnerName = dbPlayer.GetName();
                house.OwnerId = dbPlayer.Id;
                dbPlayer.ownHouse[0] = house.Id;
                house.SaveOwner();
                dbPlayer.Save();
            }
            else
            {
                dbPlayer.SendNewNotification("Das Haus hat bereits einen Besitzer.", title: "Fehler", notificationType: PlayerNotification.NotificationType.ERROR);
            }
        }

        //TODO: Refelction
        [CommandPermission]
        [Command]
        public void spende(Player player, string commandParams)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;


                var command = commandParams.Split(new[] { ' ' }, 1, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

                if (command.Length <= 0) return;

                if (!int.TryParse(command[0], out int betrag)) return;
                if (betrag <= 0 || betrag > 9999999)
                {
                    dbPlayer.SendNewNotification("Ungueltiger Betrag");
                    return;
                }

                if (!dbPlayer.TakeMoney(betrag))
                {
                    dbPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(betrag));
                    return;
                }

                Logger.AddSpendeLog(dbPlayer.Id, betrag);

                KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, betrag);

                foreach (var user in Players.Instance.GetValidPlayers())
                {
                    if (!user.IsValid()) continue;
                    if (user.Player.Dimension == player.Dimension)
                    {
                        if (user.Player.Position.DistanceTo(dbPlayer.Player.Position) < 10.0f)
                        {
                            user.SendNewNotification("* " + dbPlayer.Player.Name + " hat $" + betrag +
                                                  " an den Staat gespendet!", title: "", notificationType: PlayerNotification.NotificationType.SUCCESS);
                        }
                    }
                }
            }));
        }

        [CommandPermission(AllowedDeath = true)]
        [Command(GreedyArg = true)]
        public void report(Player player, string reason = "")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (reason.Equals(""))
            {
                dbPlayer.SendNewNotification("Verwendung : /report [Grund]", PlayerNotification.NotificationType.ADMIN);
                return;
            }

            if (DateTime.Compare(dbPlayer.LastReport.AddMinutes(5), DateTime.Now) < 0)
            {
                String playerList = "";

                foreach (var user in Players.Instance.GetValidPlayers())
                {
                    if (user == null || !user.IsValid()) continue;
                    if (user.Id == dbPlayer.Id) continue;
                    if (user.Player.Dimension == player.Dimension)
                    {
                        if (user.Player.Position.DistanceTo(dbPlayer.Player.Position) < 50.0f)
                        {
                            if (!playerList.Equals("")) playerList += ",";

                            playerList += user.Id;
                        }
                    }
                }

                dbPlayer.SendNewNotification("Deine Nachricht wurde an die Administration gesendet und wurde zusätzlich registriert.", notificationType: PlayerNotification.NotificationType.ADMIN);

                if (playerList.Equals("")) return;

                Players.Instance.SendMessageToAuthorizedUsers("log", $"REPORT: [{dbPlayer.Player.Name}({dbPlayer.ForumId})]: {reason}", time: 10000);

                Logger.AddReportLog(dbPlayer.Id, playerList, reason);
                dbPlayer.LastReport = DateTime.Now;
                return;
            }

            dbPlayer.SendNewNotification("Du hast in den letzten 5 Minuten bereits einen Report abgesendet.", PlayerNotification.NotificationType.ADMIN);


        }

        [CommandPermission(AllowedDeath = true)]
        [Command(GreedyArg = true)]
        public void flimit(Player player)
        {
            var dbPlayer = player.GetPlayer();
            
            if (DateTime.Compare(dbPlayer.LastReport.AddMinutes(2), DateTime.Now) < 0)
            {
                uint myTeam = dbPlayer.TeamId;

                Dictionary<uint, int> playerLimitCounts = new Dictionary<uint, int>();

                foreach (var user in Players.Instance.GetValidPlayers())
                {
                    if (user == null || !user.IsValid()) continue;
                    if (user.Player.Dimension == player.Dimension)
                    {
                        if (user.Player.Position.DistanceTo(dbPlayer.Player.Position) < 500.0f)
                        {
                            if (playerLimitCounts.ContainsKey(user.TeamId))
                            {
                                playerLimitCounts[user.TeamId]++;
                            }
                            else playerLimitCounts.Add(user.TeamId, 1);
                        }
                    }
                }

                dbPlayer.SendNewNotification("Deine Nachricht wurde an die Administration gesendet und wurde zusätzlich registriert.", notificationType: PlayerNotification.NotificationType.ADMIN);

                Logger.AddFlimitReport(dbPlayer.Id, NAPI.Util.ToJson(playerLimitCounts));
                dbPlayer.LastReport = DateTime.Now;
                return;
            }

            dbPlayer.SendNewNotification("Du hast in den letzten 2 Minuten bereits einen Report abgesendet.", PlayerNotification.NotificationType.ADMIN);


        }
        
        [CommandPermission]
        [Command]
        public void tognews(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            if (Main.newsActivated(player))
            {
                dbPlayer.SendNewNotification(
                    "Newschat deaktiviert! (/tognews um ihn wieder zu aktivieren)");
                Main.setNewsActivated(player, false);
            }
            else
            {
                dbPlayer.SendNewNotification(
                    "Newschat aktiviert! (/tognews um ihn wieder zu deaktivieren)");
                Main.setNewsActivated(player, true);
            }
        }
        
        [CommandPermission]
        [Command]
        public void rob(Player player)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

                if (HalloweenModule.isActive) return;

                if (dbPlayer.Player.IsInVehicle) return;

                if (dbPlayer.IsACop())
                {
                    dbPlayer.SendNewNotification("Als Beamter koennen Sie keinen Raub begehen!");
                    return;
                }

                if (dbPlayer.Player.IsInVehicle)
                {
                    DBLogging.LogAdminAction(player, player.Name, adminLogTypes.kick, "Shoprob aus dem Fahrzeug", 0,
                        Configuration.Instance.DevMode);
                    Module.Players.Players.Instance.SendMessageToAuthorizedUsers("log",
                        $"{player.Name} wurde gekickt. Grund: Shoprob aus dem Fahrzeug!");
                    dbPlayer.Kick("Shoprob aus dem Fahrzeug ist nicht erlaubt!");
                    return;
                }

                if (dbPlayer.Team.IsInTeamfight()) return;


                // Juwe
                if (dbPlayer.Player.Position.DistanceTo(new Vector3(-622.5494, -229.5598, 38.05706)) < 10.0f)
                {
                    if (!dbPlayer.IsAGangster() && !dbPlayer.IsBadOrga())
                    {
                        dbPlayer.SendNewNotification("Große Heists sind nur fuer Gangs/Mafien!");
                        return;
                    }

                    if (Configurations.Configuration.Instance.DevMode != true)
                    {
                        // Timecheck +- 30 min restarts
                        if (!RobberyModule.Instance.CanJuweRobbed())
                        {
                            dbPlayer.SendNewNotification(
                                "Es scheint als ob die Generatoren nicht bereit sind, das geht nicht. (mind 30 min vor und nach Restarts!)");
                            return;
                        }
                    }

                    if (RobberyModule.Instance.IsActive(RobberyModule.Juwelier) ||
                        RobberyModule.Instance.LastScenario.AddHours(2) > DateTime.Now ||
                        RobberyModule.Instance.Get(RobberyModule.Juwelier) != null)
                    {
                        dbPlayer.SendNewNotification(
                            "Der Juwelier wurde bereits ausgeraubt oder ist derzeit nicht verfügbar!");
                        return;
                    }

                    if (TeamModule.Instance.DutyCops < 30 && !Configurations.Configuration.Instance.DevMode)
                    {
                        dbPlayer.SendNewNotification(
                            "Es muessen mindestens 30 Beamte im Dienst sein!");
                        return;
                    }

                    var vtc = RobberyModule.Instance.ValidTeamScenario("Juwe", dbPlayer.Team.Id);
                    if (!vtc.check)
                    {
                        dbPlayer.SendNewNotification($"Sie sind noch auf der Fahndungsliste, nächste Möglichkeit am {vtc.lastrob}");
                        return;
                    }

                    if (dbPlayer.Player.Dimension != 0)
                    {
                        DBLogging.LogAdminAction(player, dbPlayer.GetName(), adminLogTypes.perm,
                            "Community-Ausschluss Juwelier Auto Cheat", 0,
                            Configurations.Configuration.Instance.DevMode);
                        Players.Instance.SendMessageToAuthorizedUsers("anticheat",
                            "Haus Bug Use " + dbPlayer.GetName());
                        dbPlayer.warns[0] = 3;
                        SocialBanHandler.Instance.AddEntry(dbPlayer.Player);
                        dbPlayer.SendNewNotification("ANTI CHEAT (IM SUPPORT MELDEN) (juwe)");
                        dbPlayer.Player.Kick("ANTI CHEAT (IM SUPPORT MELDEN)!");
                        return;
                    }

                    if (dbPlayer.Team.IsGangsters() || dbPlayer.IsBadOrga())
                    {
                        // Get Players For Respect
                        int playersAtRob = dbPlayer.Team.GetTeamMembers().Where(m => m.Player.Position.DistanceTo(dbPlayer.Player.Position) < 300f).Count();
                        dbPlayer.Team.TeamMetaData.AddRespect(playersAtRob * 80);
                        TeamModule.Instance.SendMessageToTeam("Durch den Überfall erhält ihr Team Ansehen! (" + playersAtRob * 80 + "P)", (teams)dbPlayer.Team.Id);
                    }
                    // Juewlierrob
                    RobberyModule.Instance.Add(RobberyModule.Juwelier, dbPlayer, 20, RobType.Juwelier);
                    dbPlayer.SendNewNotification("Sie rauben nun den Juewelier aus, je laenger Sie bleiben desto mehr bekommen Sie!");
                    // Messages
                    TeamModule.Instance.SendChatMessageToDepartments("An Alle Einheiten, ein Einbruch im Juwelier wurde gemeldet!");

                    RobberyModule.Instance.LastScenario = DateTime.Now;
                    RobberyModule.Instance.SetTeamScenario("Juwe", dbPlayer.Team.Id);
                    return;
                }

                // Staatsbank
                if (dbPlayer.Player.Position.DistanceTo(StaatsbankRobberyModule.Instance.RobPosition) < 5.0f)
                {
                    if (dbPlayer.Player.Dimension != 0)
                    {
                        DBLogging.LogAdminAction(player, dbPlayer.GetName(), adminLogTypes.perm,
                            "Community-Ausschluss Staatsbank Auto Cheat", 0,
                            Configurations.Configuration.Instance.DevMode);
                        Players.Instance.SendMessageToAuthorizedUsers("anticheat",
                            "Haus Bug Use " + dbPlayer.GetName());
                        dbPlayer.warns[0] = 3;
                        SocialBanHandler.Instance.AddEntry(dbPlayer.Player);
                        dbPlayer.SendNewNotification("ANTI CHEAT (IM SUPPORT MELDEN) (staatsbank)", title: "ADMIN", notificationType: PlayerNotification.NotificationType.ADMIN);
                        dbPlayer.Player.Kick("ANTI CHEAT (IM SUPPORT MELDEN)!");
                        return;
                    }

                    // Juewlierrob
                    StaatsbankRobberyModule.Instance.StartRob(dbPlayer);
                    return;
                }


                // Vespucci Bank
                if (dbPlayer.Player.Position.DistanceTo(VespucciBankRobberyModule.Instance.RobPosition) < 2.0f)
                {
                    if (dbPlayer.Player.Dimension != 0)
                    {
                        DBLogging.LogAdminAction(player, dbPlayer.GetName(), adminLogTypes.perm,
                            "Community-Ausschluss VespucciBank Auto Cheat", 0,
                            Configurations.Configuration.Instance.DevMode);
                        Players.Instance.SendMessageToAuthorizedUsers("anticheat",
                            "Haus Bug Use VespucciBank " + dbPlayer.GetName());
                        dbPlayer.warns[0] = 3;
                        SocialBanHandler.Instance.AddEntry(dbPlayer.Player);
                        dbPlayer.SendNewNotification("ANTI CHEAT (IM SUPPORT MELDEN) (VespucciBank)", title: "ADMIN", notificationType: PlayerNotification.NotificationType.ADMIN);
                        dbPlayer.Player.Kick("ANTI CHEAT (IM SUPPORT MELDEN)!");
                        return;
                    }

                    VespucciBankRobberyModule.Instance.StartRob(dbPlayer);
                    return;
                }

                // Waffenfabrik
                if (dbPlayer.Player.Position.DistanceTo(WeaponFactoryRobberyModule.Instance.RobPosition) < 5.0f)
                {
                    if (dbPlayer.Player.Dimension != 0)
                    {
                        DBLogging.LogAdminAction(player, dbPlayer.GetName(), adminLogTypes.perm,
                            "Community-Ausschluss Juwelier Auto Cheat", 0,
                            Configurations.Configuration.Instance.DevMode);
                        Players.Instance.SendMessageToAuthorizedUsers("anticheat",
                            "Haus Bug Use " + dbPlayer.GetName());
                        dbPlayer.warns[0] = 3;
                        SocialBanHandler.Instance.AddEntry(dbPlayer.Player);
                        dbPlayer.SendNewNotification("ANTI CHEAT (IM SUPPORT MELDEN) (Waffenfabrik)", title: "ADMIN", notificationType: PlayerNotification.NotificationType.ADMIN);
                        dbPlayer.Player.Kick("ANTI CHEAT (IM SUPPORT MELDEN)!");
                        return;
                    }

                    // Juewlierrob
                    WeaponFactoryRobberyModule.Instance.StartRob(dbPlayer);
                    return;
                }
                
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("Hier können Sie nichts ausrauben!"));
            }));
        }
        
        [CommandPermission]
        [Command]
        public async void findrob(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            if (!dbPlayer.IsACop())
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            await AsyncCommands.Instance.HandleFindRob(dbPlayer);
        }

        [CommandPermission]
        [Command]
        public async void findhint(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            if (!dbPlayer.IsACop())
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            await AsyncCommands.Instance.HandleFindHint(dbPlayer);
        }

        [CommandPermission]
        [Command]
        public async void finddealer(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.TeamId != (int)teams.TEAM_FIB || !dbPlayer.IsInDuty())
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }
            await AsyncCommands.Instance.HandleFindDealer(dbPlayer);
        }

        [CommandPermission]
        [Command]
        public void ping(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            dbPlayer.SendNewNotification("Ping : " + dbPlayer.Player.Ping);

        }


        [CommandPermission]
        [Command]
        public void job(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            Jobs.Job xJob;
            if ((xJob = Jobs.JobsModule.Instance.GetThisJob(dbPlayer.Player.Position)) == null) return;

            if (xJob.disabled)
            {
                dbPlayer.SendNewNotification(
                    "Dieser Beruf ist derzeit nicht verfügbar!");
                return;
            }

            if (xJob.disablegang && dbPlayer.IsAGangster())
            {
                dbPlayer.SendNewNotification(
                    "Dieser Beruf ist nicht fuer Gangs/Mafien zulaessig!");
                return;
            }

            if (xJob.disablezivi && dbPlayer.TeamId == 0)
            {
                dbPlayer.SendNewNotification(
                    "Dieser Beruf ist nicht fuer Zivilisten zulaessig!");
                return;
            }

            if (xJob.Level > dbPlayer.Level)
            {
                dbPlayer.SendNewNotification(
                    "Sie haben nicht das benötigte Level fuer diesen Beruf!");
                return;
            }

            if (dbPlayer.job[0] != 0)
            {
                dbPlayer.SendNewNotification(
                    "Sie ueben bereits einen Beruf aus, /quitjob um diesen zu beenden!", title: "Job", notificationType: PlayerNotification.NotificationType.ERROR);
                return;
            }

            if (xJob.Legal)
            {
                if (dbPlayer.hasPerso[0] == 0)
                {
                    dbPlayer.SendNewNotification(
                        "Fuer legale Berufe benoetigen Sie einen Personalausweis.", title: "", notificationType: PlayerNotification.NotificationType.ERROR);
                    return;
                }

                dbPlayer.SendNewNotification("Beruf: " + xJob.Name, title: "", notificationType: PlayerNotification.NotificationType.SUCCESS);
            }
            else
            {
                dbPlayer.SendNewNotification("Beruf: " + xJob.Name, title: "", notificationType: PlayerNotification.NotificationType.ERROR);
            }

            DialogMigrator.CreateMenu(dbPlayer.Player, Dialogs.menu_jobaccept, "Job-System",
                "Job annehmen und beitreten");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_jobaccept, "Job annehmen",
                "Nimmt den Job an.");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_jobaccept, MSG.General.Close(), "");
            DialogMigrator.OpenUserMenu(dbPlayer, Dialogs.menu_jobaccept);
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public async Task gov(Player player, string govMessage)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.CanAccessMethod()) return;

            if ((dbPlayer.IsACop() || dbPlayer.TeamId == (uint)teams.TEAM_MEDIC) && dbPlayer.IsInDuty() && dbPlayer.TeamRank >= 8)
            {
                if (string.IsNullOrWhiteSpace(govMessage) || govMessage.Length < 2)
                {
                    dbPlayer.SendNewNotification(MSG.General.Usage("/gov", "[Message]"));
                    return;
                }
                await AsyncCommands.Instance.SendGovMessage(dbPlayer, govMessage);
            }
            if(dbPlayer.TeamId == (uint)teams.TEAM_CAYO && dbPlayer.TeamRank >= 10)
            {
                if (string.IsNullOrWhiteSpace(govMessage) || govMessage.Length < 2)
                {
                    dbPlayer.SendNewNotification(MSG.General.Usage("/gov", "[Message]"));
                    return;
                }
                await AsyncCommands.Instance.SendCayoMessage(dbPlayer, govMessage);
            }
        }
        
        [CommandPermission]
        [Command(Alias = "hilfe")]
        public void help(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            DialogMigrator.CreateMenu(dbPlayer.Player, Dialogs.menu_help, "Hilfe", "Befehle zu Fraktionen, Jobs...");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Allgemeine Hilfe", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Fraktions Hilfe", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Job Hilfe", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Haus Hilfe", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Fahrzeug Hilfe", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Inventar Hilfe", "");
            //DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Business Hilfe", "");
            if (dbPlayer.RankId > 0)
                DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, "Administrator Hilfe", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_help, MSG.General.Close(), "");

            DialogMigrator.OpenUserMenu(dbPlayer, Dialogs.menu_help);
        }
        
        [CommandPermission]
        [Command]
        public void jail(Player player)
        {

            try
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.CanAccessMethod()) return;
                if ((!dbPlayer.IsACop() || !dbPlayer.IsInDuty()) && dbPlayer.TeamId != (uint)teams.TEAM_FIB) return;

                MenuManager.Instance.Build(PlayerMenu.CrimeJailMenu, dbPlayer).Show(dbPlayer);
            }
            catch (Exception e)
            {
                Logging.Logger.Crash(e);
            }            
        }

        [CommandPermission]
        [Command]
        public async void quitjob(Player player)
        {
            
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

                dbPlayer.SendNewNotification(
                    "Wenn Sie ihren Job kuendigen, werden Sie ueber Zeit Erfahrungspunkte verlieren, bis Sie wieder in diesem Beruf arbeiten.");
                DialogMigrator.CreateMenu(dbPlayer.Player, Dialogs.menu_quitjob, "Kuendigung", "");
                DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_quitjob, "Kuendigung bestaetigen", "");
                DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_quitjob, MSG.General.Close(), "");
                DialogMigrator.OpenUserMenu(dbPlayer, Dialogs.menu_quitjob);
                return;
            
        }

        private static void IncorrectUsageOfServiceParameter(DbPlayer dbPlayer)
        {
            dbPlayer.SendNewNotification(
                MSG.General.Usage("/service",
                    "[police/medic/fahrlehrer/taxi/tow/news/cancel] [Grund/Nachricht]"));
            return;
        }

        [CommandPermission(AllowedDeath = true)]
        [Command(GreedyArg = true)]
        public async void support(Player player, string nachricht = " ")
        {
            
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;

                if (string.IsNullOrWhiteSpace(nachricht))
                {
                    dbPlayer.SendNewNotification(MSG.General.Usage("/support", "[Grund/Nachricht]"));
                    return;
            }
            
                string message = Regex.Replace(nachricht, @"[^a-zA-Z0-9\s]", "");
                message = message.Length == 0 ? "Sonderzeichen entfernt" : message;

                bool response = Support.TicketModule.Instance.Add(dbPlayer, new Support.Ticket(dbPlayer, message));

                if (response && !message.Equals("cancel"))
                {
                    AsyncCommands.Instance.HandleSupport(player.Name, dbPlayer.ForumId, nachricht);
                    dbPlayer.SendNewNotification("Deine Nachricht wurde an die Administration gesendet! Benutze \"/support cancel\" um die Anfrage wieder zu beenden.", duration:15000);
                }
                else
                {

                    if (message.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        Support.TicketModule.Instance.DeleteTicketByOwner(dbPlayer);
                        dbPlayer.SendNewNotification("Ihr Ticket wurde geschlossen!", notificationType:PlayerNotification.NotificationType.ADMIN);
                }
                    else
                    {
                        dbPlayer.SendNewNotification("Es ist bereits ein Ticket offen! Benutze \"/support cancel\" um die Anfrage wieder zu beenden.", PlayerNotification.NotificationType.ADMIN, duration: 10000);
                    }

                }
            
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void ooc(Player player, string oocText = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || dbPlayer.hasPerso[0] == 0 || !dbPlayer.CanAccessMethod()) return;

            if (string.IsNullOrWhiteSpace(oocText))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/ooc", "[Nachricht]"));
                return;
            }

            try
            {
                var surroundingUsers = Players.Instance.GetPlayersInRange(dbPlayer.Player.Position, 50.0f); // NAPI.Player.GetPlayersInRadiusOfPlayer(50.0f, dbPlayer.Player);

                foreach (DbPlayer user in surroundingUsers)
                {
                    if (user == null || !user.IsValid()) continue;

                    if (user.Player.Dimension == dbPlayer.Player.Dimension)
                    {
                        user.SendNewNotification(
                            oocText, title:$"OOC - ({dbPlayer.GetName()})", notificationType:PlayerNotification.NotificationType.OOC);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }

        [CommandPermission]
        [Command]
        public void vehunload(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (Configurations.Configuration.Instance.DevMode != true) return;
            if (dbPlayer.Player.Vehicle == null || !dbPlayer.Player.Vehicle.HasData("loadedVehicle")) return;

         //   Vehicle vehicle = dbPlayer.Player.Vehicle.GetData("loadedVehicle");
         //   if (vehicle == null) return;
        //    vehicle.Detach();
        //    dbPlayer.Player.Vehicle.ResetData("loadedVehicle");
        }

        [CommandPermission]
        [Command]
        public void buyinterior(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            House iHouse;
            if ((iHouse = HouseModule.Instance[dbPlayer.ownHouse[0]]) == null) return;
            if ((!(dbPlayer.Player.Position.DistanceTo(iHouse.Position) <= 5.0f)) &&
                (!dbPlayer.HasData("tempInt") || Main.CToInt(dbPlayer.GetData("tempInt")) <= 0)) return;
            DialogMigrator.CreateMenu(dbPlayer.Player, Dialogs.menu_shop_interior, "Innenausstattung", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_shop_interior, MSG.General.Close(), "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_shop_interior, "Interior kaufen",
                "Kauft das aktuell ausgewaehlte Interior");
            //DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_shop_interior, "Interior ansehen",
            //    "Damit kannst du das Interior frei anschauen");
            foreach (var kvp in InteriorModule.Instance.GetAll())
            {
                if (kvp.Value.Type == iHouse.Type)
                {
                    DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_shop_interior, kvp.Value.Comment + " $" + kvp.Value.Price,
                        "");
                }
            }

            DialogMigrator.OpenUserMenu(dbPlayer, Dialogs.menu_shop_interior);
        }

        /*
        [CommandPermission]
        [Command]
        public void fbank(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.DimensionType[0] != DimensionType.FSave) return;
            var teamShelter = Teams.Shelter.TeamShelterModule.Instance.GetByDimensionId(player.Dimension);
            if (teamShelter != null && dbPlayer.TeamId == teamShelter.Team.Id)
            {
                SynchronizedTaskManager.Instance.Add(new TeamShelterShowBankTask(dbPlayer, teamShelter));
            }
            else
            {
                dbPlayer.SendNewNotification(
                                         "Sie sind nicht im Gang Lagerraum!");
            }
        }*/

        [CommandPermission]
        [Command]
        public void spawnchange(Player player, string location = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (string.Equals(location.ToLower(), "haus"))
            {
                if (dbPlayer.ownHouse[0] > 0 || dbPlayer.IsTenant())
                {
                    dbPlayer.SendNewNotification(
                        "Sie spawnen nun an Ihrem Haus!");
                    dbPlayer.spawnchange[0] = 1;
                    return;
                }

                dbPlayer.SendNewNotification("Sie besitzen kein Haus / keine Wohnung!");
                return;
            }

            if (string.Equals(location.ToLower(), "fraktion"))
            {
                dbPlayer.spawnchange[0] = 0;
                dbPlayer.SendNewNotification(
                    "Sie spawnen nun an Ihrem Fraktionsort!");
                return;
            }

            dbPlayer.SendNewNotification(
                MSG.General.Usage("/spawnchange", "[Haus/Fraktion]"));
            return;
        }

        [CommandPermission]
        [Command]
        public async void givelic(Player player, string playerName = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (string.IsNullOrWhiteSpace(playerName))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/givelic", "[PlayerName]"), notificationType:PlayerNotification.NotificationType.SERVER);
                return;
            }

            await AsyncCommands.Instance.HandleGiveLic(player, playerName);
        }

        [CommandPermission]
        [Command]
        public async void takelic(Player player, string playerName = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/takelic", "[PlayerName]"), notificationType: PlayerNotification.NotificationType.SERVER);
                return;
            }

            await AsyncCommands.Instance.HandleTakeLic(player, playerName);
        }

        [CommandPermission]
        [Command]
        public async void grab(Player player, string playerName = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (string.IsNullOrWhiteSpace(playerName))
            {
                dbPlayer.SendNewNotification(MSG.General.Usage("/grab", "[playerName]"), notificationType: PlayerNotification.NotificationType.SERVER);
                return;
            }

            await AsyncCommands.Instance.HandleGrab(player, playerName);
        }

        [CommandPermission]
        [Command]
        public async Task drugtest(Player player, string commandParams)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            try
            {
                if (!dbPlayer.IsACop() && !dbPlayer.IsAMedic()) return;
                    if (string.IsNullOrWhiteSpace(commandParams))
                {
                    dbPlayer.SendNewNotification(MSG.General.Usage("/drugtest", "name"));
                    return;
                }

                var findPlayer = Players.Instance.FindPlayer(commandParams);

                if (findPlayer == null || findPlayer.Player.Position.DistanceTo(player.Position) >= 5.0f)
                {
                    dbPlayer.SendNewNotification(
                        "Person nicht gefunden oder außerhalb der Reichweite!");
                    return;
                }

                if (!findPlayer.IsCuffed && !findPlayer.IsTied)
                {
                    if (!findPlayer.isAlive())
                    {
                        dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                        return;
                    }
                }

                dbPlayer.SendNewNotification(
                    "Sie haben von " + findPlayer.GetName() +
                    " einen Drogenabstrich genommen!");
                findPlayer.SendNewNotification(
                    "Ein Beamter hat einen Drogenabstrich genommen!");

                await Task.Delay(5000);
                
                dbPlayer.SendNewNotification(
                    $"Drogentest von " + findPlayer.GetName() + " war " +
                    (findPlayer.Drugtest() ? "positiv" : "negativ") + "!");

                findPlayer.SendNewNotification(
                    $"Ihr Drogentest war " +
                    (findPlayer.Drugtest() ? "positiv" : "negativ") + "!");
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void taxi(Player player, string commandText = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            // wenn kein taxifahrer
            if (dbPlayer.Lic_Taxi[0] == 0 || dbPlayer.Lic_Transfer[0] == 0)
            {
                dbPlayer.SendNewNotification("Sie haben nicht die benötigten Lizenzen!");
                return;
            }
            if (dbPlayer.IsHomeless())
            {
                dbPlayer.SendNewNotification("Sie haben keinen offiziellen Wohnsitz und können daher kein Taxi fahren!");
                return;
            }
            if (string.IsNullOrWhiteSpace(commandText))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/taxi", "[Betrag|End]"));
                return;
            }

            if (commandText == "end")
            {
                dbPlayer.ResetData("taxi");
                dbPlayer.SendNewNotification(
                    "Du bist nun nicht mehr im Taxi-Dienst!");
                return;
            }

            var isNumeric = int.TryParse(commandText, out var betrag);

            if (isNumeric)
            {
                if (betrag < 200 || betrag > 999)
                {
                    dbPlayer.SendNewNotification("Ungueltiger Betrag! ($200-$999)");
                    return;
                }

                if (dbPlayer.HasData("taxi")) // Wenn bereits im Dienst dann setze den betrag neu
                {
                    dbPlayer.SetData("taxi", betrag);

                    dbPlayer.SendNewNotification(
                        "Du bist nun fuer $" + betrag + " im Taxi Dienst!");
                }

                if (dbPlayer.Player.Position.DistanceTo(new Vector3(895.7319, -178.6453, 74.70035)) <= 30.0f)
                {
                    dbPlayer.SetData("taxi", betrag);
                
                    dbPlayer.SendNewNotification(
                        "Du bist nun fuer $" + betrag + " im Taxi Dienst!");
                    dbPlayer.SendNewNotification(
                        "Sofern dich jemand ruft, wirst du automatisch Anfragen erhalten, diese kannst du per /acceptservice annehmen.");
                    dbPlayer.SendNewNotification(
                        "Du musst deine Fahrtkosten selbst einnehmen, zahlt jemand nicht, kannst du die Polizei verstaendigen.");
                    return;
                }
                else
                {
                    dbPlayer.SendNewNotification("Sie sind nicht an der Taxizentrale!");
                }
            }
            else
            {
                dbPlayer.SendNewNotification(
                    "Konnte Betrag nicht erkennen!");
            }
        }

        [CommandPermission]
        [Command]
        public void find(Player player, string name)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

                if (!dbPlayer.IsNSADuty &&
                !dbPlayer.FindFlags.HasFlag(FindFlags.Beamte) &&
                !dbPlayer.FindFlags.HasFlag(FindFlags.WithoutWarrant) &&
                !dbPlayer.FindFlags.HasFlag(FindFlags.Continuous))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(name) || name.Length <= 3)
                {
                    dbPlayer.SendNewNotification("Fehler beim Orten passiert...");
                    return;
                }

                var playerFromPool = Players.Instance.FindPlayer(name);
                if (!playerFromPool.IsValid())
                {
                    dbPlayer.SendNewNotification("Smartphone konnte nicht geortet werden... ");
                    return;
                }

                if (playerFromPool.IsOrtable(dbPlayer))
                {
                    NSAModule.Instance.HandleFind(dbPlayer, playerFromPool);

                    playerFromPool.SetData("isOrted_" + dbPlayer.TeamId, DateTime.Now.AddMinutes(1));

                    dbPlayer.SendNewNotification("Gesuchte Person " + playerFromPool.GetName() + " wurde geortet!");
                    NSAModule.Instance.SendMessageToNSALead($"{dbPlayer.GetName()} hat die Person {playerFromPool.GetName()} geortet!");

                    if(dbPlayer.IsNSADuty || (dbPlayer.TeamId == (int)teams.TEAM_FIB && dbPlayer.FindFlags.HasFlag(FindFlags.Continuous)))
                    {
                        dbPlayer.SetData("nsaOrtung", playerFromPool.Id);
                    }

                    Logging.Logger.AddFindLog(dbPlayer.Id, playerFromPool.Id);
                    return;
                }
                else
                {
                    dbPlayer.SendNewNotification("Smartphone konnte nicht geortet werden... ");
                    return;
                }
            }));
        }
        
        [CommandPermission]
        [Command]
        public void findhouse(Player player, string name)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                if (player == null)
                    return;

                var dbPlayer = player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

                if ((dbPlayer.TeamId != (int)teams.TEAM_FIB && dbPlayer.TeamId != (int)teams.TEAM_GOV) ||
                !dbPlayer.IsInDuty() ||
                (dbPlayer.TeamRank < 5 && dbPlayer.TeamId == (int)teams.TEAM_FIB) ||
                (dbPlayer.TeamRank <= 7 && dbPlayer.TeamId == (int)teams.TEAM_GOV))
                {
                    dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                    return;
                }

                if (string.IsNullOrWhiteSpace(name) || name.Length <= 0)
                {
                    dbPlayer.SendNewNotification("Fehler beim Orten passiert...", PlayerNotification.NotificationType.ERROR, title: "Einwohnermeldeamt");
                    return;
                }

                if(Int32.TryParse(name, out int houseId))
                {
                    House xHouse = HouseModule.Instance.Get((uint)houseId);
                    if (xHouse == null) return;
                    player.TriggerEvent("setPlayerGpsMarker", xHouse.Position.X, xHouse.Position.Y);
                    dbPlayer.SendNewNotification($"Haus mit der Nummer {houseId} wurde geortet!", PlayerNotification.NotificationType.SUCCESS, title: "Einwohnermeldeamt");
                    return;
                }
                else
                {
                    try
                    {
                        Vector3 Position = HouseModule.Instance.GetAll().Values.FirstOrDefault(house => house.OwnerName == name).Position;
                        if (Position == null || Position == new Vector3(0, 0, 0))
                        {
                            dbPlayer.SendNewNotification("Kein Eintrag im System gefunden.", PlayerNotification.NotificationType.ERROR, title: "Einwohnermeldeamt");
                            return;
                        }
                        Zone.Zone zone = ZoneModule.Instance.GetZone(Position);

                        dbPlayer.SendNewNotification($"Das Haus befindet sich in {zone.Name}.", PlayerNotification.NotificationType.SUCCESS, title: "Einwohnermeldeamt");
                    }
                    catch (Exception e)
                    {
                        Logger.Crash(e);
                    }
                }
            }));
        }

        [CommandPermission]
        [Command]
        public void phonehistory(Player player, string name)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.TeamId != (int) teams.TEAM_FIB && dbPlayer.TeamId != (int) teams.TEAM_POLICE && dbPlayer.TeamId != (int)teams.TEAM_FIB)
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if ((dbPlayer.TeamId == (int)teams.TEAM_FIB && !dbPlayer.IsNSADuty) && (dbPlayer.TeamId == (int)teams.TEAM_FIB && !dbPlayer.FindFlags.HasFlag(FindFlags.Phonehistory)))
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }
            
            if (string.IsNullOrWhiteSpace(name) || name.Length <= 3)
            {
                dbPlayer.SendNewNotification("Fehler beim finden passiert...");
                return;
            }

            var playerFromPool = Players.Instance.FindPlayer(name);
            if (playerFromPool == null || !playerFromPool.IsValid())
            {
                dbPlayer.SendNewNotification("Spieler nicht gefunden.");
                return;
            }

            // Für LSPD
            if (dbPlayer.TeamId == (int)teams.TEAM_POLICE)
            {
                if (dbPlayer.TeamRank < 10) return;
                if (playerFromPool.TeamId != (int)teams.TEAM_POLICE)
                {
                    dbPlayer.SendNewNotification("Sie können nur LSPD Mitglieder prüfen!");
                    return;
                }
            }

            dbPlayer.SetData("fib_phone_history", playerFromPool.Id);
            Menu.MenuManager.Instance.Build(PlayerMenu.FIBPhoneHistoryMenu, dbPlayer).Show(dbPlayer);
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void loadmap(Player player, string name)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod())
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            string mainPath = Configurations.Configuration.Instance.Ptr ? "C:\\MapsTest" : "C:\\MapsLive";

            foreach (string file in System.IO.Directory.EnumerateFiles(mainPath, name + ".xml"))
            {
                MapParserModule.Instance.ReadMap(file);
            }

            return;
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void unloadmap(Player player, string name)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod())
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }
            return;
        }
    
        /*
        [CommandPermission]
        [Command(GreedyArg = true)]
        public void house(Player player, string houseCommand = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            House iHouse;
            //if (string.IsNullOrWhiteSpace(houseCommand))
            
            if (!Main.validateArgs(houseCommand, 2))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/house", "withdraw", "Betrag"));
                return;
            }

            if (dbPlayer.ownHouse[0] == 0) return;

            var arg2 = houseCommand.Split(' ');

            var isNumeric = int.TryParse(arg2[1], out var betrag);
            if (!isNumeric)
            {
                dbPlayer.SendNewNotification("Ungueltiger Zahlenwert!");
                return;
            }

            var uses = Convert.ToString(arg2[0]);
            if ((iHouse = HouseModule.Instance.GetThisHouseFromPos(dbPlayer.Player.Position)) == null ||
                iHouse.Id != dbPlayer.ownHouse[0]) return;

            switch (uses.ToLower())
            {
                case "withdraw":
                    if (betrag > 0 && betrag <= iHouse.InventoryCash)
                    {
                        iHouse.InventoryCash -= betrag;
                        dbPlayer.GiveMoney(betrag);
                        dbPlayer.SendNewNotification(
                            "Sie haben " + betrag +
                            "$aus Ihrer Hauskasse entnommen.", title: "", notificationType: PlayerNotification.NotificationType.SUCCESS);
                        iHouse.SaveHouseBank();
                        dbPlayer.Save();
                        break;
                    }
                    else
                    {
                        dbPlayer.SendNewNotification("Ungueltiger Betrag!", title: "", notificationType: PlayerNotification.NotificationType.ERROR);
                        break;
                    }
                default:
                    dbPlayer.SendNewNotification(
                        MSG.General.Usage("/house", "withdraw", "Betrag"));
                    break;
            }
        }*/

        [CommandPermission]
        [Command]
        public void leitstelle(Player p_Player)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = p_Player.GetPlayer();
                if (dbPlayer == null) return;

                uint teamId = dbPlayer.TeamId;
                if(dbPlayer.IsNSADuty)
                {
                    teamId = (uint)teams.TEAM_IAA;
                }

                if (!LeitstellenPhoneModule.Instance.hasLeitstelleFunction(teamId)) return;


                TeamLeitstellenObject teamLeitstellenObject = LeitstellenPhoneModule.Instance.GetLeitstelle(teamId);
                if (teamLeitstellenObject == null) return;

                if(teamLeitstellenObject.Acceptor != null && teamLeitstellenObject.Acceptor.IsValid())
                {
                    if(teamLeitstellenObject.Acceptor.Id == dbPlayer.Id)
                    {
                        teamLeitstellenObject.Acceptor = null;
                        dbPlayer.SendNewNotification("Du hast die Einsatzleitung beendet!", title: "Info", notificationType: PlayerNotification.NotificationType.INFO);
                        dbPlayer.Player.TriggerEvent("clearcustommarks", CustomMarkersKeys.Leitstelle);
                        return;
                    }
                }

                teamLeitstellenObject.Acceptor = dbPlayer;

                // Send New EL to Team
                foreach (var l_Player in TeamModule.Instance[teamId].GetTeamMembers())
                {
                    if (l_Player == null || !l_Player.IsValid())
                        continue;

                    l_Player.SendNewNotification($"{dbPlayer.GetName()} ist nun Einsatzleiter deiner Fraktion.", title: "Einsatzleitung", notificationType: PlayerNotification.NotificationType.FRAKTION);
                }

                dbPlayer.SendNewNotification("Du hast nun die Einsatzleitung uebernommen", title: "Info", notificationType: PlayerNotification.NotificationType.INFO);
                return;
            }));
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void delnews(Player p_Player, string p_NewsID = "1")
        {
            var dbPlayer = p_Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (dbPlayer.TeamId != (uint) teams.TEAM_NEWS) return;

            if (uint.TryParse(p_NewsID, out uint l_ID)) new NewsListApp().deleteNews(l_ID);
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void staatskasse(Player p_Player, string p_Amount = "")
        {
            var l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.CanAccessMethod()) return;

            if (l_DbPlayer.TeamId != (int) teams.TEAM_GOV) return;
            if (l_DbPlayer.TeamRank < 11)
            {
                l_DbPlayer.SendNewNotification("Nur der Gouverneur oder der Premierminister kann auf die Staatskasse zugreifen.", notificationType:PlayerNotification.NotificationType.ERROR);
                return;
            }

            if (String.IsNullOrWhiteSpace(p_Amount))
            {
                l_DbPlayer.SendNewNotification($"Betrag : {KassenModule.Instance.GetMoney(KassenModule.Kasse.STAATSKASSE)}", title: "Staatskasse");
                return;
            }

            var l_Args = p_Amount.Split(' ');
            if (!int.TryParse(l_Args[1], out int l_Amount))
            {
                l_DbPlayer.SendNewNotification("Ungueltiger Betrag!");
                l_DbPlayer.SendNewNotification("Usage: /staatskasse [deposit/withdraw] betrag");
                return;
            }

            var l_Fbank = KassenModule.Instance.GetMoney(KassenModule.Kasse.STAATSKASSE);
            var l_Use = l_Args[0];

            switch (l_Use.ToLower())
            {
                case "deposit":
                    if (!l_DbPlayer.TakeMoney(l_Amount)) return;
                    l_DbPlayer.SendNewNotification($"Du hast {l_Amount.ToString()} in die Staatskasse eingezahlt.");
                    KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, +l_Amount);
                    Logger.SaveToStaatsKasse(l_Amount, l_DbPlayer.Id, l_DbPlayer.Player.Name, true);

                    break;
                case "withdraw":
                    if (KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, -l_Amount))
                    {
                        l_DbPlayer.GiveMoney(l_Amount);
                        l_DbPlayer.SendNewNotification($"Du hast {l_Amount.ToString()} aus der Staatskasse entnommen!");
                        Logger.SaveToStaatsKasse(l_Amount, l_DbPlayer.Id, l_DbPlayer.Player.Name, false);
                    }
                    break;
                default:
                    break;
            }
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void blackbudget(Player p_Player, string p_Amount = "")
        {
            var l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null || !l_DbPlayer.CanAccessMethod()) return;

            if (l_DbPlayer.TeamId != (int)teams.TEAM_FIB) return;
            if (l_DbPlayer.TeamRank != 11)
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(p_Amount))
            {
                l_DbPlayer.SendNewNotification($"Betrag : {KassenModule.Instance.GetMoney(KassenModule.Kasse.BLACKBUDGET)}", title: "Black Budget");
                return;
            }

            var l_Args = p_Amount.Split(' ');
            if (!int.TryParse(l_Args[1], out int l_Amount))
            {
                l_DbPlayer.SendNewNotification("Ungueltiger Betrag!");
                l_DbPlayer.SendNewNotification("Usage: /blackbudget [deposit/withdraw] betrag");
                return;
            }

            var l_Fbank = KassenModule.Instance.GetMoney(KassenModule.Kasse.BLACKBUDGET);
            var l_Use = l_Args[0];

            switch (l_Use.ToLower())
            {
                case "deposit":
                    if (!l_DbPlayer.TakeMoney(l_Amount)) return;
                    l_DbPlayer.SendNewNotification($"Du hast {l_Amount.ToString()} in das Black Budget eingezahlt.");
                    KassenModule.Instance.ChangeMoney(KassenModule.Kasse.BLACKBUDGET, +l_Amount);
                    Logger.SaveToStaatsKasse(l_Amount, l_DbPlayer.Id, l_DbPlayer.Player.Name, true);

                    break;
                case "withdraw":
                    if (KassenModule.Instance.ChangeMoney(KassenModule.Kasse.BLACKBUDGET, -l_Amount))
                    {
                        l_DbPlayer.GiveMoney(l_Amount);
                        l_DbPlayer.SendNewNotification($"Du hast {l_Amount.ToString()} aus dem Black Budget entnommen!");
                        Logger.SaveToStaatsKasse(l_Amount, l_DbPlayer.Id, l_DbPlayer.Player.Name, false);
                    }
                    break;
                default:
                    break;
            }
        }


        [CommandPermission]
        [Command(GreedyArg = true)]
        public void eventkasse(Player player, string eventDonation = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            var newsShelter = TeamShelterModule.Instance.GetByTeam((uint)teams.TEAM_NEWS);
            if (newsShelter == null)
                return;

            if (string.IsNullOrWhiteSpace(eventDonation) || !Main.validateArgs(eventDonation, 2, true))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/eventkasse", "[giveplayer/deposit]", "Betrag"));
                if (dbPlayer.TeamId == (int) teams.TEAM_NEWS)
                {
                    dbPlayer.SendNewNotification(
                        "Es befinden sich $" + newsShelter.Money +
                        " in der Eventkasse.");
                }

                return;
            }

            var arg2 = eventDonation.Split(' ');
            if (arg2.Length < 2) return;
            if (!int.TryParse(arg2[1], out var betrag))
            {
                dbPlayer.SendNewNotification("Ungueltiger Betrag!");
                return;
            }

            var uses = arg2[0];

            switch (uses.ToLower())
            {
                case "giveplayer":
                    if (!Main.validateArgs(eventDonation, 3))
                    {
                        dbPlayer.SendNewNotification(
                            MSG.General.Usage("/eventkasse", "[giveplayer]",
                                "[Betrag] [Spieler]"));
                        return;
                    }

                    if (dbPlayer.TeamId != (int) teams.TEAM_NEWS || dbPlayer.TeamRank < 8) return;
                    if (betrag > 0 && betrag <= newsShelter.Money)
                    {
                        var findPlayer = Players.Instance.FindPlayer(arg2[2]);

                        if (findPlayer == null || !findPlayer.IsValid())
                        {
                            dbPlayer.SendNewNotification(
                                "Person nicht gefunden!");
                            return;
                        }

                        newsShelter.Disburse(findPlayer, betrag);
                        findPlayer.Save();
                        dbPlayer.SendNewNotification(
                            "Sie haben " + betrag +
                            "$ aus der Eventkasse entnommen und " + findPlayer.GetName() +
                            " gegeben.", title: "", notificationType: PlayerNotification.NotificationType.SUCCESS);

                        Players.Instance.SendMessageToAuthorizedUsers("log",
                            dbPlayer.GetName() + " hat aus der Eventkasse $" + betrag + " an " +
                            findPlayer.GetName() + " gezahlt!");

                        Logger.SaveToEventKasse(betrag, dbPlayer.Id, dbPlayer.GetName(), false);
                        Logging.Logger.SaveToFbankLog(dbPlayer.TeamId, betrag, dbPlayer.Id, dbPlayer.GetName(), false);
                        break;
                    }
                    else
                    {
                        dbPlayer.SendNewNotification("Ungueltiger Betrag!");
                        break;
                    }
                case "deposit":
                    if (!(dbPlayer.Player.Position.DistanceTo(new Vector3(-608.6405, -938.4097, 23.85956)) <= 5.0f))
                    {
                        dbPlayer.SendNewNotification(
                            "Sie muessen an der Eventkasse sein!");
                        return;
                    }

                    if (!Main.validateArgs(eventDonation, 2))
                    {
                        dbPlayer.SendNewNotification(
                            MSG.General.Usage("/eventkasse",
                                "[giveplayer/deposit]",
                                "Betrag"));
                        return;
                    }

                    if (betrag <= 0)
                    {
                        dbPlayer.SendNewNotification("Ungueltiger Betrag!");
                        return;
                    }

                    if (!dbPlayer.TakeMoney(betrag))
                    {
                        dbPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(betrag));
                        return;
                    }

                    newsShelter.GiveMoney(betrag);
                    dbPlayer.SendNewNotification(
                        "Sie haben " + betrag +
                        "$ in die Eventkasse eingezahlt.", title: "Kasse", notificationType: PlayerNotification.NotificationType.SUCCESS);

                    Logger.SaveToEventKasse(betrag, dbPlayer.Id, dbPlayer.GetName(), true);
                    Logging.Logger.SaveToFbankLog(dbPlayer.TeamId, betrag, dbPlayer.Id, dbPlayer.GetName(), true);
                    return;
                default:
                    dbPlayer.SendNewNotification(
                        MSG.General.Usage("/eventkasse",
                            "[giveplayer/deposit]", "Betrag"));
                    break;
            }
        }
        
        [CommandPermission]
        [Command]
        public void resetfakename(Player p_Player)
        {
            var l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null) return;
            if (l_DbPlayer.TeamId != (int)teams.TEAM_FIB) return;

            l_DbPlayer.fakePerso = false;
            l_DbPlayer.fakeName = "";
            l_DbPlayer.fakeSurname = "";

            l_DbPlayer.SendNewNotification("Name zurückgesetzt!", notificationType: PlayerNotification.NotificationType.SUCCESS);
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void fakename(Player p_Player, string p_Name = " ")
        {
            var l_DbPlayer = p_Player.GetPlayer();
            if (l_DbPlayer == null) return;
            if (l_DbPlayer.TeamId != (int)teams.TEAM_FIB) return;
            if (l_DbPlayer.TeamRank == 0) return;

            if (p_Name == "")
                if (!p_Name.ToLower().Contains("_"))
                {
                    l_DbPlayer.SendNewNotification("Ungueltiger Name!", notificationType: PlayerNotification.NotificationType.ERROR);
                    return;
                }

            var l_Crumbs = p_Name.Split('_');

            if (Players.Instance.GetValidPlayers().Find(p => p.GetName().ToLower() == p_Name.ToLower()) != null)
            {
                l_DbPlayer.SendNewNotification("Dieser Name ist nicht verfuegbar!", notificationType: PlayerNotification.NotificationType.ERROR);
                return;
            }

            if (l_DbPlayer.HasData("lastFakeNameChange"))
            {
                DateTime lastChange = l_DbPlayer.GetData("lastFakeNameChange");
                if (lastChange.AddMinutes(10) > DateTime.Now)
                {
                    l_DbPlayer.SendNewNotification("Diese Aktion kann nur alle 10 Minuten ausgefuehrt werden!", notificationType: PlayerNotification.NotificationType.ERROR);
                    return;
                }
            }
            l_DbPlayer.SetData("lastFakeNameChange", DateTime.Now);

            l_DbPlayer.fakePerso = true;
            l_DbPlayer.fakeName = l_Crumbs[0];
            l_DbPlayer.fakeSurname = l_Crumbs[1];
            l_DbPlayer.SendNewNotification($"Name geändert zu {p_Name}", notificationType: PlayerNotification.NotificationType.SUCCESS);
            LogHandler.LogFakename(l_DbPlayer.Id, l_DbPlayer.Player.Name, l_DbPlayer.GetName());
        }


        #region JobCarMechanic

        [CommandPermission]
        [Command]
        public void hornsave(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;
            if (dbPlayer.TeamId != (int)teams.TEAM_LSC) return;

            if (!dbPlayer.Player.IsInVehicle) return;
            var sxVeh = dbPlayer.Player.Vehicle.GetVehicle();
            if (sxVeh.ownerId > 0 && sxVeh.jobid == 0)
            {
                var horn = 0;
                if (dbPlayer.HasData("n_horn"))
                {
                    horn = dbPlayer.Player.GetData<int>("n_horn");
                }

                if (horn < 0) return;
                if (dbPlayer.Container.GetItemAmount(244) <= 0)
                {
                    dbPlayer.SendNewNotification("Sie haben nicht die benoetigten Materialien! (Resonator)");
                    return;
                }

                dbPlayer.ResetData("hornCar");
                dbPlayer.ResetData("n_horn");
                sxVeh.AddSavedMod(14, horn);
                sxVeh.SaveMods();

                dbPlayer.SendNewNotification(
                                         "Die Hupe des Fahrzeuges wurde erfolgreich eingestellt!");
                dbPlayer.JobSkillsIncrease(5);
                dbPlayer.Container.RemoveItem(244, 1);
            }
            else
            {
                dbPlayer.SendNewNotification( "Dieses Fahrzeug ist kein Privatfahrzeug!");
            }
        }

        [CommandPermission]
        [Command]
        public void neonsave(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;
            if (dbPlayer.TeamId != (int)teams.TEAM_LSC) return;

            if (!dbPlayer.Player.IsInVehicle) return;
            var sxVeh = player.Vehicle.GetVehicle();

            if (sxVeh.ownerId > 0 && sxVeh.jobid == 0)
            {
                var color1 = 0;
                var color2 = 0;
                var color3 = 0;

                if (dbPlayer.HasData("n_color1") && dbPlayer.HasData("n_color2") &&
                    dbPlayer.HasData("n_color3"))
                {
                    color1 = dbPlayer.GetData("n_color1");
                    color2 = dbPlayer.GetData("n_color2");
                    color3 = dbPlayer.GetData("n_color3");
                }

                if (color1 >= 0 && color2 >= 0 && color3 >= 0)
                {
                    var l_Shelter = TeamShelterModule.Instance.GetAll().FirstOrDefault(s => s.Value.Team.Id == (uint)teams.TEAM_LSC).Value;
                    if (l_Shelter == null || l_Shelter.Team == null) return;
                    
                    if (l_Shelter.Money < 10000)
                    {
                        dbPlayer.SendNewNotification("Nicht genügend Geld auf der Fraktionskasse! Benötigt: 10000!");
                        return;
                    }
                    l_Shelter.TakeMoney(10000);

                    var neon = color1 + "," + color2 + "," + color3;
                    dbPlayer.ResetData("neonCar");
                    dbPlayer.ResetData("n_color1");
                    dbPlayer.ResetData("n_color2");
                    dbPlayer.ResetData("n_color3");

                    sxVeh.neon = neon;
                    sxVeh.SetNeon(neon);

                    dbPlayer.SendNewNotification(
                        "Die Neon Farben wurden erfolgreich eingestellt! Kosten $10.000");
                    dbPlayer.JobSkillsIncrease(5);

                    //Update DB
                    var query = String.Format("UPDATE `vehicles` SET Neon = '{0}' WHERE id = '{1}';", neon,
                        sxVeh.databaseId);
                    MySQLHandler.ExecuteAsync(query);

                    dbPlayer.Container.RemoveItem(245, 4);
                }
            }
            else
            {
                dbPlayer.SendNewNotification("Dieses Fahrzeug ist kein Privatfahrzeug!");
            }
        }


        /*
        [CommandPermission]
        [Command]
        public void perlsave(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;
            if (dbPlayer.job[0] != (int) jobs.JOB_MECH) return;

            if (!dbPlayer.Player.IsInVehicle) return;
            var sxVeh = player.Vehicle.GetVehicle();

            if (sxVeh.ownerId > 0 && sxVeh.jobid == 0)
            {
                var color1 = 0;
                var color2 = 0;

                if (dbPlayer.HasData("n_perl1") && dbPlayer.HasData("n_perl2"))
                {
                    color1 = player.GetData("n_perl1");
                    color2 = player.GetData("n_perl2");
                }

                if (color1 < 0 || color2 < 0) return;

                int count;

                if (sxVeh.Data.Price < 1000000)
                {
                    count = 10;
                }
                else if (sxVeh.Data.Price >= 1000000 && sxVeh.Data.Price < 2000000)
                {
                    count = 40;
                }
                else if (sxVeh.Data.Price >= 2000000 && sxVeh.Data.Price < 3000000)
                {
                    count = 60;
                }
                else
                {
                    count = 80;
                }

                if (ItemHandler.Instance.GetInventoryItem(dbPlayer.inventory[0],
                        ItemHandler.Instance.GetItemById(315)) < count)
                {
                    dbPlayer.SendNewNotification(
                                             $"Sie haben nicht die benoetigten Materialien! Es werden {count} benötigt.");
                    return;
                }

                dbPlayer.ResetData("perlCar");
                dbPlayer.ResetData("n_perl1");
                dbPlayer.ResetData("n_perl2");

                if (color1 > 0 && color2 > 0)
                {
                    sxVeh.AddMod(66, color1);
                    sxVeh.AddMod(67, color2);
                }
                else
                {
                    sxVeh.RemoveMod(66);
                    sxVeh.RemoveMod(67);
                }

                sxVeh.SaveMods();
                dbPlayer.SendNewNotification(
                                         "Der Perleffekt des Fahrzeuges wurde erfolgreich eingestellt!");

                dbPlayer.JobSkillsIncrease(5);
                dbPlayer.RemoveInventoryItem(ItemHandler.Instance.GetItemById(315), count);
                return;
            }

            dbPlayer.SendNewNotification( "Dieses Fahrzeug ist kein Privatfahrzeug!");
        }
        */

        [CommandPermission]
        [Command]
        public void changelock(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.job[0] != (int)jobs.JOB_MECH)
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            SxVehicle sxVeh = VehicleHandler.Instance.GetClosestVehicle(dbPlayer.Player.Position);
            if (sxVeh != null && sxVeh.IsValid())
            {
                if (sxVeh.ownerId > 0 && sxVeh.jobid == 0)
                {
                    var carparts = 5;
                    if (dbPlayer.Container.GetItemAmount(297) < carparts)
                    {
                        dbPlayer.SendNewNotification(
                            "Sie haben nicht die benoetigten Materialien!");
                        return;
                    }


                    DbPlayer vehOwner = Players.Instance.GetByDbId(sxVeh.ownerId);
                    if (vehOwner == null || !vehOwner.IsValid()) return;

                    if(vehOwner.Player.Position.DistanceTo(sxVeh.entity.Position) > 10.0f)
                    {
                        dbPlayer.SendNewNotification(
                            "Besitzer muss in der Nähe des Fahrzeuges sein!");
                        return;
                    }

                    dbPlayer.Container.RemoveItem(297, carparts);

                    VehicleKeyHandler.Instance.DeleteAllVehicleKeys(sxVeh.databaseId);
                    BusinessVehicle.Instance.DeleteAllVehicleKeys(sxVeh.databaseId);

                    // Delete Rents
                    MySQLHandler.ExecuteAsync($"DELETE FROM player_vehicle_rent WHERE `vehicle_id` = '{sxVeh.databaseId}';");
                    VehicleRentModule.PlayerVehicleRentKeys.RemoveAll(k => k.VehicleId == sxVeh.databaseId);

                    dbPlayer.SendNewNotification(
                        "Sie haben das Schloss des Fahrzeuges erfolgreich ausgetauscht!");
                    return;
                }

                dbPlayer.SendNewNotification(
                    "Dieses Fahrzeug ist kein Privatfahrzeug!");
                return;
            }
            else
            {
                var iHouse = HouseModule.Instance.GetThisHouse(dbPlayer.Player);
                if (iHouse != null)
                {
                    if (iHouse.Locked == false)
                    {
                        if (dbPlayer.Container.GetItemAmount(297) < 15)
                        {
                            dbPlayer.SendNewNotification(
                                "Sie haben nicht die benoetigten Materialien!");
                            return;
                        }

                        dbPlayer.Container.RemoveItem(297, 15);
                        HouseKeyHandler.Instance.DeleteAllHouseKeys(iHouse);
                        dbPlayer.SendNewNotification(
                            "Sie haben das Schloss des Hauses erfolgreich ausgetauscht!");
                        return;
                    }
                    else dbPlayer.SendNewNotification("Das Haus muss aufgeschlossen sein");
                }


                var storageRoom = StorageRoomModule.Instance.GetClosest(dbPlayer);
                if (storageRoom != null)
                {
                    if (storageRoom.Locked == false)
                    {
                        if (dbPlayer.Container.GetItemAmount(297) < 15)
                        {
                            dbPlayer.SendNewNotification(
                                "Sie haben nicht die benoetigten Materialien!");
                            return;
                        }

                        dbPlayer.Container.RemoveItem(297, 15);
                        StorageKeyHandler.Instance.DeleteAllStorageKeys(storageRoom);
                        BusinessStorage.Instance.DeleteAllStorageKeys(storageRoom.Id);
                        dbPlayer.SendNewNotification(
                            "Sie haben das Schloss des Lagerraumes erfolgreich ausgetauscht!");
                        return;
                    }
                    else dbPlayer.SendNewNotification("Der Lagerraum muss aufgeschlossen sein");
                }
            }
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void perlcar(Player player, string lack = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            // Deaktiviert
            /*if (dbPlayer.job[0] != (int) jobs.JOB_MECH)
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if (string.IsNullOrWhiteSpace(lack) || !Main.validateArgs(lack, 2))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/perlcar", "color1", "color2"));
                dbPlayer.SendNewNotification("Moegliche Farbwerte: 0 bis 74");
                return;
            }

            var arg2 = lack.Split(' ');
            if (!int.TryParse(arg2[0], out var color1) || !int.TryParse(arg2[1], out var color2))
            {
                dbPlayer.SendNewNotification("Die Farbe(n) wurde(n) nicht erkannt!");
                return;
            }

            if (color1 < 0 || color2 < 0 || color1 > 74 || color2 > 74)
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/perlcar", "color1", "color2"));
                dbPlayer.SendNewNotification("Moegliche Farbwerte: 0 bis 74");
                return;
            }

            if (!player.IsInVehicle) return;
            var sxVeh = player.Vehicle.GetVehicle();
            if (sxVeh.ownerId > 0 && sxVeh.jobid == 0)
            {
                if (dbPlayer.jobskill[0] < 5000)
                {
                    dbPlayer.SendNewNotification(
                        "Ihnen fehlt die notwenige Erfahrung!");
                    return;
                }

                dbPlayer.SetData("n_perl1", color1);
                dbPlayer.SetData("n_perl2", color2);

                if (color1 == 0 && color2 == 0)
                {
                    dbPlayer.SetData("perlCar", 0);
                    sxVeh.RemoveMod(66);
                    sxVeh.RemoveMod(67);

                    dbPlayer.SendNewNotification(
                        "Perleffekt entfernt!");
                }
                else
                {
                    dbPlayer.SetData("perlCar", 1);
                    sxVeh.SetMod(66, color1);
                    sxVeh.SetMod(67, color2);

                    dbPlayer.SendNewNotification(
                        "Sie haben den Perleffekt (Primaer ID " + color1 +
                        " / Sekundaer ID " + color2 + ") eingebaut!");
                }

                return;
            }

            dbPlayer.SendNewNotification(
                "Dieses Fahrzeug ist kein Privatfahrzeug!");*/
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void neoncar(Player player, string colors = " ")
        {
            DbPlayer iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;

            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if (string.IsNullOrWhiteSpace(colors) || !Main.validateArgs(colors, 3))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/neoncar", "R", "G B"));
                dbPlayer.SendNewNotification(
                    "Die Farbwerte muessen zwischen 0 und 255 liegen.");
                return;
            }

            var arg3 = colors.Split(' ');
            if (!int.TryParse(arg3[0], out var color1) || !int.TryParse(arg3[1], out var color2) ||
                !int.TryParse(arg3[2], out var color3))
            {
                dbPlayer.SendNewNotification("Die Farbe wurde nicht erkannt!");
                return;
            }

            if (color1 < 0 || color2 < 0 || color3 < 0 || color1 > 255 || color2 > 255 || color3 > 255)
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/neoncar", "Name", "Primaer Sekundaer"));
                dbPlayer.SendNewNotification(
                    "Die Farbwerte muessen zwischen 0 und 255 liegen.");
                return;
            }

            if (!player.IsInVehicle) return;

            var sxVeh = player.Vehicle.GetVehicle();
            var newneon = color1 + "," + color2 + "," + color3;
            if (sxVeh.ownerId > 0 && sxVeh.jobid == 0)
            {
                dbPlayer.SetData("neonCar", 1);
                dbPlayer.SetData("n_color1", color1);
                dbPlayer.SetData("n_color2", color2);
                dbPlayer.SetData("n_color3", color3);
                sxVeh.SetNeon(newneon);
                return;
            }

            dbPlayer.SendNewNotification(
                "Dieses Fahrzeug ist kein Privatfahrzeug!");
        }

        [CommandPermission]
        [Command]
        public void horncar(Player player, string hornCarId = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (!dbPlayer.Player.IsInVehicle) return;
            if (dbPlayer.TeamId != (int)teams.TEAM_LSC)
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if (string.IsNullOrWhiteSpace(hornCarId))
            {
                dbPlayer.SendNewNotification(MSG.General.Usage("/horncar", "ID"));
                dbPlayer.SendNewNotification(
                    "Hupen koennen zwischen 0 und 51 gewaehlt werden.");
                return;
            }

            if (!int.TryParse(hornCarId, out var horn))
            {
                dbPlayer.SendNewNotification("Die Hupe # wurde nicht erkannt!");
                return;
            }

            if (horn < 0 || horn > 51)
            {
                dbPlayer.SendNewNotification(MSG.General.Usage("/horncar", "ID"));
                dbPlayer.SendNewNotification(
                    "Hupen koennen zwischen 0 und 51 gewaehlt werden.");
                return;
            }

            var sxVeh = player.Vehicle?.GetVehicle();

            if (sxVeh == null || !sxVeh.IsPlayerVehicle())
            {
                dbPlayer.SendNewNotification(
                    "Dieses Fahrzeug ist kein Privatfahrzeug!");
                return;
            }


            dbPlayer.SetData("hornCar", 1);
            dbPlayer.SetData("n_horn", horn);

            sxVeh.entity.SetMod(14, horn);

            dbPlayer.SendNewNotification(
                "Sie haben die Hupe (ID " + horn + ") eingebaut!");
        }

        #endregion

        [CommandPermission]
        [Command]
        public void deletecar(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.CanAccessMethod()) return;

            if (!(dbPlayer.Player.Position.DistanceTo(new Vector3(-388.7151, -2282.08, 7.608183)) <= 10.0f) &&
                !(dbPlayer.Player.Position.DistanceTo(new Vector3(-432.7557, -2255.613, -0.08292308)) <= 10.0f)) return;
            if (!dbPlayer.Player.IsInVehicle) return;

            var sxVeh = player.Vehicle.GetVehicle();
            if (sxVeh == null) return;

            var price = VehicleShopModule.Instance.GetVehiclePriceFromHash(sxVeh.Data);
            if (price > 10000000) price = 0;
            if (sxVeh.ownerId == dbPlayer.Id && sxVeh.jobid == 0 && sxVeh.databaseId > 0)
            {
                Logger.AddToVehicleDestroyLog(sxVeh.databaseId, dbPlayer.Id, price);

                Main.DeletePlayerVehicle(dbPlayer, sxVeh);
                dbPlayer.GiveMoney(price);
                dbPlayer.SendNewNotification("Fahrzeug erfolgreich fuer $" + price + " verschrottet!");
                KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, price * 4);
                RegistrationOfficeFunctions.UpdateVehicleRegistrationToDb(sxVeh, dbPlayer, dbPlayer, sxVeh.plate, false);
                return;
            }

            if (sxVeh.Team.Id != 0)
            {
                if (dbPlayer.Team.IsGangsters())
                {
                    if (dbPlayer.TeamRankPermission.Manage >= 1)
                    {
                        Main.DeleteTeamVehicle(dbPlayer, sxVeh);
                        TeamShelter teamShelter = TeamShelterModule.Instance.GetByTeam(dbPlayer.Team.Id);
                        teamShelter.GiveMoney(price);
                        dbPlayer.SendNewNotification("Fahrzeug erfolgreich fuer $" + price + " verschrottet!");
                        string carName = sxVeh.Data.modded_car == 1 ? sxVeh.Data.mod_car_name : sxVeh.Data.Model;
                        dbPlayer.Team.SendNotification(dbPlayer.Player.Name + " hat das Fahrzeug " + carName + " verschrottet");
                        KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, price * 4);
                        RegistrationOfficeFunctions.UpdateVehicleRegistrationToDb(sxVeh, dbPlayer, dbPlayer, sxVeh.plate, false);
                        return;
                    }
                    else
                    {
                        dbPlayer.SendNewNotification("Du hast nicht die Befugnis die Fahrzeuge zu verschrotten");
                        return;
                    }
                }
                else
                {
                    dbPlayer.SendNewNotification("Du kannst die Fahrzeuge nicht verschrotten.");
                    return;
                }
            }
            dbPlayer.SendNewNotification("Sie sind nicht am Schrottplatz!");
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void sellcar(Player player, string commandText = " ")
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;
                if (dbPlayer == null || !dbPlayer.IsValid()) return;

                if (dbPlayer.job[0] != (int)jobs.JOB_Makler) return;
                if (!ServerFeatures.IsActive("makler-fahrzeuge"))
                {
                    dbPlayer.SendNewNotification("Diese Funktion ist derzeit deaktiviert. Weitere Informationen findest du im Forum.");
                    return;
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(commandText) || !Main.validateArgs(commandText, 4))
                    {
                        dbPlayer.SendNewNotification(
                            MSG.General.Usage("/sellcar", " Fahrzeug ID Besitzer", "Kunde Preis"));
                        return;
                    }

                    var arg3 = commandText.Split(' ');

                    DbPlayer Owner = null;
                    DbPlayer Customer = null;
                    //Get Player 1

                    if (!uint.TryParse(arg3[0], out uint l_VehID))
                    {
                        dbPlayer.SendNewNotification("Ungültige Fahrzeug ID!");
                        dbPlayer.SendNewNotification(MSG.General.Usage("/sellcar", " Fahrzeug ID Besitzer", "Kunde Preis"));
                        return;
                    }

                    Owner = Players.Instance.FindPlayer(arg3[1]);
                    Customer = Players.Instance.FindPlayer(arg3[2]);

                    if (Owner == null || Customer == null
                                        || Owner.Dimension[0] != dbPlayer.Dimension[0]
                                        || Customer.Dimension[0] != dbPlayer.Dimension[0]
                                        || dbPlayer.Player.Position.DistanceTo(Owner.Player.Position) >= 10.0f
                                        || dbPlayer.Player.Position.DistanceTo(Customer.Player.Position) >= 10.0f)
                    {
                        dbPlayer.SendNewNotification(
                            "Es konnten nicht alle Personen gefunden werden.");
                        return;
                    }

                    var vehicle = VehicleHandler.Instance.GetByVehicleDatabaseId(l_VehID);

                    if (vehicle == null || !vehicle.IsValid())
                    {
                        dbPlayer.SendNewNotification(
                            "Es konnte kein Fahrzeug in der Naehe gefunden werden!");
                    }

                    if (vehicle.Registered)
                    {
                        dbPlayer.SendNewNotification("Das Fahrzeug muss abgemeldet sein damit es verkauft werden kann.");
                        return;
                    }

                    if (!Owner.IsOwner(vehicle) || Owner.Player.Vehicle != vehicle.entity
                                                || vehicle.teamid >= 1
                                                || Owner.CurrentSeat != -1) return;

                    if (!int.TryParse(arg3[3], out var price))
                    {
                        dbPlayer.SendNewNotification("Ungueltiger Preis.");
                        return;
                    }

                    if (price > 100000000 || price < 0)
                    {
                        dbPlayer.SendNewNotification("Ungueltiger Preis.");
                        return;
                    }

                    if (price > 50000 && dbPlayer.jobskill[0] < 1000)
                    {
                        dbPlayer.SendNewNotification(
                            "Sie benötigen mindestens 1000 Skillpunkte bei einem Wert ueber $50.000");
                        return;
                    }

                    if (price > 100000 && dbPlayer.jobskill[0] < 2500)
                    {
                        dbPlayer.SendNewNotification(
                            "Sie benötigen mindestens 2500 Skillpunkte bei einem Wert ueber $100.000");
                        return;
                    }

                    if (price > 2000000 && dbPlayer.jobskill[0] < 5000)
                    {
                        dbPlayer.SendNewNotification(
                            "Sie benötigen mindestens 5000 Skillpunkte bei einem Wert ueber $2.000.000");
                        return;
                    }

                    var data = vehicle.Data;

                    if (data == null)
                    {
                        dbPlayer.SendNewNotification(
                            "Auto kann nicht verkauft werden.");
                        return;
                    }

                    if (price < data.Price / 2)
                    {
                        dbPlayer.SendNewNotification(
                            "Der Preis muss mindestens der haelfte des originalen Kaufpreises entsprechen.");
                        return;
                    }

                    Owner.SetData("mMakler_Owner", dbPlayer.Player.Name);
                    Customer.SetData("mMakler_Customer", dbPlayer.Player.Name);
                    dbPlayer.SetData("mMakler_CustomerName", Customer.Player.Name);
                    dbPlayer.SetData("mMakler_OwnerName", Owner.Player.Name);
                    dbPlayer.SetData("mOwnerAccepted", 0);
                    dbPlayer.SetData("mCustomerAccepted", 0);
                    dbPlayer.SetData("mPrice", price);
                    dbPlayer.SetData("mType", "Fahrzeug");
                    dbPlayer.SetData("mVehicleToSell", vehicle);

                    dbPlayer.SendNewNotification("Angebot an " + Customer.GetName() + " unterbreitet, Preis: $" + price);
                    Customer.SendNewNotification($"Makler {dbPlayer.GetName()} hat ihnen ein Angebot für das Fahrzeug {vehicle.databaseId.ToString()} ({vehicle.GetName()}) unterbreitet. Preis: $ {price.ToString()}. Benutzen Sie /acceptmakler um das Angebot anzunehmen.");
                    Owner.SendNewNotification("Makler " + dbPlayer.GetName() + " moechte Ihr Fahrzeug (" + vehicle.GetName() + ") an " + Customer.GetName() + " verkaufen, Preis: $" + price + " Sie bekommen davon $" + (price - ((int)(price / 10) * 2)) + " ausgezahlt. /acceptmakler um das Angebot anzunehmen.");
                }
                catch (Exception e)
                {
                    Logger.Crash(e);
                }
            }));
        }

       /* [CommandPermission]
        [Command(GreedyArg = true)]
        public void sellhouse(Player player, string command = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.job[0] != (int) jobs.JOB_Makler) return;
            if (!ServerFeatures.IsActive("makler-haus"))
            {
                dbPlayer.SendNewNotification("Diese Funktion ist derzeit deaktiviert. Weitere Informationen findest du im Forum.");
                return;
            }

            try
            {
                var arg2 = command.Split(' ');

                if (string.IsNullOrWhiteSpace(command) || !Main.validateArgs(command, 3))
                {
                    dbPlayer.SendNewNotification(
                        MSG.General.Usage("/sellhouse", "Besitzer", "Kunde Preis"));
                    return;
                }

                House iHouse;
                if ((iHouse = HouseModule.Instance.GetThisHouseFromPos(dbPlayer.Player.Position, true)) == null)
                    return;
                if (!int.TryParse(arg2[2], out var price))
                {
                    dbPlayer.SendNewNotification("Der Preis war nicht gueltig!");
                    return;
                }

                

                var Owner = Players.Instance.FindPlayer(arg2[0]);
                var Customer = Players.Instance.FindPlayer(arg2[1]);

                if (Owner == null || Customer == null
                                  || dbPlayer.Player.Position.DistanceTo(Owner.Player.Position) >= 10.0f
                                  || dbPlayer.Player.Position.DistanceTo(Customer.Player.Position) >= 10.0f)
                {
                    dbPlayer.SendNewNotification(
                        "Besitzer oder Kunde nicht gefunden, oder nicht in Reichweite!");
                    return;
                }

                if (iHouse.OwnerId != Owner.Id)
                {
                    dbPlayer.SendNewNotification(
                        "Verkaeufer nicht gefunden oder nicht an seinem Haus!");
                    return;
                }

                if (Customer.ownHouse[0] > 0)
                {
                    dbPlayer.SendNewNotification(
                        "Der Kunde besitzt bereits ein Haus!");
                    return;
                }

                Owner.SetData("mMakler_Owner", dbPlayer.GetName());
                Customer.SetData("mMakler_Customer", dbPlayer.GetName());
                dbPlayer.SetData("mMakler_CustomerName", Customer.GetName());
                dbPlayer.SetData("mMakler_OwnerName", Owner.GetName());
                dbPlayer.SetData("mOwnerAccepted", 0);
                dbPlayer.SetData("mCustomerAccepted", 0);
                dbPlayer.SetData("mPrice", price);
                dbPlayer.SetData("mType", "Haus");

                dbPlayer.SendNewNotification(
                    "Angebot an " + Customer.GetName() +
                    " unterbreitet, Preis: $" +
                    price);
                Customer.SendNewNotification(
                    "Makler " + dbPlayer.GetName() +
                    " hat Ihnen ein Angebot fuer diese Immobilie unterbreitet, Preis: $" + price);
                Customer.SendNewNotification(
                    "Benutzen Sie /acceptmakler um das Angebot anzunehmen.");
                Owner.SendNewNotification(
                    "Makler " + dbPlayer.GetName() + " moechte Ihr Haus an " +
                    Customer.GetName() + " verkaufen, Preis: $" + price);
                Owner.SendNewNotification(
                    "Sie bekommen davon $" +
                    (price - (price / 10 * 3)) +
                    " ausgezahlt. /acceptmakler um das Angebot anzunehmen.");
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }
       */
        [CommandPermission]
        [Command(GreedyArg = true)]
        public void sellstorage(Player player, string command = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.job[0] != (int) jobs.JOB_Makler) return;
            if (!ServerFeatures.IsActive("makler-lager"))
            {
                dbPlayer.SendNewNotification("Diese Funktion ist derzeit deaktiviert. Weitere Informationen findest du im Forum.");
                return;
            }

            try
            {
                var arg2 = command.Split(' ');

                if (string.IsNullOrWhiteSpace(command) || !Main.validateArgs(command, 3))
                {
                    dbPlayer.SendNewNotification(
                        MSG.General.Usage("/sellstorage", "Besitzer", "Kunde Preis"));
                    return;
                }

                StorageRoom storageRoom;
                if ((storageRoom = StorageRoomModule.Instance.GetClosest(dbPlayer)) == null)
                    return;
                if (!int.TryParse(arg2[2], out var price))
                {
                    dbPlayer.SendNewNotification("Der Preis war nicht gueltig!");
                    return;
                }

                if (price > 10000000 || price < storageRoom.Price)
                {
                    dbPlayer.SendNewNotification(
                        "Ungueltiger Preis, dieser muss mindestens ($" +
                        (storageRoom.Price) + ") des Hauspreises sein.");
                    return;
                }

                if (dbPlayer.jobskill[0] < 5000)
                {
                    dbPlayer.SendNewNotification(
                        "Sie benötigen mindestens 5000 Skillpunkte");
                    return;
                }

                var Owner = Players.Instance.FindPlayer(arg2[0]);
                var Customer = Players.Instance.FindPlayer(arg2[1]);

                if (Owner == null || Customer == null
                                  || dbPlayer.Player.Position.DistanceTo(Owner.Player.Position) >= 10.0f
                                  || dbPlayer.Player.Position.DistanceTo(Customer.Player.Position) >= 10.0f)
                {
                    dbPlayer.SendNewNotification(
                        "Besitzer oder Kunde nicht gefunden, oder nicht in Reichweite!");
                    return;
                }

                if (!Owner.GetStoragesOwned().ContainsKey(storageRoom.Id) || storageRoom.OwnerId != Owner.Id)
                {
                    dbPlayer.SendNewNotification("Verkaeufer nicht gefunden oder nicht an seinem Lagerraum!");
                    return;
                }

                if (Customer.GetStoragesOwned().Count >= StorageModule.Instance.LimitPlayerStorages)
                {
                    dbPlayer.SendNewNotification(
                        "Der Kunde besitzt bereits die maximale Anzahl!");
                    return;
                }

                Owner.SetData("mMakler_Owner", dbPlayer.GetName());
                Customer.SetData("mMakler_Customer", dbPlayer.GetName());
                dbPlayer.SetData("mMakler_CustomerName", Customer.GetName());
                dbPlayer.SetData("mMakler_OwnerName", Owner.GetName());
                dbPlayer.SetData("mOwnerAccepted", 0);
                dbPlayer.SetData("mCustomerAccepted", 0);
                dbPlayer.SetData("mPrice", price);
                dbPlayer.SetData("mType", "Storage");
                dbPlayer.SetData("objectId", storageRoom.Id);

                dbPlayer.SendNewNotification(
                    "Angebot an " + Customer.GetName() +
                    " unterbreitet, Preis: $" +
                    price);
                Customer.SendNewNotification(
                    "Makler " + dbPlayer.GetName() +
                    " hat Ihnen ein Angebot fuer diesen Lagerraum unterbreitet, Preis: $" + price);
                Customer.SendNewNotification(
                    "Benutzen Sie /acceptmakler um das Angebot anzunehmen.");
                Owner.SendNewNotification(
                    "Makler " + dbPlayer.GetName() + " moechte Ihr Lagerraum an " +
                    Customer.GetName() + " verkaufen, Preis: $" + price);
                Owner.SendNewNotification(
                    "Sie bekommen davon $" +
                    (price - (price / 10 * 3)) +
                    " ausgezahlt. /acceptmakler um das Angebot anzunehmen.");
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }

        [CommandPermission]
        [Command]
        public void acceptmakler(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (!dbPlayer.HasData("mMakler_Owner") && !dbPlayer.HasData("mMakler_Customer")) return;
            DbPlayer makler = null;

            // Verkaeufer
            if (dbPlayer.HasData("mMakler_Owner"))
            {
                string maklername = dbPlayer.GetData("mMakler_Owner");

                makler = Players.Instance.FindPlayer(maklername);

                if (makler == null)
                {
                    dbPlayer.SendNewNotification(
                        "Makler nicht gefunden!");
                    return;
                }

                if (makler.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f)
                {
                    dbPlayer.SendNewNotification(
                        "Makler nicht in Reichweite!");
                    return;
                }

                dbPlayer.SendNewNotification(
                    "Sie haben das Angebot angenommen.");

                makler.SetData("mOwnerAccepted", 1);
                makler.SendNewNotification(
                    "Angebot wurde von " + dbPlayer.GetName() + " angenommen!");
            }
            else if (dbPlayer.HasData("mMakler_Customer"))
            {
                string maklername = dbPlayer.GetData("mMakler_Customer");

                makler = Players.Instance.FindPlayer(maklername);

                if (makler == null)
                {
                    dbPlayer.SendNewNotification(
                        "Makler nicht gefunden, oder nicht in Reichweite!");
                    return;
                }

                if (makler.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f)
                {
                    dbPlayer.SendNewNotification(
                        "Makler nicht in Reichweite!");
                    return;
                }

                dbPlayer.SendNewNotification(
                    "Sie haben das Angebot angenommen.");

                makler.SetData("mCustomerAccepted", 1);
                makler.SendNewNotification(
                    "Angebot wurde von " + dbPlayer.GetName() + " angenommen!");
            }
            else
            {
                return;
            }

            int ownerAccept = makler.GetData("mOwnerAccepted");
            int customerAccept = makler.GetData("mCustomerAccepted");

            // Wenn Beide bestaetigt haben
            if (ownerAccept != 1 || customerAccept != 1) return;
            string type = makler.GetData("mType");

            int price = makler.GetData("mPrice");

            DbPlayer owner = null;
            DbPlayer customer = null;

            if (!makler.HasData("mMakler_OwnerName") ||
                !makler.HasData("mMakler_CustomerName")) return;

            string ownername = makler.GetData("mMakler_OwnerName");
            string customername = makler.GetData("mMakler_CustomerName");

            owner = Players.Instance.GetByName(ownername);
            customer = Players.Instance.GetByName(customername);

            if (owner == null || customer == null)
            {
                dbPlayer.SendNewNotification(
                    "Besitzer oder Kunde nicht gefunden, oder nicht in Reichweite!");
                return;
            }

            if (owner.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f) return;
            if (customer.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10.0f) return;


            switch (type)
            {
                // Haus wird verkauft
                case "Haus":
                {
                    House iHouse;
                    if ((iHouse = HouseModule.Instance.GetThisHouseFromPos(dbPlayer.Player.Position, true)) ==
                        null) return;

                    if (!customer.TakeBankMoney(price, $"Makler-Hauskauf - Haus {iHouse.Id}"))
                    {
                        dbPlayer.SendNewNotification(
                            "Der Kunde hat nicht genug Geld!");
                        customer.SendNewNotification(MSG.Money.NotEnoughMoney(price));
                        return;
                    }

                    // Haus switch Process
                    owner.ownHouse[0] = 0;
                    if(owner.IsTenant()) owner.RemoveTenant();
                    customer.ownHouse[0] = iHouse.Id;

                    HouseKeyHandler.Instance.DeleteAllHouseKeys(iHouse);
                    iHouse.OwnerId = customer.Id;
                    iHouse.OwnerName = customer.GetName();
                    iHouse.SaveOwner();

                    var provision = price / 10;

                    owner.GiveBankMoney(price - provision * 3, $"Makler-Hausverkauf - Haus {iHouse.Id}");
                    makler.GiveBankMoney(provision, $"Makler-Provision - Haus {iHouse.Id}");

                    KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, provision * 3);

                    owner.ResetData("mMakler_Owner");
                    customer.ResetData("mMakler_Customer");
                    makler.ResetData("mOwnerAccepted");
                    makler.ResetData("mCustomerAccepted");
                    makler.ResetData("mPrice");
                    makler.ResetData("mMakler_CustomerName");
                    makler.ResetData("mMakler_OwnerName");

                    makler.SendNewNotification(
                        "Sie haben das Haus erfolgreich verkauft! Ihre Provision $" + provision);
                    customer.SendNewNotification(
                        "Sie haben das Haus erfolgreich fuer $" +
                        price +
                        " erworben!");
                    owner.SendNewNotification(
                        "Ihr Haus wurde an " + customer.GetName() +
                        " fuer $" +
                        (price - provision * 3) + " verkauft!");
                    makler.JobSkillsIncrease(7);
                    KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, price - provision * 2);
                    return;
                }
                // Storage wird verkauft
                case "Storage":
                {
                    if (!makler.HasData("objectId")) return;

                    StorageRoom storageRoom = StorageRoomModule.Instance.Get(makler.GetData("objectId"));
                    if (storageRoom == null || makler.Player.Position.DistanceTo(storageRoom.Position) > 5.0f)
                    {
                        dbPlayer.SendNewNotification("Lager nicht gefunden oder in der nähe!");
                        return;
                    }

                    if (!customer.TakeBankMoney(price, $"Makler-Lagerkauf - Lager {storageRoom.Id}"))
                    {
                        dbPlayer.SendNewNotification(
                            "Der Kunde hat nicht genug Geld!");
                        customer.SendNewNotification(MSG.Money.NotEnoughMoney(price));
                        return;
                    }

                    // Haus switch Process
                    storageRoom.SetOwnerTo(customer);

                    var provision = price / 10;

                    owner.GiveBankMoney(price - provision * 3, $"Makler-Lagerverkauf - Lager {storageRoom.Id}");
                    makler.GiveBankMoney(provision, $"Makler-Provision - Lager {storageRoom.Id}");

                    KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, provision * 3);

                    owner.ResetData("mMakler_Owner");
                    customer.ResetData("mMakler_Customer");
                    makler.ResetData("mOwnerAccepted");
                    makler.ResetData("mCustomerAccepted");
                    makler.ResetData("mPrice");
                    makler.ResetData("mMakler_CustomerName");
                    makler.ResetData("mMakler_OwnerName");
                    makler.ResetData("objectId");

                        makler.SendNewNotification(
                        "Sie haben den Lagerraum erfolgreich verkauft! Ihre Provision $" + provision);
                    customer.SendNewNotification(
                        "Sie haben den Lagerraum erfolgreich fuer $" +
                        price +
                        " erworben!");
                    owner.SendNewNotification(
                        "Ihr Lagerraum wurde an " + customer.GetName() +
                        " fuer $" +
                        (price - provision * 3) + " verkauft!");
                    makler.JobSkillsIncrease(7);
                    KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, price - provision * 2);
                    return;
                }
                case "Fahrzeug":
                {
                    foreach (var vehicle in VehicleHandler.Instance.GetAllVehicles())
                    {
                        if (vehicle == null) continue;
                        if (!(dbPlayer.Player.Position.DistanceTo(vehicle.entity.Position) <= 10.0f)) continue;
                        if (!owner.IsOwner(vehicle) || makler.GetData("mVehicleToSell") != vehicle) continue;

                        if (vehicle.Registered)
                        {
                            dbPlayer.SendNewNotification("Das Fahrzeug muss abgemeldet sein damit es verkauft werden kann.");
                            return;
                        }
                        if (!owner.IsOwner(vehicle) || makler.GetData("mVehicleToSell") != vehicle) continue;
                        if (!customer.TakeBankMoney(price, $"Makler-Fahrzeugkauf - Modell: {vehicle.Data.Model}"))
                        {
                            dbPlayer.SendNewNotification(
                                "Der Kaeufer hat nicht genug Geld!");
                            customer.SendNewNotification(MSG.Money.NotEnoughMoney(price));
                            return;
                        }
                        // Vehicle switch Process
                        Main.ChangePlayerVehicleOwner(owner, customer, vehicle);

                        var provision = price / 10;

                        owner.GiveBankMoney(price - (provision * 2), $"Makler-Fahrzeugverkauf - Modell: {vehicle.Data.Model}");
                        makler.GiveBankMoney(provision, $"Makler-Provision - Modell: {vehicle.Data.Model}");

                        KassenModule.Instance.ChangeMoney(KassenModule.Kasse.STAATSKASSE, provision);

                        owner.ResetData("mMakler_Owner");
                        customer.ResetData("mMakler_Customer");
                        makler.ResetData("mOwnerAccepted");
                        makler.ResetData("mCustomerAccepted");
                        makler.ResetData("mPrice");
                        makler.ResetData("mMakler_CustomerName");
                        makler.ResetData("mMakler_OwnerName");
                        makler.ResetData("mVehicleToSell");

                        makler.SendNewNotification(
                            "Sie haben das Fahrzeug erfolgreich verkauft! Ihre Provision $" +
                            provision);
                        customer.SendNewNotification(
                            "Sie haben das Fahrzeug erfolgreich fuer $" +
                            price + " erworben!");
                        owner.SendNewNotification(
                            "Ihr Fahrzeug wurde an " +
                            customer.GetName() +
                            " fuer $" + (price - (provision * 2)) + " verkauft!");
                            RegistrationOfficeFunctions.GiveVehicleContract(customer, vehicle, owner.Player.Name);
                            makler.JobSkillsIncrease(3);
                        return;
                    }

                    return;
                }
            }
        }

        [CommandPermission]
        [Command]
        public void starttaxometer(Player player, string price = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (!dbPlayer.Player.IsInVehicle || dbPlayer.Player.VehicleSeat == -1) return;
            var vehicle = dbPlayer.Player.Vehicle.GetVehicle();
            if (vehicle.entity.GetModel() != VehicleHash.Taxi) return;
            if (vehicle.ownerId != dbPlayer.Id) return;

            if (!int.TryParse(price, out var priceInt))
            {
                dbPlayer.SendNewNotification("Der Preis war nicht gueltig!");
                return;
            }

            if (priceInt <= 0)
            {
                dbPlayer.SendNewNotification("Ungueltige Geldanzahl");
                return;
            }

            dbPlayer.SendNewNotification(
                "Taxometer fuer " + price + "$ gestartet.");
            dbPlayer.Player.TriggerEvent("startTaxometer", price);
        }

        [CommandPermission]
        [Command]
        public void stoptaxometer(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (!dbPlayer.Player.IsInVehicle || dbPlayer.Player.VehicleSeat == -1) return;
            var vehicle = dbPlayer.Player.Vehicle.GetVehicle();
            if (vehicle.entity.GetModel() != VehicleHash.Taxi) return;
            if (vehicle.ownerId != dbPlayer.Id) return;
            dbPlayer.SendNewNotification("Taxometer gestoppt.");
            dbPlayer.Player.TriggerEvent("stopTaxometer");
        }

        [CommandPermission]
        [Command]
        public void stoprob(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.GetData("doingJuwe"))
            {
                dbPlayer.StopAnimation();
                dbPlayer.ResetData("doingJuwe");
                Task x = dbPlayer.GetData("doingJuweT");
                dbPlayer.ResetData("doingJuweT");
                x?.Dispose();
            }
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public async void news(Player player, string newsText = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.TeamId != (int) teams.TEAM_NEWS)
            {
                return;
            }

            if (!Main.newsActivated(player))
            {
                dbPlayer.SendNewNotification(
                    "Sie muessen den Newschat zuerst aktivieren!");
                return;
            }

            if (string.IsNullOrWhiteSpace(newsText))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/news", "Nachricht"));
                return;
            }

            await AsyncCommands.Instance.HandleNews(newsText);
        }

        [CommandPermission]
        [Command]
        public void weed(Player player, string weedCommand = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.IsACop() && dbPlayer.RankId == (int) adminlevel.Player) return;
            if (string.IsNullOrWhiteSpace(weedCommand))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/weed", "[plant/harvest/grind]"));
                return;
            }

            if (!string.Equals(weedCommand, "grind")) return;
            //Grind
            if (dbPlayer.DimensionType[0] != DimensionType.House) return;

            //paper = 158
            //plastiktuete = 6
            //grinded weed = 8
            //weed = 19

            if (dbPlayer.Container.CanInventoryItemAdded(8, 1))
            {
                if (dbPlayer.Container.GetItemAmount(6) == 0 || dbPlayer.Container.GetItemAmount(19) < 5)
                {
                    dbPlayer.SendNewNotification("Du benoetigst 5 Weed & eine Plastiktuete um das Gras zu grinden und zu verpacken!");
                    return;
                }

                dbPlayer.Container.RemoveItem(6, 1);
                dbPlayer.Container.RemoveItem(19, 5);

                dbPlayer.Container.AddItem(8, 1);
                dbPlayer.SendNewNotification("Du hast eine Tuete Grindedweed aus 5 Weed hergestellt!");
                return;
            }
            else
            {
                dbPlayer.SendNewNotification("Dein Inventar ist voll!");
            }
        }

        [CommandPermission]
        [Command]
        public void fspawnchange(Player player, string spawnCommand = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (dbPlayer.TeamId != (int) teams.TEAM_MEDIC && dbPlayer.TeamId != (int) teams.TEAM_POLICE) return;
            if (string.IsNullOrWhiteSpace(spawnCommand))
            {
                if (dbPlayer.TeamId == (int) teams.TEAM_POLICE)
                    dbPlayer.SendNewNotification(
                        MSG.General.Usage("/fspawnchange", "[LS/County/LS2]"));
                if (dbPlayer.TeamId == (int) teams.TEAM_MEDIC)
                    dbPlayer.SendNewNotification(
                        MSG.General.Usage("/fspawnchange", "[LS/LS2/County/Fire]"));
                return;
            }

            if (string.Equals(spawnCommand.ToLower(), "ls"))
            {
                dbPlayer.SendNewNotification("Sie spawnen nun am LS Department!");
                dbPlayer.fspawn[0] = 0;
                return;
            }

            if (string.Equals(spawnCommand.ToLower(), "county"))
            {
                dbPlayer.fspawn[0] = 1;
                dbPlayer.SendNewNotification(
                    "Sie spawnen nun am County Department!");
                return;
            }

            if (string.Equals(spawnCommand.ToLower(), "ls2"))
            {
                dbPlayer.fspawn[0] = 2;
                dbPlayer.SendNewNotification("Sie spawnen nun am LS2 HQ!");
                return;
            }

            if (string.Equals(spawnCommand.ToLower(), "fire") && dbPlayer.TeamId == (int) teams.TEAM_MEDIC)
            {
                dbPlayer.fspawn[0] = 3;
                dbPlayer.SendNewNotification(
                    "Sie spawnen nun am Fire Department!");
                return;
            }
        }

        [CommandPermission]
        [Command]
        public void friskhouse(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null) return;

            if (!iPlayer.CanAccessMethod())
            {
                iPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if (iPlayer.TeamId != (int)teams.TEAM_FIB || !iPlayer.IsInDuty()) return;

            if (iPlayer.DimensionType[0] == DimensionType.House && iPlayer.HasData("inHouse"))
            {
                ItemsModuleEvents.resetFriskInventoryFlags(iPlayer);
                ItemsModuleEvents.resetDisabledInventoryFlag(iPlayer);

                House house;
                if ((house = HouseModule.Instance.Get(iPlayer.GetData("inHouse"))) != null)
                {
                    house.Container.ShowHouseFriskInventory(iPlayer, iPlayer.Player.Dimension);
                    Logger.SaveToFriskHouseLog(iPlayer.Id, (int)house.Id, iPlayer.GetName());
                }
            }
        }
        
        [CommandPermission]
        [Command]
        public void createlic(Player player, string createlicCommand = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            Job xjob;
            xjob = JobsModule.Instance.GetJob(dbPlayer.job[0]);
            if (dbPlayer.job[0] != (int) jobs.JOB_PLAGIAT)
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if (string.IsNullOrWhiteSpace(createlicCommand))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/createlic", "Name"));
                return;
            }

            var findPlayer = Players.Instance.FindPlayer(createlicCommand);

            if (findPlayer == null)
            {
                dbPlayer.SendNewNotification("Buerger nicht gefunden");
                return;
            }

            if (findPlayer.Player.Position.DistanceTo(dbPlayer.Player.Position) > 5.0f)
            {
                dbPlayer.SendNewNotification(MSG.General.notInRange);
                return;
            }

            dbPlayer.SetData("fakeLic", findPlayer);

            DialogMigrator.CreateMenu(dbPlayer.Player, Dialogs.menu_job_createlicenses, "Lizenzen",
                "Zum erstellen von Plagiaten (Lizenzen)");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.Car + " " + JobContent.Plagiat.Materials.Car + " Materialien",
                "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.Lkw + " " + JobContent.Plagiat.Materials.Lkw + " Materialien",
                "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.Bike + " " + JobContent.Plagiat.Materials.Bike + " Materialien",
                "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.Boot + " " + JobContent.Plagiat.Materials.Boot + " Materialien",
                "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.PlaneA + " " + JobContent.Plagiat.Materials.PlaneA +
                " Materialien", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.PlaneB + " " + JobContent.Plagiat.Materials.PlaneB +
                " Materialien", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.Biz + " " + JobContent.Plagiat.Materials.Biz + " Materialien",
                "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.Gun + " " + JobContent.Plagiat.Materials.Gun + " Materialien",
                "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses,
                Content.License.Transfer + " " + JobContent.Plagiat.Materials.Transfer +
                " Materialien", "");
            DialogMigrator.AddMenuItem(dbPlayer.Player, Dialogs.menu_job_createlicenses, "Menu schließen", "");
            DialogMigrator.OpenUserMenu(dbPlayer, Dialogs.menu_job_createlicenses);
        }
        
        [CommandPermission()]
        [Command]
        public void dropguns(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;
            if (!dbPlayer.CanInteract() || dbPlayer.Player.IsInVehicle) return;

            if (dbPlayer.HasData("no-packgun") || dbPlayer.HasData("do-packgun")) return;


            var weapons = dbPlayer.Weapons;

            Main.m_AsyncThread.AddToAsyncThread(new Task(async () =>
            {
                dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@mp_snowball", "pickup_snowball");
                dbPlayer.Player.TriggerEvent("freezePlayer", true);
                await Task.Delay(5000);
                dbPlayer.Player.TriggerEvent("freezePlayer", false);
                dbPlayer.StopAnimation();
                dbPlayer.RemoveWeapons();
                dbPlayer.ResetAllWeaponComponents();
            }));
            dbPlayer.SaveWeapons();
        }
                
        private static void PlayerNotFoundOrNoService(DbPlayer dbPlayer, bool emergency = false)
        {
            dbPlayer.SendNewNotification(
                $"Der Buerger konnte nicht gefunden werden " +
                "oder hat keine" + (emergency ? "n Notruf" : " Anfrage") + " gesendet!");
        }

        private static void GetPlayerpositionAndInformService(DbPlayer findPlayer, DbPlayer dbPlayer,
            bool informTeam = false, bool emergency = false)
        {
            findPlayer.SendNewNotification(
                "Ihre" + (emergency ? "n Notruf" : " Anfrage") +
                "wurde entgegen genommen, warten Sie an ihrer aktuellen Position!");
            dbPlayer.SendNewNotification(
                $"Sie haben " + (emergency ? "den Notruf" : "die Anfrage") + " von " +
                findPlayer.GetName() +
                " entgegen genommen. (im GPS makiert)!");
            dbPlayer.SetWaypoint(findPlayer.Player.Position.X,
                findPlayer.Player.Position.Y);
            if (informTeam)
            {
                TeamModule.Instance.SendChatMessageToDutyTeamMembers(dbPlayer,
                    " hat " + (emergency ? "den Notruf" : "die Anfrage") + " von " + findPlayer.GetName() +
                    " angenommen.");
            }
            findPlayer.ResetData("service");
            findPlayer.ResetData("service_r");
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void buildweapon(Player player, string commandParameter = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (string.IsNullOrWhiteSpace(commandParameter))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/buildweapon", "[start/end]"));
                return;
            }

            var command = commandParameter.Split(new[] {' '}, 1, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

            if (dbPlayer.job[0] != (int) jobs.JOB_WEAPONDEALER || !dbPlayer.IsAGangster()) return;
            if (dbPlayer.DimensionType[0] != DimensionType.WeaponFactory)
            {
                dbPlayer.SendNewNotification(
                    "Sie muessen in einer Waffenfabrik sein!");
                return;
            }

            //Waffenshop
            DialogMigrator.CreateMenu(player, Dialogs.menu_weapondealer, "Waffenfabrik", "");
            DialogMigrator.AddMenuItem(player, Dialogs.menu_weapondealer, MSG.General.Close(), "");
            DialogMigrator.OpenUserMenu(dbPlayer, Dialogs.menu_weapondealer);
            return;
        }

        [CommandPermission]
        [Command(GreedyArg = true)]
        public void materials(Player player, string commandParameter = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (string.IsNullOrWhiteSpace(commandParameter))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/materials", "[get/deliver]"));
                return;
            }

            var command = commandParameter.Split(new[] {' '}, 1, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

            if (dbPlayer.job[0] == (int) jobs.JOB_PLAGIAT)
            {
                if (string.Equals(command[0], "get"))
                {
                    if (dbPlayer.Player.Position.DistanceTo(new Vector3(1439.223f, 6332.424f, 23.956f)) <= 2.0f)
                    {
                        if (dbPlayer.Container.CanInventoryItemAdded(25, 10))
                        {
                            if (!dbPlayer.TakeMoney(300))
                            {
                                dbPlayer.SendNewNotification(
                                    MSG.Money.NotEnoughMoney(300));
                                return;
                            }

                            dbPlayer.Container.AddItem(25, 10);
                            dbPlayer.SendNewNotification(
                                "10 Rohlingsvorlagen fuer 300$ gekauft!", title: "", notificationType: PlayerNotification.NotificationType.SUCCESS);
                            dbPlayer.JobSkillsIncrease();
                            return;
                        }
                        else
                        {
                            dbPlayer.SendNewNotification(
                                "Ihr Inventar ist voll!");
                            return;
                        }
                    }
                    return;
                }
                else if (string.Equals(command[0], "deliver"))
                {
                    Job xjob;
                    if ((xjob = JobsModule.Instance.GetJob((int) jobs.JOB_PLAGIAT)) != null)
                    {
                        if (dbPlayer.Player.Position.DistanceTo(new Vector3(xjob.Position.X, xjob.Position.Y, xjob.Position.Z)) <= 2.0f)
                        {
                            if (dbPlayer.Container.GetItemAmount(25) >= 10 && dbPlayer.Container.CanInventoryItemAdded(24, 10))
                            {
                                dbPlayer.Container.RemoveItem(25, 10);
                                dbPlayer.Container.AddItem(24, 10);
                                dbPlayer.SendNewNotification(
                                    "Sie haben 10 Rohlingsvorlagen zu 10 Rohlingen umgebaut.");
                                dbPlayer.JobSkillsIncrease();
                                return;
                            }
                            else
                            {
                                dbPlayer.SendNewNotification(
                                    "Sie haben keine Rohlingsvorlagen oder Ihr Inventar ist voll!");
                            }

                            return;
                        }
                    }
                }
                else
                {
                    dbPlayer.SendNewNotification(
                        MSG.General.Usage("/materials", "[get/deliver]"));
                    return;
                }
            }
        }


        [CommandPermission]
        [Command(GreedyArg = true)]
        public void takeweapons(Player player, string commandParameter = " ")
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.CanAccessMethod()) return;

            if (string.IsNullOrWhiteSpace(commandParameter))
            {
                dbPlayer.SendNewNotification(
                    MSG.General.Usage("/takeweapons", "[name]"));
                return;
            }

            if (!dbPlayer.IsACop() || !dbPlayer.IsInDuty())
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            var command = commandParameter.Split(new[] {' '}, 1, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

            var findPlayer = Players.Instance.FindPlayer(command[0]);
            if (findPlayer == null) return;

            if (!dbPlayer.IsCuffed && !dbPlayer.IsTied && !dbPlayer.isInjured())
            {
                dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                return;
            }

            if (dbPlayer.Player.Position.DistanceTo(findPlayer.Player.Position) <= 2f)
            {
                //ToDo: Window "are you sure?"
                findPlayer.RemoveWeapons();
                findPlayer.ResetAllWeaponComponents();
                findPlayer.SendNewNotification(
                    $"Sie haben {findPlayer.GetName()} entwaffnet!");
                findPlayer.SendNewNotification(
                    "Ein Beamter hat Sie entwaffnet!");
                return;
            }
        }

        [CommandPermission]
        [Command]
        public void loadwk(Player player, string loadParameter = " ")
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod() || !dbPlayer.Player.IsInVehicle) return;

                if (dbPlayer.TeamId == (int)teams.TEAM_ARMY)
                {
                    if (dbPlayer.TeamRank < 7 && (int)dbPlayer.RankId < (int)adminlevel.Manager) return;
                    var count = 0;

                    foreach (var currPlayer in TeamModule.Instance.Get((int)teams.TEAM_ARMY).GetTeamMembers())
                    {
                        if (currPlayer.TeamRank < 1) continue;
                        if (!currPlayer.IsInDuty()) continue;
                        count++;
                        if (count > 4)
                        {
                            break;
                        }
                    }

                    if (count < 4)
                    {
                        dbPlayer.SendNewNotification("Es muessen mindestens 4 Rang 1er und höher online sein!");
                        return;
                    }

                    if (dbPlayer.Player.Position.DistanceTo(new Vector3(3621.52, 3734.86, 28.6901)) > 10.0f)
                    {
                        dbPlayer.SendNewNotification("Sie sind nicht an der zentralen Waffenkammer!");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(loadParameter))
                    {
                        dbPlayer.SendNewNotification(
                            MSG.General.Usage("/loadwk", "Amount"));
                        return;
                    }

                    //var item = ItemHandler.Instance.GetItemById(303);
                    var item = ItemModelModule.Instance.GetById(303);
                    if (!int.TryParse(loadParameter, out var amount)) return;
                    var sxVeh = dbPlayer.Player.Vehicle.GetVehicle();
                    if (sxVeh == null) return;
                    if (amount < 0 || !sxVeh.Container.CanInventoryItemAdded(item, amount))
                    {
                        dbPlayer.SendNewNotification(
                            "So viele Waffenkisten können Sie nicht laden!");
                        return;
                    }

                    // Load WKS
                    sxVeh.Container.AddItem(item, amount);
                    dbPlayer.SendNewNotification($"Sie haben " + amount + " " + item.Name + " geladen!");

                    //TeamModule.Instance[(int) teams.TEAM_ARMY].Members
                    foreach (var iPlayer in TeamModule.Instance[(int)teams.TEAM_ARMY].GetTeamMembers())
                    {
                        if (iPlayer == null || !iPlayer.IsValid())
                            continue;

                        iPlayer.SendNewNotification($"{dbPlayer.GetName()} hat " + amount + " " + item.Name + " geladen!");
                    }
                }
            }));
        }

        [CommandPermission]
        [Command]
        public void unloadmk(Player player)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;

                if (!dbPlayer.Player.IsInVehicle || (dbPlayer.TeamId != (int)teams.TEAM_ARMY)) return;

                if (dbPlayer.Player.Position.DistanceTo(new Vector3(3621.52, 3734.86, 28.6901)) > 10.0f)
                {
                    dbPlayer.SendNewNotification("Sie sind nicht an der zentralen Waffenkammer!");
                    return;
                }

                var item = ItemModelModule.Instance.GetById(MAZModule.MilitaryChestId);
                if (item == null)
                    return;

                var sxVeh = dbPlayer.Player.Vehicle.GetVehicle();
                var amount = sxVeh.Container.GetItemAmount(item);
                sxVeh.Container.RemoveItem(item, amount);

                dbPlayer.SendNewNotification($"Sie haben " + amount + " Militärkisten entladen!");

                if (dbPlayer.TeamId == (int)teams.TEAM_ARMY)
                {
                    foreach (var iPlayer in TeamModule.Instance[(int)teams.TEAM_ARMY].GetTeamMembers())
                    {
                        if (iPlayer == null || !iPlayer.IsValid())
                            continue;

                        iPlayer.SendNewNotification($"{dbPlayer.GetName()} hat " + amount + " " + item.Name + " am Human Labs entladen!");
                    }
                }
            }));
        }

        [CommandPermission]
        [Command]
        public void unloadwk(Player player)
        {
            Main.m_AsyncThread.AddToAsyncThread(new Task(() =>
            {
                var dbPlayer = player.GetPlayer();
                if (!dbPlayer.CanAccessMethod()) return;

                if (!dbPlayer.Player.IsInVehicle || (dbPlayer.TeamId != (int)teams.TEAM_ARMY)) return;

                var Armory = ArmoryModule.Instance.GetByLoadPosition(dbPlayer.Player.Position);
                if (Armory == null || Armory.UnlimitedPackets) return;

                var item = ItemModelModule.Instance.GetById(303);
                if (item == null)
                    return;

                var sxVeh = dbPlayer.Player.Vehicle.GetVehicle();
                var amount = sxVeh.Container.GetItemAmount(item);
                sxVeh.Container.RemoveItem(item, amount);

                dbPlayer.SendNewNotification($"Sie haben " + amount + " " +
                                          item.Name + " in die Waffenkammer entladen!");
                Armory.AddPackets(amount * ArmoryModule.Instance.WeaponChestMultiplier);

                if (dbPlayer.TeamId == (int)teams.TEAM_ARMY)
                {
                    foreach (var iPlayer in TeamModule.Instance[(int)teams.TEAM_ARMY].GetTeamMembers())
                    {
                        if (iPlayer == null || !iPlayer.IsValid())
                            continue;

                        iPlayer.SendNewNotification($"{dbPlayer.GetName()} hat " + amount + " " + item.Name + " in die Waffenkammer entladen!");
                    }
                }
            }));
        }
    }
}