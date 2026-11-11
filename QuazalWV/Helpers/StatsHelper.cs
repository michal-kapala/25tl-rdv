using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;

namespace QuazalWV
{
    /// <summary>
    /// Helper methods for 'stats' SQLite table.
    /// </summary>
    public static class StatsHelper
    {
        /// <summary>
        /// Name of PS2 stats table.
        /// </summary>
        public const string Ps2Table = "stats_ps2";
        /// <summary>
        /// Name of PC stats table.
        /// </summary>
        public const string PcTable = "stats_pc";

        /// <summary>
        /// Checks if a player's stat record exists.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static bool Exists(ClientInfo client)
        {
            _ = new List<List<string>>();
            string table = client.PS2 ? Ps2Table : PcTable;
            string sql = $"SELECT pid FROM {table} WHERE pid={client.PID}";
            List<List<string>> result = SqliteHelper.GetQueryResults(sql);

            return result.Count > 0;
        }

        /// <summary>
        /// Creates default stats for a new player with ranking 1600.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static bool AddDefault(ClientInfo client) 
        {
            if (!Exists(client))
            {
                string sql = $"INSERT INTO {Ps2Table} (pid) VALUES ({client.PID})";
                SQLiteCommand cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
                try { 
                    int ps2Count = cmd.ExecuteNonQuery();
                    sql = $"INSERT INTO {PcTable} (pid) VALUES ({client.PID})";
                    cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
                    return cmd.ExecuteNonQuery() + ps2Count > 1;
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                    return false;
                }
            }
            Log.WriteLine(1, $"[StatsHelper] AddDefault: Player {client.PID} already has a stats record", Color.Gray);
            return false;
        }

        /// <summary>
        /// Gets player's current ranking.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static float GetRanking(ClientInfo client)
        {
            string table = client.PS2 ? Ps2Table : PcTable;
            string sql = $"SELECT ranking FROM {table} WHERE pid={client.PID}";
            try
            {
                List<List<string>> query = SqliteHelper.GetQueryResults(sql);
                if (query.Count > 0)
                    return Convert.ToSingle(query[0][0]);
                // Guest accounts (used on every login)
                else
                    return 0f;
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return 0f;
            }
        }

        /// <summary>
        /// Update player's ranking score.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="ranking"></param>
        /// <returns></returns>
        public static bool UpdateRanking(ClientInfo client)
        {
            string table = client.PS2 ? Ps2Table : PcTable;
            string sql = $"UPDATE {table} SET ranking={(uint)client.Rating} WHERE pid={client.PID}";
            SQLiteCommand cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
            try { return cmd.ExecuteNonQuery() > 0; }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Updates player's statistics after a round.
        /// </summary>
        /// <param name="roundStats"></param>
        /// <returns></returns>
        public static bool UpdateLifetimeStats(ClientInfo client, List<float> roundStats)
        {
            // Convert to uint list
            List<uint> stats = new List<uint>();
            foreach (float stat in roundStats)
                stats.Add((uint)stat);

            // Build the update sql
            string table = client.PS2 ? Ps2Table : PcTable;
            string sql = $"UPDATE {table} SET ";
            int index;
            string colName;
            // Doesnt update the ranking
            for (index = 1; index < 87; index++)
            {
                colName = Enum.GetName(typeof(EStats), index);
                if(index == 86)
                    sql += $"{colName} = {colName} + {stats[index]} ";
                else
                    sql += $"{colName} = {colName} + {stats[index]}, ";
            }
            sql += $"WHERE pid={client.PID}";

            SQLiteCommand cmd = new SQLiteCommand(sql, SqliteHelper.Connection);
            try { return cmd.ExecuteNonQuery() > 0; }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return false;
            }
        }

