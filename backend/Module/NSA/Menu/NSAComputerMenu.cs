using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.FIB;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.NSA.Observation;
using VMP_CNR.Module.PlayerName;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Vehicles.InteriorVehicles;

namespace VMP_CNR.Module.NSA.Menu
{
    public class NSAComputerMenuBuilder : MenuBuilder
    {
        public NSAComputerMenuBuilder() : base(PlayerMenu.NSAComputerMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            var l_Menu = new Module.Menu.Menu(Menu, "StaatObserv v1.0");
            l_Menu.Add($"Schließen");
            l_Menu.Add($"FIB Transaction History");
            l_Menu.Add($"FIB Energie History");
            l_Menu.Add($"Aktive Ortung beenden");
            if (p_DbPlayer.CanNSADuty() && !p_DbPlayer.IsNSADuty)
            {
                l_Menu.Add($"IT Dienst betreten");
            }
            else if(p_DbPlayer.CanNSADuty() && p_DbPlayer.IsNSADuty)
            {
                l_Menu.Add($"IT Dienst verlassen");
            }
            if (p_DbPlayer.IsNSADuty)
            {
                // light
                l_Menu.Add($"Observationen");
                l_Menu.Add($"Gespräch beenden");
                l_Menu.Add($"Laufende Anrufe");
                l_Menu.Add($"Nummer Nachverfolgung");
                l_Menu.Add($"Aktive Peilsender");

                // normal
                if (p_DbPlayer.IsNSAState >= (int)NSA.NSARangs.NORMAL)
                {
                    l_Menu.Add($"Rufnummer ändern");
                    l_Menu.Add($"Smartphone Cloning beenden");
                    l_Menu.Add($"Aktive Wanzen");
                    l_Menu.Add($"Keycard Nutzung (Tür)");
                }
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
                if (index == 0)
                {
                    MenuManager.DismissCurrent(iPlayer);
                    return true;
                }
                else if (index == 1)
                {
                    Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSATransactionHistory, iPlayer).Show(iPlayer);
                    return false;
                }
                else if (index == 2)
                {
                    Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSAEnergyHistory, iPlayer).Show(iPlayer);
                    return false;
                }
                else if (index == 3)
                {
                    if (iPlayer.HasData("nsaOrtung"))
                    {
                        iPlayer.ResetData("nsaOrtung");
                        iPlayer.SendNewNotification("Ortung beendet!");
                        return true;
                    }
                    if (iPlayer.HasData("nsaPeilsenderOrtung"))
                    {
                        iPlayer.ResetData("nsaPeilsenderOrtung");
                        iPlayer.SendNewNotification("Fahrzeug Ortung beendet!");
                        return true;
                    }
                    iPlayer.SendNewNotification("Keine aktive Ortung vorhanden!");
                    return true;
                }
                else if (index == 4)
                {
                    if (!iPlayer.CanNSADuty()) return true;
                    if(!iPlayer.IsNSADuty)
                    {
                        iPlayer.IsNSADuty = true;
                        iPlayer.SendNewNotification($"Sie sind nun im IT Dienst!");
                    }
                    else
                    {
                        iPlayer.IsNSADuty = false;
                        iPlayer.SendNewNotification($"Sie sind nun nicht mehr im IT Dienst!");
                    }
                    return true;
                }
                if (iPlayer.IsNSADuty)
                {
                    if (index == 5)
                    {
                        Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSAObservationsList, iPlayer).Show(iPlayer);
                        return false;
                    }
                    else if (index == 6)
                    {
                        if (iPlayer.HasData("nsa_activePhone"))
                        {
                            iPlayer.Player.TriggerEvent("setCallingPlayer", "");
                            iPlayer.ResetData("nsa_activePhone");
                            iPlayer.SendNewNotification("Mithören beendet!");
                            return true;
                        }
                        return true;
                    }
                    else if (index == 7)
                    {
                        Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSACallListMenu, iPlayer).Show(iPlayer);
                        return false;
                    }
                    else if (index == 8)
                    {
                        ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = "Nummer Nachverfolgung", Callback = "NSACheckNumber", Message = "Geben Sie eine Nummer ein:" });
                        MenuManager.DismissCurrent(iPlayer);
                        return true;
                    }
                    else if (index == 9)
                    {
                        Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSAPeilsenderMenu, iPlayer).Show(iPlayer);
                        return false;
                    }
                    else if (index == 10)
                    {
                        if (iPlayer.IsNSAState <= (int)NSARangs.LIGHT) return false;

                        ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = "Rufnummer ändern", Callback = "NSAChangePhoneNumber", Message = "Geben Sie eine Rufnummer ein:" });
                        MenuManager.DismissCurrent(iPlayer);
                        return true;
                    }
                    else if (index == 11)
                    {
                        if (iPlayer.IsNSAState <= (int)NSARangs.LIGHT) return false;

                        iPlayer.ResetData("nsa_smclone");
                        iPlayer.SendNewNotification($"Smartphone Clone beendet!");
                        MenuManager.DismissCurrent(iPlayer);
                        return true;
                    }
                    else if (index == 12)
                    {
                        if (iPlayer.IsNSAState <= (int)NSARangs.LIGHT) return false;

                        Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSAWanzeMenu, iPlayer).Show(iPlayer);
                        return false;
                    }
                    else if (index == 13)
                    {

                        if (iPlayer.IsNSAState <= (int)NSARangs.LIGHT) return false;

                        Module.Menu.MenuManager.Instance.Build(VMP_CNR.Module.Menu.PlayerMenu.NSADoorUsedsMenu, iPlayer).Show(iPlayer);
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
