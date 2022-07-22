﻿using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Commands;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.InteriorProp
{
    public enum InteriorPropListsType
    {
        Kokainlabor = 1,
        Lagerraum = 4,
    }

    public class InteriorPropModule : Module<InteriorPropModule>
    {
        public override bool Load(bool reload = false)
        {
            return base.Load(reload);
        }
        
        public void LoadInteriorForPlayer(DbPlayer dbPlayer, InteriorPropListsType interiorPropListsType)
        {
            InteriorsProp interiorsProp = InteriorsPropModule.Instance.Get((uint)interiorPropListsType);
            if (interiorsProp == null) return;
            LoadInteriorForPlayer(dbPlayer, interiorsProp);
        }

        private void LoadInteriorForPlayer(DbPlayer dbPlayer, InteriorsProp interiorsProp)
        {
            if (interiorsProp == null) return;
            foreach (string name in interiorsProp.Props)
            {
                dbPlayer.Player.TriggerEvent("enableInteriorProp", interiorsProp.InteriorId, name);
            }
            dbPlayer.Player.TriggerEvent("refreshinterior", interiorsProp.InteriorId);
        }

        public void UnloadInteriorForPlayer(DbPlayer dbPlayer, InteriorPropListsType interiorPropListsType)
        {
            InteriorsProp interiorsProp = InteriorsPropModule.Instance.Get((uint)interiorPropListsType);
            if (interiorsProp == null) return;
            UnloadInteriorForPlayer(dbPlayer, interiorsProp);
        }

        private void UnloadInteriorForPlayer(DbPlayer dbPlayer, InteriorsProp interiorsProp)
        {
            if (interiorsProp == null) return;
            foreach (string name in interiorsProp.Props)
            {
                dbPlayer.Player.TriggerEvent("disableInteriorProp", interiorsProp.InteriorId, name);
            }
            dbPlayer.Player.TriggerEvent("refreshinterior", interiorsProp.InteriorId);
        }

        [CommandPermission(PlayerRankPermission = true)]
        [Command]
        public void Commandgetiid(Player player)
        {
            var iPlayer = player.GetPlayer();
            if (iPlayer == null || !iPlayer.CanAccessMethod()) return;

            if (!iPlayer.IsValid()) return;

            iPlayer.Player.TriggerEvent("getInteriorId", true);
            iPlayer.SendNewNotification("Event triggered");
        }

        public override bool OnColShapeEvent(DbPlayer dbPlayer, ColShape colShape, ColShapeState colShapeState)
        {
            if(colShape.HasData("interiorPropId"))
            {
                InteriorsProp interiorsProp = InteriorsPropModule.Instance.Get(colShape.GetData<uint>("interiorPropId"));
                if(interiorsProp != null)
                {
                    if(colShapeState == ColShapeState.Enter)
                    {
                        LoadInteriorForPlayer(dbPlayer, interiorsProp);
                    }
                    else UnloadInteriorForPlayer(dbPlayer, interiorsProp);
                }
            }
            return false;
        }
    }
}
