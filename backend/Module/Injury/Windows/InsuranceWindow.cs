using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Voice;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Business.NightClubs;
using System.Threading.Tasks;
using VMP_CNR.Module.Schwarzgeld;
using VMP_CNR.Module.NSA;
using VMP_CNR.Module.Teams;
using VMP_CNR.Module.Events.CWS;
using VMP_CNR.Module.Teams.Shelter;

namespace VMP_CNR.Module.Injury.Windows
{
    
    public class InsuranceWindow : Window<Func<DbPlayer, bool>>
    {
        private class ShowEvent : Event
        {
            public ShowEvent(DbPlayer dbPlayer) : base(dbPlayer)
            {
            }
        }

        public InsuranceWindow() : base("Insurance")
        {
        }

        public override Func<DbPlayer, bool> Show()
        {
            return (player) => OnShow(new ShowEvent(player));
        }

        [RemoteEvent]
        public void setInsurance(Player Player, int insuranceType)
        {
            DbPlayer iPlayer = Player.GetPlayer();

            if (iPlayer == null || !iPlayer.IsValid()) return;

            if (insuranceType < 0 || insuranceType > 2) return;

            if(iPlayer.InsuranceType == insuranceType)
            {
                iPlayer.SendNewNotification("Du hast diese Art von Krankenversicherung bereits aktiv!");
                return;
            }

            switch(insuranceType)
            {
                case 0:
                    iPlayer.SendNewNotification("Du hast dich für keine Krankenversicherung entschieden, alle Kosten trägst du nun selbst!");
                    iPlayer.InsuranceType = insuranceType;
                    break;
                case 1:
                    iPlayer.SendNewNotification("Du hast dich für eine Krankenversicherung entschieden, es werden 50% der Behandlungs und Komakosten übernommen!");
                    iPlayer.InsuranceType = insuranceType;
                    break;
                case 2:
                    iPlayer.SendNewNotification("Du hast dich für eine private Krankenversicherung entschieden, es werden 100% der Behandlungs und Komakosten übernommen!");
                    iPlayer.InsuranceType = insuranceType;
                    break;
            }


            string insurance = "keine";
            if (iPlayer.InsuranceType == 1)
            {
                insurance = "vorhanden";
            }
            else if (iPlayer.InsuranceType == 2)
            {
                insurance = "privat";
            }


            iPlayer.Player.TriggerEvent("setInsurance", insurance);
            iPlayer.SaveInsurance();
        }
    }
}
