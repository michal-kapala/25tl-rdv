using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanBlockInvitation : RMCPRequest
    {
        public uint ClanId { get; set; }
        public string Inviter { get; set; }

        public RMCPacketRequestEidosService_ClanBlockInvitation(Stream s)
        {
            ClanId = Helper.ReadU32(s);
            Inviter = Helper.ReadStringWithAccents(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[ClanId: {ClanId}]");
            sb.AppendLine($"\t[Inviter: {Inviter}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, ClanId);
            Helper.WriteString(m, Inviter);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanBlockInvitation Request]";
        }
    }
}
