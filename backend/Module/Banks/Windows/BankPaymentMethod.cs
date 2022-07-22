using System;
using System.Collections.Generic;
using VMP_CNR.Module.Players.Db;
using Newtonsoft.Json;
using GTANetworkAPI;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Business;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Teams.Shelter;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Tattoo;
using VMP_CNR.Module.Gangwar;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.NSA;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Boerse;

namespace VMP_CNR.Module.Banks.Windows
{
    public class BankPaymentMethod : Window<Func<DbPlayer, int, bool>>
    {
        private class ShowEvent : Event
        {

            [JsonProperty(PropertyName = "price")]
            private int Price { get; }


            public ShowEvent(DbPlayer dbPlayer, int price) :
         base(dbPlayer)
            {
                Price = price;

            }
        }

        public BankPaymentMethod() : base("PaymentMethods")
        {
        }

        public override Func<DbPlayer, int, bool> Show()
        {
            return (player, price) => OnShow(new ShowEvent(player, price));
        }
        [RemoteEvent]
        public void selectPaymentMethod(Player Player, string method)
        {
            NAPI.Task.Run(() =>
            {

                var iPlayer = Player.GetPlayer();
                if (iPlayer == null || !iPlayer.IsValid()) return;

                iPlayer.SetData("selected", method);
                iPlayer.Player.TriggerEvent("Moneywindownocursor");
            });
        
        }
    }
}