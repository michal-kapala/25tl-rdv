using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_ReportStats : RMCPRequest
    {
        public uint GathId { get; set; }
        public List<GatheringStats> GathStats { get; set; }

        public RMCPacketRequestMatchMakingService_ReportStats(Stream s)
        {
            GathStats = new List<GatheringStats>();
            GathId = Helper.ReadU32(s);
            uint count = Helper.ReadU32(s);
            for (uint i = 0; i < count; i++)
                GathStats.Add(new GatheringStats(s));
        }

        public override string ToString()
        {
            return $"[ReportStats Request gathId=0x{GathId:X2}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[GathId: 0x{GathId:X2}]");
            sb.AppendLine($"\t[Players: {GathStats.Count}]");
            foreach (GatheringStats s in GathStats)
                sb.AppendLine(s.ToString());
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GathId);
            Helper.WriteU32(m, (uint)GathStats.Count);
            foreach (GatheringStats gs in GathStats)
                gs.ToBuffer(m);
            return m.ToArray();
        }
    }
}
