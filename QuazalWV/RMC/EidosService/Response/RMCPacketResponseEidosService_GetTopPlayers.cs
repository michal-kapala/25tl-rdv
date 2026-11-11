using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_GetTopPlayers : RMCPResponse
    {
        public List<TTLStats> GameStats { get; set; }

        public RMCPacketResponseEidosService_GetTopPlayers(ClientInfo client, uint nbEntries)
        {
            // Number of entries limited to 4 due to uncompressed payload size limitation.
            GameStats = StatsHelper.GetTopPlayers(client);
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
            return $"[GetTopPlayers Response, count={GameStats.Count}]";
        }

        public override string PayloadToString()
        {
            return "";
        }

        
    }
}
