﻿using System.Collections.Generic;
using System.Linq;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Clothes.Shops;
using VMP_CNR.Module.Clothes.Team;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR
{
    public class TeamWardrobeClothesSelectionMenu : MenuBuilder
    {
        public TeamWardrobeClothesSelectionMenu() : base(PlayerMenu.TeamWardrobeClothesSelection)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            if (!iPlayer.HasData("teamWardrobeSlot"))
            {
                return null;
            }

            int slotId = iPlayer.GetData("teamWardrobeSlot");
            var slots = ClothesShopModule.Instance.GetClothesSlots();
            if (!slots.ContainsKey(slotId))
            {
                return null;
            }

            var slot = slots[slotId];
            var menu = new Menu(Menu, slot.Name);
            menu.Add(MSG.General.Close());

            if (iPlayer.IsFreeMode())
            {
                var clothesForSlot = ClothModule.Instance.GetTeamWarerobe(iPlayer, slotId);
                if (clothesForSlot != null && clothesForSlot.Count > 0)
                {
                    foreach (var cloth in clothesForSlot)
                    {
                        menu.Add(cloth.Name);
                    }
                }
            }
            else
            {
                var teamSkin = TeamSkinModule.Instance.GetTeamSkin(iPlayer);
                if (teamSkin == null)
                {
                    return null;
                }

                var clothes = teamSkin.Clothes.Where(cloth => cloth.Slot == slotId).ToList();
                if (clothes.Count == 0)
                {
                    return null;
                }

                foreach (var cloth in clothes)
                {
                    menu.Add(cloth.Name);
                }
            }

            return menu;
        }

        public override IMenuEventHandler GetEventHandler()
        {
            return new EventHandler();
        }

        private class EventHandler : IMenuEventHandler
        {
            public bool OnSelect(int index, DbPlayer iPlayer)
            {
                if (index == 0)
                {
                    MenuManager.DismissMenu(iPlayer.Player, (uint) PlayerMenu.TeamWardrobeClothesSelection);
                    ClothModule.SaveCharacter(iPlayer);
                    return false;
                }

                index--;
                if (!iPlayer.HasData("teamWardrobeSlot"))
                {
                    return false;
                }

                int slotId = iPlayer.GetData("teamWardrobeSlot");
                if (iPlayer.IsFreeMode())
                {
                    List<Cloth> clothesForSlot = ClothModule.Instance.GetTeamWarerobe(iPlayer, slotId);
                    if (index >= clothesForSlot.Count)
                    {
                        return false;
                    }

                    var cloth = clothesForSlot[index];
                    if (iPlayer.Character.Clothes.ContainsKey(cloth.Slot))
                    {
                        iPlayer.Character.Clothes[cloth.Slot] = cloth.Id;
                    }
                    else
                    {
                        iPlayer.Character.Clothes.Add(cloth.Slot, cloth.Id);
                    }
                }
                else
                {
                    var teamSkin = TeamSkinModule.Instance.GetTeamSkin(iPlayer);
                    if (teamSkin == null)
                    {
                        return false;
                    }

                    var clothes = teamSkin.Clothes.Where(cloth => cloth.Slot == slotId).ToList();
                    if (clothes.Count == 0 || index >= clothes.Count)
                    {
                        return false;
                    }

                    var currCloth = clothes[index];
                    if (iPlayer.Character.Clothes.ContainsKey(currCloth.Slot))
                    {
                        iPlayer.Character.Clothes[currCloth.Slot] = currCloth.Id;
                    }
                    else
                    {
                        iPlayer.Character.Clothes.Add(currCloth.Slot, currCloth.Id);
                    }
                }

                ClothModule.Instance.RefreshPlayerClothes(iPlayer);
                ClothModule.SaveCharacter(iPlayer);
                return false;
            }
        }
    }
}