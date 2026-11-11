using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    /// <summary>
    /// Model for 'clan_invites' DB table.
    /// </summary>
    public class ClanInvite : SqliteModel
    {
        public uint InviterPid { get; set; }
        public string InviterName { get; set; }
        public uint InviteePid { get; set; }
        public string InviteeName { get; set; }
        public bool Blocked { get; set; }
        public uint ClanId { get; set; }
        public string ClanAbbr { get; set; }
        public string ClanName { get; set; }
        public uint InviterClanRank { get; set; }

        /// <summary>
        /// Gets the clan invitation sent from the inviter to the invitee.
        /// </summary>
        /// <param name="inviterPid"></param>
        /// <param name="inviteePid"></param>
        /// <returns></returns>
        public static ClanInvite Get(uint inviterPid, uint inviteePid)
        {
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM clan_invites WHERE inviter={inviterPid} AND invitee={inviteePid}");
                ClanInvite invite = new ClanInvite
                {
                    Id = Convert.ToUInt32(results[0][0]),
                    InviterPid = Convert.ToUInt32(results[0][1]),
                    InviterName = results[0][2],
                    InviteePid = Convert.ToUInt32(results[0][3]),
                    InviteeName = results[0][4],
                    Blocked = Convert.ToUInt32(results[0][5]) > 0,
                    ClanId = Convert.ToUInt32(results[0][6]),
                    ClanAbbr = results[0][7],
                    ClanName = results[0][8],
                    InviterClanRank = Convert.ToUInt32(results[0][9])
                };
                return invite;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanInvite] {e}", Color.Red);
                return null;
            }
        }

        /// <summary>
        /// Adds a clan invitation to SQLite database.
        /// </summary>
        /// <returns></returns>
        public bool Add()
        {
            // TODO: check if this is a clan member already (can't invite a clan member to be a clan member)
            // TODO: Invitation dates to be added in future to allow invitation timeouts (sqlite doesnt support a date type)
            SQLiteCommand cmd = new SQLiteCommand(
                $"INSERT INTO clan_invites (inviter,inviter_name,invitee,invitee_name,blocked,clan_id,clan_abbr,clan_name,inviter_clan_rank) " +
                $"VALUES ({InviterPid},'{InviterName}',{InviteePid},'{InviteeName}',{Convert.ToInt32(Blocked)},{ClanId},'{ClanAbbr}','{ClanName}',{InviterClanRank});",
            SqliteHelper.Connection);
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM clan_invites WHERE inviter={InviterPid} AND invitee={InviteePid}");
                // There is already an invite for this invitee
                if (results.Count > 0)
                    return false;
                bool result = cmd.ExecuteNonQuery() > 0;
                // Add alert
                ClanInvite inv = Get(InviterPid, InviteePid);
                Alert alert = new Alert
                {
                    InviteId = inv.Id,
                    InviteType = "C",
                    Inviter = InviterPid,
                    Invitee = InviteePid,
                    // Clan ID needed? Could be retrieved by the inviter's pid from a clan member table
                    TargetId = InviterPid
                };
                if (!Alert.Add(alert))
                    Log.WriteLine(1, $"[ClanInvite] Failed to add alert for clan invite {inv.Id}", Color.Red);
                return result;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanInvite] {e}", Color.Red);
                return false;
            }
        }

        public static EClanError Cancel(ClientInfo client, string invitee)
        {
            string sql = $"DELETE FROM clan_invites WHERE inviter='{client.Name}' AND invitee='{invitee}'";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.InvalidInvitation;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanInvite] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }

        public static EClanError Accept(ClientInfo client, uint clanId, string inviter)
        {
            // Delete the invitation
            string sql = $"DELETE FROM clan_invites WHERE invitee='{client.Name}' AND inviter='{inviter}' AND clan_id={clanId}";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            try
            {
                if(cmd.ExecuteNonQuery() == 0)
                    return EClanError.InvalidInvitation;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanInvite] {e}", Color.Red); 
                return EClanError.InvalidInvitation;
            }
            // Check membership
            sql = $"SELECT id FROM clan_members WHERE player_name='{client.Name}'";
            List<List<string>> lookup = SqliteHelper.GetQueryResults(sql);
            if(lookup.Count > 0)
            {
                // Add to clan members
                sql = $"INSERT INTO clan_members (clan_id, pid, player_name, is_officer) VALUES ({clanId},{client.PID},'{client.Name}',0);";
                cmd = new SQLiteCommand(sql);
                try
                {
                    return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.NoError;
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[ClanInvite] {e}", Color.Red);
                    return EClanError.UnspecifiedClanError;
                }
            }
            return EClanError.YouAreInAClan;
        }

        public static EClanError Reject(ClientInfo client, uint clanId, string inviter)
        {
            string sql = $"DELETE FROM clan_invites WHERE inviter='{inviter}' AND invitee='{client.Name}' AND clan_id={clanId}";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.InvalidInvitation;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanInvite] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }

        public static EClanError Block(ClientInfo client, uint clanId, string inviter)
        {
            string sql = $"UPDATE clan_invites SET blocked=1 WHERE inviter='{inviter}' AND invitee='{client.Name}' AND clan_id={clanId}";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.InvalidInvitation;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanInvite] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }
    }
}
