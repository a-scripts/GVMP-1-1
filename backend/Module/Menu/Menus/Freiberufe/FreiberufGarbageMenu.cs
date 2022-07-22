using VMP_CNR.Module.Freiberuf.Garbage;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR
{
    public class FreiberufGarbageMenuBuilder : MenuBuilder
    {
        public FreiberufGarbageMenuBuilder() : base(PlayerMenu.FreiberufGarbageMenu)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu(Menu, "Freiberuf Müllabfuhr", "Müllabfuhr Los Santos");
            menu.Add("Arbeit starten");
            menu.Add("Fahrzeug mieten 500$");
            menu.Add("Rückgabe");
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
                        GarbageJobModule.Instance.StartGarbageJob(iPlayer);
                        break;
                    case 1:
                        GarbageJobModule.Instance.RentGarbageVeh(iPlayer);
                        break;
                    case 2:
                        GarbageJobModule.Instance.FinishGarbageJob(iPlayer);
                        break;
                    default:
                        MenuManager.DismissCurrent(iPlayer);
                        break;
                }

                return true;
            }
        }
    }
}
