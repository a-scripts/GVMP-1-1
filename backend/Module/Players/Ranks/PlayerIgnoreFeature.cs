using System.Collections.Generic;
using GTANetworkAPI;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Players.Ranks
{
    public static class PlayerIgnoreFeature
    {
        public static bool HasFeatureIgnored(this Player Player, string name)
        {
            DbPlayer iPlayer = Player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return false;

            if (!iPlayer.HasData("ignored_features")) return false;
            HashSet<string> features = iPlayer.GetData("ignored_features");
            return features.Contains(name);
        }

        public static void SetFeatureIgnored(this Player Player, string name)
        {

            DbPlayer iPlayer = Player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;

            HashSet<string> features;
            if (!iPlayer.HasData("ignored_features"))
            {
                features = new HashSet<string>();
                iPlayer.SetData("ignored_features", features);
            }
            else
            {
                features = iPlayer.GetData("ignored_features");
            }

            if (features.Contains(name)) return;
            features.Add(name);
        }

        public static void RemoveFeatureIgnored(this Player Player, string name)
        {
            DbPlayer iPlayer = Player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;

            if (!iPlayer.HasData("ignored_features")) return;
            HashSet<string> features = iPlayer.GetData("ignored_features");
            if (!features.Contains(name)) return;
            features.Remove(name);
        }
    }
}