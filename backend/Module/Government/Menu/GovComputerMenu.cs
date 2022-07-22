using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;

namespace VMP_CNR.Module.Government.Menu
{
    public class GovComputerMenuBuilder : MenuBuilder
    {
        public GovComputerMenuBuilder() : base(PlayerMenu.GOVComputerMenu)
        {

        }

        public override Module.Menu.Menu Build(DbPlayer p_DbPlayer)
        {
            var l_Menu = new Module.Menu.Menu(Menu, "GOV Computermenü");
            l_Menu.Add($"Schließen");
            l_Menu.Add($"Scheidung bearbeiten");
            l_Menu.Add($"Namensänderung");
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
                    if(iPlayer.TeamId != (uint)teams.TEAM_GOV || (iPlayer.Player.Position.DistanceTo(GovernmentModule.ComputerBuero1Pos) > 5.0 && iPlayer.Player.Position.DistanceTo(GovernmentModule.ComputerBuero2Pos) > 5.0))
                    {
                        return true;
                    }

                    ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject() { Title = "Scheidung bearbeiten", Callback = "DivorceGovConfirm", Message = "Bitte gib den Namen der Person ein die du Scheiden möchtest. ((Kosten belaufen sich auf (VisumstufePartner1 + VisumstufePartner2) * 40.000$) /2" });
                    return false;
                }
                else if (index == 2)
                {
                    iPlayer.SendNewNotification("Kein Anschluss unter dieser Nummer! ... gibts noch nicht");
                    return false;
                }
                return true;
            }
        }
    }

    public class GovComputerEvents : Script
    {
        /*
        [RemoteEvent]
        public void NameChangeChoose(Player p_Player, string nameChangePlayer, string nothing)
        {
            DbPlayer dbPlayer = p_Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid())
            {
                return;
            }

            if (dbPlayer.TeamId != (uint)teams.TEAM_GOV) return;

            DbPlayer playerToDivorce = Players.Players.Instance.FindPlayer(nameChangePlayer);
            if (playerToDivorce == null || !playerToDivorce.IsValid()) return;

            
        }
         */
        [RemoteEvent]
        public void DivorceGovConfirm(Player p_Player, string divorcePersonName)
        {
            DbPlayer dbPlayer = p_Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid())
            {
                return;
            }

            if (dbPlayer.TeamId != (uint)teams.TEAM_GOV) return;

            DbPlayer playerToDivorce = Players.Players.Instance.FindPlayer(divorcePersonName);
            if (playerToDivorce == null || !playerToDivorce.IsValid()) return;

            if (playerToDivorce.Player.Position.DistanceTo(dbPlayer.Player.Position) > 10) return;

            if (playerToDivorce.married[0] != 0)
            {
                string marryName = "";
                int marryLevel = 0;
                using (MySqlConnection conn = new MySqlConnection(Configuration.Instance.GetMySqlConnection()))
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = $"SELECT name, level FROM player WHERE id = '{playerToDivorce.married[0]}' LIMIT 1";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.HasRows)
                            {
                                marryName = reader.GetString("name");
                                marryLevel = reader.GetInt32("level");
                            }
                        }
                        conn.Close();
                    }
                }
                if (marryName != "")
                {
                    if (!playerToDivorce.TakeBankMoney(40000 * (playerToDivorce.Level + marryLevel) / 2, $"Scheidung von - {marryName}"))
                    {
                        dbPlayer.SendNewNotification($"Die Scheidung würde {40000 * (playerToDivorce.Level + marryLevel) / 2} $ kosten. Diese Summe konnte nicht abgebucht werden!");
                        return;
                    }

                    playerToDivorce.SendNewNotification($"Du hast dich erfolgreich von {marryName} scheiden lassen.");

                    var findPlayer = Players.Players.Instance.FindPlayer(playerToDivorce.married[0]);
                    if (findPlayer == null || !findPlayer.IsValid())
                    {
                        MySQLHandler.ExecuteAsync($"UPDATE player SET married = 0 WHERE id = '{playerToDivorce.married[0]}'");
                    }
                    else
                    {
                        findPlayer.married[0] = 0;
                        findPlayer.SendNewNotification($"{playerToDivorce.Player.Name} hat sich von dir scheiden lassen.");
                    }

                    Logger.AddDivorceLog(playerToDivorce.Id, playerToDivorce.Level, playerToDivorce.married[0]);
                    playerToDivorce.married[0] = 0;

                    dbPlayer.Team.SendNotification($"{dbPlayer.GetName()} hat die Scheidung von {playerToDivorce.GetName()} und {marryName} durchgeführt!");

                    return;
                }



                return;
            }

            dbPlayer.SendNewNotification("Diese Person ist nicht verheiratet!");
            return;




        }
    }
}
