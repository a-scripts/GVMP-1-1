using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR
{
    public class TeamWardrobeMenuBuilder : MenuBuilder
    {
        public TeamWardrobeMenuBuilder() : base(PlayerMenu.TeamWardrobe)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu(Menu, "Kleiderschrank");
            menu.Add("Skins");
            menu.Add("Kleidung");
            menu.Add("Accessoires");
            menu.Add("Outfits");
            menu.Add(MSG.General.Close());
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
                switch (index)
                {
                    case 0:
                        MenuManager.Instance.Build(PlayerMenu.TeamWardrobeSkins, iPlayer).Show(iPlayer);
                        break;
                    case 1:
                        MenuManager.Instance.Build(PlayerMenu.TeamWardrobeClothes, iPlayer).Show(iPlayer);
                        break;
                    case 2:
                        MenuManager.Instance.Build(PlayerMenu.TeamWardrobeProps, iPlayer).Show(iPlayer);
                        break;
                    case 3:
                        MenuManager.Instance.Build(PlayerMenu.OutfitsMenu, iPlayer).Show(iPlayer);
                        break;
                    default:
                        MenuManager.DismissCurrent(iPlayer);
                        break;
                }

                return false;
            }
        }
    }
}