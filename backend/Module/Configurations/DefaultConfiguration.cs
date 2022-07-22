using System.Collections.Generic;

namespace VMP_CNR.Module.Configurations
{
    public class DefaultConfiguration
    {
        public bool DevLog { get; }
        public bool Ptr { get; }
        public bool DevMode { get; }
        public string VoiceChannel { get; }
        public string VoiceChannelPassword { get; }
        public bool IsServerOpen { get; set; }
        public bool InventoryActivated { get; set; }
        public bool EKeyActivated { get; set; }
        public bool BlackMoneyEnabled { get; set; }
        public bool MeertraeubelEnabled { get; set; }
        public bool JailescapeEnabled { get; set; }
        public bool MethLabEnabled { get; set; }
        public bool JumpPointsEnabled { get; set; }
        public string mysql_pw { get; }
        public string mysql_user { get; }
        public string mysql_user_forum { get; }
        public string mysql_pw_forum { get; }
        public bool disableAPILogin { get; set; }
        public bool LipsyncActive { get; set; }
        public bool TuningActive { get; set; }

        public bool CanBridgeUsed { get; set; }
        public int MaxPlayers { get; set; }
        public bool IsUpdateModeOn { get; set; }

        public bool PlayerSync { get; set; } = true;
        public bool VehicleSync { get; set; } = true;

        public float WeaponDamageMultipier { get; set; }
        public float MeeleDamageMultiplier { get; set; }
        public string RESET_API_KEY { get; set; }
        public string CLEAR_API_KEY { get; set; }
        public string MAINTENACE_API_KEY { get; set; }
        public bool ShowAllJumppoints { get; set; }
        public bool EventActive { get; set; }
        
        public string DevServerName { get; set; }
        public DefaultConfiguration(IReadOnlyDictionary<string, dynamic> data)
        {
            DevLog = bool.Parse(data["devlog"]);
            Ptr = bool.Parse(data["ptr"]);
            DevMode = bool.Parse(data["devmode"]);
            VoiceChannel = data["V_Channel"];
            VoiceChannelPassword = data["V_PW"];
            IsServerOpen = false;
            InventoryActivated = true;
            EKeyActivated = true;
            BlackMoneyEnabled = true;  //set to true later
            MethLabEnabled = true;  //set to true later
            MeertraeubelEnabled = true; //set to true later
            JailescapeEnabled = false;
            JumpPointsEnabled = true;
            mysql_pw = "";//data["mysql_pw"];
            mysql_user = "root";//data["mysql_user"];
            mysql_user_forum = "root";//data["mysql_user_forum"];
            mysql_pw_forum = "";//data["mysql_pw_forum"];

            DevServerName = "";//data["processname"];

            CanBridgeUsed = false;
            MaxPlayers = data.ContainsKey("max_players") ? int.Parse(data["max_players"]) : 1000;

            // Damage Multipliers
            MeeleDamageMultiplier = 0.5f;
            WeaponDamageMultipier = 0.3f; // Rage default vermutlich so 0.3 iwas
            PlayerSync = true;
            VehicleSync = true;
            disableAPILogin = false;
            LipsyncActive = data.ContainsKey("lipsync") ? bool.Parse(data["lipsync"]) : false;
            TuningActive = true;
            IsUpdateModeOn = bool.Parse(data["updatemode"]);
            RESET_API_KEY = data.ContainsKey("reset_api_key") ? data["reset_api_key"] : "";
            CLEAR_API_KEY = data.ContainsKey("clear_api_key") ? data["clear_api_key"] : "";
            MAINTENACE_API_KEY = data.ContainsKey("maintenance_api_key") ? data["maintenance_api_key"] : "";
            ShowAllJumppoints = false;
            EventActive = false;
        }
        
        public string GetMySqlConnection()
        {
            return Ptr
                ? "server='localhost'; uid='root'; pwd=''; database='gvmp';max pool size=999;SslMode=none;convert zero datetime=True;"
                : "server='localhost'; uid='root'; pwd=''; database='gvmp';max pool size=999;SslMode=none;convert zero datetime=True;";
        }
        
        public string GetMySqlConnectionBoerse()
        {
            return
                ""; // "server='localhost'; uid='" + mysql_user + "'; pwd='" + mysql_pw + "'; database='venom';max pool size=999;SslMode=none;convert zero datetime=True;";
        }

        public string GetMySqlConnectionForum()
        {
            return
          "server='88.99.120.124'; uid='" + "gvmpde_wcf" + "'; pwd='" + "password" + "'; database='gvmpde_wcf';max pool size=999;SslMode=none;";
        }

        public string GetMySqlConnectionWhitelist()
        {
            return
             "";   //"server='localhost'; uid='" + mysql_user + "'; pwd='" + mysql_pw + "'; database='gvmp';max pool size=999;SslMode=none;";
        }

        public override string ToString()
        {
            return $"Devlog: {DevLog}\n" +
                   $"Ptr: {Ptr}\n" +
                   $"DevMode: {DevMode}\n" +
                   $"VoiceChannel: {VoiceChannel}\n" +
                   $"VoiceChannelPassword: {VoiceChannelPassword}\n";
        }
    }
}