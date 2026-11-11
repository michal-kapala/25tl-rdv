using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_GetStatsForPlayers : RMCPResponse
    {
        public List<TTLStats> GameStats { get; set; }

        public RMCPacketResponseEidosService_GetStatsForPlayers(ClientInfo client, List<uint> pids)
        {
            GameStats = StatsHelper.GetByPids(client, pids);
            // Limit the stats to ranking and rank if there are more than 4 friends
            if(GameStats.Count > 4)
            {
                for(int index = 0; index < GameStats.Count; index++)
                {
                    for (int statIndex = 16; statIndex < GameStats[index].Values.Count; statIndex++)
                    {
                        GameStats[index].Values.RemoveAt(statIndex);
                    }
                }
            }
            // Order seems to be ignored by the game
            //GameStats = GameStats.OrderByDescending(stats => stats.Values[0]).ToList();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)GameStats.Count);
            foreach (TTLStats s in GameStats)
                s.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetStatsForPlayers Response, count={GameStats.Count}]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
