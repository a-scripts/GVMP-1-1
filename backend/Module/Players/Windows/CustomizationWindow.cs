using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Players.Db;
using System.Threading.Tasks;
using VMP_CNR.Handler;

namespace VMP_CNR.Module.Players.Windows
{
    public class CustomizationWindow : Window<Func<DbPlayer, CharacterCustomization, bool>>
    {
        private class ShowEvent : Event
        {
            //private string InventoryContent { get; } // --- appears to be empty if used?
            [JsonProperty(PropertyName = "customization")] private CharacterCustomization Customization { get; }
            [JsonProperty(PropertyName = "level")] private int Level { get; }

            public ShowEvent(DbPlayer dbPlayer, CharacterCustomization customization) : base(dbPlayer)
            {
                Customization = customization;
                Level = dbPlayer.HasData("firstCharacter") ? 0 : dbPlayer.Level;
            }
        }
        public override Func<DbPlayer, CharacterCustomization, bool> Show()
        {
            return (player, customization) => OnShow(new ShowEvent(player, customization));
        }

        public CustomizationWindow() : base("CharacterCreator")
        {
        }

        [RemoteEvent]
        public async void UpdateCharacterCustomization(Player player, string charakterJSON, int price)
        {
            try { 
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || String.IsNullOrEmpty(charakterJSON)) return;
            if (!charakterJSON.StartsWith("{") && !charakterJSON.EndsWith("}")) {
                DiscordHandler.SendMessage("Fehlerhafter Customization String", charakterJSON + " | " + dbPlayer.Player.Name + " | "+ dbPlayer.Player.Address);

                return;

            }
            CharacterCustomization customization = JsonConvert.DeserializeObject<CharacterCustomization>(charakterJSON);
            int result = dbPlayer.TakeAnyMoney(price);

            if (result != -1)
            {
                // Buy Customization
                dbPlayer.Customization = customization;
                dbPlayer.SaveCustomization();
                dbPlayer.SendNewNotification($"Aussehen geaendert, dir wurden {price}$ vom Konto abgezogen", title: "Info", notificationType: PlayerNotification.NotificationType.INFO);
            }
            else
            {
                dbPlayer.SendNewNotification("Nicht genug Geld", notificationType: PlayerNotification.NotificationType.ERROR);
            }

            // Update Charakter
            //dbPlayer.ApplyCharacter();
            dbPlayer.ResetData("firstCharacter");

            await dbPlayer.StopCustomization();

        }
            catch (Exception ex)
            {
               Console.WriteLine(ex);
            }
}
        [RemoteEvent]
        public async void cutsceneEnded(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null) return;

            var pos = new GTANetworkAPI.Vector3(-1144.26, -2792.27, 27.708);
            float heading = 237.428f;
            uint dimension = 0;

            player.Freeze(true, true, true);
            player.SetPosition(pos);
            iPlayer.SetDimension(dimension);
            player.SetRotation(heading);

            await Task.Delay(20000);
            iPlayer.Player.Freeze(false, true, true);
        }

        [RemoteEvent]
        public async void StopCustomization(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null) return;

            //dbPlayer.ApplyCharacter();
            await dbPlayer.StopCustomization();
        }

    }
}
