using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMP_CNR.Module.Chat;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Customization;
using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Outfits;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
//Possible problem. Removed on use, but not possible to add without weapon. Readd item?
namespace VMP_CNR.Module.Items.Scripts
{
    public static partial class ItemScript
    {
        public static async Task<bool> Outfit(DbPlayer iPlayer, ItemModel ItemData, Item item)
        {
            try
            {
                if (iPlayer.Player.IsInVehicle) return false;

                string outfit = ItemData.Script.ToLower().Replace("outfit_", "");

                if (outfit.Length <= 0) return false;

                if (outfit == "original")
                {
                    Chats.sendProgressBar(iPlayer, 4000);
                    iPlayer.Player.TriggerEvent("freezePlayer", true);

                    if(item.Data == null || !item.Data.ContainsKey("owner") || item.Data["owner"] != iPlayer.Id)
                    {
                        iPlayer.SendNewNotification("Du kannst keine Kleidung von anderen Personen anziehen!");
                        iPlayer.Player.TriggerEvent("freezePlayer", false);
                        return false;
                    }
                    iPlayer.Container.RemoveItem(ItemData, 1);

                    if (item.Data != null && item.Data.ContainsKey("props") && item.Data.ContainsKey("cloth"))
                    {
                        string clothesstring = item.Data["cloth"];
                        Dictionary<int, uint> clothDic = clothesstring.TrimEnd(';').Split(';').ToDictionary(it => Convert.ToInt32(it.Split('=')[0]), it => Convert.ToUInt32(it.Split('=')[1]));

                        string propsstring = item.Data["props"];
                        Dictionary<int, uint> PropsDic = propsstring.TrimEnd(';').Split(';').ToDictionary(it => Convert.ToInt32(it.Split('=')[0]), it => Convert.ToUInt32(it.Split('=')[1]));

                        iPlayer.Character.Clothes = clothDic;
                        iPlayer.Character.EquipedProps = PropsDic;
                    }

                    iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);

                    await Task.Delay(4000);
                    iPlayer.StopAnimation();
                    iPlayer.ApplyCharacter(false, true);
                    iPlayer.Player.TriggerEvent("freezePlayer", false);

                    ClothModule.SaveCharacter(iPlayer);

                    if (iPlayer.HasData("outfitactive")) iPlayer.ResetData("outfitactive");

                    iPlayer.SendNewNotification("Sie haben die Kleidung erfolgreich angezogen!");
                    return true;
                }

                if (!Int32.TryParse(outfit, out int outfitid))
                {
                    return false;
                }

                // Heist check
                if (outfitid == 66 && !iPlayer.HasData("heistActive"))
                {
                    iPlayer.SendNewNotification("Kann nur angezogen werden, wenn ein Heist aktiv ist!", PlayerNotification.NotificationType.ERROR);
                    return false;
                }

                Chats.sendProgressBar(iPlayer, 4000);
                iPlayer.Player.TriggerEvent("freezePlayer", true);
                iPlayer.Container.RemoveItem(ItemData, 1);

                Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
                Data.Add("cloth", String.Join(';', string.Join(";", iPlayer.Character.Clothes.Select(x => x.Key + "=" + x.Value).ToArray())));
                Data.Add("props", String.Join(',', string.Join(";", iPlayer.Character.EquipedProps.Select(x => x.Key + "=" + x.Value).ToArray())));
                Data.Add("owner", iPlayer.Id);

                iPlayer.Container.AddItem(737, 1, Data);
                iPlayer.PlayAnimation(
                    (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);

                await Task.Delay(4000);
                iPlayer.StopAnimation();

                // Armor westen
                if (ItemData.Id == 865 || ItemData.Id == 1346) iPlayer.SetArmor(100);

                iPlayer.Player.TriggerEvent("freezePlayer", false);
                OutfitsModule.Instance.SetPlayerOutfit(iPlayer, outfitid, true);

                ClothModule.SaveCharacter(iPlayer);

                iPlayer.SendNewNotification(
                    "Sie haben die Kleidung erfolgreich angezogen!");
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }

            return false;
        }
        public static async Task<bool> ClothesBag(DbPlayer iPlayer, ItemModel ItemData, Item item)
        {
            try
            {
                return false;

                if (iPlayer.Player.IsInVehicle) return false;

                if (!iPlayer.IsAGangster() && iPlayer.IsBadOrga()) return false;

                DbPlayer closestPlayer = Players.Players.Instance.GetClosestPlayerForPlayer(iPlayer, 2.0f);

                if (closestPlayer == null || !closestPlayer.IsValid()) return false;

                if (!closestPlayer.IsTied || !closestPlayer.IsInDuty() || !closestPlayer.IsACop())
                {
                    iPlayer.SendNewNotification("Der Beamte muss im Dienst und gefesselt sein!");
                    return false;
                }

                Chats.sendProgressBar(iPlayer, 30000);
                iPlayer.Player.TriggerEvent("freezePlayer", true);

                iPlayer.PlayAnimation(
                    (int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);

                await Task.Delay(10000);
                // AUSZIEHEN
                if (closestPlayer.IsMale())
                {
                    closestPlayer.SetClothes(3, 15, 0); //Torso
                    closestPlayer.SetClothes(8, 15, 0); //Undershirt
                    closestPlayer.SetClothes(11, 15, 0); //Tops
                }
                else
                {
                    closestPlayer.SetClothes(3, 15, 0); //Torso
                    closestPlayer.SetClothes(8, 15, 0); //Undershirt
                    closestPlayer.SetClothes(11, 15, 0); //Tops
                }

                closestPlayer.SetData("lastAusgezogen", DateTime.Now);

                await Task.Delay(5000);
                if (closestPlayer.IsMale())
                {
                    closestPlayer.SetClothes(4, 61, 0);
                }
                else
                {
                    closestPlayer.SetClothes(4, 15, 0);
                }


                await Task.Delay(5000);
                if (closestPlayer.IsMale())
                {
                    closestPlayer.SetClothes(6, 34, 0);
                }
                else
                {
                    closestPlayer.SetClothes(6, 35, 0);
                }
                ClothModule.SaveCharacter(closestPlayer);

                await Task.Delay(10000);

                if (iPlayer.IsCuffed || iPlayer.IsTied || iPlayer.isInjured())
                {
                    return false;
                }

                // something happend to officer || Out of range
                if (closestPlayer.isInjured() || closestPlayer.Player.Position.DistanceTo(iPlayer.Player.Position) > 3.0f)
                {
                    return false;
                }


                Dictionary<string, dynamic> Data = new Dictionary<string, dynamic>();
                Data.Add("cloth", String.Join(';', string.Join(";", closestPlayer.Character.Clothes.Select(x => x.Key + "=" + x.Value).ToArray())));
                Data.Add("props", String.Join(',', string.Join(";", closestPlayer.Character.EquipedProps.Select(x => x.Key + "=" + x.Value).ToArray())));
                Data.Add("gender", closestPlayer.IsMale());

                iPlayer.Container.AddItem(1104, 1, Data);


                iPlayer.Player.TriggerEvent("freezePlayer", false);
                iPlayer.StopAnimation();

                iPlayer.SendNewNotification("Sie haben die Kleidung erfolgreich eingepackt!");
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }

            return false;
        }

        public static async Task<bool> PackedClothesBag(DbPlayer iPlayer, ItemModel ItemData, Item item)
        {
            try
            {
                if (iPlayer.Player.IsInVehicle) return false;

                if (!iPlayer.IsAGangster() && iPlayer.IsBadOrga()) return false;


                Chats.sendProgressBar(iPlayer, 4000);
                iPlayer.Player.TriggerEvent("freezePlayer", true);

                if (item.Data == null)
                {
                    return false;
                }

                if (item.Data != null && item.Data.ContainsKey("props") && item.Data.ContainsKey("cloth") && item.Data.ContainsKey("gender") && (item.Data["gender"] == iPlayer.IsMale()))
                {
                    string clothesstring = item.Data["cloth"];
                    Dictionary<int, uint> clothDic = clothesstring.TrimEnd(';').Split(';').ToDictionary(it => Convert.ToInt32(it.Split('=')[0]), it => Convert.ToUInt32(it.Split('=')[1]));

                    string propsstring = item.Data["props"];
                    Dictionary<int, uint> PropsDic = propsstring.TrimEnd(';').Split(';').ToDictionary(it => Convert.ToInt32(it.Split('=')[0]), it => Convert.ToUInt32(it.Split('=')[1]));

                    iPlayer.Character.Clothes = clothDic;
                    iPlayer.Character.EquipedProps = PropsDic;
                }
                else
                {
                    iPlayer.Player.TriggerEvent("freezePlayer", false);
                    return false;
                }

                iPlayer.Container.RemoveItem(ItemData, 1);

                iPlayer.PlayAnimation((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), Main.AnimationList["fixing"].Split()[0], Main.AnimationList["fixing"].Split()[1]);

                await Task.Delay(4000);
                iPlayer.StopAnimation();
                iPlayer.ApplyCharacter(false, true);
                iPlayer.Player.TriggerEvent("freezePlayer", false);

                ClothModule.SaveCharacter(iPlayer);

                iPlayer.SendNewNotification("Sie haben die Kleidung erfolgreich angezogen!");

                return true;
            }
            catch (Exception e)
            {
                Logger.Crash(e);
            }

            return false;
        }
    }
}
