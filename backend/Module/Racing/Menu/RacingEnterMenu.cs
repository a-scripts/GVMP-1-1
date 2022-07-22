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

namespace VMP_CNR.Module.Racing.Menu
{
    public class RacingEnterMenuBuilder : MenuBuilder
    {
        public RacingEnterMenuBuilder() : base(PlayerMenu.RacingEnterMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            var l_Menu = new Module.Menu.Menu(Menu, "Racing Arena");

            l_Menu.Add($"Schließen");
            foreach (RacingLobby racingLobby in RacingModule.Lobbies)
            {
                l_Menu.Add($"Lobby {racingLobby.LobbyId} {racingLobby.Desc} | {racingLobby.RacingPlayers.Count}/{RacingModule.MaxLobbyPlayers}");
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
                    MenuManager.DismissCurrent(iPlayer);
                    return true;
                }
                else
                {
                    int idx = 1;
                    
                    foreach (RacingLobby racingLobby in RacingModule.Lobbies)
                    {
                        if(idx == index)
                        {
                            if(racingLobby.RacingPlayers.Count >= RacingModule.MaxLobbyPlayers)
                            {
                                iPlayer.SendNewNotification("Diese Lobby ist bereits voll!");
                                return false;
                            }
                            
                            if(!iPlayer.TakeMoney(RacingModule.LobbyEnterPrice))
                            {
                                iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(RacingModule.LobbyEnterPrice));
                                return false;
                            }

                            iPlayer.SetPlayerIntoRacing(racingLobby);
                            iPlayer.SendNewNotification("Sie sind dem Rennen beigetreten. Fahren Sie ihre Bestzeit!");
                            return true;
                        }
                        idx++;
                    }
                }

                return true;
            }
        }
    }
}
