using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MySql.Data.MySqlClient;
using VMP_CNR.Module.Configurations;
using VMP_CNR.Module.Players.Db;

namespace VMP_CNR.Module.Players
{
    public static class PlayerLoginDataValidationModule
    {
        public static int FreigeschaltetGroupId = 63;

        public static void SyncUserBanToForum(int forumId)
        {
            // Remove Freigeschaltet Gruppe
            if (!Configuration.Instance.DevMode && ServerFeatures.IsActive("forumsync"))
                MySQLHandler.ExecuteForum($"DELETE FROM wcf1_user_to_group WHERE userID = '{forumId}' AND groupID = '18'");


            string url = "https://www.gvrp.to/index.php?perma-ban-ddos-weil-baum"; // Just a sample url
            WebClient wc = new WebClient();

            wc.QueryString.Add("forumid", "" + forumId);
            wc.QueryString.Add("reason", "§2 Accounts | Abschnitt 4");
            wc.QueryString.Add("dauer", "permanent");

            var data = wc.UploadValues(url, "POST", wc.QueryString);

            // data here is optional, in case we recieve any string data back from the POST request.
            var responseString = UnicodeEncoding.UTF8.GetString(data);
        }

        public static bool HasValidForumAccount(int forumid)
        {
 
            return true;
        }
    }
}
