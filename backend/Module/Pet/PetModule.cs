using System;
using GTANetworkAPI;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Pet;

namespace VMP_CNR.Module.Pet
{
    public sealed class PetModule : SqlModule<PetModule, PetData, uint>
    {
        protected override string GetQuery()
        {
            return "SELECT * FROM `pets`;";
        }

        protected override bool OnLoad()
        {
            return base.OnLoad();
        }

        public override void OnPlayerDisconnected(DbPlayer dbPlayer, string reason)
        {
            dbPlayer.RemovePet();
        }
    }
}