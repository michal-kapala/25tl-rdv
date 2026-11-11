using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    /// <summary>
    /// Model for 'alerts' DB table.
    /// </summary>
    public class Alert : SqliteModel
    {
        /// <summary>
        /// Sqlite foreign key of the associated invitation.
        /// </summary>
        public uint InviteId { get; set; }
        /// <summary>
        /// Invitation table identifier (C/F/M for Clan/Friend/Match).
        /// </summary>
        public string InviteType { get; set; }
        /// <summary>
        /// Inviter's PID.
        /// </summary>
        public uint Inviter { get; set; }
        /// <summary>
        /// Invitee's PID.
        /// </summary>
        public uint Invitee { get; set; }
        /// <summary>
        /// The ID of the clan/friend/match that ends up in EidosAlerts lists.
        /// </summary>
        public uint TargetId { get; set; }

        /// <summary>
        /// Adds an alert to SQLite database.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool Add(Alert a)
        {
            SQLiteCommand cmd = new SQLiteCommand(
                   $"INSERT INTO alerts (invite_id,invite_type,inviter,invitee,target_id) " +
                   $"VALUES ({a.InviteId},'{a.InviteType}',{a.Inviter},{a.Invitee},{a.TargetId});",
               SqliteHelper.Connection);
            try { return cmd.ExecuteNonQuery() > 0; }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Gets all of player's alerts, optionally filtered by type (C/F/M).
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Alert> GetAllOfType(uint pid, string type = "all")
        {
            List<Alert> alerts = new List<Alert>();
            string sql = type == "all"
                ? $"SELECT * FROM alerts WHERE invitee={pid}"
                : $"SELECT * FROM alerts WHERE invitee={pid} AND invite_type='{type}'";
            try
            {
                List<List<string>> results = SqliteHelper.GetQueryResults(sql);
                Alert alert;
                foreach (List<string> entry in results)
                {
                    alert = new Alert
                    {
                        Id = Convert.ToUInt32(entry[0]),
                        InviteId = Convert.ToUInt32(entry[1]),
                        InviteType = type,
                        Inviter = Convert.ToUInt32(entry[3]),
                        Invitee = pid,
                        TargetId = Convert.ToUInt32(entry[5])
                    };
                    alerts.Add(alert);
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return null;
            }
            return alerts;
        }

        /// <summary>
        /// Clears all invitation alerts for a player.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static bool DeleteAll(uint pid)
        {
            SQLiteCommand cmd = new SQLiteCommand($"DELETE FROM alerts WHERE invitee={pid}", SqliteHelper.Connection);
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[DBHelper] {e}", Color.Red);
                return false;
            }
        }
    }
}
