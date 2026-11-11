using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_GetParticipantsURLs : RMCPRequest
    {
        public uint GathId { get; set; }

        public RMCPacketRequestMatchMakingService_GetParticipantsURLs(Stream s)
        {
            GathId = Helper.ReadU32(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GathId);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[GetParticipantsURLs Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[GathId: 0x{GathId:X2}]");
            return sb.ToString();
        }
        
    }
}
