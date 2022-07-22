using System.Collections.Generic;
using VMP_CNR.Module.AnimationMenu;
using VMP_CNR.Module.Farming;
using VMP_CNR.Module.Helper;
using VMP_CNR.Module.Houses;
using GTANetworkAPI;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR
{
    public class AnimationMenuBuilder : MenuBuilder
    {
        public AnimationMenuBuilder() : base(PlayerMenu.AnimationMenuOv)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu(Menu, "Animation Menu");
            menu.Add("Schließen", "");
            menu.Add("Animation beenden", "");
            foreach (KeyValuePair<uint, AnimationCategory> kvp in AnimationCategoryModule.Instance.GetAll())
            {
                if (kvp.Value == null) continue;
                menu.Add($"{kvp.Value.Name}", "");
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
                var player = iPlayer.Player;
                if (index == 0)
                {
                    return true;
                }
                else if (index == 1)
                {
                    if (!iPlayer.CanInteract()) return false;
                    iPlayer.StopAnimation();
                    MenuManager.DismissCurrent(iPlayer);
                    iPlayer.Player.TriggerEvent("freezePlayer", false);
                    return true;
                }
                else
                {
                    int idx = 0;

                    foreach (KeyValuePair<uint, AnimationCategory> kvp in AnimationCategoryModule.Instance.GetAll())
                    {
                        if (kvp.Value == null) continue;
                        if(index-2 == idx)
                        {
                            // Animation Category Clicked
                            iPlayer.SetData("animCat", (int)kvp.Value.Id);
                            break;
                        }
                        idx++;
                    }
                    
                    MenuManager.Instance.Build(PlayerMenu.AnimationMenuIn, iPlayer).Show(iPlayer);
                    return false;
                }
            }
        }
    }

    public static class AnimationExtension
    {
        public static bool StartAnimation(DbPlayer iPlayer, AnimationItem animationItem)
        {
    
                var player = iPlayer.Player;
                if (animationItem == null)
                {
                    return true;
                }

                if (!AnimationMenuModule.Instance.animFlagDic.ContainsKey((uint)animationItem.AnimFlag)) return true;
            NAPI.Task.Run(() =>
            {
                iPlayer.PlayAnimation(AnimationMenuModule.Instance.animFlagDic[(uint)animationItem.AnimFlag], animationItem.AnimDic, animationItem.AnimName);
            });

            return false;
        }
    }
}