﻿using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Forum
{
    public static class PlayerForumSync
    {
        public static void SynchronizeJobForum(this DbPlayer iPlayer)
        {
            if (Configuration.Instance.DevMode) return;
            if (!ServerFeatures.IsActive("forumsync")) return;

            string completeQuery = iPlayer.GetJobRemoveQuery();
            if (iPlayer.job[0] != 0)
            {
                uint forumGroup = iPlayer.GetForumGroupForJob();

                if (forumGroup > 0)
                {
                    // Set user into group
                    completeQuery +=
                        $"INSERT INTO wcf1_user_to_group (`userID`, `groupID`) VALUES ('{iPlayer.ForumId}', '{@forumGroup}');";
                }

                using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnectionForum()))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = completeQuery;
                        cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        //Todo: can be made async
        public static void SynchronizeForum(this DbPlayer iPlayer)
        {
            if (Configuration.Instance.DevMode) return;
            if (!ServerFeatures.IsActive("forumsync")) return;

            string completeQuery = iPlayer.GetRemoveQuery();
            if (iPlayer.TeamId > 0)
            {
                int forumGroup = iPlayer.GetForumGroupForTeam();

                if (forumGroup > 0)
                {
                    // Set user into group
                    completeQuery +=
                        $"INSERT INTO wcf1_user_to_group (`userID`, `groupID`) VALUES ('{iPlayer.ForumId}', '{@forumGroup}');";
                }
            }

            using (var conn = new MySqlConnection(Configuration.Instance.GetMySqlConnectionForum()))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = completeQuery;
                    cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private static string GetJobRemoveQuery(this DbPlayer iPlayer)
        {
            var removeGroups = new List<int>();
            foreach (var kvp in JobForumSync.Instance.GetAll())
            {
                removeGroups.Add((int)kvp.Value.group_id);
            }

            string addRemSql = "";
            foreach (var group in removeGroups)
            {
                if (addRemSql == "") addRemSql = $"groupID = '{@group}'";
                else addRemSql = addRemSql + $" OR groupID = '{@group}'";
            }

            return $"DELETE FROM wcf1_user_to_group WHERE userID = '{iPlayer.ForumId}' AND ({addRemSql});";
        }

        // Returns all Groups (used in group sync) for removing first
        private static string GetRemoveQuery(this DbPlayer iPlayer)
        {
            var removeGroups = new List<int>();

            foreach (var kvp in TeamForumSync.Instance.GetAll())
            {
                removeGroups.Add(kvp.Value.LeaderGroup);
                removeGroups.Add(kvp.Value.MemberGroup);
            }

            string addRemSql = "";

            foreach (var group in removeGroups)
            {
                if (addRemSql == "") addRemSql = $"groupID = '{@group}'";
                else addRemSql = addRemSql + $" OR groupID = '{@group}'";
            }

            return $"DELETE FROM wcf1_user_to_group WHERE userID = '{iPlayer.ForumId}' AND ({addRemSql});";
        }


        public static string GetRemoveQueryByForumId(uint forumId)
        {
            var removeGroups = new List<int>();

            foreach (var kvp in TeamForumSync.Instance.GetAll())
            {
                removeGroups.Add(kvp.Value.LeaderGroup);
                removeGroups.Add(kvp.Value.MemberGroup);
            }

            string addRemSql = "";

            foreach (var group in removeGroups)
            {
                if (addRemSql == "") addRemSql = $"groupID = '{@group}'";
                else addRemSql = addRemSql + $" OR groupID = '{@group}'";
            }

            return $"DELETE FROM wcf1_user_to_group WHERE userID = '{forumId}' AND ({addRemSql});";
        }

        private static int GetForumGroupForTeam(this DbPlayer iPlayer)
        {
            if (!TeamForumSync.Instance.GetAll().ContainsKey(iPlayer.TeamId)) return 0;

            switch (iPlayer.TeamRankPermission.Manage)
            {
                case 2:
                    return TeamForumSync.Instance[iPlayer.TeamId].LeaderGroup;
                case 1:
                    return TeamForumSync.Instance[iPlayer.TeamId].LeaderGroup;
                default:
                    return TeamForumSync.Instance[iPlayer.TeamId].MemberGroup;
            }
        }

        private static uint GetForumGroupForJob(this DbPlayer iPlayer)
        {
            if (!JobForumSync.Instance.GetAll().ContainsKey((uint)iPlayer.job[0])) return 0;

            return JobForumSync.Instance[(uint)iPlayer.job[0]].group_id;
        }
    }
}