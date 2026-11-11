using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanDemote : RMCPRequest
    {
        public uint Pid { get; set; }

        public RMCPacketRequestEidosService_ClanDemote(Stream s)
        {
            Pid = Helper.ReadU32(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Demoted player: {Pid}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Pid);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanDemote Request]";
        }
    }
}
