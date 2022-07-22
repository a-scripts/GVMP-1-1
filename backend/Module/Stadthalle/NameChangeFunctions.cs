using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Houses;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Stadthalle
{
    public class NameChangeFunctions
    {
        public static void DoNameChange(DbPlayer dbPlayer, String newName, bool marriage)
        {
            var split = newName.Split("_");
            if (split[0].Length < 3 || split[1].Length < 3)
            {
                dbPlayer.SendNewNotification("Der Vor & Nachname muss jeweils mindestens 3 Buchstaben beinhalten!");
                return;
            }

            int addSum = 0;

            if(split[0].Contains("-"))
            {
                addSum = split[0].Length * 150000;
            }
            else if(split[1].Contains("-"))
            {
                addSum = split[1].Length * 150000;
            }


            int kosten = marriage ? dbPlayer.Level * 10000 : dbPlayer.Level * 50000;
            kosten += addSum;
            if (!dbPlayer.TakeBankMoney(kosten, $"Namensänderung - {newName}"))
            {
                dbPlayer.SendNewNotification($"Die Namensänderung würde {kosten} $ kosten. Diese Summe hast du nicht auf dem Konto");
                return;
            }

            if (dbPlayer.ownHouse[0] != 0)
            {
                House house = HouseModule.Instance.GetByOwner(dbPlayer.Id);
                house.OwnerName = newName;
                house.SaveOwner();
            }

            if (marriage) dbPlayer.Container.RemoveItem(670);
            Logger.AddNameChangeLog(dbPlayer.Id, dbPlayer.Level, dbPlayer.Player.Name, newName, marriage);
            Players.Players.Instance.SendMessageToAuthorizedUsers("log",
                dbPlayer.GetName() + $"({dbPlayer.Id}) hat den Namen zu {newName} geändert");
            MySQLHandler.ExecuteAsync($"UPDATE player SET name = '{newName}' WHERE id = '{dbPlayer.Id}'");
            dbPlayer.SendNewNotification($"Du hast deinen Namen erfolgreich zu {newName} geändert! Bitte beende nun das Spiel und trag deinen neuen Namen in den GVRP-Launcher ein!", PlayerNotification.NotificationType.ADMIN, duration: 30000);
            dbPlayer.Kick("Namensaenderung");

        }
    }
}
