using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using VMP_CNR.Module.Items;
using VMP_CNR.Module.Players;
using VMP_CNR.Module.Players.Db;
using VMP_CNR.Module.Teams.Shelter;
using VMP_CNR.Module.Teams.Spawn;

namespace VMP_CNR.Module.Teams
{
    public sealed class TeamModule : SqlModule<TeamModule, Team, uint>
    {
        public int DutyCops => TeamModule.Instance.Get(1).GetTeamMembers().Where(member => member.Duty).Count() + // LSPD
                               TeamModule.Instance.Get(5).GetTeamMembers().Where(member => member.Duty).Count(); // SWAT

        public override Type[] RequiredModules()
        {
            return new[] {typeof(TeamSpawnModule), typeof(ItemModelModule) };
        }

        protected override string GetQuery()
        {
            return "SELECT * FROM `team`;";
        }

        public override void OnPlayerLoadData(DbPlayer dbPlayer, MySqlDataReader reader)
        {
            dbPlayer.LastUninvite = reader.GetDateTime("lastuninvite");
            dbPlayer.LastPaydayChanged = reader.GetDateTime("lastpaydaychanged");

            if (dbPlayer.LastUninvite == null) dbPlayer.LastUninvite = DateTime.Now;
            if (dbPlayer.LastPaydayChanged == null) dbPlayer.LastPaydayChanged = DateTime.Now;

            Console.WriteLine("TeamModule");

        }

        public override int GetOrder()
        {
            return 2;
        }
        
        protected override bool OnLoad()
        {
            var result = base.OnLoad();
            foreach (var teamSpawn in TeamSpawnModule.Instance.GetAll())
            {
                var team = this[teamSpawn.Value.TeamId];
                team?.TeamSpawns.Add(teamSpawn.Value.Index, teamSpawn.Value);
            }
            Console.WriteLine("TEAMMODULE DATA: " + result);
            return true;
        }

        public bool IsWeaponTeamId(uint Id)
        {
            return Id == (int)teams.TEAM_HUSTLER || Id == (int)teams.TEAM_ICA;
        }
        public bool IsMethTeamId(uint Id)
        {
            return Id == (int)teams.TEAM_YAKUZA;
        }
        public bool IsWeedTeamId(uint Id)
        {
            return Id == (int)teams.TEAM_BALLAS || Id == 8;
        }

        public bool IsGangsterTeamId(uint Id)
        {
            return IsWeedTeamId(Id) || IsMethTeamId(Id) || IsWeaponTeamId(Id);
        }
        public override void OnPlayerConnected(DbPlayer dbPlayer)
        {
            Console.WriteLine(dbPlayer.TeamId + " SEXY IK BIN GAY");
            dbPlayer.SetTeam(dbPlayer.TeamId, false);
        }

        public override void OnPlayerDisconnected(DbPlayer dbPlayer, string reason)
        {
            if (dbPlayer.TeamId != 0)
            {
                dbPlayer.Team.RemoveMember(dbPlayer);
            }
        }

        public Team GetByName(string name)
        {
            foreach (var team in GetAll().Values)
            {
                if (string.Equals(team.Name, name, StringComparison.CurrentCultureIgnoreCase)
                    || string.Equals(team.ShortName, name, StringComparison.CurrentCultureIgnoreCase)
                    || team.Name.ToLower().Contains(name.ToLower())) return team;
            }
            
            return null;
        }

        public Team GetById(int id)
        {
            foreach (var team in GetAll().Values)
            {
                if (team.Id == id) return team;
            }

            return null;
        }

        public void SendChatMessage(string message, params uint[] ids)
        {
            foreach (var id in ids)
            {
                this[id].SendNotification(message);
            }
        }

        public void SendMessageToTeam(string message, teams teamId, int time = 5000, int requiredRang = 0)
        {
            Team team = Get((uint)teamId);

            team.SendNotification(message, time:time, rang: requiredRang);
            return;
        }

        public void SendMessageToTeam(string message, uint teamId, int time = 5000, int requiredRang = 0)
        {
            Team team = Get(teamId);

            team.SendNotification(message, time: time, rang: requiredRang);
            return;
        }

