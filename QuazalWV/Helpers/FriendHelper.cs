using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    /// <summary>
    /// Helper methods which access both 'friends' and 'friend_invites' SQLite tables.
    /// </summary>
    public static class FriendHelper
    {
        public static List<EidosFriend> GetFriends(uint requesterPid)
        {
            List<EidosFriend> friends = new List<EidosFriend>();
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM friends WHERE friend_of={requesterPid}");
                FriendData fd;
                foreach (List<string> entry in results)
                {
                    // Always set to 2 - friend (confirmed)
                    fd = new FriendData { ByRelationship = 2 };
                    fd.Pid = Convert.ToUInt32(entry[2]);
                    fd.Name = entry[3];
                    fd.Details = Convert.ToUInt32(entry[4]);
                    // Get friend's current status
                    ClientInfo friend = Global.Clients.Find(c => c.PID == fd.Pid);
                    fd.Status = friend != null ? friend.Status : "Offline";
                    // TODO: Add friend's current lobby if they're in one - wrong gathering type for now
                    friends.Add(
                        new EidosFriend
                        {
                            FriendData = fd,
                            ClanAbbr = entry[5] ?? "",
                            IsLookingToPlay = Convert.ToUInt32(entry[6]) > 0
                        }
                    );
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[FriendHelper] {e}", Color.Red);
                return null;
            }

            return friends;
        }

        public static bool AreFriends(uint pid1, uint pid2)
        {
            // 2 records should exist for a friendship, so the value order doesnt matter
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM friends WHERE friend_of={pid1} AND pid={pid2}");
                return results.Count > 0;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[FriendHelper] {e}", Color.Red);
                return false;
            }
        }

        public static bool DeleteFriendOrInvite(uint selfPid, uint friendPid)
        {
            // Try to delete friend record pair (bidirectional)
            SQLiteCommand cmd = new SQLiteCommand(
                $"DELETE FROM friends WHERE (friend_of={selfPid} AND pid={friendPid}) OR (friend_of={friendPid} AND pid={selfPid})",
            SqliteHelper.Connection);
            try
            {
                int delFriendRowsAff = cmd.ExecuteNonQuery();
                if (delFriendRowsAff > 0)
                    Log.WriteLine(2, $"[FriendHelper] Player 0x{selfPid:X2} deleted friend 0x{friendPid:X2}");
                // Cancel outgoing friend invite (unidirectional if invite cancellation or bidirectional if friend deletion)
                string sql = delFriendRowsAff > 0
                    // For friend deletion the cleanup is there just in case, both invites should've been removed on acceptance
                    ? $"DELETE FROM friend_invites WHERE (inviter={selfPid} AND invitee={friendPid}) OR (inviter={friendPid} AND invitee={selfPid})"
                    : $"DELETE FROM friend_invites WHERE inviter={selfPid} AND invitee={friendPid}";
                cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
                int delInviteRowsAff = cmd.ExecuteNonQuery();
                // If it was invite cancellation
                if (delFriendRowsAff == 0 && delInviteRowsAff > 0)
                    Log.WriteLine(2, $"[FriendHelper] Player 0x{selfPid:X2} cancelled friend invite to 0x{friendPid:X2}");
                // If nothing was deleted (should have been)
                else if (delFriendRowsAff == 0 && delInviteRowsAff == 0)
                    Log.WriteLine(1, $"[FriendHelper] FriendDelete with no effect, requester: 0x{selfPid:X2}, friend: 0x{friendPid:X2}", Color.Red);
                return delFriendRowsAff + delInviteRowsAff > 0;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[FriendHelper] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Adds friendship to 'friends'; remember to always add two records on friend request acceptance (duplicate for bidirectional relation).
        /// </summary>
        /// <param name="inviterPid"></param>
        /// <param name="inviteePid"></param>
        /// <returns></returns>
        public static bool AddFriendship(uint inviterPid, uint inviteePid)
        {
            // Get the invitation being accepted
            FriendInvite fi = FriendInvite.Get(inviterPid, inviteePid);
            if (fi == null)
            {
                Log.WriteLine(1, $"[FriendHelper] AddFriendship: the accepted invite for inviter 0x{inviterPid:X2} and invitee 0x{inviteePid:X2} not found", Color.Red);
                return false;
            }
            // No idea what details are for so far
            uint details = 0;
            // Get clan tags from DB
            string inviteeClanAbbr = User.GetByPid(inviteePid).ClanTag;
            string inviterClanAbbr = User.GetByPid(inviterPid).ClanTag;
            // No idea where IsLoogkingToPlay is used
            bool isLookingToPlay = true;
            // Add 2 records for bidirectional friendship
            SQLiteCommand cmd = new SQLiteCommand(
                "INSERT INTO friends (friend_of,pid,name,details,clan_abbr,wanna_play) " +
                $"VALUES({inviterPid}," +
                $"{inviteePid}," +
                $"'{fi.InviteeName}'," +
                $"{details}," +
                $"'{inviteeClanAbbr}'," +
                $"{Convert.ToInt32(isLookingToPlay)}); " +
                "INSERT INTO friends (friend_of,pid,name,details,clan_abbr,wanna_play) " +
                $"VALUES({inviteePid}," +
                $"{inviterPid}," +
                $"'{fi.InviterName}'," +
                $"{details}," +
                $"'{inviterClanAbbr}'," +
                $"{Convert.ToInt32(isLookingToPlay)});",
            SqliteHelper.Connection);
            try
            {
                int rowsAff = cmd.ExecuteNonQuery();
                if (rowsAff == 2)
                {
                    // Delete friend invitations
                    cmd = new SQLiteCommand(
                        $"DELETE FROM friend_invites WHERE (inviter={inviterPid} AND invitee={inviteePid}) OR (inviter={inviteePid} AND invitee={inviterPid})",
                    SqliteHelper.Connection);
                    rowsAff = cmd.ExecuteNonQuery();
                    if (rowsAff > 0)
                        return true;
                    Log.WriteLine(1, $"[FriendHelper] AddFriendship: failed to delete friend invite(s) for 0x{inviterPid:X2} and 0x{inviteePid:X2}", Color.Red);
                    return false;
                }
                Log.WriteLine(1, $"[FriendHelper] AddFriendship: failed to add friend records for 0x{inviterPid:X2} and 0x{inviteePid:X2}", Color.Red);
                return false;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[FriendHelper] {e}", Color.Red);
                return false;
            }
        }
    }
}
