using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GTANetworkAPI;
using VMP_CNR.Module.Events;
using VMP_CNR.Module.Events.EventMaps;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.MapParser
{
    public sealed class MapParserModule : Module<MapParserModule>
    {
        Dictionary<string, List<GTANetworkAPI.Object>> objectList = new Dictionary<string, List<GTANetworkAPI.Object>>();

        public override Type[] RequiredModules()
        {
            return new[]
            {
                typeof(EventMapModule),
                typeof(EventModule)
            };
        }

        public override bool Load(bool reload = false)
        {
            int loaded = 0;
            Dictionary<string, List<GTANetworkAPI.Object>> objectList = new Dictionary<string, List<GTANetworkAPI.Object>>();
            
            // Load Maps :)
            string mainPath = Configurations.Configuration.Instance.Ptr ? "C:\\MapsTest" : "C:\\MapsLive"; 

            foreach (string file in Directory.EnumerateFiles(mainPath, "*.xml"))
            {
                if (EventMapModule.Instance.IsEventMap(file))
                    continue;

                ReadMap(file);
                Logging.Logger.Debug($"Loaded Map: {file}!");
                loaded++;
            }

            foreach (string file in Directory.EnumerateFiles(mainPath, "*.xmml"))
            {
                if (EventMapModule.Instance.IsEventMap(file))
                    continue;

                ReadMenyooMap(file);
                Logging.Logger.Debug($"Loaded Map: {file}!");
                loaded++;
            }
            Logging.Logger.Debug($"Loaded {loaded} Maps from XML-Map Parser!");
            return true;
        }

        public void ReadMenyooMap(string file)
        {
            MMap data = new MMap();
            int dimension = Convert.ToInt32(file.Split('\\')[2].Split('_')[0]);

            XmlSerializer mySerializer = new XmlSerializer(typeof(MMap));
            StreamReader streamReader = new StreamReader(file);

            data = (MMap)mySerializer.Deserialize(streamReader);
            streamReader.Close();
            List<MMapObject> mapObjects = data.MapObjects;

            foreach (var mapObject in mapObjects)
            {
                NAPI.Object.CreateObject(Convert.ToInt32(mapObject.Hash, 16), new Vector3(mapObject.PositionRotation.x, mapObject.PositionRotation.y, mapObject.PositionRotation.z), new Vector3(mapObject.PositionRotation.rx, mapObject.PositionRotation.ry, mapObject.PositionRotation.rz), 255, (uint)dimension);
            }
            
        }

        public void ReadMap(string file)
        {
            Map data = new Map();
            int dimension = Convert.ToInt32(file.Split('\\')[2].Split('_')[0]);

            XmlSerializer mySerializer = new XmlSerializer(typeof(Map));
            StreamReader streamReader = new StreamReader(file);

            data = (Map)mySerializer.Deserialize(streamReader);
            streamReader.Close();
            List<MapObject> mapObjects = data.Objects.MapObjects;
            
            foreach (var mapObject in mapObjects)
            {
                NAPI.Object.CreateObject(mapObject.Hash, new Vector3(mapObject.Position.x, mapObject.Position.y, mapObject.Position.z), new Vector3(mapObject.Rotation.x, mapObject.Rotation.y, mapObject.Rotation.z), 255, (uint)dimension);
            }
            
        }
    }

    /* MENYOO XML */
    [XmlRoot("SpoonerPlacements")]
    public class MMap
    {
        [XmlElement("Placement")]
        public List<MMapObject> MapObjects { get; set; }
    }
    public class MMapObject
    {
        [XmlElement("PositionRotation")]
        public PositionRotation PositionRotation { get; set; }
        
        [XmlElement("HashName")]
        public string Hash { get; set; }
    }
    
    public class PositionRotation
    {
        [XmlElement("X")]
        public float x { get; set; }

        [XmlElement("Y")]
        public float y { get; set; }

        [XmlElement("Z")]
        public float z { get; set; }

        [XmlElement("Pitch")]
        public float rx { get; set; }

        [XmlElement("Roll")]
        public float ry { get; set; }

        [XmlElement("PositionRot")]
        public float rz { get; set; }
    }

    /* DEFAULT MAP EDITOR XML */
    [XmlRoot("Map")]
    public class Map
    {
        [XmlElement("Objects")]
        public Object Objects { get; set; }
    }


    public class Object
    {
        [XmlElement("MapObject")]
        public List<MapObject> MapObjects { get; set; }
    }
    public class MapObject
    {
        [XmlElement("Position")]
        public ObjectPosition Position { get; set; }

        [XmlElement("Rotation")]
        public ObjectRotation Rotation { get; set; }

        [XmlElement("Hash")]
        public int Hash { get; set; }
    }

    public class ObjectPosition
    {
        [XmlElement("X")]
        public float x { get; set; }

        [XmlElement("Y")]
        public float y { get; set; }

        [XmlElement("Z")]
        public float z { get; set; }
    }

    public class ObjectRotation
    {
        [XmlElement("X")]
        public float x { get; set; }

        [XmlElement("Y")]
        public float y { get; set; }

        [XmlElement("Z")]
        public float z { get; set; }
    }
}