        /// <summary>
        /// Returns stat values in AddStatsPersonal's list format.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static List<float> GetValues(ClientInfo client)
        {
            string table = client.PS2 ? Ps2Table : PcTable;
            string sql = $"SELECT * FROM {table} WHERE pid={client.PID}";
            List<float> result = new List<float>();
            List<List<string>> query = SqliteHelper.GetQueryResults(sql);
            try
            {
                List<string> values = query[0];
                for (int index = 2; index < 89; index++)
                    result.Add(Convert.ToSingle(values[index]));
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Maps db stat values to TTLStats.Values list.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static LadderStats Get(ClientInfo client)
        {
            List<float> values = GetValues(client);
            float ranking = values[0];
            int rank = GetRank(client, (uint)ranking);
            LadderStats stats = new LadderStats()
            {
                Ranking = ranking,
                Stat2 = 1f,
                Stat3 = 1f,
                RoundsWonLaw = values[1],
                // TODO: session count to be added from 'participants' table, the sum cannot be 0
                SessionsLaw = 1f,
                Stat6 = 1f,
                RoundsWonCriminal = values[4],
                // TODO: session count to be added from 'participants' table, the sum cannot be 0
                SessionsCriminal = 0f,
                Stat9 = 1f,
                KillsLaw = values[7],
                DeathsLaw = values[9],
                KillsCriminal = values[8],
                DeathsCriminal = values[10],
                // MVPs are not in the stats, should be added to the db by the server
                MvpsLaw = 0f,
                MvpsCriminal = 0f,
                Rank = rank,
                Stat17 = 1f,
                RoundsLostLaw = values[2],
                Stat19 = 1f,
                Stat20 = 1f,
                RoundsLostCriminal = values[5],
                Stat22 = 1f,
                Stat23 = 1f,
                Stat24 = 1f,
                // Duplicates of DeathsLaw and DeathsCriminal
                DeathsLawKdRatio = values[9],
                DeathsCriminalKdRatio = values[10],
                SuicidesLaw = values[11],
                SuicidesCriminal = values[12],
                HeadshotsLaw = values[13],
                HeadshotsCriminal = values[14],
                TeamKillsLaw = values[15],
                TeamKillsCriminal = values[16],
                TeamKilledLaw = values[17],
                TeamKilledCriminal = values[18],
                NpcKillsLaw = values[19],
                NpcKillsCriminal = values[20],
                ApprehensionsLaw = values[21],
                ApprehendedCriminal = values[22],
                ShotsFiredLaw = values[23],
                ShotsFiredCriminal = values[24],
                ShotsHitLaw = values[25],
                ShotsHitCriminal = values[26],
                Stat43 = 1f,
                Stat44 = 1f,
                RobberyEscapes = values[29],
                RaidCaptures = values[30],
                Tags = values[31]
            };
            return stats;
        }

        /// <summary>
        /// Calculates player's new ranking score for a round.
        /// </summary>
        /// <param name="oldRanking"></param>
        /// <param name="roundStats"></param>
        /// <param name="gathId"></param>
        /// <returns></returns>
        public static float NewRanking(ClientInfo player, Round round)
        {
            // Get old ranking score (cached on login)
            float ranking = player.Rating;
            // Get list of opponents at the end of the round
            List<ClientInfo> opponents = round.RankedParticipants;
            // Get number of players
            uint nbPlayers = (uint)opponents.Count;
            // Filter for opponents
            switch(GetTeam(player.RoundStats))
            {
                case "law":
                    // Get opponents - criminals
                    opponents = opponents.FindAll(p => GetTeam(p.RoundStats) == "criminals");
                    break;
                case "criminals":
                    // Get opponents - law enforcement
                    opponents = opponents.FindAll(p => GetTeam(p.RoundStats) == "law");
                    break;
            }
            float rankingChange = RankingChange(player, opponents, nbPlayers);
            return ranking + rankingChange;
        }

        /// <summary>
        /// Calculcates the change in player's ranking after a round.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="opponents"></param>
        /// <see href="https://web.archive.org/web/20071223122600/http://stats.25tolife.com/faq/#qprating2"/>
        /// <returns></returns>
        public static float RankingChange(ClientInfo player, List<ClientInfo> opponents, uint nbPlayers)
        {
            float result = 16f + nbPlayers;
            foreach(ClientInfo opp in opponents)
                result *= WdFactor(player, opp);

            return result;
        }

        /// <summary>
        /// Calculates Wd factor for a player-opponent pair.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="opponent"></param>
        /// <returns></returns>
        /// <see href="https://web.archive.org/web/20071223122600/http://stats.25tolife.com/faq/#qprating2"/>
        public static float WdFactor(ClientInfo player, ClientInfo opponent)
        {
            double result;
            // Calculate MVP scores
            float playerMvpScore = MvpScore(player.RoundStats);
            float oppMvpScore = MvpScore(opponent.RoundStats);
            // Add W variable
            if (playerMvpScore > oppMvpScore) result = 1;
            else if (playerMvpScore == oppMvpScore) result = 0.5f;
            else result = 0;
            // Calculate We variable.
            float weExponent = (opponent.Rating - player.Rating) / 400f;
            double we = 1f/(Math.Pow(10, weExponent) + 1);
            result -= we;
            return Convert.ToSingle(result);
        }
        /// <summary>
        /// Calculates player's MVP score for a round.
        /// </summary>
        /// <returns></returns>
        /// <see href="https://web.archive.org/web/20071223122600/http://stats.25tolife.com/faq/#qprating%22/%3E
        public static float MvpScore(List<float> roundStats)
        {
            float kills = roundStats[7] + roundStats[8];
            // Team kills may be wrong, need testing
            float teamKills = roundStats[15] + roundStats[16];
            float deaths = roundStats[9] + roundStats[10];
            float suicides = roundStats[11] + roundStats[12];
            float npcKillsLaw = roundStats[19];
            float apprehensions = roundStats[21];
            float tags = roundStats[31];
            float lootCaptures = roundStats[29] + roundStats[30];

            return (kills - teamKills) * 2 - deaths - suicides - npcKillsLaw + 3 * apprehensions + 2 * tags + 5 * lootCaptures;
        }

        /// <summary>
        /// Returns the player's team off round stats.
        /// </summary>
        /// <param name="roundStats"></param>
        /// <returns></returns>
        public static string GetTeam(List<float> roundStats)
        {
            // Vitories + defeats + draws as law enforcement
            float sum = roundStats[1] + roundStats[2] + roundStats[3];
            return sum > 0 ? "law" : "criminals";
        }

        /// <summary>
        /// Returns the list for GetTopPlayers method (Eidos Service 0x5).
        /// </summary>
        /// <param name="nbPlayers"></param>
        /// <returns></returns>
        public static List<TTLStats> GetTopPlayers(ClientInfo client, uint nbPlayers = 4)
        {
            List<TTLStats> topStats = new List<TTLStats>();
            try
            {
                string table = client.PS2 ? Ps2Table : PcTable;
                // TODO: join to be replaced
                string sql = $"SELECT users.pid, username FROM users INNER JOIN {table} ON users.pid = {table}.pid ORDER BY ranking DESC LIMIT {nbPlayers}";
                List<List<string>> queryResult = SqliteHelper.GetQueryResults(sql);
                ClientInfo player = new ClientInfo();
                foreach(List<string> entry in queryResult)
                {
                    player.PID = Convert.ToUInt32(entry[0]);
                    player.Name = entry[1];
                    topStats.Add(new TTLStats(player));
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return null;
            }
            return topStats;
        }

        /// <summary>
        /// Returns player's no-draws rank by ranking.
        /// </summary>
        /// <param name="ranking"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int NoDrawsRank(ClientInfo client, uint ranking)
        {
            _ = new List<List<string>>();
            List<List<string>> rankQuery;
            try
            {
                string table = client.PS2 ? Ps2Table : PcTable;
                string sql = $"SELECT pid, ranking FROM {table} WHERE ranking >= {ranking} ORDER BY ranking DESC";
                rankQuery = SqliteHelper.GetQueryResults(sql);
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return -1;
            }
            // Filter out the preceding rank draws
            int drawOffset = 0, aboveCount = 0;
            List<uint> drawPids = new List<uint>();
            uint ePid, eRanking;
            foreach (List<string> entry in rankQuery)
            {
                ePid = Convert.ToUInt32(entry[0]);
                eRanking = Convert.ToUInt32(entry[1]);
                // Add a draw (includes self)
                if (eRanking == ranking)
                    drawPids.Add(ePid);
                if (eRanking > ranking)
                    aboveCount++;
            }
            // Add the draws with players higher in the ladder (currently account creation chronological precedence)
            while(drawPids[0] != client.PID)
            {
                drawPids.RemoveAt(0);
                ++drawOffset;
            }

            return aboveCount + drawOffset + 1;
        }

        /// <summary>
        /// Returns player's rank by ranking.
        /// </summary>
        /// <param name="ranking"></param>
        /// <returns></returns>
        public static int GetRank(ClientInfo client, uint ranking)
        {
            _ = new List<List<string>>();
            List<List<string>> rankQuery;
            try
            {
                string table = client.PS2 ? Ps2Table : PcTable;
                string sql = $"SELECT pid FROM {table} WHERE ranking > {ranking}";
                rankQuery = SqliteHelper.GetQueryResults(sql);
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return -1;
            }
            return rankQuery.Count + 1;
        }

        /// <summary>
        /// Returns the ladder part pivotted by the requesting player, they appear as 5th unless they're in the top 5.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="nbPlayers"></param>
        /// <returns></returns>
        public static List<TTLStats> GetLadderPart(ClientInfo client, uint nbPlayers = 4)
        {
            // Displays the top 4 - uncompressed payload size limitation
            List<TTLStats> ladderPart = new List<TTLStats>();
            int noDrawRank = NoDrawsRank(client, (uint)client.Rating);
            if(noDrawRank <= 2 && noDrawRank != -1)
                return GetTopPlayers(client, nbPlayers);
            // Display the current player as 2nd
            int offset = noDrawRank - 2;
            // TODO: join to be replaced
            string table = client.PS2 ? Ps2Table : PcTable;
            string sql = $"SELECT users.pid, username FROM users INNER JOIN {table} ON users.pid = {table}.pid ORDER BY ranking DESC LIMIT {offset},{nbPlayers}";
            try
            {
                List<List<string>> queryResult = SqliteHelper.GetQueryResults(sql);
                ClientInfo player = new ClientInfo();
                foreach (List<string> entry in queryResult)
                {
                    player.PID = Convert.ToUInt32(entry[0]);
                    player.Name = entry[1];
                    ladderPart.Add(new TTLStats(player));
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return null;
            }
            return ladderPart;
        }

        /// <summary>
        /// Returns stats for the requested players.
        /// </summary>
        /// <param name="pids"></param>
        /// <returns></returns>
        public static List<TTLStats> GetByPids(ClientInfo client, List<uint> pids)
        {
            List<TTLStats> stats = new List<TTLStats>();
            // Make SQL query
            uint firstPid = pids[0];
            string sql = $"SELECT pid, username FROM users WHERE pid={firstPid} ";
            pids.Remove(firstPid);
            foreach (uint pid in pids)
                sql += $"OR pid={pid} ";
            try
            {
                List<List<string>> queryResult = SqliteHelper.GetQueryResults(sql);
                ClientInfo player;
                foreach (List<string> entry in queryResult)
                {
                    player = new ClientInfo()
                    {
                        PID = Convert.ToUInt32(entry[0]),
                        Name = entry[1],
                        PS2 = client.PS2
                    };
                    stats.Add(new TTLStats(player));
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(1, $"[StatsHelper] {e}", Color.Red);
                return null;
            }
            return stats;
        }
    }
}
