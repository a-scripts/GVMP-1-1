using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.NSA;
using VMP_CNR.Module.NSA.Observation;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.ReversePhone;
using VMP_CNR.Module.Space;
using VMP_CNR.Module.Telefon.App;
using VMP_CNR.Module.Voice;

namespace VMP_CNR.Module.Players.Phone
{
    public static class PhoneCall
    {
        public static string PHONECALL_TYPE = "phone_calling";
        public static string PHONENUMBER = "phone_number";

        public static bool IsPlayerInCall(Player player)
        {
            DbPlayer iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return false;
            // is the requested player in phone a call
            if (!iPlayer.HasData(PHONECALL_TYPE)) return false;

            return iPlayer.GetData(PHONECALL_TYPE) == "waiting" ||
                   iPlayer.GetData(PHONECALL_TYPE) == "incoming" ||
                   iPlayer.GetData(PHONECALL_TYPE) == "active" ||
                   iPlayer.HasData("current_caller");
        }
        
        public static async void CancelPhoneCall(this DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid() || !dbPlayer.CanInteract()) return;

            if (dbPlayer.HasData("current_caller"))
            {
                int callNumber = dbPlayer.GetData("current_caller");
                if (callNumber == 0) return;

                DbPlayer dbCalledPlayer = await CallManageApp.GetPlayerByPhoneNumber(callNumber);
                if (dbCalledPlayer != null && dbCalledPlayer.IsValid() && dbCalledPlayer.HasData("current_caller"))
                {
                    if (dbCalledPlayer.GetData("current_caller") == dbPlayer.handy[0])
                    {
                        dbCalledPlayer.SetData("current_caller", 0);
                        dbCalledPlayer.ResetData("current_caller");
                        dbCalledPlayer.Player.TriggerEvent("cancelCall", 0);
                        dbCalledPlayer.Player.TriggerEvent("setCallingPlayer", "");

                        ReversePhoneModule.Instance.AddPhoneHistory(dbPlayer, (int)dbCalledPlayer.handy[0], 0);
                        ReversePhoneModule.Instance.AddPhoneHistory(dbCalledPlayer, (int)dbPlayer.handy[0], 0);

                        NSAObservationModule.CancelPhoneHearing((int)dbCalledPlayer.handy[0]);

                        if (!NAPI.Player.IsPlayerInAnyVehicle(dbCalledPlayer.Player) && dbCalledPlayer.CanInteract())
                            dbCalledPlayer.StopAnimation();
                    }
                    dbCalledPlayer.playerWhoHearRingtone = new List<DbPlayer>();

                }
            }

            NSAObservationModule.CancelPhoneHearing((int)dbPlayer.handy[0]);
            dbPlayer.ResetData("current_caller");
            dbPlayer.Player.TriggerEvent("cancelCall", 0);
            dbPlayer.Player.TriggerEvent("setCallingPlayer", "");

            if (!NAPI.Player.IsPlayerInAnyVehicle(dbPlayer.Player))
                dbPlayer.StopAnimation();
        }

        public static bool CanUserstartCall(DbPlayer dbPlayer)
        {
            // can player have a call
            if (!CanPlayerHaveCall(dbPlayer))
            {
                dbPlayer.SendNewNotification(
                    
                    "Fuer diese Aktion benötigst du ein verfuegbares " +
                    ItemModelModule.Instance.Get(174).Name);
                return false;
            }

            // is player already in call
            if (IsPlayerInCall(dbPlayer.Player))
            {
                dbPlayer.SendNewNotification(
                     "Du befindest dich bereits in einem Gespraech.");
                return false;
            }

            // does player have enough money for code
            /*if (dbPlayer.guthaben[0] < 10)
            {
                dbPlayer.SendNewNotification(
                    Chats.MsgHandy +
                    "Dein Guthaben reicht nicht aus. Ein Anruf kostet $10.");
                return false;
            }*/

            // is player on another planet
            if (dbPlayer.IsOnMars())
            {
                dbPlayer.SendNewNotification("Auf dem Mars existieren keine Telefon-Masten!");
                return false;
            }

            // player can have a phone call
            return true;
        }

        public static bool CanPlayerHaveCall(DbPlayer dbPlayer)
        {
            // verify is not cuffed or tied
            if (dbPlayer.IsCuffed || dbPlayer.IsTied) return false;

            // verify has item smartphone
            return dbPlayer.Container.GetItemAmount(174) >= 1;
        }
        
        public static void SetPlayerCallStatus(Player player, string state = "waiting",
            uint phoneNumber = 0)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            // set player in phone a call
            dbPlayer.SetData(PHONECALL_TYPE, state);

            // reset connected number on 0
            if (phoneNumber == 0)
            {
                dbPlayer.ResetData(PHONENUMBER);
                return;
            }

            // set the connected phone number
            dbPlayer.SetData(PHONENUMBER, phoneNumber);
        }
        
        public static void StartPhoneCall(DbPlayer dbPlayer, uint number)
        {
            if (dbPlayer.Container.GetItemAmount(
                    174) < 1)
            {
                dbPlayer.SendNewNotification(
                    
                    "Sie besitzen kein Telefon.");
                return;
            }

            // is not own phone number
            if (number == dbPlayer.handy[0])
            {
                dbPlayer.SendNewNotification(
                     "Sie können sich nicht selber anrufen.");
                return;
            }

            // player can start a phone call
            if (!CanUserstartCall(dbPlayer)) return;

            try
            {
                dbPlayer.CancelPhoneCall();

                // find number of reqested player in the Users lists
                foreach (var user in Players.Instance.GetValidPlayers())
                {
                    // player object not valid
                    if (!user.IsValid()) continue;

                    // player found by phone number
                    if (user.handy[0] == number &&
                        user.Container.GetItemAmount(174) < 1)
                    {
                        // No phone on Mars
                        if (user.IsOnMars())
                        {
                            dbPlayer.SendNewNotification(Chats.MsgHandy + "Die von Ihnen gewaehlte Nummer ist derzeit nicht verfuegbar!");
                            return;
                        }

                        var requestorNumber = dbPlayer.handy[0];
                        var requestedNumber = user.handy[0];
                        
                        // is requested player already in phone call
                        if (IsPlayerInCall(user.Player))
                        {
                            dbPlayer.SendNewNotification(
                                
                                "Der Anschluss ist zurzeit besetzt!");
                            return;
                        }


                        // set requested player in state incoming
                        SetPlayerCallStatus(user.Player, "incoming", requestorNumber);
                        
                        // set requestor player in state waiting
                        dbPlayer.guthaben[0] = dbPlayer.guthaben[0] - 10;
                        SetPlayerCallStatus(dbPlayer.Player, "waiting", requestedNumber);

                        // Set Funk to push-to-talk if active (dauersenden)
                        if(dbPlayer.funkStatus == FunkStatus.Active)
                        {
                            dbPlayer.funkStatus = FunkStatus.Deactive;
                            VoiceModule.Instance.refreshFQVoiceForPlayerFrequenz(dbPlayer);
                        }
                        return;
                    }
                }

                // not a valid number
                dbPlayer.SendNewNotification(
                    Chats.MsgHandy +
                    "Die von Ihnen gewaehlte Nummer ist derzeit nicht verfuegbar!");
            }
            catch (Exception e)
            {
                Logger.Print(e.ToString());
            }
        }
    }
}
