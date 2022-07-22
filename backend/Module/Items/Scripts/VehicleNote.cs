﻿using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.PlayerAnimations;
using VMP_CNR.Module.Players.Windows;

namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> VehicleNote(DbPlayer iPlayer, Item ItemData)
        {
            if (!iPlayer.Player.IsInVehicle) return false;
            await Task.Delay(1);
            NAPI.Task.Run(() => ComponentManager.Get<TextInputBoxWindow>().Show()(
                iPlayer, new TextInputBoxWindowObject() { Title = "Notiz", Callback = "SetVehicleNote", Message = "Gib eine Notiz ein (15 Zeichen) Nur Buchstaben und Zahlen" }));
            return false;
        }
    }
}