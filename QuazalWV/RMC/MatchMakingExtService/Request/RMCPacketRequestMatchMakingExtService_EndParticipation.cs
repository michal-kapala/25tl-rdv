using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingExtService_EndParticipation : RMCPRequest
    {
        public uint GatheringId { get; set; }
        public string Message { get; set; }

        public RMCPacketRequestMatchMakingExtService_EndParticipation(Stream s)
        {
            GatheringId = Helper.ReadU32(s);
            Message = Helper.ReadString(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GatheringId);
            Helper.WriteString(m, Message);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[EndParticipation Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Gathering id : {GatheringId}]");
            sb.AppendLine($"\t[Message      : {Message}]");
            return sb.ToString();
        }

    }
}
