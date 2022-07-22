﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.Assets.Hair;
using VMP_CNR.Module.Assets.HairColor;
using VMP_CNR.Module.Assets.Tattoo;
using VMP_CNR.Module.Barber.Windows;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;

using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Tattoo.Windows;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.Freiberuf.Mower
{
    public sealed class MowerModule : Module<MowerModule>
    {
        public static int MowerJobVehMarkId = 20;
        public static Vector3 MowerGetPoint = new Vector3(-949.348, 332.97, 71.3311);
        public static Vector3 MowerSpawnPoint = new Vector3(-938.013, 329.984, 70.8813);
        public static float MowerSpawnRotation = 267.621f;
        public static Vector3 MowerMowPoint = new Vector3(-980.331, 318.863, 70.0861);
        public static List<DbPlayer> PlayersInJob = new List<DbPlayer>();

        public override bool Load(bool reload = false)
        {
            PlayerNotifications.Instance.Add(MowerGetPoint,
            "Freiberuf Rasenarbeiten",
            "Benutze \"E\" um den Freiberuf zu starten!"); // Perso
            return true;
        }

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            try { 
            if (key != Key.E || dbPlayer.Player.IsInVehicle) return false;

            if (dbPlayer.Player.Position.DistanceTo(MowerGetPoint) < 2.0f)
            {
                MenuManager.Instance.Build(PlayerMenu.FreiberufMowerMenu, dbPlayer).Show(dbPlayer);
                return true;
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public override void OnTenSecUpdate()
        {     
            try { 
            foreach (DbPlayer iPlayer in PlayersInJob.ToList())
            {
                if (iPlayer.Player.IsInVehicle && iPlayer.Player.Vehicle.HasData("loadage") && iPlayer.Player.Vehicle.GetModel().Equals(VehicleHash.Mower))
                {
                    if (iPlayer.Player.Vehicle.GetVehicle().GetSpeed() > 5.0f && iPlayer.Player.Position.DistanceTo(MowerMowPoint) < 30.0f)
                    {
                        if (iPlayer.HasData("lastRasenPoint"))
                        {
                            if (iPlayer.GetData("lastRasenPoint").DistanceTo(iPlayer.Player.Position) < 4.0f) continue; //Anti Kreisfahren
                        }
                        iPlayer.SetData("lastRasenPoint", iPlayer.Player.Position);

                        Random random = new Random();
                        int rnd = random.Next(1, 5);
                        iPlayer.Player.Vehicle.SetData("loadage", (iPlayer.Player.Vehicle.GetData<int>("loadage") + rnd));
                        iPlayer.SendNewNotification($"Rasen gemaeht! (Inhalt {iPlayer.Player.Vehicle.GetData<int>("loadage") - rnd} (+{rnd}))");
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}