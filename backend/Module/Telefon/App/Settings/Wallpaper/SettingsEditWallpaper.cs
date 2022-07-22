using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Telefon.App.Settings.Wallpaper;

namespace VMP_CNR.Module.Telefon.App.Settings
{
    public class SettingsEditWallpaper : SimpleApp
    {
        public SettingsEditWallpaper() : base("SettingsEditWallpaperApp") { }

        [RemoteEvent]
        public void requestWallpaperList(Player player)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            TriggerEvent(player, "responseWallpaperList", WallpaperModule.Instance.getJsonWallpapersForPlayer(dbPlayer));

        }

        [RemoteEvent]
        public void saveWallpaper(Player player, int wallpaperId)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            dbPlayer.wallpaper = WallpaperModule.Instance.Get((uint)wallpaperId);
            dbPlayer.SaveWallpaper();
        }
        
    }

}