        public void SendMessageToNSA(string message)
        {
            foreach (var iPlayer in NSA.NSAModule.Instance.NSAMember.Where(p => p.IsNSADuty).ToList())
            {
                iPlayer.SendNewNotification(message, PlayerNotification.NotificationType.FRAKTION);
            }
            return;
        }

        public void SendChatMessageToDepartments(DbPlayer sourceDbPlayer, string message)
        {
            foreach (var team in GetAll())
            {
                if (!team.Value.IsCops()) continue;
                
                team.Value.SendNotification(sourceDbPlayer.Team.ShortName + " Rang " +
                        sourceDbPlayer.TeamRank + " | " + sourceDbPlayer.GetName() + ": " + message);
            }
        }

        public void SendChatMessageToDepartmentsInRange(string message, Vector3 pos, float range)
        {
            foreach (var team in GetAll())
            {
                if (!team.Value.IsCops()) continue;

                team.Value.SendNotificationInRange(message, pos, range);
            }
        }


        public void SendChatMessageToDepartments(string message)
        {
            foreach (var team in GetAll())
            {
                if (!team.Value.IsCops()) continue;

                team.Value.SendNotification(message, 10000);
            }
        }
        
        public DbPlayer CheckIfDutyCopIsInRange(DbPlayer sourceDbPlayer, float range)
        {
            foreach (var team in GetAll())
            {
                if (!team.Value.IsCops()) continue;
                foreach (var dbPlayer in team.Value.Members.Values)
                {
                    if (dbPlayer.Duty && 
                        dbPlayer.Player.Position.DistanceTo(sourceDbPlayer.Player.Position) < range && 
                        dbPlayer.Player.Dimension == sourceDbPlayer.Player.Dimension &&
                        dbPlayer.DimensionType == sourceDbPlayer.DimensionType) return dbPlayer;
                }
            }
            return null;
        }

        public override void OnFifteenMinuteUpdate()
        {
            foreach(Team team in GetAll().Values.Where(t => t.IsGangsters()).ToList())
            {
                int teamsumm = 0;
                foreach(DbPlayer member in team.Members.Values.ToList())
                {
                    if(member.blackmoneybank[0] > 5000)
                    {
                        member.blackmoneybank[0] -= 400;
                        teamsumm += 340; // 90% kurs
                        member.SaveBlackMoneyBank();
                    }
                }
                TeamShelter shelter = TeamShelterModule.Instance.GetByTeam(team.Id);
                if(shelter != null)
                {
                    shelter.GiveMoney(teamsumm);
                }
            }
        }
        public void SendChatMessageToDutyTeamMembers(DbPlayer sourceDbPlayer, string message)
        {
            foreach (var dbPlayer in sourceDbPlayer.Team.Members)
            {
                if (dbPlayer.Value == null || !dbPlayer.Value.IsValid()) continue;
                if (dbPlayer.Value.IsACop() && !dbPlayer.Value.Duty) continue;
                dbPlayer.Value.SendNewNotification( sourceDbPlayer.Team.ShortName + " Rang " +
                    sourceDbPlayer.TeamRank + " | " + sourceDbPlayer.GetName() + ": " + message);
            }
        }
    }

    public static class TeamPlayerExtension
    {
        public static void SaveLastUninvite(this DbPlayer xPlayer)
        {
            MySQLHandler.ExecuteAsync($"UPDATE player SET `lastuninvite` = '{xPlayer.LastUninvite.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE `id` = '{xPlayer.Id}'");
        }

        public static void SaveLastPaydayChanged(this DbPlayer xPlayer)
        {
            MySQLHandler.ExecuteAsync($"UPDATE player SET `lastpaydaychanged` = '{xPlayer.LastPaydayChanged.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE `id` = '{xPlayer.Id}'");
        }

        public static void SaveLastBankRobbery(this DbPlayer dbPlayer)
        {
            MySQLHandler.ExecuteAsync($"UPDATE team SET `lastbankrob` = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE `id` = '{dbPlayer.Team.Id}'");
        }

        public static void SaveLastOutfitPreQuest(this DbPlayer dbPlayer)
        {
            MySQLHandler.ExecuteAsync($"UPDATE team SET `lastoutfitprequest` = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE `id` = '{dbPlayer.Team.Id}'");
        }
    }
}