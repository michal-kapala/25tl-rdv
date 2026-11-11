using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_ReportStats : RMCPResponse
    {
        public uint GathId { get; set; }
        public bool Result { get; set; }

        public RMCPacketResponseMatchMakingService_ReportStats(uint gathId, List<GatheringStats> stats)
        {
            Result = true;
            GathId = gathId;
            // Do sth with the stats (what are they even?)
        }

        public override string ToString()
        {
            return $"[ReportStats Response gathId=0x{GathId:X2}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[GathId: 0x{GathId:X2}]");
            sb.AppendLine($"\t[Result: {Result}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, Result);
            return m.ToArray();
        }
    }
}
