using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.PlayerUI.Apps;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Telefon.App.Settings.Ringtone;

namespace VMP_CNR.Module.Telefon.App.Settings
{
    public class RingtoneResponseObject
    {
        public List<Ringtone.Ringtone> ringtones { get; set; }

        public string volume { get; set; }
    }

    public class SettingsEditRingtones : SimpleApp
    {
        public SettingsEditRingtones() : base("SettingsEditRingtonesApp") { }


        [RemoteEvent]
        public void requestRingtoneList(Player player)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null) return;

            Console.WriteLine(NAPI.Util.ToJson(RingtoneModule.Instance.getRingtonesForPlayer(dbPlayer)));
            player.TriggerEvent("responseRingtoneList", NAPI.Util.ToJson(RingtoneModule.Instance.getRingtonesForPlayer(dbPlayer)));

        }

        [RemoteEvent]
        public void saveRingtone(Player player, int ringtoneId)
        {
            DbPlayer dbPlayer = player.GetPlayer();
            if (dbPlayer == null) return;


            dbPlayer.ringtone = RingtoneModule.Instance.Get((uint)ringtoneId);

            dbPlayer.SaveRingtone();


            dbPlayer.Player.TriggerEvent("RingtoneFile", dbPlayer.ringtone.File);
            dbPlayer.Player.TriggerEvent("setActiveRingtone", ringtoneId);
        }

    }

}
