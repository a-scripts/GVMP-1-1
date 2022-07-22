using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Spawners;
using VMP_CNR.Module.Teams;
using GTANetworkAPI;
using Newtonsoft.Json;
using GTANetworkMethods;

namespace VMP_CNR.Module.Paintball
{

    public struct vars
    {
        public uint life { get; set; }
        public uint kills { get; set; }
        public uint deaths { get; set; }
        public uint killstreak { get; set; }
    }
    public class Weaponz
    {
        public string name;
        public int ammo;
    }
    public class Area
    {
        public float x;
        public float y;
        public float z;
        public float r;
    }
    public class PaintballArea : Loadable<uint>
    {
        public uint Id { get; set; }
        public List<Weaponz> Weapons;
        public string Mode;
        public List<Area> Area;
        public string Name;
        public uint PaintBallDimension;
        public uint RespawnTime;
        public uint SpawnProtection;
        public int LobbyEnterPrice;
        public uint MaxLobbyPlayers;
        public uint MaxLife;
        public string Password;
        public uint Ownerid;
        public Dictionary<DbPlayer, vars> pbPlayers = new Dictionary<DbPlayer, vars>();

        public PaintballArea(MySqlDataReader reader) : base(reader)
        {
            Id = reader.GetUInt32("sortid");
            Ownerid = reader.GetUInt32("ownerid");

            Name = reader.GetString("name");
            Password = reader.GetString("password");
            PaintBallDimension = reader.GetUInt32("dimension");
            RespawnTime = reader.GetUInt32("respawnTime");
            SpawnProtection = reader.GetUInt32("spawnprotection");
            LobbyEnterPrice = reader.GetInt32("lobbyenterprice");
            MaxLobbyPlayers = reader.GetUInt32("maxlobbyplayers");
            MaxLife = reader.GetUInt32("maxlife");
            Weapons = JsonConvert.DeserializeObject<List<Weaponz>>(reader.GetString("weapons"));
            Area = JsonConvert.DeserializeObject<List<Area>>(reader.GetString("area"));

            System.Threading.Tasks.Task.Run(() =>
            {
                NAPI.Task.Run(() =>
                {
                    // Some non-thread safe API methods
                    Vector3 pos = new Vector3(Area[0].x, Area[0].y, Area[0].z);
                    var l_ColShape = ColShapes.Create(pos, Area[0].r, PaintBallDimension);
                    l_ColShape.SetData("paintballId", Id);

                    if (Configurations.Configuration.Instance.DevMode)
                    {
                        Color color = new Color(255, 140, 0, 255);
                        NAPI.Marker.CreateMarker(1, pos, new Vector3(), new Vector3(), (float)Area[0].r * 2, color, true, PaintBallDimension);
                    }
                    Console.WriteLine(Name + " | " + Ownerid);
                }, delayTime: 2000); // delay time in ms
            });



        }



        public override uint GetIdentifier()
        {
            return Id;
        }
    }




}
