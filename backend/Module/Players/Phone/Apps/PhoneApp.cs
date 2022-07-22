using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using VMP_CNR.Module.PlayerUI.Apps;
using System;

namespace VMP_CNR.Module.Players.Phone.Apps
{
    //TODO: rename to PlayerApp because it is used in computer as well
    public class PhoneApp : Loadable<string>
    {
        [JsonProperty(PropertyName = "id")] public string Id { get; }
        [JsonProperty(PropertyName = "name")] public string Name { get; }
        [JsonProperty(PropertyName = "icon")] public string Icon { get; }

        public PhoneApp(MySqlDataReader reader) : base(reader)
        {
            Id = reader.GetString("id");
            Name = reader.GetString("name");
            Icon = reader.GetString("icon");
            Console.WriteLine("APP LOADED: " + Name);
        }

        public PhoneApp(string id, string name, string icon) : base(null)
        {
            Id = id;
            Name = name;
            Icon = icon;
        }

        public override string GetIdentifier()
        {
            return Id;
        }
    }
    
    public class HomeApp : SimpleApp
    {
        public HomeApp() : base("HomeApp")
        {
        }

        [RemoteEvent]
        public void requestApps(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            Console.WriteLine(dbPlayer.PhoneApps?.GetJson());
            TriggerEvent(player, "responseApps", dbPlayer.PhoneApps?.GetJson());
        }
        [RemoteEvent]
        public void requestPhoneWallpaper(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            TriggerEvent(player, "responsePhoneWallpaper", dbPlayer.wallpaper.File);
        }

    }
}