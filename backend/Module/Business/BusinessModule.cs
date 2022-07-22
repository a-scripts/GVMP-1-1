﻿using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VMP_CNR.Module.Banks.BankHistory;
using VMP_CNR.Module.Banks.Windows;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Business
{
    public class BusinessModule : SqlModule<BusinessModule, Business, uint>
    {
        public static readonly Vector3 BusinessPosition = new Vector3(-78.9559, -829.376, 243.386);

        private static readonly Vector3 BusinessEnterPosition = new Vector3(-66.8634, -802.6156, 44.2273);

        //private static readonly Vector3 BusinessEnterPosition = new Vector3(-79.6059, -796.427, 44.2273);

        private static readonly Vector3 BusinessBankPosition = new Vector3(248.977, 212.425, 106.287);

        private static readonly Vector3 BusinessTresurePosition = new Vector3(-59.5738, -812.895, 243.386);

        public static Vector3 BusinessKeyInsertPosition = new Vector3(-83.051, -814.94, 36.1299);

        public static Vector3 BusinessKeyInsertAirport = new Vector3(-1279.88, -2615.68, 13.9449);

        protected override string GetQuery()
        {
            return "SELECT * FROM `business`;";
        }

        protected override void OnItemLoaded(Business business)
        {
            // Load Branches
            business.LoadBusinessBranch();

            // Load Keys
            business.LoadVehicleKeys();
            business.LoadStorageKeys();

            business.LoadBankHistory();
            return;
        }

        protected override void OnLoaded()
        {
            ColShape colShape = Spawners.ColShapes.Create(BusinessEnterPosition, 2.5f);
            colShape.SetData("businessTower", true);


            PlayerNotifications.Instance.Add(BusinessKeyInsertAirport,
                "Business Fahrzeuge",
                "Hier kannst du Fahrzeugschlüssel für dein Business anfertigen lassen!");

            PlayerNotifications.Instance.Add(BusinessKeyInsertPosition,
                "Business Fahrzeuge",
                "Hier kannst du Fahrzeugschlüssel für dein Business anfertigen lassen!");
        }

        public override void OnPlayerLoggedIn(DbPlayer dbPlayer)
        {
            dbPlayer.Player.TriggerEvent("setPlayerInfoVoiceHash", dbPlayer.VoiceHash);
            dbPlayer.Player.TriggerEvent("setPlayerInfoId", dbPlayer.ForumId);
            dbPlayer.LoadBusinessMembership();
        }

        public IEnumerable<Business> GetOpenBusinesses()
        {
            return from business in this.GetAll() where !business.Value.Locked select business.Value;
        }

        public Business GetById(uint id)
        {
            return Instance.GetAll().Where(b => b.Value.Id == id).FirstOrDefault().Value;
        }

        public Business GetByName(string name)
        {
            return this.GetAll().FirstOrDefault(b =>
                string.Equals(b.Value.Name, name, StringComparison.CurrentCultureIgnoreCase)
                || b.Value.Name.ToLower().Contains(name.ToLower())).Value;
        }
        
        public override bool OnColShapeEvent(DbPlayer dbPlayer, ColShape colShape, ColShapeState colShapeState)
        {
            if (colShapeState == ColShapeState.Enter)
            {
                if (colShape.HasData("businessTower"))
                {
                    dbPlayer.SendNewNotification("Drücke E um den Business-Tower zu betreten.", PlayerNotification.NotificationType.BUSINESS);
                    return true;
                }
            }
            return false;
        }
        
        public async Task CreatePlayerBusiness(DbPlayer iPlayer)
        {
            var name = $"Business von {iPlayer.GetName()}";
            var query = $"INSERT INTO `business` (`name`) VALUES ('{MySqlHelper.EscapeString(name)}'); select last_insert_id();";
            using (var conn = new MySqlConnection(Configurations.Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();
                cmd.CommandText = @query;
                var id = Convert.ToUInt32(cmd.ExecuteScalar());

                BusinessModule.Instance.Load(true);
                Business business = this.Get(id);

                iPlayer.AddBusinessOwnership(business);
                iPlayer.ActiveBusiness = business;
                iPlayer.UpdateApps();
                await conn.CloseAsync();
            }
        }

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            if (key == Key.L)
            {
                if (!(dbPlayer.Player.Position.DistanceTo(BusinessPosition) < 3.0f)) return false;
                if (dbPlayer.DimensionType[0] != DimensionType.Business ||
                    dbPlayer.ActiveBusiness != null && dbPlayer.ActiveBusiness.Id != dbPlayer.Player.Dimension) return false;
                var biz = GetById((uint)dbPlayer.Player.Dimension);

                if (biz == null) return true;
                NAPI.Task.Run(() =>
                {
                    if (biz.Locked)
                    {
                        biz.Locked = false;
                        dbPlayer.SendNewNotification("Tür aufgeschlossen!", title: "Business", notificationType: PlayerNotification.NotificationType.SUCCESS);
                    }
                    else
                    {
                        biz.Locked = true;
                        dbPlayer.SendNewNotification("Tür abgeschlossen!", title: "Business", notificationType: PlayerNotification.NotificationType.ERROR);
                    }
                });
                return true;
            }

            if (key == Key.E)
            {
                if (dbPlayer.Player.Position.DistanceTo(BusinessEnterPosition) < 3.0f)
                {
                    MenuManager.Instance.Build(PlayerMenu.BusinessEnter, dbPlayer).Show(dbPlayer);
                    return true;
                }

                if (dbPlayer.DimensionType[0] == DimensionType.Business &&
                    dbPlayer.Player.Position.DistanceTo(BusinessTresurePosition) < 3.0f)
                {
                    var biz = GetById((uint)dbPlayer.Player.Dimension);
                    if (biz == null || dbPlayer.ActiveBusiness == null ||
                        dbPlayer.ActiveBusiness.Id != biz.Id) return true;
                    NAPI.Task.Run(() =>
                    {
                        ComponentManager.Get<BankWindow>().Show()(dbPlayer, "Business Tresor", biz.Name, dbPlayer.money[0], biz.Money, 0, biz.BankHistory);
                    });
                    return true;
                }

                if (dbPlayer.Player.Position.DistanceTo(BusinessPosition) < 3.0f)
                {
                    if (dbPlayer.DimensionType[0] == DimensionType.Business)
                    {
                        NAPI.Task.Run(() =>
                        {
                            var biz = GetById((uint)dbPlayer.Player.Dimension);
                            biz.Visitors.Remove(dbPlayer);
                            dbPlayer.Player.SetPosition(BusinessEnterPosition);
                            dbPlayer.DimensionType[0] = DimensionType.World;
                            dbPlayer.SetDimension(0);
                        });
                        return true;
                    }

                    return true;
                }

                if (dbPlayer.Player.Position.DistanceTo(BusinessBankPosition) < 3.0f)
                {
                    MenuManager.Instance.Build(PlayerMenu.BusinessBank, dbPlayer).Show(dbPlayer);
                    //dbPlayer.SendNewNotification("Update Banksoftware: Businessgründung ist zur Zeit deaktiviert.", title: "Business", notificationType: PlayerNotification.NotificationType.ERROR);
                    return true;
                }

                return false;
            }
            return false;
        }
    }

    public class BusinessEvents : Script
    {
        //NameChangeBiz
        [RemoteEvent]
        public void NameChangeBiz(Player player, string returnString)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.IsMemberOfBusiness() || !dbPlayer.BusinessMembership.Manage) return;

            if (!Regex.IsMatch(returnString, @"^[a-zA-Z_-]+$"))
            {
                dbPlayer.SendNewNotification("Bitte gib einen Namen mit Buchstaben (optional _ und -) an!.");
                return;
            }

            if (returnString.Length > 40 || returnString.Length < 7)
            {
                dbPlayer.SendNewNotification("Der Name ist zu lang oder zu kurz!");
                return;
            }

            if(!dbPlayer.TakeBankMoney(50000))
            {
                dbPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(50000));
                return;
            }

            MySQLHandler.ExecuteAsync($"UPDATE business SET name = '{MySqlHelper.EscapeString(returnString)}' WHERE id = '{dbPlayer.ActiveBusiness.Id}';");

            dbPlayer.SendNewNotification($"Der Name des Businesses wurde auf {returnString} gesetzt! (Änderung ab nächster Wende)");
            return;
        }
    }
}
