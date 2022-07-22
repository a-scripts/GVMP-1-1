using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Blitzer;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> Blitzer70(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.IsCuffed || iPlayer.IsTied ||
                iPlayer.Team.Id != (uint)teams.TEAM_POLICE ||
                iPlayer.Player.IsInVehicle)
            {
                return false;
            }

            return true;
            // TESTE DEINEN CODE OB ER ÜBERHAUPT COMPILED MAAAAAAAN
            /*if(BlitzerModule.Instance.aufgestellt >= 4)
            {
                iPlayer.SendNewNotification( "Maximale Anzahl an Blitzern erreicht!");
                return false;
            }

            if (PoliceObjectModule.Instance.IsMaxReached())
            {
                iPlayer.SendNewNotification( "Maximale Anzahl an Polizeiabsperrungen erreicht!");
                return false;
            }

            PoliceObjectModule.Instance.Add(1382242693, iPlayer.Player, ItemData, false);

            Vector3 pos = iPlayer.Player.Position;
            pos.Z = pos.Z - 5.0f;
            BlitzerModule.Instance.AddBlitzer(pos, iPlayer.GetName(), (int)iPlayer.TeamId, 70);

            iPlayer.SendNewNotification( ItemData.Name + " erfolgreich platziert!");
            
                iPlayer.PlayAnimation(
                    (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);
                iPlayer.Player.TriggerEvent("freezePlayer", true);
                await Task.Delay(4000);
                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.StopAnimation();
            
            return true;*/
        }

        public static async Task<bool> Blitzer120(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.IsCuffed || iPlayer.IsTied || iPlayer.Team.Id != (uint)teams.TEAM_POLICE || iPlayer.Player.IsInVehicle)
                return false;

            return true;

            // TESTE DEINEN CODE BEVOR DU PUSHT AMENAKOYKARPFEN
            /*if (BlitzerModule.Instance.aufgestellt >= 4)
            {
                iPlayer.SendNewNotification( "Maximale Anzahl an Blitzern erreicht!");
                return false;
            }

            if (PoliceObjectModule.Instance.IsMaxReached())
            {
                iPlayer.SendNewNotification( "Maximale Anzahl an Polizeiabsperrungen erreicht!");
                return false;
            }

            PoliceObjectModule.Instance.Add(1382242693, iPlayer.Player, ItemData, false);

            BlitzerModule.Instance.AddBlitzer(iPlayer.Player.Position, iPlayer.GetName(), (int)iPlayer.TeamId, 120);

            iPlayer.SendNewNotification( ItemData.Name + " erfolgreich platziert!");
            
                iPlayer.PlayAnimation(
                    (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);
                iPlayer.Player.TriggerEvent("freezePlayer", true);
                await Task.Delay(4000);
                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.StopAnimation();*/
            
            return true;
        }
    }
}