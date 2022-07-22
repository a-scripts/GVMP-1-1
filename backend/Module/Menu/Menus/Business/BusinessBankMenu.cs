using VMP_CNR.Module.Business;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.RemoteEvents;

namespace VMP_CNR
{
    public class BusinessBankMenuBuilder : MenuBuilder
    {
        public BusinessBankMenuBuilder() : base(PlayerMenu.BusinessBank)
        {
        }

        public override Menu Build(DbPlayer iPlayer)
        {
            var menu = new Menu(Menu, "Business Verwaltung");

            menu.Add(MSG.General.Close(), "");

            if (!iPlayer.IsMemberOfBusiness())
            {
                menu.Description = $"Ein Unternehmen Gruenden!";
                menu.Add("Business kaufen - 250.000$", "");
            }
            else
            {
                menu.Add("Business Namen ändern - 50.000$", "");
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
                switch (index)
                {
                    case 1: // Kaufen
                        
                        if (!iPlayer.IsMemberOfBusiness())
                        {
                            if (!iPlayer.IsHomeless())
                            {

                                if (!iPlayer.CheckForSpam(DbPlayer.OperationType.BusinessCreate)) return false;

                                if (!iPlayer.TakeBankMoney(250000))
                                {
                                    iPlayer.SendNewNotification(MSG.Money.NotEnoughMoney(250000));
                                    break;
                                }

                                BusinessModule.Instance.CreatePlayerBusiness(iPlayer);
                                iPlayer.SendNewNotification("Business erfolgreich fuer $250000 erworben!");
                            }
                            else
                            {
                                iPlayer.SendNewNotification("Sie haben keinen Wohnsitz!");
                                return true;
                            }
                        }
                        else
                        {
                            // Namechange
                            if(!iPlayer.BusinessMembership.Manage)
                            {
                                iPlayer.SendNewNotification("Sie müssen Manager eines Businesses sein um den Namen zu ändern!");
                                return true;
                            }
                            else
                            {
                                ComponentManager.Get<TextInputBoxWindow>().Show()(iPlayer, new TextInputBoxWindowObject()
                                {
                                    Title = $"Namensänderung beantragen | {iPlayer.ActiveBusiness.Name}",
                                    Callback = "NameChangeBiz",
                                    Message = "Bitte geben Sie den neuen Namen ein, nutzen Sie hierfür Buchstaben (A-Z) optional (_ -). Die Kosten betragen $50000"
                                });
                                return true;
                            }
                        }
                        break;
                    default:
                        break;
                }

                return true;
            }
        }
    }
}