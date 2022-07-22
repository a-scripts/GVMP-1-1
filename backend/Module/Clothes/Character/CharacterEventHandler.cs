using GTANetworkAPI;
using Newtonsoft.Json;
using System.Linq;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.Einreiseamt;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Clothes.Character
{
    public class CharacterEventHandler : Script
    {
        [RemoteEvent]
        public void SetGender(Player Player, int gender)
        {
            var iPlayer = Player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;

            var character = iPlayer.Character;
            
            var Armor = iPlayer.Player.Armor;
            character.Skin = gender == 0 ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01;
            Player.SetSkin(character.Skin);
            iPlayer.SetArmorPlayer(Armor);
            iPlayer.SetData("ChangedGender", true);
            iPlayer.SetCreatorClothes();
        }

        [RemoteEvent]
        public async void SaveCharacter(Player Player, int gender, byte fatherShape, byte motherShape, byte fatherSkin, byte motherSkin, float similarity, float skinSimilarity,
            string featuredData, string appearenceData, string hairData)
        {
            var iPlayer = Player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;
            
            var character = iPlayer.Character;
            var price = 500 * iPlayer.Level;

            if (iPlayer.Level <= 3 || iPlayer.HasData("firstCharacter"))
            {
                price = 0;
            }

            if (iPlayer.NeuEingereist())
            {
                foreach (DbPlayer xPlayer in Players.Players.Instance.GetValidPlayers().Where(p => p.IsEinreiseAmt()))
                {
                    xPlayer.SendNewNotification("Spieler ist aus der Schoenheitsklinik: " + iPlayer.GetName());
                }
            }
            
            if (price > 0 && !iPlayer.TakeMoney(price))
            {
                iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(price), notificationType:PlayerNotification.NotificationType.ERROR);
                // revert back to last save data
                if (iPlayer.HasData("ChangedGender"))
                {
                    //character.customization =TODO: old customization 
                    iPlayer.ResetData("ChangedGender");
                }

                await iPlayer.StopCustomization();
                return;
            }
            else
            {
                iPlayer.SendNewNotification("Sie haben ihren Character fuer $" + price + " geaendert!", notificationType:PlayerNotification.NotificationType.SUCCESS);
            }
            
            if (character == null) return;

            // gender
            iPlayer.Customization.Gender = gender;

            // parents
            iPlayer.Customization.Parents.FatherShape = fatherShape;
            iPlayer.Customization.Parents.MotherShape = motherShape;
            iPlayer.Customization.Parents.FatherSkin = fatherSkin;
            iPlayer.Customization.Parents.MotherSkin = motherSkin;
            iPlayer.Customization.Parents.Similarity = similarity;
            iPlayer.Customization.Parents.SkinSimilarity = skinSimilarity;

            // features
            var featureData = JsonConvert.DeserializeObject<float[]>(featuredData);
            iPlayer.Customization.Features = featureData;
            
            // appearance
            var appearanceData =
                JsonConvert.DeserializeObject<AppearanceItem[]>(appearenceData);
            iPlayer.Customization.Appearance = appearanceData;

            // hair & colors
            var hairAndColorData = JsonConvert.DeserializeObject<byte[]>(hairData);
            for (var i = 0; i < hairAndColorData.Length; i++)
            {
                switch (i)
                {
                    // Hair
                    case 0:
                        {
                            iPlayer.Customization.Hair.Hair = hairAndColorData[i];
                            break;
                        }

                    // Hair Color
                    case 1:
                        {
                            iPlayer.Customization.Hair.Color = hairAndColorData[i];
                            break;
                        }

                    // Hair Highlight Color
                    case 2:
                        {
                            iPlayer.Customization.Hair.HighlightColor = hairAndColorData[i];
                            break;
                        }

                    // Eyebrow Color
                    case 3:
                        {
                            iPlayer.Customization.EyebrowColor = hairAndColorData[i];
                            break;
                        }

                    // Beard Color
                    case 4:
                        {
                            iPlayer.Customization.BeardColor = hairAndColorData[i];
                            break;
                        }

                    // Eye Color
                    case 5:
                        {
                            iPlayer.Customization.EyeColor = hairAndColorData[i];
                            break;
                        }

                    // Blush Color
                    case 6:
                        {
                            iPlayer.Customization.BlushColor = hairAndColorData[i];
                            break;
                        }

                    // Lipstick Color
                    case 7:
                        {
                            iPlayer.Customization.LipstickColor = hairAndColorData[i];
                            break;
                        }

                    // Chest Hair Color
                    case 8:
                        {
                            iPlayer.Customization.ChestHairColor = hairAndColorData[i];
                            break;
                        }
                }
            }

            iPlayer.SaveCustomization();
            await iPlayer.StopCustomization();
        }
        
        [RemoteEvent]
        public async void LeaveCreator(Player Player, object[] args)
        {
            var iPlayer = Player.GetPlayer();
            if (iPlayer == null || !iPlayer.IsValid()) return;

            var character = iPlayer.Character;

            await iPlayer.StopCustomization();
        }
    }
}
