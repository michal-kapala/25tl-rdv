using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_CancelParticipation : RMCPRequest
    {
        public uint GathId { get; set; }
        public string Message { get; set; }

        public RMCPacketRequestMatchMakingService_CancelParticipation(Stream s)
        {
            GathId = Helper.ReadU32(s);
            Message = Helper.ReadString(s);
        }

        public override string ToString()
        {
            return $"[CancelParticipation Request gathId=0x{GathId:X2}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[GathId: 0x{GathId:X2}]");
            sb.AppendLine($"\t[Message: {Message}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GathId);
            Helper.WriteString(m, Message);
            return m.ToArray();
        }
    }
}
