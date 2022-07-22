﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using VMP_CNR.Module.Gangwar;
using VMP_CNR.Module.GTAN;
using VMP_CNR.Module.Menu;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Teams.Shelter
{
    public class TeamShelterModule : SqlModule<TeamShelterModule, TeamShelter, uint>
    {
        public const int InventorySize = 25000;

        public override Type[] RequiredModules()
        {
            return new[] { typeof(TeamModule) };
        }

        protected override string GetQuery()
        {
            return "SELECT * FROM `team_shelter`;";
        }

        public TeamShelter GetByTeam(uint teamId)
        {
            return GetAll().Values.Where(t => t.Team.Id == teamId).FirstOrDefault();
        }
        
        public override bool OnColShapeEvent(DbPlayer dbPlayer, ColShape colShape, ColShapeState colShapeState)
        {
            if (colShape.TryData("teamShelterMenuId", out uint teamShelterMenuId))
            {
                if (colShapeState == ColShapeState.Enter)
                {
                    dbPlayer.SetData("teamShelterMenuId", teamShelterMenuId);
                    return true;
                }
                else
                {
                    dbPlayer.ResetData("teamShelterMenuId");
                    return true;
                }
            }

            return false;
        }

        protected override void OnLoaded()
        {
            MenuManager.Instance.AddBuilder(new ShelterMenuBuilder());
            MenuManager.Instance.AddBuilder(new ShelterFightMenuBuilder());
            MenuManager.Instance.AddBuilder(new PistoleCreateMenuBuilder());



        }

        public override bool OnKeyPressed(DbPlayer dbPlayer, Key key)
        {
            try { 
            if (key != Key.E) return false;
            if (dbPlayer.Team.Id == 0) return false;
            if (dbPlayer.HasData("teamShelterMenuId"))
            {
                var currentShelter = Get(dbPlayer.GetData("teamShelterMenuId"));
                if (currentShelter == null) return false;
                
                // Range check for menu
                if (dbPlayer.Player.Position.DistanceTo(currentShelter.MenuPosition) < 3.0f)
                {
                    if (dbPlayer.TeamId != currentShelter.Team.Id) return false;
                    Console.WriteLine("SEXY | " + currentShelter.Team.Id);

                    MenuManager.Instance.Build(PlayerMenu.ShelterMenu, dbPlayer).Show(dbPlayer);
                    return true;
                }
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public TeamShelter GetByInventoryPosition(Vector3 Position, uint dimension = 0)
        {
            foreach (var shelter in GetAll())
            {
                if (shelter.Value != null && shelter.Value.InventarPosition.DistanceTo(Position) < 3.0f && shelter.Value.Dimension == dimension)
                    return shelter.Value;
            }
            return null;
        }
        
    }
}