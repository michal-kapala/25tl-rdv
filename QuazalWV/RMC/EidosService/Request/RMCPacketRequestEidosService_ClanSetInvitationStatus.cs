using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanSetInvitationStatus : RMCPRequest
    {
        public bool IsAcceptingInvites { get; set; }

        public RMCPacketRequestEidosService_ClanSetInvitationStatus(Stream s)
        {
            IsAcceptingInvites = Helper.ReadBool(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Accepting: {IsAcceptingInvites}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, IsAcceptingInvites);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanSetInvitationStatus Request]";
        }
    }
}
