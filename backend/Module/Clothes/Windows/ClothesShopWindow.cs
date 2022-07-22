using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Newtonsoft.Json;
using VMP_CNR.Module.PlayerUI.Windows;
using VMP_CNR.Module.Clothes.Props;
using VMP_CNR.Module.Clothes.Shops;
using VMP_CNR.Module.Clothes.Slots;
using VMP_CNR.Module.Events.CWS;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Logging;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Clothes.Windows
{
    public class ClothesShopWindow : Window<Func<DbPlayer, List<Slot>, string, bool>>
    {
        private const int MaxWearablesToSent = 500;

        /// <summary>
        /// Simple abstract version of a piece of cloth or prop.
        /// </summary>
        public class SimpleCloth
        {
            public uint Id { get; set; }

            public string Slot { get; set; }

            public bool IsProp { get; set; }
        }

        /// <summary>
        /// JSON data which will be delivered on clothes request.
        /// </summary>
        public class JsonCloth
        {
            public uint Id { get; }

            public string Name { get; }

            public int Price { get; }

            public int Slot { get; }

            public bool IsProp { get; }

            public JsonCloth(uint id, string name, int price, int slot, bool isProp = false)
            {
                Id = id;
                Name = name;
                Price = price;
                Slot = slot;
                IsProp = isProp;
            }
        }

        /// <summary>
        /// JSON data which will be delivered if the player enters the shop.
        /// </summary>
        private class ShowEvent : Event
        {
            [JsonProperty(PropertyName = "slots")] private List<Slot> Slots { get; }

            [JsonProperty(PropertyName = "name")] private string Name { get; }

            public ShowEvent(DbPlayer dbPlayer, List<Slot> slots, string shopName) : base(dbPlayer)
            {
                Slots = slots;
                Name = shopName;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClothesShopWindow() : base("ClothingShop")
        {
        }

        /// <summary>
        /// Event handler if the window will be shown.
        /// </summary>
        /// <returns></returns>
        public override Func<DbPlayer, List<Slot>, string, bool> Show()
        {
            return (player, slots, name) => OnShow(new ShowEvent(player, slots, name));
        }

        /// <summary>
        /// Get categories for a given slot.
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="slotId"></param>
        [RemoteEvent]
        public void clothingShopLoadCategories(Player Player, string slotId)
        {
            if (slotId == null) return;

            DbPlayer dbPlayer = Player.GetPlayer();

            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            try
            {
                if (!dbPlayer.HasData("clothShopId")) return;

                uint shopId = dbPlayer.GetData("clothShopId");

                ClothesShop currentShop = ClothesShopModule.Instance.GetShopById(shopId);

                if (currentShop == null) return;

                int slot;

                // Handle Prop.
                if (slotId.StartsWith("p-"))
                {
                    if (!int.TryParse(slotId.Remove(0, 2), out slot)) return;

                    var propsSlots = ClothesShopModule.Instance.GetPropsSlots();
                    if (propsSlots == null || !propsSlots.ContainsKey(slot) || propsSlots[slot] == null) return;

                    var pCats = propsSlots[slot].Categories
                        .Where(
                            cat => currentShop.GetPropsBySlotAndCategoryForPlayer(slot, cat.Id, dbPlayer).Count > 0
                        ).ToList();

                    dbPlayer.Player.TriggerEvent(
                        "componentServerEvent",
                        "ClothingShop",
                        "responseClothingShopCategories",
                        JsonConvert.SerializeObject(pCats)
                    );

                    return;
                }

                // Handle Cloth.
                if (!int.TryParse(slotId, out slot)) return;

                var clothesSlots = ClothesShopModule.Instance.GetClothesSlots();
                if (clothesSlots == null || !clothesSlots.ContainsKey(slot) || clothesSlots[slot] == null) return;

                var cCats = clothesSlots[slot].Categories
                    .Where(
                        cat => currentShop.GetClothesBySlotAndCategoryForPlayer(slot, cat.Id, dbPlayer).Count > 0
                    ).ToList();

                dbPlayer.Player.TriggerEvent(
                    "componentServerEvent",
                    "ClothingShop",
                    "responseClothingShopCategories",
                    JsonConvert.SerializeObject(cCats)
                );
            }
            catch (Exception e)
            {
                dbPlayer.SendNewNotification("Ein Fehler ist aufgetreten...");

                Logger.Crash(e);
            }
        }

        /// <summary>
        /// Get clothes or props within a given category and slot.
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="categoryId"></param>
        /// <param name="slotId"></param>
        [RemoteEvent]
        public void clothingShopLoadClothes(Player Player, string categoryId, string slotId)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            try
            {
                if (!dbPlayer.HasData("clothShopId")) return;

                uint shopId = dbPlayer.GetData("clothShopId");

                ClothesShop currentShop = ClothesShopModule.Instance.GetShopById(shopId);

                if (currentShop == null) return;

                var isProp = false;

                if (!int.TryParse(categoryId, out var id)) return;

                if (slotId.StartsWith("p-"))
                {
                    isProp = true;
                    slotId = slotId.Remove(0, 2);
                }

                if (!int.TryParse(slotId, out var slot)) return;

                if (isProp)
                {
                    var propsToSent = currentShop.GetPropsBySlotAndCategoryForPlayer(slot, id, dbPlayer)
                        .ConvertAll(x => new JsonCloth(x.Id, x.Name, x.Price, x.Slot, true)).ToList();

                    // If we have toooo much props, the shop can not display them. So we cut the list.
                    if (propsToSent.Count > MaxWearablesToSent)
                    {
                        propsToSent.RemoveRange(MaxWearablesToSent, propsToSent.Count - MaxWearablesToSent);
                    }

                    dbPlayer.Player.TriggerEvent(
                        "componentServerEvent",
                        "ClothingShop",
                        "responseClothingShopClothes",
                        JsonConvert.SerializeObject(propsToSent)
                    );

                    return;
                }

                var clothesToSent = currentShop.GetClothesBySlotAndCategoryForPlayer(slot, id, dbPlayer)
                    .ConvertAll(x => new JsonCloth(x.Id, x.Name, x.Price, x.Slot)).ToList();

                // If we have toooo much clothes, the shop can not display them. So we cut the list.
                if (clothesToSent.Count > MaxWearablesToSent)
                {
                    clothesToSent.RemoveRange(MaxWearablesToSent, clothesToSent.Count - MaxWearablesToSent);
                }

                dbPlayer.Player.TriggerEvent(
                    "componentServerEvent",
                    "ClothingShop",
                    "responseClothingShopClothes",
                    JsonConvert.SerializeObject(clothesToSent)
                );
            }
            catch (Exception e)
            {
                dbPlayer.SendNewNotification("Ein Fehler ist aufgetreten...");
                Logger.Crash(e);
            }
        }

        /// <summary>
        /// Dresses the selected garment.
        ///
        /// So that it can be found faster, we use the slot and categorization.
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="clothId"></param>
        /// <param name="categoryId"></param>
        /// <param name="slotId"></param>
        [RemoteEvent]
        public void clothingShopDress(Player Player, string clothId, string categoryId, string slotId)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            try
            {
                if (!dbPlayer.HasData("clothShopId")) return;

                uint shopId = dbPlayer.GetData("clothShopId");

                ClothesShop currentShop = ClothesShopModule.Instance.GetShopById(shopId);

                if (currentShop == null) return;

                var isProp = false;

                if (!int.TryParse(clothId, out var clothIdParsed)) return;
                if (!int.TryParse(categoryId, out var categoryIdParsed)) return;

                if (slotId.StartsWith("p-"))
                {
                    isProp = true;
                    slotId = slotId.Remove(0, 2);
                }

                if (!int.TryParse(slotId, out var slotIdParsed)) return;

                if (isProp)
                {
                    var props = currentShop.GetPropsBySlotAndCategoryForPlayer(slotIdParsed, categoryIdParsed,
                        dbPlayer);

                    if (props == null) return;

                    Prop prop = props.Find(p => p != null && p.Id == clothIdParsed);

                    if (prop == null) return;

                    ClothModule.Instance.SetPlayerAccessories(dbPlayer, slotIdParsed, prop.Variation, prop.Texture);

                    return;
                }

                var clothes =
                    currentShop.GetClothesBySlotAndCategoryForPlayer(slotIdParsed, categoryIdParsed, dbPlayer);

                if (clothes == null) return;

                Cloth cloth = clothes.Find(c => c != null && c.Id == clothIdParsed);

                if (cloth == null) return;

                dbPlayer.SetClothes(slotIdParsed, cloth.Variation, cloth.Texture);
            }
            catch (Exception e)
            {
                dbPlayer.SendNewNotification("Ein Fehler ist aufgetreten...");

                Logger.Crash(e);
            }
        }

        /// <summary>
        /// Resets the previously worn garment for the given slot.
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="slotId">Slot to reset.</param>
        [RemoteEvent]
        public void clothingShopUndress(Player Player, string slotId)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            try
            {
                if (!dbPlayer.HasData("clothShopId")) return;

                uint shopId = dbPlayer.GetData("clothShopId");

                ClothesShop currentShop = ClothesShopModule.Instance.GetShopById(shopId);

                if (currentShop == null) return;

                var isProp = false;

                if (slotId.StartsWith("p-"))
                {
                    isProp = true;
                    slotId = slotId.Remove(0, 2);
                }

                if (!int.TryParse(slotId, out var slotIdParsed)) return;

                if (isProp)
                {
                    if (!dbPlayer.Character.EquipedProps.ContainsKey(slotIdParsed))
                    {
                        dbPlayer.Player.ClearAccessory(slotIdParsed);

                        return;
                    }

                    Prop prop = PropModule.Instance.GetWardrobeBySlot(dbPlayer, slotIdParsed)
                        .Find(p => p.Id == dbPlayer.Character.EquipedProps[slotIdParsed]);

                    if (prop == null) return;

                    ClothModule.Instance.SetPlayerAccessories(dbPlayer, slotIdParsed, prop.Variation, prop.Texture);

                    return;
                }

                if (!dbPlayer.Character.Clothes.ContainsKey(slotIdParsed)) return;

                Cloth cloth = ClothModule.Instance
                    .GetWardrobeBySlot(dbPlayer, dbPlayer.Character, slotIdParsed)
                    .Find(c => c.Id == dbPlayer.Character.Clothes[slotIdParsed]);

                if (cloth == null) return;

                dbPlayer.SetClothes(slotIdParsed, cloth.Variation, cloth.Texture);
            }
            catch (Exception e)
            {
                dbPlayer.SendNewNotification("Ein Fehler ist aufgetreten...");

                Logger.Crash(e);
            }
        }

        /// <summary>
        /// Reset all previously worn garments.
        /// </summary>
        /// <param name="Player"></param>
        [RemoteEvent]
        public void clothingShopReset(Player Player)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            try
            {
                if (!dbPlayer.HasData("clothShopId")) return;
            }
            catch (Exception e)
            {
                dbPlayer.SendNewNotification("Ein Fehler ist aufgetreten...");

                Logger.Crash(e);
            }

            ClothModule.Instance.ResetClothes(dbPlayer);
        }

        /// <summary>
        /// Buy clothes and props logic.
        /// 
        /// Will be called after the user has pressed buy on Player site.
        /// Cart will only checked for cloth or prop ids. Price data will be loaded
        /// from server site data. 
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="cart">Shopping cart applied from Player side.</param>
        /// <param name="wearing">We apply wearing clothes to the character if the player had buy the clothes.</param>
        [RemoteEvent]
        public void clothingShopBuy(Player Player, string cart, string wearing)
        {
            DbPlayer dbPlayer = Player.GetPlayer();
            if (dbPlayer == null || !dbPlayer.IsValid()) return;

            try
            {
                if (!dbPlayer.HasData("clothShopId")) return;

                uint shopId = dbPlayer.GetData("clothShopId");

                ClothesShop currentShop = ClothesShopModule.Instance.GetShopById(shopId);

                if (currentShop == null) return;

                var parsedCart = JsonConvert.DeserializeObject<List<SimpleCloth>>(cart);
                var parsedWearing = JsonConvert.DeserializeObject<List<SimpleCloth>>(wearing);

                var relevantClothes = new List<Cloth>();
                var relevantProps = new List<Prop>();

                var propsForShop = PropModule.Instance.GetPropsForShop(currentShop.Id);
                var clothesForShop = ClothModule.Instance.GetClothesForShop(currentShop.Id);

                // Get full data foreach cart item. To make sure that nobody do manipulate our prices.
                foreach (SimpleCloth simpleCloth in parsedCart)
                {
                    if (simpleCloth == null) continue;

                    if (simpleCloth.Slot.StartsWith("p-"))
                    {
                        simpleCloth.IsProp = true;
                        simpleCloth.Slot = simpleCloth.Slot.Remove(0, 2);
                    }

                    if (simpleCloth.IsProp)
                    {
                        Prop prop = propsForShop.Find(p => p != null && p.Id == simpleCloth.Id);

                        if (prop == null) continue;

                        relevantProps.Add(prop);

                        continue;
                    }

                    Cloth cloth = clothesForShop.Find(c => c != null && c.Id == simpleCloth.Id);

                    if (cloth == null) continue;

                    relevantClothes.Add(cloth);
                }

                var parsedWearingCleaned = new List<SimpleCloth>();

                // Validate wearing data.
                foreach (SimpleCloth simpleCloth in parsedWearing)
                {
                    if (simpleCloth == null) continue;

                    if (simpleCloth.Slot.StartsWith("p-"))
                    {
                        simpleCloth.IsProp = true;
                        simpleCloth.Slot = simpleCloth.Slot.Remove(0, 2);
                    }

                    if (simpleCloth.IsProp)
                    {
                        Prop prop = relevantProps.Find(p => p.Id == simpleCloth.Id);

                        if (prop != null) parsedWearingCleaned.Add(simpleCloth);

                        continue;
                    }

                    Cloth cloth = relevantClothes.Find(c => c.Id == simpleCloth.Id);

                    if (cloth != null) parsedWearingCleaned.Add(simpleCloth);
                }

                // Calculate price
                Character.Character character = dbPlayer.Character;

                if (character == null) return;

                // We do not calculate items the player already owns.
                var price = relevantClothes
                    .Where(cloth => !character.Wardrobe.Contains(cloth.Id))
                    .Sum(cloth => cloth.Price);

                price += relevantProps
                    .Where(prop => !character.Props.Contains(prop.Id))
                    .Sum(prop => prop.Price);

                if (price > 0)
                {
                    // Apply Coupon.
                    var couponPercent = 0;
                    uint whatCoupon = 0;

                    if (currentShop.CouponUsable)
                    {
                        foreach ((_, Item value) in dbPlayer.Container.Slots
                            .Where(kvp => kvp.Value.Model?.Script != null)
                            .Where(kvp => kvp.Value.Model.Script.Contains("discount_cloth_")))
                        {
                            try
                            {
                                couponPercent = int.Parse(value.Model.Script.Replace("discount_cloth_", ""));

                                price -= (int) (price * (couponPercent / 100.0d));
                                whatCoupon = value.Id;
                            }
                            catch (Exception)
                            {
                                dbPlayer.SendNewNotification("Ein Fehler ist aufgetreten...");

                                clothingShopReset(Player);

                                return;
                            }

                            break;
                        }
                    }

                    if (currentShop.CWSId > 0)
                    {
                        if (!dbPlayer.TakeCWS((CWSTypes)currentShop.CWSId, price))
                        {
                            dbPlayer.SendNewNotification(MSG.Money.NotEnoughCW(price, (CWSTypes)currentShop.CWSId));

                            clothingShopReset(Player);

                            return;
                        }

                        dbPlayer.SendNewNotification(
                            "Sie haben diese Kleidung fuer " + price +
                            " " + (CWSTypes)currentShop.CWSId + " erworben!"
                        );
                    }
                    else
                    {
                       
            
                            var task = dbPlayer.TakeMoneyMenu(price);
                            task.Wait();
                            bool isValid = task.Result;
                            if (!isValid)
                            {
                                dbPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(price));
                                clothingShopReset(Player);

                                return;
                            }

                      

                        dbPlayer.SendNewNotification(
                            "Sie haben diese Kleidung fuer $" + price + " erworben!"
                        );
                  

                    }
                    if (whatCoupon != 0 && currentShop.CouponUsable)
                    {
                        dbPlayer.SendNewNotification(
                            "- " + couponPercent + " % Rabatt", title: "",
                            notificationType: PlayerNotification.NotificationType.SUCCESS
                        );

                        dbPlayer.Container.RemoveItem(whatCoupon);
                    }

                    Logger.SaveClothesShopBuyAction(currentShop.Id, price);
                }

                ClothesShopModule.Instance.Buy(dbPlayer, parsedCart);
                ClothesShopModule.Instance.Dress(dbPlayer, parsedWearingCleaned);
            }
            catch (Exception e)
            {
                dbPlayer.SendNewNotification("Ein Fehler ist aufgetreten...");

                Logger.Crash(e);
            }
        }
    }
}