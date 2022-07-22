using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace VMP_CNR.Module.NutritionPlayer
{
    public class NutritionItem : Loadable<uint>
    {
        public uint Id { get; }
        public uint Items_gd_id;
        public float Kcal;
        public float Fett;
        public float Wasser;
        public float Zucker;
        public string Effect;


        public NutritionItem(MySqlDataReader reader) : base(reader)
        {
            Id = reader.GetUInt32("id");
            Items_gd_id = reader.GetUInt32("items_gd_id");
            Kcal = reader.GetFloat("kcal");
            Fett = reader.GetFloat("fett");
            Wasser = reader.GetFloat("wasser");
            Zucker = reader.GetFloat("zucker");
            Effect = reader.GetString("effect");
        }

        public override uint GetIdentifier()
        {
            return Id;
        }
    }
}
