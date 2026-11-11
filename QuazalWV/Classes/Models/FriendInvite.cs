using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    /// <summary>
    /// Model for 'friend_invites' DB table.
    /// </summary>
    public class FriendInvite : SqliteModel
    {
        public uint InviterPid { get; set; }
        public string InviterName { get; set; }
        public uint InviteePid { get; set; }
        public string InviteeName { get; set; }
        public bool Blocked { get; set; }

        /// <summary>
        /// Gets the friend invitation sent from the inviter to the invitee.
        /// </summary>
        /// <param name="inviterPid"></param>
        /// <param name="inviteePid"></param>
        /// <returns></returns>
        public static FriendInvite Get(uint inviterPid, uint inviteePid)
        {
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM friend_invites WHERE inviter={inviterPid} AND invitee={inviteePid}");
                if (results.Count > 0)
                {
                    FriendInvite invite = new FriendInvite
                    {
                        Id = Convert.ToUInt32(results[0][0]),
                        InviterPid = Convert.ToUInt32(results[0][1]),
                        InviterName = results[0][2],
                        InviteePid = Convert.ToUInt32(results[0][3]),
                        InviteeName = results[0][4],
                        Blocked = Convert.ToUInt32(results[0][5]) > 0
                    };
                    return invite;
                }
                Log.WriteLine(1, $"[DBHelper] No invite found for inviter 0x{inviterPid:X2} and invitee 0x{inviteePid:X2}", Color.Red);
                return null;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return null;
            }
        }

        /// <summary>
        /// Adds a friend invitation to SQLite database.
        /// </summary>
        /// <param name="inviterPid"></param>
        /// <param name="inviterName"></param>
        /// <param name="inviteePid"></param>
        /// <param name="inviteeName"></param>
        /// <returns></returns>
        public static bool Add(FriendInvite fi)
        {
            // Check if these are friends already (can't invite a friend to be a friend)
            if (FriendHelper.AreFriends(fi.InviterPid, fi.InviteePid))
                return false;
            // TODO: Invitation dates to be added in future to allow invitation timeouts (sqlite doesnt support a date type)
            SQLiteCommand cmd = new SQLiteCommand(
                $"INSERT INTO friend_invites (inviter,inviter_name,invitee,invitee_name,blocked) " +
                $"VALUES ({fi.InviterPid},'{fi.InviterName}',{fi.InviteePid},'{fi.InviteeName}',{Convert.ToInt32(fi.Blocked)});",
            SqliteHelper.Connection);
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM friend_invites WHERE inviter={fi.InviterPid} AND invitee={fi.InviteePid}");
                // There is already an invite for this invitee
                if (results.Count > 0)
                    return false;
                bool result = cmd.ExecuteNonQuery() > 0;
                // Add alert
                FriendInvite inv = Get(fi.InviterPid, fi.InviteePid);
                Alert alert = new Alert
                {
                    InviteId = inv.Id,
                    InviteType = "F",
                    Inviter = fi.InviterPid,
                    Invitee = fi.InviteePid,
                    TargetId = fi.InviterPid
                };
                if (!Alert.Add(alert))
                    Log.WriteLine(2, $"[DBHelper] AddFriendship: failed to add alert for friend invite {fi.Id}");
                return result;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Gets all incoming friend invites and maps them to EidosFriends.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<EidosFriend> GetAllIncoming(uint pid)
        {
            List<EidosFriend> invites = new List<EidosFriend>();
            try
            {
                // Get all incoming invites which arent blocked (if blocked it should persist to prevent further invitations)
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM friend_invites WHERE invitee={pid} AND blocked=0");
                FriendData fd;
                foreach (List<string> entry in results)
                {
                    // Always set to 0 - incoming invite
                    fd = new FriendData { ByRelationship = 0 };
                    fd.Pid = Convert.ToUInt32(entry[1]);
                    fd.Name = entry[2];
                    // So far no idea what details do
                    fd.Details = 0;
                    // Check friend's status
                    ClientInfo friend = Global.Clients.Find(c => c.PID == fd.Pid);
                    fd.Status = friend != null ? "25ToLife|PC|ALL" : "Offline";
                    invites.Add(
                        new EidosFriend
                        {
                            FriendData = fd,
                            // Clan data to be cleaned from everywhere for now
                            ClanAbbr = "",
                            IsLookingToPlay = true
                        }
                    );
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return null;
            }
            return invites;
        }

        /// <summary>
        /// Gets all outgoing friend invites and maps them to EidosFriends.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<EidosFriend> GetAllOutgoing(uint pid)
        {
            List<EidosFriend> invites = new List<EidosFriend>();
            try
            {
                // Get all incoming invites which arent blocked (if blocked it should persist to prevent further invitations)
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM friend_invites WHERE inviter={pid} AND blocked=0");
                FriendData fd;
                foreach (List<string> entry in results)
                {
                    // Always set to 1 - outgoing invite
                    fd = new FriendData
                    {
                        ByRelationship = 1,
                        Pid = Convert.ToUInt32(entry[3]),
                        Name = entry[4],
                        // So far no idea what details do
                        Details = 0
                    };
                    // Check friend's status
                    ClientInfo friend = Global.Clients.Find(c => c.PID == fd.Pid);
                    fd.Status = friend != null ? "25ToLife|PC|ALL" : "Offline";
                    invites.Add(
                        new EidosFriend
                        {
                            FriendData = fd,
                            // Clan data to be cleaned from everywhere for now
                            ClanAbbr = "",
                            IsLookingToPlay = true
                        }
                    );
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return null;
            }
            return invites;
        }

        public static bool RejectOrBlock(FriendInvite fi)
        {
            string sql = fi.Blocked
                // 'Block', block the invite
                ? $"UPDATE friend_invites SET blocked=1 WHERE inviter={fi.InviterPid} AND invitee={fi.InviteePid}"
                // 'Deny', delete the invite
                : $"DELETE FROM friend_invites WHERE inviter={fi.InviterPid} AND invitee={fi.InviteePid}";
            SQLiteCommand cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
            try { return cmd.ExecuteNonQuery() > 0; }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return false;
            }
        }
    }
}
