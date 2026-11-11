using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_LaunchSession : RMCPRequest
    {
        public uint GatheringId { get; set; }
        public string SessionUrl { get; set; }

        public RMCPacketRequestMatchMakingService_LaunchSession(Stream s)
        {
            GatheringId = Helper.ReadU32(s);
            SessionUrl = Helper.ReadString(s);
        }
        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GatheringId);
            Helper.WriteString(m, SessionUrl);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[LaunchSession Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Gathering id : {GatheringId}]");
            sb.AppendLine($"\t[Session URL  : {SessionUrl}]");
            return sb.ToString();
        }

    }
}
