using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Handler;
using VMP_CNR.Module.AnimationMenu;
using VMP_CNR.Module.Attachments;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.NSA.Observation;
using VMP_CNR.Module.NutritionPlayer;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Buffs;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Events;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Space;
using VMP_CNR.Module.Spawners;
using VMP_CNR.Module.Telefon.App;
using VMP_CNR.Module.Vehicles;
using VMP_CNR.Module.Voice;
using VMP_CNR.Module.Weapons.Component;
using static VMP_CNR.Module.Players.Db.DbPlayer;

namespace VMP_CNR.Module.Injury
{
    public static class InjuryPlayerExtension
    {
        private static uint FirstAidKitGangsterId = 676;
        private static uint FirstAidKitCops = 677;

        public static bool isInjured(this DbPlayer dbPlayer)
        {
            return dbPlayer.Injury.Id != 0;
        }
        
        public static bool isAlive(this DbPlayer dbPlayer)
        {
            return !isInjured(dbPlayer);
        }

        public static void revive(this DbPlayer dbPlayer)
        {
            if (dbPlayer.isInjured())
            {
                dbPlayer.Freeze(false, false, true);
                dbPlayer.Freezed = false;
                dbPlayer.StopAnimation();
                dbPlayer.Player.SetSharedData("death", false);
                dbPlayer.Injury = InjuryTypeModule.Instance.Get(0); // Gets Alive Injury
                dbPlayer.deadtime[0] = 0;

                if (dbPlayer.DimensionType[0] == DimensionType.World) NutritionModule.Instance.setHealthy(dbPlayer);

                VoiceListHandler.RemoveFromDeath(dbPlayer);
                dbPlayer.Player.SendNative(0x71BC8E838B9C6035, dbPlayer.Player.Handle);

                // no need  keine autom. Services mehr
                // ServiceModule.Instance.RemoveInjuredPlayerService(dbPlayer);

                //dbPlayer.Player.TriggerEvent("stopScreenEffect", "DeathFailMPIn");
                dbPlayer.Player.TriggerEvent("disableAllPlayerActions", false);
                ComponentManager.Get<DeathWindow>().Close(dbPlayer.Player);
                NutritionModule.Instance.setHealthy(dbPlayer);
                PlayerSpawn.InitPlayerSpawnData(dbPlayer.Player);
                dbPlayer.StopAnimation();
            }
            return;
        }

        public static void SetParamedicLicense(this DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            // Gebe lizenz
            MySQLHandler.ExecuteAsync($"UPDATE player SET mediclic = 1 WHERE id = '{dbPlayer.Id}'");
            MySQLHandler.ExecuteAsync($"UPDATE team SET medicslotsused = medicslotsused+1 WHERE id = '{dbPlayer.Team.Id}'");

            dbPlayer.Team.MedicSlotsUsed += 1;
            dbPlayer.ParamedicLicense = true;

            return;
        }


        public static void RemoveParamedicLicense(this DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (!dbPlayer.ParamedicLicense) return;

            // Gebe lizenz
            MySQLHandler.ExecuteAsync($"UPDATE player SET mediclic = 0 WHERE id = '{dbPlayer.Id}'");
            MySQLHandler.ExecuteAsync($"UPDATE team SET medicslotsused = medicslotsused-1 WHERE id = '{dbPlayer.Team.Id}'");

            dbPlayer.Team.MedicSlotsUsed -= 1;
            dbPlayer.ParamedicLicense = false;

            return;
        }
        
        public static void SetWayToKH(this DbPlayer dbPlayer)
        {
            if (dbPlayer.isInjured() && dbPlayer.Player.IsInVehicle)
            {
                InjuryType WayToKh = InjuryTypeModule.Instance.Get(InjuryModule.Instance.InjuryKrankentransport);
                dbPlayer.SetPlayerInjury(WayToKh);
            }
            return;
        }

