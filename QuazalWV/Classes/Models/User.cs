using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;

namespace QuazalWV
{
    /// <summary>
    /// Model for 'users' DB table.
    /// </summary>
    public class User : SqliteModel
    {
        public uint Pid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public uint Groups { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Sex { get; set; }
        public bool EidosInfo { get; set; }
        public bool PartnersInfo { get; set; }
        public bool SavePassword { get; set; }
        public string Zipcode { get; set; }
        /// <summary>
        /// UTC timestamp.
        /// </summary>
        public long Birthday { get; set; }
        /// <summary>
        /// Player's clan tag (up to 3 characters); set from clan or manually for PC players.
        /// </summary>
        public string ClanTag { get; set; }
        /// <summary>
        /// Defaults to 1 on db insert.
        /// </summary>
        public bool AcceptingClanInvites { get; set; }

        /// <summary>
        /// Returns the highest pid in SQLite database.
        /// </summary>
        /// <returns></returns>
        public static uint GetHighestPid()
        {
            List<List<string>> results = new List<List<string>>();
            results = SqliteHelper.GetQueryResults("SELECT pid FROM users");
            List<uint> pids = new List<uint>();
            foreach (List<string> entry in results)
                pids.Add(Convert.ToUInt32(entry[0]));

            try
            {
                return pids.Max();
            }
            catch
            {
                // fresh database
                return 0;
            }
        }

        /// <summary>
        /// Returns the player's clan tag by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultTag"></param>
        /// <returns></returns>
        public static string GetClanTagByName(string name, string defaultTag)
        {
            uint clanId = ClanMember.ClanIdFromName(name);
            // if the player has a clan, use its tag (PS2)
            if (clanId != 0)
            {
                List<List<string>> clanResults = SqliteHelper.GetQueryResults($"SELECT * FROM clans WHERE id={clanId}");
                return clanResults[0][1];
            }
            // use clan tag for PC players and non-member PS2 players
            else
                return defaultTag;
        }

        /// <summary>
        /// Returns the user by their name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static User GetByName(string name)
        {
            User result = null;
            List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM users WHERE username='{name}'");

            foreach (List<string> entry in results)
            {
                result = new User
                {
                    Id = Convert.ToUInt32(entry[0]),
                    Pid = Convert.ToUInt32(entry[1]),
                    Username = name,
                    Password = entry[3],
                    Groups = Convert.ToUInt32(entry[4])
                };
                // Add email if available
                if (entry[5] != null && entry[5] != "")
                    result.Email = entry[5];
                // Add custom data if available
                if(entry[6] != null && entry[6] != "")
                {
                    string clanAbbr = GetClanTagByName(name, results[0][13] ?? "");
                    result.Country = entry[6].Replace(" ", "");
                    result.Sex = entry[7];
                    result.EidosInfo = Convert.ToUInt32(entry[8]) > 0;
                    result.PartnersInfo = Convert.ToUInt32(entry[9]) > 0;
                    result.SavePassword = Convert.ToUInt32(entry[10]) > 0;
                    result.Zipcode = entry[11];
                    result.Birthday = Convert.ToInt64(entry[12]);
                    result.ClanTag = clanAbbr;
                    result.AcceptingClanInvites = Convert.ToUInt32(entry[14] ?? "1") > 0;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the user by their PID.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="defaultTag"></param>
        /// <returns></returns>
        public static string GetClanTagByPid(uint pid, string defaultTag)
        {
            uint clanId = ClanMember.ClanIdFromPid(pid);
            // if the player has a clan, use its tag (PS2)
            if (clanId != 0)
            {
                List<List<string>> clanResults = SqliteHelper.GetQueryResults($"SELECT * FROM clans WHERE id={clanId}");
                return clanResults[0][1];
            }
            // use clan tag for PC players and non-member PS2 players
            else
                return defaultTag;
        }

        /// <summary>
        /// Returns the user by their pid.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static User GetByPid(uint pid)
        {
            User result = null;
            List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM users WHERE pid={pid}");
            
            foreach (List<string> entry in results)
            {
                result = new User
                {
                    Id = Convert.ToUInt32(entry[0]),
                    Pid = pid,
                    Username = entry[2],
                    Password = entry[3],
                    Groups = Convert.ToUInt32(entry[4])
                };
                // Add email if available
                if (entry[5] != null && entry[5] != "")
                    result.Email = entry[5];
                // Add custom data if available
                if (entry[6] != null && entry[6] != "")
                {
                    string clanAbbr = GetClanTagByPid(pid, results[0][13] ?? "");
                    result.Country = entry[6].Replace(" ", "");
                    result.Sex = entry[7];
                    result.EidosInfo = Convert.ToUInt32(entry[8]) > 0;
                    result.PartnersInfo = Convert.ToUInt32(entry[9]) > 0;
                    result.SavePassword = Convert.ToUInt32(entry[10]) > 0;
                    result.Zipcode = entry[11];
                    result.Birthday = Convert.ToInt64(entry[12]);
                    result.ClanTag = clanAbbr;
                }
            }
            return result;
        }

        /// <summary>
        /// Adds user's core data to SQLite database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool Add(User u)
        {
            //pidCounter needs to be unique
            SQLiteCommand cmd = new SQLiteCommand(
                $"INSERT INTO users (pid, username, password, groups) VALUES ({u.Pid},'{u.Username}','{u.Password}',{u.Groups});",
                SqliteHelper.Connection);
            if (GetByName(u.Username) != null)
            {
                Log.WriteLine(1, $"[User] User {u.Username} already exists", Color.Orange);
                return false;
            }
            try 
            { 
                bool result = cmd.ExecuteNonQuery() > 0;
                if(!result)
                {
                    Log.WriteLine(1, $"[User] Couldn't save user {u.Username} in db", Color.Red);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[User] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Updates the account with custom form data (except for email) in SQLite database.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static bool UpdateCustomData(User u)
        {
            if (GetByName(u.Username) == null)
            {
                Log.WriteLine(1, $"[User] User {u.Username} doesnt exist, cannot update custom data", Color.Red);
                return false;
            }

            SQLiteCommand cmd = new SQLiteCommand(
                $"UPDATE users " +
                $"SET country='{u.Country}', " +
                $"sex='{u.Sex}', " +
                $"eidos_info={Convert.ToInt32(u.EidosInfo)}, " +
                $"partners_info={Convert.ToInt32(u.PartnersInfo)}, " +
                $"save_password={Convert.ToInt32(u.SavePassword)}, " +
                $"zipcode='{u.Zipcode}'," +
                $"birthday_date={u.Birthday} " +
                $"WHERE username='{u.Username}';",
                SqliteHelper.Connection);

            try { return cmd.ExecuteNonQuery() > 0; }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[User] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Updates user's email in SQLite database.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool UpdateEmail(string email, string username)
        {
            if (GetByName(username) == null)
            {
                Log.WriteLine(1, $"[User] User {username} doesnt exist, cannot update email", Color.Red);
                return false;
            }

            SQLiteCommand cmd = new SQLiteCommand($"UPDATE users SET email='{email}' WHERE username='{username}';", SqliteHelper.Connection);

            try { return cmd.ExecuteNonQuery() > 0; }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[User] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Updates user's password key in SQLite database.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool UpdatePassword(string hash, string username)
        {
            if (GetByName(username) == null)
            {
                Log.WriteLine(1, $"[User] User {username} doesnt exist, cannot update password", Color.Red);
                return false;
            }

            SQLiteCommand cmd = new SQLiteCommand($"UPDATE users SET password='{hash}' WHERE username='{username}';", SqliteHelper.Connection);

            try { return cmd.ExecuteNonQuery() > 0; }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[User] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Returns a list of pid-name pairs (BasicAccountInfos).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<BasicAccountInfo> GetBasicAccountInfos(string name)
        {
            List<BasicAccountInfo> result = new List<BasicAccountInfo>();
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT pid, username FROM users WHERE username='{name}'");
                foreach (List<string> entry in results)
                    result.Add(new BasicAccountInfo(Convert.ToUInt32(entry[0]), entry[1]));
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[User] {e}", Color.Red);
                return null;
            }
            return result;
        }

        public static EClanError SetAcceptClanInvites(ClientInfo client, bool accepting)
        {
            client.AcceptClanInvites = accepting;
            string sql = $"UPDATE users SET accept_clan_invites={(accepting ? 1 : 0)} WHERE pid={client.PID}";
            SQLiteCommand cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.UnspecifiedClanError;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[User] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }
    }
}
