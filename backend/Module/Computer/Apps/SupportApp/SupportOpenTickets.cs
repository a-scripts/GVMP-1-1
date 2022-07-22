﻿using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Support;

namespace VMP_CNR.Module.Computer.Apps.SupportApp
{
    public class SupportOpenTickets : SimpleApp
    {
        public SupportOpenTickets() : base("SupportOpenTickets") { }

        [RemoteEvent]
        public async void requestOpenSupportTickets(Player Player)
        {
            
                var dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid()) return;

                if (dbPlayer.RankId == 0)
                {
                    dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                    return;
                }

                List<ticketObject> ticketList = new List<ticketObject>();
                var tickets = TicketModule.Instance.GetOpenTickets();

                foreach (var ticket in tickets)
                {
                    string accepted = string.Join(',', ticket.Accepted);

                    ticketList.Add(new ticketObject() { id = (int)ticket.Player.Id, creator = ticket.Player.GetName(), text = ticket.Description, created_at = ticket.Created_at, accepted_by = accepted });
                }

                var serviceJson = NAPI.Util.ToJson(ticketList);
                
                TriggerEvent(Player, "responseOpenTicketList", serviceJson);
            
        }

        [RemoteEvent]
        public async void acceptOpenSupportTicket(Player Player, string name)
        {
            
                var dbPlayer = Player.GetPlayer();
                if (dbPlayer == null || !dbPlayer.IsValid()) return;

            if (!MySQLHandler.IsValidNoSQLi(dbPlayer, name)) return;

            if (dbPlayer.RankId == 0)
                {
                    dbPlayer.SendNewNotification(MSG.Error.NoPermissions());
                    return;
                }

                var findplayer = Players.Players.Instance.FindPlayer(name);
                if (findplayer == null) return;

                bool response = TicketModule.Instance.Accept(dbPlayer, findplayer);

                dbPlayer.SendNewNotification(response ? $"Sie haben das Ticket von {findplayer.GetName()} angenommen!" : $"Das Ticket von {findplayer.GetName()} wurde bereits angenommen!");
                findplayer.SendNewNotification("Ihr Ticket wurde angenommen!");
            
        }

        public class ticketObject
        {
            [JsonProperty(PropertyName = "id")]
            public int id { get; set; }

            [JsonProperty(PropertyName = "creator")]
            public string creator { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string text { get; set; }

            [JsonProperty(PropertyName = "created_at")]
            public DateTime created_at { get; set; }

            [JsonProperty(PropertyName = "accepted_by")]
            public string accepted_by { get; set; }
        }
    }
}
