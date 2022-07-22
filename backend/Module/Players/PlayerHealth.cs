using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Players
{
    public static class PlayerHealth
    {
        public static void ApplyPlayerHealth(this DbPlayer iPlayer)
        {
            if (iPlayer.Hp > 0)
            {
                if (iPlayer.Hp > 99) iPlayer.Hp = 99;
                iPlayer.SetHealth(iPlayer.Hp);
            }
            if (iPlayer.Armor[0] > 0)
            {
                if(iPlayer.Armor[0] > 99)
                {
                    iPlayer.Armor[0] = 99;
                }

                iPlayer.SetArmor(iPlayer.Armor[0]);
            }
        }

        public static void UpdatePlayerHealthAndArmor(this DbPlayer iPlayer)
        {
            iPlayer.Hp = iPlayer.Player.Health;
            iPlayer.Armor[0] = iPlayer.Player.Armor;
        }

        public static void SetHealth(this DbPlayer iPlayer, int health)
        {
            if (iPlayer.AccountStatus != AccountStatus.LoggedIn) return;

            if (health >= 99) health = 99;

            iPlayer.SetData("ac-healthchange", 2);
            iPlayer.SetData("ac_lastHealth", health);
            iPlayer.Hp = health;
            iPlayer.Player.Health = health;
        }

        public static void SetArmor(this DbPlayer iPlayer, int Armor, bool Schutzweste = false)
        {
            if (iPlayer.AccountStatus != AccountStatus.LoggedIn) return;

            iPlayer.SetArmorPlayer(Armor);
            iPlayer.visibleArmor = Schutzweste;
            iPlayer.ApplyArmorVisibility();
        }
    }
}