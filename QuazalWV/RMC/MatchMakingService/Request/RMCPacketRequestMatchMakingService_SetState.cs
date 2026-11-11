using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_SetState : RMCPRequest
    {
        public uint GathId { get; set; }
        public uint State { get; set; }

        public RMCPacketRequestMatchMakingService_SetState(Stream s)
        {
            GathId = Helper.ReadU32(s);
            State = Helper.ReadU32(s);
        }

        public override string ToString()
        {
            return $"[SetState Request gathId=0x{GathId:X2} state={State}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[GathId: 0x{GathId:X2}]");
            sb.AppendLine($"\t[State: {State}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GathId);
            Helper.WriteU32(m, State);
            return m.ToArray();
        }
    }
}
