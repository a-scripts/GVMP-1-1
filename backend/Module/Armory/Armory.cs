﻿using System;
using System.Collections.Generic;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Spawners;
using VMP_CNR.Module.Swat;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Weapons.Component;

//Todo: module
namespace VMP_CNR.Module.Armory
{
    public class Armory
    {
        public int Id { get; set; }
        public int Packets { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 LoadPosition { get; set; }
        public List<ArmoryWeapon> ArmoryWeapons { get; set; }
        public List<ArmoryWeaponComponent> ArmoryWeaponComponents { get; set; }
        public List<ArmoryItem> ArmoryItems { get; set; }
        public List<ArmoryArmor> ArmoryArmors { get; set; }
        public List<Team> AccessableTeams { get; set; }
        public bool UnlimitedPackets { get; set; }
        public bool HasArmor { get; set; }
        public bool HasHeavyArmor { get; set; }
        public ColShape ColShape { get; set; }
        public bool SwatDuty { get; set; }

        public uint PoolId { get; set; }
        public void RemovePackets(int packets)
        {
            GetArmoryPacketsDb();
            Packets -= packets;
            SetArmoryPacketsDb();
        }

        public void AddPackets(int packets)
        {
            GetArmoryPacketsDb();
            Packets += packets;
            SetArmoryPacketsDb();
        }

        public int GetPackets()
        {
            GetArmoryPacketsDb();
            return Packets;
        }

        private void GetArmoryPacketsDb()
        {
            if (UnlimitedPackets)
            {
                Packets = 50000;
                return;
            }

            var query = string.Format($"SELECT `packets` FROM `armories` WHERE `id` = '{Id}' LIMIT 1");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {
                        Packets = reader.GetInt32(0);
                        return;
                    }
                }
                conn.Close();
            }
        }

        private void SetArmoryPacketsDb()
        {
            MySQLHandler.ExecuteAsync(
                $"UPDATE `armories` SET `packets` = '{Packets}' WHERE `id` = '{Id}'");
        }

