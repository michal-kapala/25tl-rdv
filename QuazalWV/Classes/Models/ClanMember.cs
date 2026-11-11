using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    public class ClanMember : SqliteModel
    {
        public uint ClanId { get; set; }
        public uint Pid { get; set; }
        public string PlayerName { get; set; }
        public bool IsOfficer { get; set; }

        public static EidosClanMember GetEidosClanMember(uint pid)
        {
            List<List<string>> clanMembers = SqliteHelper.GetQueryResults($"SELECT * FROM clan_members INNER JOIN clans ON clan_members.clan_id = clans.id WHERE clan_members.pid={pid}");
            if(clanMembers.Count == 0)
            {
                Log.WriteLine(2, $"[ClanMember] Player {pid} doesn't exist", Color.Orange);
                return null;
            }
            List<string> member = clanMembers[0];
            bool online = Global.Clients.Find(c => c.PID == pid) != null;
            EidosClanMember ecm = new EidosClanMember(member[3], Convert.ToUInt32(member[2]))
            {
                ClanIsOfficer = Convert.ToUInt32(member[4]) > 0,
                ClanAbbr = member[6],
                ClanIsLeader = member[3] == member[8],
                IsOnline = online,
            };

            return ecm;
        }

        /// <summary>
        /// Adds a clan member.
        /// </summary>
        /// <returns></returns>
        public bool Add()
        {
            SQLiteCommand cmd = new SQLiteCommand(
                   $"INSERT INTO clan_members (clan_id,pid,player_name,is_officer) " +
                   $"VALUES ({ClanId},{Pid},'{PlayerName}',{(IsOfficer ? 1 : 0)});",
               SqliteHelper.Connection);
            try
            {
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanMember] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Returns all clan ids of the player (should be only 1).
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<uint> GetClanIds(uint pid)
        {
            List<uint> ids = new List<uint>();
            List<List<string>> results = SqliteHelper.GetQueryResults($"SELECT clan_id FROM clan_members WHERE pid={pid}");
            foreach(List<string> member in results)
                ids.Add(Convert.ToUInt32(member[1]));
            return ids;
        }

        /// <summary>
        /// Removes a different player from the clan.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static EClanError Remove(ClientInfo client, uint pid)
        {
            // get the clan and check the leadership
            List<List<string>> requesterClan = SqliteHelper.GetQueryResults($"SELECT * FROM clans WHERE leader='{client.Name}'");
            if(requesterClan.Count > 0)
            {
                uint clanId = Convert.ToUInt32(requesterClan[0][0]);
                string sql = $"DELETE FROM clan_members WHERE pid={pid} AND clan_id={clanId}";
                SQLiteCommand cmd = new SQLiteCommand(sql);
                try
                {
                    return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.PlayerNotInClan;
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[ClanMember] {e}", Color.Red);
                    return EClanError.UnspecifiedClanError;
                }
            }
            return EClanError.NoPermission;
        }

        /// <summary>
        /// Removes the requesting player from the clan.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static EClanError Leave(ClientInfo client)
        {
            string sql = $"DELETE FROM clan_members WHERE pid={client.PID}";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.PlayerNotInClan;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanMember] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }

        public static EClanError Promote(uint pid)
        {
            string sql = $"UPDATE clan_members SET is_officer=1 WHERE pid={pid}";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.PlayerNotInClan;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanMember] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }

        public static EClanError Demote(uint pid)
        {
            string sql = $"UPDATE clan_members SET is_officer=0 WHERE pid={pid}";
            SQLiteCommand cmd = new SQLiteCommand(sql);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.PlayerNotInClan;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[ClanMember] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }

        public static uint ClanIdFromPid(uint pid)
        {
            List<List<string>> members = SqliteHelper.GetQueryResults($"SELECT clan_id FROM clan_members WHERE pid={pid}");
            if (members.Count == 0)
            {
                Log.WriteLine(2, $"[ClanMember] Player {pid} has no clan", Color.Orange);
                return 0;
            }
            return Convert.ToUInt32(members[0][0]);
        }

        public static uint ClanIdFromName(string name)
        {
            List<List<string>> members = SqliteHelper.GetQueryResults($"SELECT clan_id FROM clan_members WHERE player_name='{name}'");
            if (members.Count == 0)
            {
                Log.WriteLine(2, $"[ClanMember] Player {name} has no clan", Color.Orange);
                return 0;
            }
            return Convert.ToUInt32(members[0][0]);
        }
    }
}
