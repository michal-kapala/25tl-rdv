using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_GetStatsForPlayers : RMCPRequest
    {     
        public List<uint> Pids { get; set; }

        public RMCPacketRequestEidosService_GetStatsForPlayers(Stream s)
        {
            Pids = new List<uint>();
            uint count = Helper.ReadU32(s);
            for(uint i = 0; i < count; i++)
                Pids.Add(Helper.ReadU32(s));
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)Pids.Count);
            foreach(uint pid in Pids)
                Helper.WriteU32(m, pid);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetStatsForPlayers Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            uint count = 1;
            foreach (uint pid in Pids)
                sb.AppendLine($"\t[Player{count++}: {pid}]");
            return sb.ToString();
        }

    }
}
