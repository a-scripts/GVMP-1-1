using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Players.Db;
using GTANetworkAPI;
using System.Linq;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Players;
using System.Threading.Tasks;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Logging;

namespace VMP_CNR.Module.NutritionPlayer
{
    /*
     * 
     * Nutrition v.02
     * 
     * - WeaponSwitch AVG low/high disabled (injuryModule)
     * - Interactions
     * - TrainingStation
     * - TakeAShit
     * 
     * 
     * 
     * 
     * 
     */

    public class NutritionModule : Module<NutritionModule>
    {
        public float StandardKcal = 2500f;
        public float StandardFett = 70f;
        public float StandardWasser = 2000f;
        public float StandardZucker = 50f;

        public bool NutritionActive = true;

        public class Calc
        {
            public float wasser { get; set; }
            public float kcal { get; set; }
            public float zucker { get; set; }
            public float fett { get; set; }
            public float max { get; set; }
            public float min { get; set; }
            public float avg { get; set; }
        }

        public override void OnPlayerFirstSpawn(DbPlayer dbPlayer)
        {
            if (!NutritionActive) return;

            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            dbPlayer.Nutrition = new Nutrition(dbPlayer);
            dbPlayer.Nutrition.GetNutritionBySQL();
        }

        public override void OnPlayerDisconnected(DbPlayer dbPlayer, string reason)
        {
            dbPlayer.Nutrition.save();
        }

        public override void OnPlayerFirstSpawnAfterSync(DbPlayer dbPlayer)
        {
            //PUT PlayerSIDE HIER
            if (!NutritionActive) return;

            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            dbPlayer.SetData("showNutrition", true);
            PushNutritionToPlayer(dbPlayer);
        }

        public void TakeAShit(DbPlayer dbPlayer)
        {
            if (!NutritionActive) return;
            //Reduce Nutrition by shit
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (dbPlayer.Nutrition.Wasser > StandardWasser) { dbPlayer.Nutrition.Wasser = StandardWasser; }
            if (dbPlayer.Nutrition.Kcal > StandardKcal) { dbPlayer.Nutrition.Kcal = StandardKcal; }
            if (dbPlayer.Nutrition.Fett > StandardFett) { dbPlayer.Nutrition.Fett = StandardFett; }
            if (dbPlayer.Nutrition.Zucker > StandardZucker) { dbPlayer.Nutrition.Zucker = StandardZucker; }
           
        }

