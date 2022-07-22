﻿using GTANetworkAPI;
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

namespace VMP_CNR.Module.Paintball.Menu
{
    public class PaintballEnterMenuBuilder : MenuBuilder
    {
        public PaintballEnterMenuBuilder() : base(PlayerMenu.PaintballEnterMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            var l_Menu = new Module.Menu.Menu(Menu, "Paintball Arenen");
            l_Menu.Add($"Schließen");
            l_Menu.Add("Eigene Lobby erstellen (15.000$)");

            Console.WriteLine(NAPI.Util.ToJson(PaintballAreaModule.Instance.GetAll()));

            foreach (KeyValuePair<uint, PaintballArea> pba in PaintballAreaModule.Instance.GetAll())
            {
                l_Menu.Add($"{pba.Value.Name} ${pba.Value.LobbyEnterPrice} {pba.Value.pbPlayers.Count}/{pba.Value.MaxLobbyPlayers}");
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
                    if (iPlayer.Level < 5)
                    {
                        iPlayer.SendNewNotification("Dein Level ist zu niedrig!");
                    }
                    else
                    {

                        if (iPlayer.money[0] < 15000)
                        {
                            iPlayer.SendNewNotification("Du hast nicht genug Geld!");

                            return true;
                        }
                        ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = $"Passwort festlegen", Callback = "PbaSetPassword" });

                    }
                }
                else
                {
                    uint idx = 1;
                    uint finalcount = 0;
                    foreach (KeyValuePair<uint, PaintballArea> pba in PaintballAreaModule.Instance.GetAll())
                    {
                        finalcount = pba.Value.Id + 1;
                        Console.WriteLine("" + finalcount);
                    }

                    while (idx < finalcount) {
                        foreach (KeyValuePair<uint, PaintballArea> pba in PaintballAreaModule.Instance.GetAll())
                        {
                            Console.WriteLine(idx + " | " + index);

                            if (idx == index)
                            {
                                Console.WriteLine("GEFUNDEN!!!");
                                if (pba.Value.pbPlayers.Count >= pba.Value.MaxLobbyPlayers)
                                {
                                    iPlayer.SendNewNotification("Diese Lobby ist bereits voll!");
                                    return false;
                                }
                                if (pba.Value.Password.Length >= 1)
                                {
                                    iPlayer.SetData("pba_choose", pba.Value.Id);
                                    ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = $"Beitritt Paintball", Callback = "PbaConfirmPassword", CustomData = new { id = pba.Value.Id.ToString() } });
                                }
                                else
                                {
                                    ComponentManager.Get<ConfirmationWindow>().Show()(iPlayer, new ConfirmationObject($"Beitritt Paintball ${pba.Value.LobbyEnterPrice}", $"ACHTUNG: Sämtliche Waffen werden Dir vom Sicherheitspersonal abgenommen, es gibt keine Garantie, dass du deine Waffen wiederbekommst. Hinweis: Das Paintball-Team entlässt Dich sofort mit einem /quit.", "PbaConfirm", pba.Value.Id.ToString(), ""));
                                }
                                idx = finalcount;
                                return true;
                            }
                            else
                            {
                                Console.WriteLine(idx + " | WRONG " + index);

                            }
                            idx = idx + 1;
                        }

                        
                    }
                }

                return true;
            }
        }
    }
}
