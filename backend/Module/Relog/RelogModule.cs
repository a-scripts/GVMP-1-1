//using System;
//using System.Collections.Generic;
//using GTANetworkAPI;
//using VMP_CNR.Module.Configurations;
//using VMP_CNR.Module.Players.Db;

//namespace VMP_CNR.Module.Relog
//{
//    public class RelogModule : Module<RelogModule>
//    {
//        private readonly Dictionary<string, DateTime> relogs;

//        public RelogModule()
//        {
//            relogs = new Dictionary<string, DateTime>();
//        }

//        /*public override bool OnPlayerConnected(Player Player)
//        {
//            if (Configuration.Instance.DevMode) return false;
//            if (!relogs.ContainsKey(Player.Name)) return true;
//            var lastRelog = relogs[Player.Name];
//            if (lastRelog < DateTime.Now)
//            {
//                // Remove expired relog
//                relogs.Remove(Player.Name);
//            }
//            else if (!Main.devmode)
//            {
//                Player.SendNotification("Bitte warte 10 Sekunden.");
//                Player.Kick("Bitte warte 10 Sekunden.");
//                return false;
//            }

//            return true;
//        }*/

//        public override void OnPlayerDisconnected(DbPlayer dbPlayer, string reason)
//        {
//            if (relogs.ContainsKey(dbPlayer.GetName()))
//            {
//                relogs[dbPlayer.GetName()] = DateTime.Now.AddSeconds(10);
//            }
//            else
//            {
//                relogs.Add(dbPlayer.GetName(), DateTime.Now.AddSeconds(10));
//            }
//        }
//    }
//}