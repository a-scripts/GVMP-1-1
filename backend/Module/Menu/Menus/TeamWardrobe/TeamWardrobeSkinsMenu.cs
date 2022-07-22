using GTANetworkAPI;
using System;
using VMP_CNR.Module.Clothes;
using VMP_CNR.Module.Clothes.Team;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR
{
    public class TeamWardrobeSkinsMenu : MenuBuilder
    {
        public TeamWardrobeSkinsMenu() : base(PlayerMenu.TeamWardrobeSkins)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu(Menu, "Fraktionskleiderschrank");
            menu.Add(MSG.General.Close());
            menu.Add("Normal");
            foreach (var skin in TeamSkinModule.Instance.GetSkinsForTeam(iPlayer.TeamId))
            {
                menu.Add(skin.Name);
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
                    MenuManager.DismissMenu(iPlayer.Player, (uint) PlayerMenu.TeamWardrobeSkins);
                    ClothModule.SaveCharacter(iPlayer);
                    return false;
                }

                if (index == 1)
                {
                    if (iPlayer.Customization != null)
                    {
                        if (iPlayer.Customization.Gender == 0)
                        {
                            iPlayer.Character.Skin = PedHash.FreemodeMale01;
                            iPlayer.SetSkin(PedHash.FreemodeMale01);
                        }
                        else
                        {
                            iPlayer.Character.Skin = PedHash.FreemodeFemale01;
                            iPlayer.SetSkin(PedHash.FreemodeFemale01);
                        }
                    }
                    else
                    {
                        iPlayer.SetSkin(PedHash.FreemodeMale01);
                    }

                    iPlayer.Character.Clothes?.Clear();
                    iPlayer.Character.EquipedProps?.Clear();
                    ClothModule.Instance.ApplyPlayerClothes(iPlayer);
                    return false;
                }

                index -= 2;
                var skins = TeamSkinModule.Instance.GetSkinsForTeam(iPlayer.TeamId);
                if (skins.Count <= index)
                {
                    return false;
                }

                var currentSkin = skins[index];
                iPlayer.Character.Skin = currentSkin.Hash;
                iPlayer.Character.Clothes?.Clear();
                iPlayer.Character.EquipedProps?.Clear();
                iPlayer.SetSkin(currentSkin.Hash);
                ClothModule.Instance.ApplyPlayerClothes(iPlayer);
                return false;
            }
        }
    }
}