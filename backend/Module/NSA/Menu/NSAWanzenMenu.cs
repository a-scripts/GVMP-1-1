using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Handler;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.NSA.Observation;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Telefon.App;
using VMP_CNR.Module.Vehicles;

namespace VMP_CNR.Module.NSA.Menu
{
    public class NSAWanzenMenuBuilder : MenuBuilder
    {
        public NSAWanzenMenuBuilder() : base(PlayerMenu.NSAWanzeMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            var l_Menu = new Module.Menu.Menu(Menu, "NSA Aktive Wanzen");
            l_Menu.Add($"Schließen");
            l_Menu.Add("Abhören beenden");

            foreach (NSAWanze nSAPeilsender in NSAObservationModule.NSAWanzen.ToList().Where(w => w.active))
            {
                l_Menu.Add($"{nSAPeilsender.Name}");
            }

            return l_Menu;
        }

        public override IMenuEventHandler GetEventHandler()
        {
            return new EventHandler();
        }

        private class EventHandler : IMenuEventHandler
        {
            public bool OnSelect(int index, DbPlayer iPlayer)
            {
                if(index == 0)
                {
                    return true;
                }
                else if(index == 1)
                {
                    NSAWanze nsaWanze = NSAObservationModule.NSAWanzen.ToList().Where(w => w.HearingAgents.Contains(iPlayer)).FirstOrDefault();

                    if (nsaWanze != null)
                    {
                        iPlayer.Player.TriggerEvent("setCallingPlayer", "");
                        iPlayer.SendNewNotification("Abhören beendet.");

                        nsaWanze.HearingAgents.Remove(iPlayer);
                        return true;
                    }

                    iPlayer.SendNewNotification("Sie haben keine aktive Wanze.");
                    return true;
                }

                int i = 2;

                foreach (NSAWanze nSAPeilsender in NSAObservationModule.NSAWanzen.ToList().Where(w => w.active))
                {
                    if(i == index)
                    {
                        if(nSAPeilsender.PlayerId != 0)
                        {
                            DbPlayer target = Players.Players.Instance.GetByDbId(nSAPeilsender.PlayerId);
                            if (target == null || !target.IsValid()) return true;

                            if (!iPlayer.Player.IsInVehicle) return true;

                            SxVehicle sxVehicle = iPlayer.Player.Vehicle.GetVehicle();

                            if (sxVehicle == null || !sxVehicle.IsValid() || sxVehicle.Data.Id != 1296) return true;

                            NSAWanze nsaWanze = NSAObservationModule.NSAWanzen.ToList().Where(w => w.HearingAgents.Contains(iPlayer)).FirstOrDefault();

                            if (nsaWanze != null)
                            {
                                iPlayer.SendNewNotification("Du hörst bereits eine Wanze ab!");
                            }


                            if (iPlayer.HasData("current_caller"))
                            {
                                iPlayer.SendNewNotification("Während eines Anrufes kannst du keine Wanze mithören!");
                                return false;
                            }

                            if(iPlayer.Player.Position.DistanceTo(target.Player.Position) > 400)
                            {
                                iPlayer.SendNewNotification("Kein Empfang zur Wanze..");
                                return false;
                            }

                            nSAPeilsender.HearingAgents.Add(iPlayer);

                            iPlayer.Player.TriggerEvent("setCallingPlayer", target.VoiceHash);

                            // Orten
                            iPlayer.SendNewNotification("Wanze wird gestartet...!");
                            return true;
                        }
                        
                        return true;
                    }
                    i++;
                }

                MenuManager.DismissCurrent(iPlayer);
                return true;
            }
        }
    }
}
