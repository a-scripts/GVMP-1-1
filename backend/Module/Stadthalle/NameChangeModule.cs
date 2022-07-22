﻿using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using VMP_CNR.Module.PlayerUI.Components;
using VMP_CNR.Module.Einreiseamt;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Kasino;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Players.Windows;
using VMP_CNR.Module.Spawners;

namespace VMP_CNR.Module.Stadthalle
{
    public class NameChangeModule : Module<NameChangeModule>
    {
        public static Vector3 NameChangePosition = new Vector3(-555.256, -197.16, 38.2224);
        public static ColShape NameChangeColShape = null;


        protected override bool OnLoad()
        {
            NameChangeColShape = ColShapes.Create(NameChangePosition, 3.0f, 0);
            NameChangeColShape.SetData("name_change", true);
            MenuManager.Instance.AddBuilder(new StadtHalleMenu());
            return base.OnLoad();
        }

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            if (dbPlayer.Player.IsInVehicle) return false;

            if (!dbPlayer.HasData("name_change"))
                return false;

            if (key == Key.E)
            {
                MenuManager.Instance.Build(PlayerMenu.StadtHalleMenu, dbPlayer).Show(dbPlayer);
                return true;
            }
            return false;
        }


    }
}
