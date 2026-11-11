using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    /// <summary>
    /// Model for 'match_invites' DB table.
    /// </summary>
    public class MatchInvite : SqliteModel
    {
        public uint GathId { get; set; }
        public uint Invitee { get; set; }
        public uint Inviter { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Gets the match invitation sent from the inviter to the invitee.
        /// </summary>
        /// <param name="inviterPid"></param>
        /// <param name="inviteePid"></param>
        /// <returns></returns>
        public static MatchInvite Get(uint inviterPid, uint inviteePid)
        {
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT * FROM match_invites WHERE inviter={inviterPid} AND invitee={inviteePid}");
                MatchInvite invite = new MatchInvite
                {
                    Id = Convert.ToUInt32(results[0][0]),
                    GathId = Convert.ToUInt32(results[0][1]),
                    Invitee = Convert.ToUInt32(results[0][2]),
                    Inviter = Convert.ToUInt32(results[0][3]),
                    Message = results[0][4]
                };
                return invite;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return null;
            }
        }

        /// <summary>
        /// Adds a list of match invitations to SQLite database.
        /// </summary>
        /// <param name="invites"></param>
        /// <returns></returns>
        public static bool AddMultiple(List<MatchInvite> invites)
        {
            int opCount = 0;
            int rowsAff;
            foreach (MatchInvite inv in invites)
            {
                // TODO: Invitation dates to be added in future to allow invitation timeouts (sqlite doesnt support a date type)
                SQLiteCommand cmd = new SQLiteCommand(
                    $"INSERT INTO match_invites (gath_id, invitee, inviter, message) " +
                    $"VALUES ({inv.GathId},{inv.Invitee},{inv.Inviter},'{inv.Message}');",
                SqliteHelper.Connection);
                try
                {
                    rowsAff = cmd.ExecuteNonQuery();
                    if (rowsAff > 0)
                    {
                        // Add alert
                        MatchInvite mi = Get(inv.Inviter, inv.Invitee);
                        Alert alert = new Alert
                        {
                            InviteId = mi.Id,
                            InviteType = "M",
                            Inviter = mi.Inviter,
                            Invitee = mi.Invitee,
                            // Gathering id or the inviter?
                            TargetId = mi.GathId
                        };
                        if (!Alert.Add(alert))
                            Log.WriteLine(2, $"[DBHelper] AddMatchInvites: failed to add alert for match invite {mi.Id}");
                        opCount += rowsAff;
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                    return false;
                }
            }
            return opCount > 0;
        }

    }
}
