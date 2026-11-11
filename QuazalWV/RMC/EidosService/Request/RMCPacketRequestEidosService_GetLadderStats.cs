using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_GetLadderStats : RMCPRequest
    {
        public uint PivotPlayer { get; set; }
        public uint Count { get; set; }

        public RMCPacketRequestEidosService_GetLadderStats(Stream s)
        {
            PivotPlayer = Helper.ReadU32(s);
            Count = Helper.ReadU32(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, PivotPlayer);
            Helper.WriteU32(m, Count);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetLadderStats Request, count={Count}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[PivotPlayer : {PivotPlayer}]");
            sb.AppendLine($"\t[Count       : {Count}]");
            return sb.ToString();
        }

    }
}
