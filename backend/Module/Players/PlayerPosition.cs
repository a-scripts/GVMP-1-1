

using GTANetworkAPI;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Players
{
    public static class PlayerPosition
    {
        public static void SetRotation(this Player Player, float rotation)
        {
            Player.Rotation = new Vector3(0, 0, rotation);
        }

        public static void SetPosition(this Player player, Vector3 pos)
        {


            DbPlayer iPlayer = player.GetPlayer();
            if (iPlayer != null && iPlayer.IsValid())
            {
                iPlayer.SetData("Teleport", 5);
                iPlayer.SetData("ac_lastPos", pos);
            }

            player.Position = pos;
            if (iPlayer != null && iPlayer.IsValid())
            {
                // Fix GodMode if Aduty
                if (iPlayer.IsInAdminDuty())
                {
                    iPlayer.Player.TriggerEvent("setPlayerAduty", true);
                }
            }

        }
        
        public static void SetWaypoint(this DbPlayer iPlayer, float x, float y)
        {
            iPlayer.SetData("waypoint_x", x);
            iPlayer.SetData("waypoint_y", y);

            iPlayer.Player.SendWayPoint(x, y);
        }

        public static Vector3 GetWaypoint(this DbPlayer iPlayer)
        {
            float x = iPlayer.GetData("waypoint_x");
            float y = iPlayer.GetData("waypoint_y");
            float z = 0.0f;
            return new Vector3(x, y, z);
        }

        public static void SendWayPoint(this Player player, float x, float y)
        {
            player.TriggerEvent("setPlayerGpsMarker", x, y);
        }
    }
}