        public static void SetPlayerInjury(this DbPlayer iPlayer, InjuryType injuryType)
        {
            iPlayer.Injury = injuryType;
            iPlayer.deadtime[0] = 0;
        }

        // Injury Time over? Set Deathscreen
        public static void SetDeathScreen(this DbPlayer dbPlayer)
        {
            dbPlayer.Weapons.Clear();
            dbPlayer.Player.TriggerEvent("emptyWeaponAmmo");
            dbPlayer.Container.ClearInventory();

            dbPlayer.ResetBuffs();
            dbPlayer.TakeBlackMoney(dbPlayer.blackmoney[0]); // reset lul

            VoiceListHandler.AddToDeath(dbPlayer);

            dbPlayer.SendNewNotification($"Du bist deinen {dbPlayer.Injury.Name} erlegen und befindest dich nun im Koma!");
            dbPlayer.Injury = InjuryTypeModule.Instance.Get(InjuryModule.Instance.InjuryDeathScreenId);
            dbPlayer.deadtime[0] = 0;
            dbPlayer.UHaftTime = 0;
            dbPlayer.Player.Transparency = 0;
            dbPlayer.Player.TriggerEvent("disableAllPlayerActions", true);


            NAPI.Task.Run(async () =>
            {
                GTANetworkAPI.Object obj = ObjectSpawn.Create(3469410940, dbPlayer.Player.Position, dbPlayer.Player.Rotation);

                dbPlayer.DeathObject = obj;
            });
            LogHandler.LogKilled(dbPlayer.Player.Name, dbPlayer.GetData("killername"), dbPlayer.GetData("killerweapon"));            
            // no need... keine automatischen Services mehr ... 
            //ServiceModule.Instance.RemoveInjuredPlayerService(dbPlayer);
            ComponentManager.Get<DeathWindow>().Show()(dbPlayer);
            dbPlayer.RemoveAllServerWeapons();
            dbPlayer.ResetAllWeaponComponents();
        }

        // Spawn Player after DeathScreen
        public static void SetPlayerDied(this DbPlayer dbPlayer)
        {
            dbPlayer.Player.TriggerEvent("disableAllPlayerActions", false);
            ComponentManager.Get<DeathWindow>().Close(dbPlayer.Player);

            dbPlayer.Injury = InjuryTypeModule.Instance.Get(0);
            if(dbPlayer.jailtime[0] <= 0) dbPlayer.SetData("komaSpawn", true);
            PlayerSpawn.OnPlayerSpawn(dbPlayer.Player);
            if (dbPlayer.DeathObject != null)
            {
                dbPlayer.DeathObject.Delete();
            }
            dbPlayer.Player.Transparency = 255;
            VoiceListHandler.RemoveFromDeath(dbPlayer);
        }

        // Set Player to Stabilized Injury if exists
        public static void Stabilize(this DbPlayer dbPlayer)
        {
            if (dbPlayer.isInjured())
            {
                if (dbPlayer.Injury.StabilizedInjuryId != 0 && dbPlayer.Injury.Id != InjuryModule.Instance.InjuryGangwar)
                {
                    dbPlayer.SendNewNotification($"Sie wurden stabilisiert!");
                    dbPlayer.SetPlayerInjury(InjuryTypeModule.Instance.Get((uint)dbPlayer.Injury.StabilizedInjuryId));

                    VoiceListHandler.RemoveFromDeath(dbPlayer);
                }
            }
        }

