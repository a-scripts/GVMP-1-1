﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Players;

namespace VMP_CNR.Module.Pet
{
    public class PetCommands : Script
    {
        [Command(GreedyArg = true)]
        public void getpet(Player player, string petName)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.IsValid()) return;

            foreach (var pet in PetModule.Instance.GetAll())
            {
                pet.Value.Name.ToLower().Contains(petName.ToLower());
                dbPlayer.LoadPet(pet.Value, dbPlayer.Player.Position, dbPlayer.Player.Heading, dbPlayer.Player.Dimension);
                break;
            }

        }

        [Command(GreedyArg = true)]
        public void followme(Player player)
        {
            var dbPlayer = player.GetPlayer();
            if (!dbPlayer.IsValid()) return;

            dbPlayer.SetPetFollow(dbPlayer.Player);
        }
    }
}
