using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanCancelInvitation : RMCPRequest
    {
        public string PlayerToUninvite { get; set; }
        
        public RMCPacketRequestEidosService_ClanCancelInvitation(Stream s)
        {
            PlayerToUninvite = Helper.ReadStringWithAccents(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Cancelled player: {PlayerToUninvite}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, PlayerToUninvite);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanCancelInvitation Request]";
        }
    }
}
