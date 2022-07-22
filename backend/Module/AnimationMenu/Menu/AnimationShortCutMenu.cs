using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.AnimationMenu
{
    public class AnimationShortCutMenuBuilder : MenuBuilder
    {
        public AnimationShortCutMenuBuilder() : base(PlayerMenu.AnimationShortCutMenu)
        {
        }

        public override Menu.Menu Build(DbPlayer iPlayer)
        {
            if (!iPlayer.HasData("animSlot")) return null;

            var menu = new Menu.Menu(Menu, $"Animation für Slot {iPlayer.GetData("animSlot")} auswählen");

            menu.Add($"Schließen");

            foreach (AnimationItem animationItem in AnimationItemModule.Instance.GetAll().Values)
            {

                if (animationItem.RestrictedToTeams.Contains((uint)teams.TEAM_IAA) && !iPlayer.IsNSADuty) continue;
                else if (!animationItem.RestrictedToTeams.Contains((uint)teams.TEAM_IAA) && animationItem.RestrictedToTeams.Count > 0 && !animationItem.RestrictedToTeams.Contains(iPlayer.TeamId)) continue;

                menu.Add($"{animationItem.Name}");
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
                if (!iPlayer.HasData("animSlot")) return false;
                if (index == 0)
                {
                    MenuManager.DismissCurrent(iPlayer);
                    return false;
                }

                int idx = 1;
                foreach (AnimationItem animationItem in AnimationItemModule.Instance.GetAll().Values)
                {
                    if (index == idx)
                    {
                        // Open Secound Menu
                        if (!iPlayer.AnimationShortcuts.ContainsKey(iPlayer.GetData("animSlot"))) return false;

                        if (animationItem.RestrictedToTeams.Contains((uint)teams.TEAM_IAA) && !iPlayer.IsNSADuty) return false;
                        else if (!animationItem.RestrictedToTeams.Contains((uint)teams.TEAM_IAA) && animationItem.RestrictedToTeams.Count > 0 && !animationItem.RestrictedToTeams.Contains(iPlayer.TeamId)) return false;

                        iPlayer.AnimationShortcuts[iPlayer.GetData("animSlot")] = animationItem.Id;
                        iPlayer.SendNewNotification($"Animationsslot {iPlayer.GetData("animSlot")} mit {animationItem.Name} belegt!");
                        iPlayer.SaveAnimationShortcuts();
                        iPlayer.UpdateAnimationShortcuts();
                        return true;
                    }
                    idx++;
                }

                MenuManager.DismissCurrent(iPlayer);
                return true;
            }
        }
    }
}
