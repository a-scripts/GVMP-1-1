using VMP_CNR.Module.Injury;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Players
{
    public static class PlayerCommunications
    {
        public static bool CanCommunicate(this DbPlayer iPlayer)
        {
            return !(iPlayer.isInjured() || iPlayer.jailtime[0] > 0 ||
                     iPlayer.IsCuffed || iPlayer.IsTied);
        }

        public static void BlockCommunications(this DbPlayer iPlayer)
        {
        }
    }
}