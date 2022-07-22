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
        public static async Task<bool> carusorosso(DbPlayer iPlayer, ItemModel ItemData)
        {
            
                iPlayer.PlayAnimation( 
                    (int) (AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["bro"].Split()[0], Main.AnimationList["bro"].Split()[1]); 
                iPlayer.Player.TriggerEvent("freezePlayer", true); 
                await Task.Delay(3000); 
                iPlayer.Player.TriggerEvent("freezePlayer", false); 
                iPlayer.StopAnimation(); 
            
            return true;
        }
    }
}