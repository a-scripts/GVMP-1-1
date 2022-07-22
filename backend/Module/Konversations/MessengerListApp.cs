﻿using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Konversations
{
    public class MessengerListApp : SimpleApp
    {
        public MessengerListApp() : base("MessengerListApp") { }

        [RemoteEvent]
        public void requestKonversations(Player p_Player)
        {
            try
            {                    
                DbPlayer iPlayer = p_Player.GetPlayer();
                if (iPlayer == null || !iPlayer.IsValid()) return;

                List<PlayerKonversation> PlayerKonversations = new List<PlayerKonversation>();

                // NSA Cut
                if (iPlayer.HasData("nsa_smclone"))
                {
                    DbPlayer targetPlayer = Players.Players.Instance.GetByDbId(iPlayer.GetData("nsa_smclone"));
                    if (targetPlayer == null || !targetPlayer.IsValid() || targetPlayer.phoneSetting.flugmodus)
                    {
                        iPlayer.ResetData("nsa_smclone");
                        return;
                    }
                    


                    foreach (var kvp in KonversationsModule.Instance.konversations.ToList().Where(ks => ks.Key.Player1 == targetPlayer.handy[0] || ks.Key.Player2 == targetPlayer.handy[0]))
                    {
                        if (kvp.Value == null) continue;

                        PlayerKonversation PlayerKonversation = new PlayerKonversation();
                        PlayerKonversation.KonversationId = kvp.Key.Id;
                        PlayerKonversation.KonversationPartnerName = (kvp.Key.Player1 == targetPlayer.handy[0]) ?
                            (targetPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player2) != null ? targetPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player2) : "" + kvp.Key.Player2) :
                            (targetPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player1) != null ? targetPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player1) : "" + kvp.Key.Player1);

                        PlayerKonversation.KonversationPartnerNumber = (kvp.Key.Player1 == targetPlayer.handy[0]) ? kvp.Key.Player2 : kvp.Key.Player1;

                        PlayerKonversation.KonversationUpdatedTime = kvp.Key.LastUpdated.ToString();
                        PlayerKonversation.KonversationMessages = new List<PlayerKonversationMessage>();

                        foreach (ConvMessage konversationMessage in kvp.Value)
                        {
                            if (konversationMessage == null) continue;
                            string l_Sender = "";
                            if (konversationMessage.SenderId == targetPlayer.handy[0])
                                l_Sender = "Ich";
                            else if (targetPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(konversationMessage.SenderId) != null)
                                l_Sender = targetPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(konversationMessage.SenderId);
                            else
                                l_Sender = "" + konversationMessage.SenderId;

                            string l_Message = konversationMessage.Message;
                            l_Message = l_Message.Replace("\"", "");
                            l_Message = l_Message.Replace("'", "");
                            l_Message = l_Message.Replace("`", "");
                            l_Message = l_Message.Replace("´", "");

                            PlayerKonversationMessage PlayerKonversationMessage = new PlayerKonversationMessage();
                            PlayerKonversationMessage.Id = konversationMessage.Id;
                            PlayerKonversationMessage.KonversationMessageUpdatedTime = GetUpdatedTimeFormated(konversationMessage.TimeStamp, true);
                            PlayerKonversationMessage.MessageSenderName = l_Sender;
                            PlayerKonversationMessage.Message = l_Message;
                            PlayerKonversationMessage.Receiver = (targetPlayer.handy[0] == konversationMessage.SenderId);
                            PlayerKonversation.KonversationMessages.Add(PlayerKonversationMessage);
                        }
                        PlayerKonversations.Add(PlayerKonversation);
                    }
                }
                else
                {
                    foreach (var kvp in KonversationsModule.Instance.konversations.ToList().Where(ks => ks.Key.Player1 == iPlayer.handy[0] || ks.Key.Player2 == iPlayer.handy[0]))
                    {
                        if (kvp.Value == null) continue;

                        PlayerKonversation PlayerKonversation = new PlayerKonversation();
                        PlayerKonversation.KonversationId = kvp.Key.Id;
                        PlayerKonversation.KonversationPartnerName = (kvp.Key.Player1 == iPlayer.handy[0]) ?
                            (iPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player2) != null ? iPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player2) : "" + kvp.Key.Player2) :
                            (iPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player1) != null ? iPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(kvp.Key.Player1) : "" + kvp.Key.Player1);

                        PlayerKonversation.KonversationPartnerNumber = (kvp.Key.Player1 == iPlayer.handy[0]) ? kvp.Key.Player2 : kvp.Key.Player1;

                        PlayerKonversation.KonversationUpdatedTime = kvp.Key.LastUpdated.ToString();
                        PlayerKonversation.KonversationMessages = new List<PlayerKonversationMessage>();

                        foreach (ConvMessage konversationMessage in kvp.Value)
                        {
                            if (konversationMessage == null) continue;
                            string l_Sender = "";
                            if (konversationMessage.SenderId == iPlayer.handy[0])
                                l_Sender = "Ich";
                            else if (iPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(konversationMessage.SenderId) != null)
                                l_Sender = iPlayer.PhoneContacts.TryGetPhoneContactNameByNumber(konversationMessage.SenderId);
                            else
                                l_Sender = "" + konversationMessage.SenderId;

                            string l_Message = konversationMessage.Message;
                            l_Message = l_Message.Replace("\"", "");
                            l_Message = l_Message.Replace("'", "");
                            l_Message = l_Message.Replace("`", "");
                            l_Message = l_Message.Replace("´", "");

                            PlayerKonversationMessage PlayerKonversationMessage = new PlayerKonversationMessage();
                            PlayerKonversationMessage.Id = konversationMessage.Id;
                            PlayerKonversationMessage.KonversationMessageUpdatedTime = GetUpdatedTimeFormated(konversationMessage.TimeStamp, true);
                            PlayerKonversationMessage.MessageSenderName = l_Sender;
                            PlayerKonversationMessage.Message = l_Message;
                            PlayerKonversationMessage.Receiver = (iPlayer.handy[0] == konversationMessage.SenderId);
                            PlayerKonversation.KonversationMessages.Add(PlayerKonversationMessage);
                        }
                        PlayerKonversations.Add(PlayerKonversation);
                    }
                }
                
                PlayerKonversations.Sort((a, b) =>
                {
                    DateTime l_FirstElement = DateTime.Parse(a.KonversationUpdatedTime);
                    DateTime l_SecondElement = DateTime.Parse(b.KonversationUpdatedTime);

                    return l_SecondElement.CompareTo(l_FirstElement);
                });

                foreach (var l_Konv in PlayerKonversations)
                {
                    DateTime l_Date = DateTime.Parse(l_Konv.KonversationUpdatedTime);
                    l_Konv.KonversationUpdatedTime = GetUpdatedTimeFormated(l_Date, true);
                }

                var l_Json = NAPI.Util.ToJson(PlayerKonversations);
                TriggerEvent(p_Player, "responseKonversations", l_Json);
            }
            catch(Exception e)
            {
                Logging.Logger.Crash(e);
            }
            
        }
        public string GetUpdatedTimeFormated(DateTime dateTime, bool detailed = false)
        {
            string reverseDate = dateTime.Day + "/" + dateTime.Month;
            if (dateTime.AddDays(1) > DateTime.Now) // innerhalb 1 Day
            {
                reverseDate = "Heute";
            }
            else if (dateTime.AddDays(2) > DateTime.Now && dateTime.AddDays(3) < DateTime.Now)
            {
                reverseDate = "Gestern";
            }

            string result = reverseDate;
            if (detailed)
            {
                result = (dateTime.AddMinutes(1) > DateTime.Now) ? "Jetzt" : reverseDate + " " + dateTime.Hour + ":" + dateTime.Minute;
            }

            return result;
            // Heute 12:40 || Gestern 12:40 || 12.07 12:40 || Jetzt
        }
    }
    
    public class PlayerKonversation
    {
        [JsonProperty(PropertyName = "messagesId")]
        public uint KonversationId { get; set; }

        [JsonProperty(PropertyName = "messageSender")]
        public string KonversationPartnerName { get; set; }

        [JsonProperty(PropertyName = "messageSenderNumber")]
        public uint KonversationPartnerNumber { get; set; }

        [JsonProperty(PropertyName = "lastMessage")]
        public string KonversationUpdatedTime { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public List<PlayerKonversationMessage> KonversationMessages { get; set; }
    }

    public class PlayerKonversationMessage
    {
        [JsonProperty(PropertyName = "id")]
        public uint Id { get; set; }

        [JsonProperty(PropertyName = "sender")]
        public string MessageSenderName { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string KonversationMessageUpdatedTime { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "receiver")]
        public bool Receiver { get; set; }
    }
}
