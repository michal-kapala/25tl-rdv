using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    public class Clan : SqliteModel
    {
        public string Abbr { get; set; }
        public string Name { get; set; }
        public string Leader { get; set; }
        public uint Rating { get; set; }
        public uint Kills { get; set; }
        public string Notice { get; set; }

        public Clan()
        {

        }

        /// <summary>
        /// Gets clan by its ID.
        /// </summary>
        /// <param name="clanId"></param>
        public Clan(uint clanId)
        {
            List<List<string>> clan = SqliteHelper.GetQueryResults($"SELECT * FROM clans WHERE id={clanId}");
            Id = clanId;
            Abbr = clan[0][1];
            Name = clan[0][2];
            Leader = clan[0][3];
            Rating = Convert.ToUInt32(clan[0][4]);
            // TODO: add and count kills properly
            Kills = 0;
            Notice = clan[0][5] ?? "";
        }

        /// <summary>
        /// Add the clan and its owner to 'clan_members' table.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public EClanError Add(ClientInfo client)
        {
            List<List<string>> clans = SqliteHelper.GetQueryResults($"SELECT * FROM clans WHERE abbr='{Abbr}' OR name='{Name}' OR leader='{Leader}'");
            if(clans.Count > 0)
            {
                if (clans[0][1] == Abbr)
                    return EClanError.ClanAbbrTaken;
                if (clans[0][2] == Name)
                    return EClanError.ClanNameTaken;
                return EClanError.YouAreInAClan;
            }
            // Save the clan to db
            SQLiteCommand cmd = new SQLiteCommand(
                   $"INSERT INTO clans (abbr,name,leader) " +
                   $"VALUES ('{Abbr}','{Name}','{Leader}');",
               SqliteHelper.Connection);
            try { 
                bool result = cmd.ExecuteNonQuery() > 0;
                if (!result)
                    return EClanError.UnspecifiedClanError;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[Clan] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
            // Get the created clan
            clans = SqliteHelper.GetQueryResults($"SELECT * FROM clans WHERE abbr='{Abbr}'");
            ClanMember member = new ClanMember()
            {
                ClanId = Convert.ToUInt32(clans[0][0]),
                Pid = client.PID,
                PlayerName = client.Name,
                IsOfficer = true
            };
            if(member.Add())
                return EClanError.NoError;
            return EClanError.UnspecifiedClanError;

        }

        /// <summary>
        /// Builds an EidosClanProps object.
        /// </summary>
        /// <returns></returns>
        public EidosClanProps ToProps()
        {
            List<List<string>> higherRated = SqliteHelper.GetQueryResults($"SELECT COUNT(*) FROM clans WHERE rating > {Rating}");
            List<List<string>> memberCount = SqliteHelper.GetQueryResults($"SELECT COUNT(*) FROM clan_members WHERE clan_id={Id}");

            return new EidosClanProps()
            {
                ClanID = Id,
                ClanAbbr = Abbr,
                ClanName = Name,
                ClanRank = Convert.ToUInt32(higherRated[0][0]) + 1,
                ClanRating = Rating,
                ClanKills = Kills,
                ClanMemberCount = Convert.ToUInt32(memberCount[0][0]),
                ClanLeader = Leader
            };
        }

        /// <summary>
        /// Updates the clan's leader.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static EClanError UpdateLeader(uint pid)
        {
            string sql = $"SELECT clan_id, player_name FROM clan_members WHERE pid={pid}";
            List<List<string>> requester = SqliteHelper.GetQueryResults(sql);
            if(requester.Count > 0)
            {
                uint clanId = Convert.ToUInt32(requester[0][0]);
                sql = $"UPDATE clans SET leader='{requester[0][1]}' WHERE id={clanId}";
                SQLiteCommand cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
                try
                {
                    return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.InvalidClan;
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[Clan] {e}", Color.Red);
                    return EClanError.UnspecifiedClanError;
                }
            }
            return EClanError.PlayerNotInClan;
        }

        public static EClanError UpdateNotice(string player, string notice)
        {
            string sql = $"UPDATE clans SET notice='{notice}' WHERE leader='{player}'";
            SQLiteCommand cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
            try
            {
                return cmd.ExecuteNonQuery() > 0 ? EClanError.NoError : EClanError.InvalidClan;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[Clan] {e}", Color.Red);
                return EClanError.UnspecifiedClanError;
            }
        }

        /// <summary>
        /// Looks the player's clan up and return a notice list (obsolete, max 1 notice can be returned).
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static List<EidosClanNotice> GetNotices(uint pid)
        {
            List<EidosClanNotice> notices = new List<EidosClanNotice>();
            string sql = $"SELECT clan_id FROM clan_members WHERE pid={pid}";
            List<List<string>> result = SqliteHelper.GetQueryResults(sql);
            // If a member
            if(result.Count > 0)
            {
                uint clanId = Convert.ToUInt32(result[0][0]);
                sql = $"SELECT leader, notice FROM clans WHERE id={clanId}";
                result = SqliteHelper.GetQueryResults(sql);
                if (result[0][0] != null && result[0][1] != null)
                {
                    notices.Add(new EidosClanNotice()
                    {
                        From = result[0][0],
                        Body = result[0][1]
                    });
                }
            }
            return notices;
        }

    }
}
