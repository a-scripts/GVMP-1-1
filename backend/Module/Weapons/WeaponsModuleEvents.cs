using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using Newtonsoft.Json;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Weapons.Data;

namespace VMP_CNR.Module.Weapons
{
    public class WeaponsModuleEvents : Script
    {
        [RemoteEvent]
        public void getWeaponAmmoAnswer(Player p_Player, string p_AnswerJson)
        {
            var l_Player = p_Player.GetPlayer();
            PlayerWeaponData[] l_Data = JsonConvert.DeserializeObject<PlayerWeaponData[]>(p_AnswerJson);

            foreach (var l_Entry in l_Data)
            {
                var l_Detail = l_Player.Weapons.Find(x => x.WeaponDataId == l_Entry.WeaponDataID);
                if (l_Detail == null)
                    continue;

                if (l_Entry.Ammo > l_Detail.Ammo && l_Detail.Ammo >= 0 && (l_Entry.Ammo - l_Detail.Ammo > 5))
                {
                    WeaponData l_WeaponData = WeaponDataModule.Instance.Get(l_Detail.WeaponDataId);
                    if (l_Data == null)
                        continue;

                    l_Player.Player.SetWeaponAmmo((WeaponHash)l_WeaponData.Hash, l_Detail.Ammo);
                    l_Player.Player.TriggerEvent("updateWeaponAmmo", l_Detail.WeaponDataId, l_Detail.Ammo);

                    if (ServerFeatures.IsActive("ac-ammocheck"))
                        continue;

                    Players.Players.Instance.SendMessageToAuthorizedUsers("log", $"DRINGENDER-Anticheat-Verdacht: {l_Player.Player.Name} (Munition versucht zu erhöhen! (Waffe: {l_WeaponData.Name} Vorher: {l_Detail.Ammo} Nachher: {l_Entry.Ammo})");
                    Logging.Logger.LogToAcDetections(l_Player.Id, Logging.ACTypes.WeaponCheat, $"(Weaponcheat (Munition): {l_WeaponData.Name} Munition versucht zu erhöhen! (Waffe: {l_WeaponData.Name} Vorher: {l_Detail.Ammo} Nachher: {l_Entry.Ammo})");
                }
                else
                    l_Detail.Ammo = l_Entry.Ammo;
            }
        }
    }

    public class PlayerWeaponData
    {
        [JsonProperty("id")]
        public int WeaponDataID { get; set; }

        [JsonProperty("ammo")]
        public int Ammo { get; set; }
    }
}
