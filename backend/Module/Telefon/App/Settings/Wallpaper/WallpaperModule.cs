﻿using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Telefon.App.Settings.Wallpaper
{
    public class WallpaperModule : SqlModule<WallpaperModule, Wallpaper, uint>
    {
        protected override string GetQuery()
        {
            return "SELECT * FROM `phone_wallpaper`;";
        }

        protected override void OnItemLoaded(Wallpaper wallpaper)
        {
            return;
        }

        public override void OnPlayerLoadData(DbPlayer dbPlayer, MySqlDataReader reader)
        {
            dbPlayer.wallpaper = Instance.Get(reader.GetUInt32("wallpaperId"));

            Console.WriteLine("WallpaperModule");

        }

        public String getJsonWallpapersForPlayer(DbPlayer dbPlayer)
        {
            bool staffMember = dbPlayer.Rank.Id == 0 ? false : true;

            List<Wallpaper> liste = new List<Wallpaper>();

            foreach (var item in this.GetAll().Values)
            {
             liste.Add(item);
            }
            return JsonConvert.SerializeObject(liste);
        }



    }
}