        public static async Task Medicate(this DbPlayer dbPlayer, DbPlayer medic)
        {
            if (dbPlayer == null || medic == null || !dbPlayer.IsValid() || !medic.IsValid()) return;
            if (dbPlayer.isInjured())
            {
                if(dbPlayer.Injury.Id == InjuryModule.Instance.InjuryDeathScreenId)
                {
                    medic.SendNewNotification("Diese Person liegt bereits im Koma!");
                    return;
                }

                if ((medic.IsAGangster() || medic.IsBadOrga()) && medic.ParamedicLicense && !medic.InParamedicDuty)
                {
                    medic.SendNewNotification("Du bist nicht im Medic-Dienst!");
                    return;
                }

                if ((medic.IsAMedic() && medic.Duty) || (medic.ParamedicLicense && (!medic.IsAGangster() || dbPlayer.TeamId == medic.TeamId)))
                {
                    if (dbPlayer.Injury.ItemToStabilizeId != 0 || dbPlayer.Player.Dimension != 0)
                    {
                        // Wenn Spieler ins KH gebracht werden muss dann in einen Krankenwagen setzen
                        if (dbPlayer.Injury.NeedHospital && !dbPlayer.IsOnMars())
                        {
                            if ((medic.Container.GetItemAmount(412) > 0 && medic.Container.GetItemAmount(dbPlayer.Injury.ItemToStabilizeId) > 0) ||
                                (medic.IsAGangster() && medic.Container.GetItemAmount(FirstAidKitGangsterId) > 0) ||
                                (!medic.IsAGangster() && medic.Container.GetItemAmount(FirstAidKitCops) > 0) || 
                                (medic.Team.Id == (int)teams.TEAM_MEDIC && medic.IsInDuty() && medic.Attachments.ContainsKey((int)Attachment.MEDICBAG)))
                            {
                                // Meidc Koffer
                                if (medic.Team.Id == (int)teams.TEAM_MEDIC && medic.IsInDuty() && medic.Attachments.ContainsKey((int)Attachment.MEDICBAG))
                                {
                                    // Remove Attachment
                                    AttachmentModule.Instance.RemoveAttachment(medic, (int)Attachment.MEDICBAG);
                                }
                                // Normal
                                else if (medic.Container.GetItemAmount(412) > 0 && medic.Container.GetItemAmount(dbPlayer.Injury.ItemToStabilizeId) > 0)
                                {
                                    // Remove Item
                                    medic.Container.RemoveItem(dbPlayer.Injury.ItemToStabilizeId, 1);
                                }
                                // Bad Notfallmedics
                                else if (medic.IsAGangster() && medic.Container.GetItemAmount(FirstAidKitGangsterId) > 0)
                                {
                                    // Remove Item
                                    medic.Container.RemoveItem(FirstAidKitGangsterId, 1);
                                }
                                // Beamten Notfallmedics
                                else if (medic.Team.Id != (int)teams.TEAM_MEDIC && medic.Team.IsStaatsfraktion() && medic.Container.GetItemAmount(FirstAidKitCops) > 0)
                                {
                                    // Remove Item
                                    medic.Container.RemoveItem(FirstAidKitCops, 1);
                                }
                                // Meldung zum behandeln
                                else {
                                    if (medic.Team.Id == (int)teams.TEAM_MEDIC || medic.ParamedicLicense)
                                    {
                                        medic.SendNewNotification(
                                            $"Fuer die Behandlung von {dbPlayer.Injury.Name} benötigen Sie {ItemModelModule.Instance.Get(412).Name} und {ItemModelModule.Instance.Get(dbPlayer.Injury.ItemToStabilizeId).Name}.");
                                    }
                                    else
                                    {
                                        medic.SendNewNotification(
                                            $"Fuer die Behandlung benötigen Sie {ItemModelModule.Instance.Get(412).Name} und {ItemModelModule.Instance.Get(dbPlayer.Injury.ItemToStabilizeId).Name}.");
                                    }
                                    return;
                                }
                                
                                SxVehicle sxVehicle = VehicleHandler.Instance.GetClosestVehicleFromTeamFilter(dbPlayer.Player.Position, (int)medic.TeamId, 15.0f, 4);

                                if (sxVehicle == null || !sxVehicle.IsValid()) 
                                {
                                    medic.SendNewNotification($"Kein Krankenwagen zum Transport in der naehe!");
                                    return;
                                }

                                medic.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl),
                                        Main.AnimationList["revive"].Split()[0], Main.AnimationList["revive"].Split()[1]);
                                medic.Player.TriggerEvent("freezePlayer", true);
                                if (medic.IsAGangster())
                                {
                                    Chats.sendProgressBar(medic, 15000);
                                    await Task.Delay(15000);
                                }
                                else
                                {
                                    Chats.sendProgressBar(medic, 9000);
                                    await Task.Delay(9000);
                                }
                                VehicleHandler.Instance.TrySetPlayerIntoVehicleOccupants(sxVehicle, dbPlayer);
                                await Task.Delay(1000);
                                medic.Player.TriggerEvent("freezePlayer", false);

                                // Save Old Data
                                if (dbPlayer.Injury != null && dbPlayer.Injury.Name != null)
                                {
                                    dbPlayer.SetData("injuredName", dbPlayer.Injury.Name);
                                }

                                dbPlayer.StopAnimation();
                                dbPlayer.SetWayToKH();
                                dbPlayer.Freeze(true);
                                dbPlayer.Player.TriggerEvent("noweaponsoninjury", true);
                                dbPlayer.SendNewNotification($"Du wurdest transportbereit gemacht!");
                                medic.SendNewNotification($"Du hast den Patienten transportbereit gemacht!");
                                medic.StopAnimation();

                                VoiceListHandler.RemoveFromDeath(dbPlayer);

                                return;
                            }
                            else
                            {
                                if (medic.Team.Id == (int)teams.TEAM_MEDIC || medic.ParamedicLicense)
                                {
                                    medic.SendNewNotification(
                                        $"Fuer die Behandlung von {dbPlayer.Injury.Name} benötigen Sie {ItemModelModule.Instance.Get(412).Name} und {ItemModelModule.Instance.Get(dbPlayer.Injury.ItemToStabilizeId).Name}.");
                                }
                                else
                                {
                                    medic.SendNewNotification(
                                        $"Fuer die Behandlung benötigen Sie {ItemModelModule.Instance.Get(412).Name} und {ItemModelModule.Instance.Get(dbPlayer.Injury.ItemToStabilizeId).Name}.");
                                }
                            }
                        }
                        else
                        {
                            // Meidc Koffer
                            if (medic.Team.Id == (int)teams.TEAM_MEDIC && medic.IsInDuty() && medic.Attachments.ContainsKey((int)Attachment.MEDICBAG))
                            {
                                // Remove medic
                                AttachmentModule.Instance.RemoveAttachment(medic, (int)Attachment.MEDICBAG);
                            }
                            else if (medic.Container.GetItemAmount(412) > 0 && medic.Container.GetItemAmount(dbPlayer.Injury.ItemToStabilizeId) > 0)
                            {
                                // Remove Item
                                medic.Container.RemoveItem(dbPlayer.Injury.ItemToStabilizeId, 1);
                            }
                            else if (medic.IsAGangster() && medic.Container.GetItemAmount(FirstAidKitGangsterId) > 0)
                            {
                                // Remove Item
                                medic.Container.RemoveItem(FirstAidKitGangsterId, 1);
                            }
                            else if (!medic.IsAGangster() && medic.Container.GetItemAmount(FirstAidKitCops) > 0)
                            {
                                // Remove Item
                                medic.Container.RemoveItem(FirstAidKitCops, 1);
                            }
                            else
                            {
                                if (medic.Team.Id == (int)teams.TEAM_MEDIC || medic.ParamedicLicense)
                                {
                                    medic.SendNewNotification(
                                        $"Fuer die Behandlung von {dbPlayer.Injury.Name} benötigen Sie {ItemModelModule.Instance.Get(412).Name} und {ItemModelModule.Instance.Get(dbPlayer.Injury.ItemToStabilizeId).Name}.");
                                }
                                else
                                {
                                    medic.SendNewNotification(
                                        $"Fuer die Behandlung benötigen Sie {ItemModelModule.Instance.Get(412).Name} und {ItemModelModule.Instance.Get(dbPlayer.Injury.ItemToStabilizeId).Name}.");
                                }
                                return;
                            }

                            medic.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl),
                                    Main.AnimationList["revive"].Split()[0], Main.AnimationList["revive"].Split()[1]);
                            medic.Player.TriggerEvent("freezePlayer", true);
                            if (medic.IsAGangster())
                            {
                                Chats.sendProgressBar(medic, 15000);
                                await Task.Delay(15000);
                            }
                            else
                            {
                                Chats.sendProgressBar(medic, 9000);
                                await Task.Delay(9000);
                            }
                            medic.Player.TriggerEvent("freezePlayer", false);
                            medic.StopAnimation();

                            //ToDo: Make player walk injured for some time
                            dbPlayer.revive();
                            NutritionModule.Instance.setHealthy(dbPlayer);
                            dbPlayer.SendNewNotification($"Du wurdest vom Medic behandelt!");
                            
                            // Prevent Weapon-Switch after Treatment
                            dbPlayer.TimeSinceTreatment = DateTime.Now;
                            dbPlayer.RecentlyInjured = true;
                            
                            medic.SendNewNotification($"Du hast den Patienten behandelt!");

                            VoiceListHandler.RemoveFromDeath(dbPlayer);

                            // keine Behandlungskosten für Badmedics
                            if(medic.IsAMedic() || medic.TeamId == (uint)teams.TEAM_ARMY)
                            {
                                int khcosts = 0;

                                switch (dbPlayer.EconomyIndex)
                                {
                                    case EconomyIndex.Low:
                                        khcosts = 500;
                                        break;
                                    case EconomyIndex.Mid:
                                        khcosts = 1000;
                                        break;
                                    case EconomyIndex.Rich:
                                        khcosts = 2500;
                                        break;
                                    case EconomyIndex.Superrich:
                                        khcosts = 3000;
                                        break;
                                    case EconomyIndex.Jeff:
                                        khcosts = 4000;
                                        break;
                                }

                                Random random = new Random();

                                khcosts = random.Next((Convert.ToInt32(khcosts * 0.9)), (Convert.ToInt32(khcosts * 1.5)));

                                if (dbPlayer.IsACop() && dbPlayer.IsInDuty())
                                {
                                    khcosts = khcosts / 4; // wegen Beamter im Dienst weil Steuern etc...
                                }

                                if (dbPlayer.InsuranceType > 0 || dbPlayer.HasCopInsurance())
                                {
                                    // 1 wenn hat oder wenn nicht privat und copinsurance
                                    if (dbPlayer.InsuranceType == 1 || (dbPlayer.InsuranceType != 2 && dbPlayer.HasCopInsurance()))
                                    {
                                        khcosts = khcosts / 2;
                                        dbPlayer.SendNewNotification("Durch ihre Krankenversicherung wurden 50% der Behandlungskosten übernommen!");
                                    }
                                    else if (dbPlayer.InsuranceType == 2)
                                    {
                                        khcosts = 0;
                                        dbPlayer.SendNewNotification("Durch ihre private Krankenversicherung wurden 100% der Behandlungskosten übernommen!");
                                    }
                                }

                                if (khcosts > 0)
                                {
                                    dbPlayer.SendNewNotification($"Für Ihre Behandlung wurden Krankenhauskosten von ${khcosts} berechnet und von ihrem Konto abgebucht!");
                                    dbPlayer.TakeBankMoney(khcosts, "Behandlungskosten", true);
                                }
                            }
                            return;
                        }
                    }
                }
                else
                {
                    medic.SendNewNotification($"{dbPlayer.Injury.Name} koennen sie nicht behandeln!");
                }
            }
        }

        public static void SetPlayerKomaSpawn(this DbPlayer dbPlayer)
        {
            int khcosts = 0;

            switch(dbPlayer.EconomyIndex)
            {
                case EconomyIndex.Low:
                    khcosts = 500;
                    break;
                case EconomyIndex.Mid:
                    khcosts = 5000;
                    break;
                case EconomyIndex.Rich:
                    khcosts = 20000;
                    break;
                case EconomyIndex.Superrich:
                    khcosts = 50000;
                    break;
                case EconomyIndex.Jeff:
                    khcosts = 80000;
                    break;
            }

            Random random = new Random();

            khcosts = random.Next((Convert.ToInt32(khcosts * 0.9)), (Convert.ToInt32(khcosts * 1.2)));

            if(dbPlayer.IsACop() && dbPlayer.IsInDuty())
            {
                khcosts = khcosts / 4; // wegen Beamter im Dienst weil Steuern etc...
            }

            if(dbPlayer.InsuranceType > 0 || dbPlayer.HasCopInsurance())
            {
                // 1 wenn hat oder wenn nicht privat und copinsurance
                if(dbPlayer.InsuranceType == 1 || (dbPlayer.InsuranceType != 2 && dbPlayer.HasCopInsurance()))
                {
                    khcosts = khcosts / 2;
                    dbPlayer.SendNewNotification("Durch ihre Krankenversicherung wurden 50% der Kosten übernommen!");
                }
                else if(dbPlayer.InsuranceType == 2)
                {
                    khcosts = 0;
                    dbPlayer.SendNewNotification("Durch ihre private Krankenversicherung wurden 100% der Kosten übernommen!");
                }
            }

            if(khcosts > 0)
            {
                dbPlayer.SendNewNotification($"Sie wurden nach ihrem Koma aus dem Krankenhaus entlassen! Krankenhauskosten von ${khcosts} wurde von ihrem Konto abgebucht!");
                dbPlayer.TakeBankMoney(khcosts, "Krankenhauskosten", true);
            }

            return;
        }



        public static void ApplyDeathEffects(this DbPlayer dbPlayer)
        {
            try
            {
                if (dbPlayer.isInjured())
                {
                    // Set Voice To Normal
                    dbPlayer.Player.SetSharedData("voiceRange", (int)VoiceRange.normal);
                    dbPlayer.SetData("voiceType", 1);
                    dbPlayer.Player.TriggerEvent("setVoiceType", 1);

                    // Disable Funk complete
                    VoiceModule.Instance.turnOffFunk(dbPlayer);

                    dbPlayer.Player.SetSharedData("death", true);

                    // Cancel Phonecall
                    dbPlayer.Player.TriggerEvent("hangupCall");
                    dbPlayer.Player.TriggerEvent("cancelPhoneCall");
                    dbPlayer.ResetData("current_caller");

                    if (dbPlayer.HasData("current_caller"))
                    {
                        NSAObservationModule.CancelPhoneHearing((int)dbPlayer.handy[0]);
                        var result = int.TryParse(dbPlayer.GetData("current_caller"), out int number);
                        if (result)
                        {
                            DbPlayer l_CalledPlayer = TelefonInputApp.GetPlayerByPhoneNumber(number);
                            if (l_CalledPlayer != null)
                            {
                                l_CalledPlayer.Player.TriggerEvent("hangupCall");
                                l_CalledPlayer.Player.TriggerEvent("cancelPhoneCall");
                                l_CalledPlayer.ResetData("current_caller");
                                NSAObservationModule.CancelPhoneHearing(number);
                            }
                        }
                    }

                    dbPlayer.Freeze(true, false, true);

                    dbPlayer.StopAnimation();
                    dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "combat@damage@rb_writhe", "rb_writhe_loop");

                    dbPlayer.Player.SetSharedData("death", true);
                }
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }
    }
}