        public void LoadArmoryWeapons(ArmoryModule module)
        {
            var query = string.Format($"SELECT * FROM `armories_weapons` WHERE `Armory_id` = '{Id}' ORDER BY `restrictedRang` ASC");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {
                        var ArmoryWeapon = new ArmoryWeapon
                        {
                            Weapon = (WeaponHash)Enum.Parse(typeof(WeaponHash), reader.GetString("weaponHash"), true),
                            WeaponName = reader.GetString("weaponHash"),
                            MagazinPrice = reader.GetInt32("magazin_price"),
                            RestrictedRang = reader.GetInt32("restrictedRang"),
                            Packets = reader.GetInt32("packets"),
                            Price = reader.GetInt32("price"),
                            Defcon1Rang = reader.GetInt32("defcon1_rang"),
                            Defcon2Rang = reader.GetInt32("defcon2_rang"),
                            Defcon3Rang = reader.GetInt32("defcon3_rang"),
                        };


                        ArmoryWeapons.Add(ArmoryWeapon);
                    }
                }
                conn.Close();
            }
        }

        public void LoadArmoryItems(ArmoryModule module)
        {
            var query = string.Format($"SELECT * FROM `armories_items` WHERE `Armory_id` = '{Id}' ORDER BY `restricted_rang` ASC");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {
                        var armoryItem = new ArmoryItem
                        {
                            ItemId = reader.GetInt32("item_id"),
                            Item = ItemModelModule.Instance.Get(reader.GetUInt32("item_id")),
                            RestrictedRang = reader.GetInt32("restricted_rang"),
                            Packets = reader.GetInt32("packets"),
                            Price = reader.GetInt32("price")
                        };


                        ArmoryItems.Add(armoryItem);
                        module.Log("ArmoryItem " + armoryItem.Item.Name + " wurde geladen!");
                    }
                }
                conn.Close();
            }
        }

        public void LoadArmoryArmors(ArmoryModule module)
        {
            var query = string.Format($"SELECT * FROM `armories_armors` WHERE `Armory_id` = '{Id}' ORDER BY `restricted_rang` ASC");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {
                        ArmoryArmor armoryArmor = new ArmoryArmor
                        {
                            Name = reader.GetString("name"),
                            ArmorValue = reader.GetInt32("armor_value"),
                            VisibleArmorType = reader.GetInt32("visible_armor_type"),
                            RestrictedRang = reader.GetInt32("restricted_rang")
                        };
                        ArmoryArmors.Add(armoryArmor);
                    }
                }
                conn.Close();
            }
        }

        public void LoadArmoryArmorsFromPool(ArmoryModule module)
        {
            var query = string.Format($"SELECT * FROM `armories_pool_armors` WHERE `armory_pool_id` = '{PoolId}' ORDER BY `restricted_rang` ASC");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {
                        ArmoryArmor armoryArmor = new ArmoryArmor
                        {
                            Name = reader.GetString("name"),
                            ArmorValue = reader.GetInt32("armor_value"),
                            VisibleArmorType = reader.GetInt32("visible_armor_type"),
                            RestrictedRang = reader.GetInt32("restricted_rang")
                        };
                        ArmoryArmors.Add(armoryArmor);
                    }
                }
                conn.Close();
            }
        }

        public void LoadArmoryWeaponsFromPool(ArmoryModule module)
        {
            var query = string.Format($"SELECT * FROM `armories_pool_weapons` WHERE `armory_pool_id` = '{PoolId}' ORDER BY `restrictedRang` ASC");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {

                        var ArmoryWeapon = new ArmoryWeapon
                        {
                            Weapon = (WeaponHash)Enum.Parse(typeof(WeaponHash), reader.GetString("weaponHash"), true),
                            WeaponName = reader.GetString("weaponHash"),
                            MagazinPrice = reader.GetInt32("magazin_price"),
                            RestrictedRang = reader.GetInt32("restrictedRang"),
                            Packets = reader.GetInt32("packets"),
                            Price = reader.GetInt32("price"),
                            Defcon1Rang = reader.GetInt32("defcon1_rang"),
                            Defcon2Rang = reader.GetInt32("defcon2_rang"),
                            Defcon3Rang = reader.GetInt32("defcon3_rang"),
                        };


                        ArmoryWeapons.Add(ArmoryWeapon);
                    }
                }
                conn.Close();
            }
        }

        public void LoadArmoryWeaponComponentsFromPool(ArmoryModule module)
        {
            var query = string.Format($"SELECT * FROM `armories_pool_weaponcomponents` WHERE `armory_pool_id` = '{PoolId}'");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {

                        var ArmoryWeaponComponent = new ArmoryWeaponComponent
                        {
                            Id = reader.GetUInt32("id"),

                            WeaponComponentId = reader.GetUInt32("weapon_component_id"),
                            Packets = reader.GetInt32("packets"),
                            Price = reader.GetInt32("price"),
                        };


                        ArmoryWeaponComponents.Add(ArmoryWeaponComponent);
                    }
                }
                conn.Close();
            }
        }

        public void LoadArmoryItemsFromPool(ArmoryModule module)
        {
            var query = string.Format($"SELECT * FROM `armories_pool_items` WHERE `armory_pool_id` = '{PoolId}' ORDER BY `restricted_rang` ASC");

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @query;
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows) return;
                    while (reader.Read())
                    {
                        var armoryItem = new ArmoryItem
                        {
                            ItemId = reader.GetInt32("item_id"),
                            Item = ItemModelModule.Instance.Get(reader.GetUInt32("item_id")),
                            RestrictedRang = reader.GetInt32("restricted_rang"),
                            Packets = reader.GetInt32("packets"),
                            Price = reader.GetInt32("price")
                        };


                        ArmoryItems.Add(armoryItem);
                        module.Log("ArmoryItem " + armoryItem.Item.Name + " wurde geladen!");
                    }
                }
                conn.Close();
            }
        }
    }

    public sealed class ArmoryModule : Module<ArmoryModule>
    {
        private Dictionary<int, Armory> Armories;

        public int WeaponChestMultiplier = 20;

        public override Type[] RequiredModules()
        {
            return new[] {typeof(TeamModule), typeof(ItemModelModule) };
        }

        protected override bool OnLoad()
        {
            Armories = new Dictionary<int, Armory>();
            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = @"SELECT * FROM `armories`";
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var Armory = new Armory
                            {
                                Id = reader.GetInt32("id"),
                                Packets = reader.GetInt32("packets"),
                                Position = new Vector3(reader.GetFloat("pos_x"), reader.GetFloat("pos_y"), reader.GetFloat("pos_z")),
                                LoadPosition = new Vector3(reader.GetFloat("load_pos_x"), reader.GetFloat("load_pos_y"), reader.GetFloat("load_pos_z")),
                                ArmoryWeapons = new List<ArmoryWeapon>(),
                                ArmoryWeaponComponents = new List<ArmoryWeaponComponent>(),
                                ArmoryItems = new List<ArmoryItem>(),
                                ArmoryArmors = new List<ArmoryArmor>(),
                                AccessableTeams = new List<Team>(),
                                HasArmor = reader.GetInt32("armor") == 1,
                                HasHeavyArmor = reader.GetInt32("heavyarmor") == 1,
                                SwatDuty = reader.GetInt32("swat_duty") == 1,
                                PoolId = reader.GetUInt32("pool_id")
                            };
                            Armory.ColShape = ColShapes.Create(Armory.Position, 3f);
                            Armory.ColShape.SetData("ArmoryId", Armory.Id);

                            Armory.UnlimitedPackets = Armory.Packets == -1;

                            var teams = reader.GetString("teams");
                            if (!string.IsNullOrEmpty(teams))
                            {
                                if (teams.Contains(","))
                                {
                                    var ts = teams.Split(',');
                                    foreach (var x in ts)
                                    {
                                        if (!uint.TryParse(x, out var teamId)) continue;
                                        Armory.AccessableTeams.Add(TeamModule.Instance.Get(teamId));
                                    }
                                }
                                else
                                    Armory.AccessableTeams.Add(TeamModule.Instance.Get(Convert.ToUInt32(teams)));
                            }

                            Armories.Add(Armory.Id, Armory);
                            Armory.LoadArmoryItems(this);
                            Armory.LoadArmoryWeapons(this);
                            Armory.LoadArmoryArmors(this);

                            if (Armory.PoolId > 0)
                            {
                                // New Pool Loading
                                Armory.LoadArmoryItemsFromPool(this);
                                Armory.LoadArmoryWeaponsFromPool(this);
                                Armory.LoadArmoryWeaponComponentsFromPool(this);
                                Armory.LoadArmoryArmorsFromPool(this);
                            }

                            Log("Armory " + reader.GetInt32(0) + " mit " + reader.GetInt32(7) +
                                " Paketen wurde geladen!");
                        }
                    }
                }
                conn.Close();
            }

            return true;
        }

        public Armory Get(int id)
        {
            return Armories.TryGetValue(id, out var Armory) ? Armory : null;
        }

        public Armory GetByLoadPosition(Vector3 position, float range = 10.0f)
        {
            foreach (var Armory in Armories)
            {
                if (Armory.Value.LoadPosition.DistanceTo(position) <= range)
                {
                    return Armory.Value;
                }
            }

            return null;
        }

        public bool TriggerPoint(DbPlayer iPlayer)
        {
            if (!iPlayer.HasData("ArmoryId")) return false;
            var ArmoryId = iPlayer.GetData("ArmoryId");
            Armory Armory = Get(ArmoryId);
            if (Armory == null) return false;
            // Wenn kein Cop return

            Teams.Team team = iPlayer.Team;
            if (Armory.AccessableTeams.Contains(TeamModule.Instance.Get((uint)teams.TEAM_IAA)) && iPlayer.IsNSADuty) team = TeamModule.Instance.Get((uint)teams.TEAM_IAA);

            if(Armory.SwatDuty)
            {
                if (iPlayer.HasSwatRights() && !iPlayer.IsSwatDuty())
                {
                    iPlayer.SetSwatDuty(true);
                }
            }
            if (!Armory.AccessableTeams.Contains(team)) return false;
            MenuManager.Instance.Build(PlayerMenu.Armory, iPlayer).Show(iPlayer);
            return true;
        }
    }
}