using GTANetworkAPI;
using System;
using System.Text.RegularExpressions;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.RemoteEvents;

namespace VMP_CNR.Module.Players.Phone
{
    public class PhoneApp : SimpleApp
    {
        public PhoneApp() : base("PhoneApp")
        {
        }

        [RemoteEvent]
        public void updatePhoneContact(Player player, object oldNumberObj, object newNumberObj, string name)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, name)) return;
            int oldNumber = Convert.ToInt32(oldNumberObj);
            int newNumber = Convert.ToInt32(newNumberObj);
            if (oldNumber <= 0 || oldNumber > 99999999) return;
            if (newNumber <= 0 || newNumber > 99999999) return;
            if (!dbPlayer.CheckForSpam(DbPlayer.OperationType.ContactUpdate)) return;
            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_#\s-]+$"))
            {
                dbPlayer.SendNewNotification("Kontakt konnte nicht aktualisiert werden!", notificationType:PlayerNotification.NotificationType.ERROR);
                return;
            }
            dbPlayer.PhoneContacts.Update((uint)oldNumber, (uint)newNumber, name);
        }

        [RemoteEvent]
        public void addPhoneContact(Player player, string name, object numberObj)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, name)) return;
            int number = Convert.ToInt32(numberObj);
            if (number <= 0 || number > 99999999) return;
            if (!dbPlayer.CheckForSpam(DbPlayer.OperationType.ContactAdd)) return;
            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_#\s-]+$"))
            {
                dbPlayer.SendNewNotification("Kontakt konnte nicht eingespeichert werden.", notificationType:PlayerNotification.NotificationType.ERROR);
                return;
            }

            dbPlayer.PhoneContacts.Add(name, (uint)number);
        }

        [RemoteEvent]
        public void delPhoneContact(Player player, int number)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (!dbPlayer.CheckForSpam(DbPlayer.OperationType.ContactRemove)) return;
            if (number < 0 || number > 99999999) return;
            dbPlayer.PhoneContacts.Remove((uint)number);
        }

        [RemoteEvent]
        public void requestPhoneContacts(Player player) {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            player.TriggerEvent("responsePhoneContacts", dbPlayer.PhoneContacts.GetJson());
        }
    }
}