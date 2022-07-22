﻿using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Customization
{
    public sealed class CustomizationModule : Module<CustomizationModule>
    {
        public static Vector3 KlinikMenuPosition = new Vector3(366.35364, -593.46265, 28.69);

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            if (key == Key.E)
            {
                if (dbPlayer.Player.Position.DistanceTo(KlinikMenuPosition) < 3.0f)
                {
                    MenuManager.Instance.Build(PlayerMenu.CustomizationMenu, dbPlayer).Show(dbPlayer);
                    return true;
                }
            }
            return false;
        }

        protected override bool OnLoad()
        {
            PlayerNotifications.Instance.Add(KlinikMenuPosition,
                "Info",
                "Benutze E um die Schönheitsklinik zu nutzen!");

            return base.OnLoad();
        }

        public override void OnPlayerLoadData(DbPlayer dbPlayer, MySqlDataReader reader)
        {
     
                try
                {
                    string characterString = reader.GetString("customization");

                    dynamic character = JsonConvert.DeserializeObject<dynamic>(characterString);
                    dbPlayer.Customization = JsonConvert.DeserializeObject<CharacterCustomization>(characterString);



                    if (character.Parents != null && character.Parents.Father != null)
                    {
                        ParentData parentData = new ParentData(
                            (byte)character.Parents.Father,
                            (byte)character.Parents.Mother,
                            (byte)character.Parents.Father,
                            (byte)character.Parents.Mother,
                            (float)character.Parents.Similarity,
                            (float)character.Parents.SkinSimilarity
                        );

                        dbPlayer.Customization.Parents = parentData;
                    }

                    if (characterString.Equals(""))
                    {
                        dbPlayer.SetData("firstCharacter", true);
                        dbPlayer.Customization = new CharacterCustomization();
                    }

                    if (dbPlayer.Customization.Appearance.Length <= 10)
                    {
                        var temp = new AppearanceItem[11];
                        Array.Copy(dbPlayer.Customization.Appearance, temp, 10);
                        temp[10] = new AppearanceItem((byte)255, 0.0f);
                        dbPlayer.Customization.Appearance = temp;
                    }

                    Console.WriteLine("CustomizationModule");

                }
                catch (Exception e)
                {
                    Logger.Crash(e);
                }

        }

        public override void OnPlayerFirstSpawnAfterSync(DbPlayer dbPlayer)
        {
            dbPlayer.ApplyCharacter(true);
        }
    }
}