        public void DoAnimation(DbPlayer dbPlayer, string anim, int milliseconds = 1000)
        {
            try { 
            if (!NutritionActive) return;
            /*Aktuell keine Animationen*/
            return;
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (dbPlayer.isInjured()) return;
            if (dbPlayer.Player.IsInVehicle) return;
            if (!dbPlayer.CanInteract()) return;
            switch (anim)
            {
                case "sboden":
                    Task.Run(async () =>
                    {
                        dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "missmic2leadinmic_2_intleadout", "ko_on_floor_idle");
                        dbPlayer.Player.TriggerEvent("freezePlayer", true);
                        dbPlayer.SetCannotInteract(true);
                        await Task.Delay(milliseconds);

                        dbPlayer.Player.TriggerEvent("freezePlayer", false);
                        dbPlayer.StopAnimation();
                        //perm gehen
                        dbPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "move_f@injured", "walk");
                    });
                    break;
                case "lboden":
                    Task.Run(async () =>
                    {
                        dbPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "combat@damage@writheidle_a", "writhe_idle_a");
                        dbPlayer.Player.TriggerEvent("freezePlayer", true);
                        dbPlayer.SetCannotInteract(true);
                        await Task.Delay(milliseconds);

                        dbPlayer.Player.TriggerEvent("freezePlayer", false);
                        dbPlayer.StopAnimation();
                        //perm gehen
                        dbPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "move_f@injured", "walk");
                    });
                    break;
                case "lgehen":
                    Task.Run(async () =>
                    {
                        dbPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "move_f@injured", "walk");
                        dbPlayer.SetCannotInteract(true);
                        await Task.Delay(milliseconds);

                        dbPlayer.SetCannotInteract(false);
                        dbPlayer.StopAnimation();
                    });
                    break;
                case "sgehen":
                    Task.Run(async () =>
                    {
                        dbPlayer.PlayAnimation((int)(AnimationFlags.AllowPlayerControl | AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody), "move_m@wading", "walk");
                        dbPlayer.SetCannotInteract(true);
                        await Task.Delay(milliseconds);

                        dbPlayer.SetCannotInteract(false);
                        dbPlayer.StopAnimation();
                    });
                    break;
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void decreaseHealth(DbPlayer dbPlayer, int health)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (dbPlayer.isInjured()) return;
            if (dbPlayer.Player.Health <= health)
            {
                dbPlayer.SetData("injured_by_nutrition", true);
            }

            dbPlayer.SetHealth(dbPlayer.Player.Health - health);
        }

        public void modifyNutrition(DbPlayer dbPlayer, float Wasser = 0f, float Kcal = 0f, float Fett = 0f, float Zucker = 0f)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            dbPlayer.Nutrition.Wasser += Wasser;
            dbPlayer.Nutrition.Kcal += Kcal;
            dbPlayer.Nutrition.Fett += Fett;
            dbPlayer.Nutrition.Zucker += Zucker;
        }

        public void WipeEffects(DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            dbPlayer.SetCannotInteract(false);
            dbPlayer.Player.TriggerEvent("freezePlayer", false);
            dbPlayer.StopAnimation();
        }

        public void DoEffects(DbPlayer dbPlayer, bool effects=true)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (dbPlayer.isInjured()) return;
            if (!effects) return;

            if (dbPlayer.Nutrition.avg >= 150)
            {
                decreaseHealth(dbPlayer, 100);
                return;
            }
            if (dbPlayer.Nutrition.avg >= 140)
            {
                decreaseHealth(dbPlayer, 10);
                DoAnimation(dbPlayer, "sboden", 180000);
                return;
            }
            if (dbPlayer.Nutrition.avg >= 130)
            {
                decreaseHealth(dbPlayer, 10);
                DoAnimation(dbPlayer, "lboden", 120000);
                return;
            }
            if (dbPlayer.Nutrition.avg >= 120)
            {
                decreaseHealth(dbPlayer, 10);
                DoAnimation(dbPlayer, "lgehen", 60000);
                return;
            }
            if (dbPlayer.Nutrition.avg >= 110)
            {
                decreaseHealth(dbPlayer, 5);
                DoAnimation(dbPlayer, "sgehen", 120000);
                return;
            }
            if (dbPlayer.Nutrition.avg >= 100)
            {
                DoAnimation(dbPlayer, "sgehen", 60000);
                return;
            }
            if (dbPlayer.Nutrition.avg >= 90)
            {
                DoAnimation(dbPlayer, "lgehen", 30000);
                return;
            }
            if (dbPlayer.Nutrition.avg >= 80)
            {
                DoAnimation(dbPlayer, "lgehen", 15000);
                return;
            }
            //--------- MINUS Effekte-------
            if (dbPlayer.Nutrition.avg <= -50)
            {
                decreaseHealth(dbPlayer, 100);
                return;
            }
            if (dbPlayer.Nutrition.avg <= -40)
            {
                decreaseHealth(dbPlayer, 10);
                DoAnimation(dbPlayer, "sboden", 180000);
                return;
            }
            if (dbPlayer.Nutrition.avg <= -30)
            {
                decreaseHealth(dbPlayer, 10);
                DoAnimation(dbPlayer, "lboden", 120000);
                return;
            }
            if (dbPlayer.Nutrition.avg <= -20)
            {
                decreaseHealth(dbPlayer, 10);
                DoAnimation(dbPlayer, "lgehen", 60000);
                return;
            }
            if (dbPlayer.Nutrition.avg <= -10)
            {
                decreaseHealth(dbPlayer, 5);
                DoAnimation(dbPlayer, "sgehen", 120000);
                return;
            }
            if (dbPlayer.Nutrition.avg <= 0)
            {
                decreaseHealth(dbPlayer, 2);
                DoAnimation(dbPlayer, "sgehen", 60000);
                return;
            }
            if (dbPlayer.Nutrition.avg <= 10)
            {
                DoAnimation(dbPlayer, "lgehen", 30000);
                return;
            }
            if (dbPlayer.Nutrition.avg <= 20)
            {
                DoAnimation(dbPlayer, "lgehen", 15000);
                return;
            }

        }

        public List<float> Calculation(DbPlayer dbPlayer)
        {
            if (dbPlayer != null && !dbPlayer.IsValid()) return new List<float>();
            List<float> calc = new List<float>
            {
                dbPlayer.Nutrition.Wasser / ((this.StandardWasser * 2) / 100),
                dbPlayer.Nutrition.Kcal / ((this.StandardKcal * 2) / 100),  
                dbPlayer.Nutrition.Fett / ((this.StandardFett * 2) / 100),
                dbPlayer.Nutrition.Zucker / ((this.StandardZucker * 2) / 100)
            };
            if (Math.Abs(calc.Max() / 50) > Math.Abs(calc.Min() / 50))
            {
                dbPlayer.Nutrition.avg = calc.Max();
            }
            else
            {
                dbPlayer.Nutrition.avg = calc.Min();
            }
            return calc;
        }
        public static bool Between(float number, float min, float max)
        {
            return number >= min && number <= max;
        }
        public void PushNutritionToPlayer(DbPlayer dbPlayer,bool effects=true)
        {
            if (!NutritionActive) return;

            try
            {
                if (dbPlayer == null || !dbPlayer.IsValid()) return;
                var calc = Calculation(dbPlayer);
                if (calc.Count() < 1) return;
    
                try
                {
                    dbPlayer.Player.TriggerEvent("responseNutrition",
                      $"{{" +
                      $"\"wasser\":{calc[0].ToString().Replace(",", ".")},\"wshow\":{(dbPlayer.GetData("showNutrition") ? "1" : "0")}," +
                      $"\"kcal\":{calc[1].ToString().Replace(",", ".")},\"kshow\":{(dbPlayer.GetData("showNutrition") ? "1" : "0")}," +
                      $"\"fett\":{calc[2].ToString().Replace(",", ".")},\"fshow\":{(dbPlayer.GetData("showNutrition") ? "1" : "0")}," +
                      $"\"zucker\":{calc[3].ToString().Replace(",", ".")},\"zshow\":{(dbPlayer.GetData("showNutrition") ? "1" : "0")}}}"
                      );

                    DoEffects(dbPlayer, effects);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Logger.Crash(e);
                    }
                if (Configurations.Configuration.Instance.DevMode)
                {
                    dbPlayer.SendNewNotification($"-->AVG:{dbPlayer.Nutrition.avg}");
                }
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }
        }

        public override void OnFifteenMinuteUpdate()
        {
            if (!NutritionActive) return;

            foreach (DbPlayer dbPlayer in Players.Players.Instance.GetValidPlayers())
            {
                try

                {
                    if (dbPlayer == null || !dbPlayer.IsValid()) return;
                    if (dbPlayer.IsInAdminDuty() || dbPlayer.IsInGuideDuty() || dbPlayer.IsInGameDesignDuty()) return;
                    //10Std IC*60/15 = Faktor 40;
                    dbPlayer.Nutrition.Wasser -= this.StandardWasser / 40f;
                    dbPlayer.Nutrition.Kcal -= this.StandardKcal / 40f;
                    dbPlayer.Nutrition.Fett -= this.StandardFett / 40f;
                    dbPlayer.Nutrition.Zucker -= this.StandardZucker / 40f;
                    dbPlayer.Nutrition.save();
                    PushNutritionToPlayer(dbPlayer);
                }
                catch (Exception e)
                {
                    Logger.Crash(e);
                }
            }
        }

        public void setHealthy(DbPlayer dbPlayer)
        {
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            dbPlayer.Nutrition.Kcal = this.StandardKcal;
            dbPlayer.Nutrition.Fett = this.StandardFett;
            dbPlayer.Nutrition.Wasser = this.StandardWasser;
            dbPlayer.Nutrition.Zucker = this.StandardZucker;
            dbPlayer.Nutrition.save();

            PushNutritionToPlayer(dbPlayer);
            WipeEffects(dbPlayer);
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandaddnutrition(Player player, string commandParams)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null) return;

            if (!Configurations.Configuration.Instance.DevMode) return;

            string[] cmd = commandParams.Split(' ');

            if (cmd.Length < 4)
            {
                iPlayer.SendNewNotification($"/addNutrition wasser kcal fett zucker");
                return;
            }


            if (!float.TryParse(cmd[0], out float wasser))
            {
                return;
            }
            if (!float.TryParse(cmd[1], out float kcal))
            {
                return;
            }
            if (!float.TryParse(cmd[2], out float fett))
            {
                return;
            }
            if (!float.TryParse(cmd[3], out float zucker))
            {
                return;
            }

            iPlayer.Nutrition.Wasser += wasser;
            iPlayer.Nutrition.Kcal += kcal;
            iPlayer.Nutrition.Fett += fett;
            iPlayer.Nutrition.Zucker += zucker;
            PushNutritionToPlayer(iPlayer);
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandsethealthy(Player player, string commandParams)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null|| !iPlayer.IsValid()) return;

            var command = commandParams.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
            if (command.Length != 1) return;

            var findPlayer = Players.Players.Instance.FindPlayer(command[0], true);
            if (findPlayer == null || !findPlayer.IsValid()) return;
            setHealthy(findPlayer);

        }
    }

    class NutritionRequest : Script
    {
        [RemoteEvent]
        public void showNutrition(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;
            if (dbPlayer.HasData("showNutrition"))
            {
                if (dbPlayer.GetData("showNutrition"))
                {
                    dbPlayer.SetData("showNutrition", false);
                }
                else
                {
                    dbPlayer.SetData("showNutrition", true);
                }
            }
            else
            {
                dbPlayer.SetData("showNutrition", false);
            }

            NutritionModule.Instance.PushNutritionToPlayer(dbPlayer,false);
        }
    }
}
