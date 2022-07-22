using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using Task = System.Threading.Tasks.Task;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async System.Threading.Tasks.Task<bool> VehiclePlate(DbPlayer iPlayer, Item item)
        {
            await Task.Delay(1);
            GTANetworkAPI.NAPI.Task.Run(() => ComponentManager.Get<TextInputBoxWindow>().Show()(
                iPlayer, new TextInputBoxWindowObject() { Title = "Kennzeichen", Callback = "SetKennzeichen", Message = "Gib ein Kennzeichen ein (8 Zeichen)" }));
            return false;
        }
    }
}