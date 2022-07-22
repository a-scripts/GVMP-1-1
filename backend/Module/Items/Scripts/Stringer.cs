using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> Stringer(DbPlayer iPlayer, ItemModel ItemData)
        {
            if (iPlayer.IsCuffed || iPlayer.IsTied ||
                !iPlayer.Team.IsCops() && !iPlayer.Team.IsDpos() ||
                iPlayer.Player.IsInVehicle)
            {
                return false;
            }

            if (PoliceObjectModule.Instance.IsMaxReached())
            {
                iPlayer.SendNewNotification(
                     "Maximale Anzahl an Polizeiabsperrungen erreicht!");
                return false;
            }

            PoliceObjectModule.Instance.Add(1276148988, iPlayer.Player, ItemData, true);
            iPlayer.SendNewNotification(
                 ItemData.Name + " erfolgreich platziert!");
            
                iPlayer.PlayAnimation( 
                    (int) (AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]); 
                iPlayer.Player.TriggerEvent("freezePlayer", true); 
                await Task.Delay(4000); 
                iPlayer.Player.TriggerEvent("freezePlayer", false); 
                iPlayer.StopAnimation(); 
            
            return true;
        }
    }
}