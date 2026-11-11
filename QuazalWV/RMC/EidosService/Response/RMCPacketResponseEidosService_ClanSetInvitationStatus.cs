using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanSetInvitationStatus : RMCPResponse
    {
        public uint RetVal { get; set; }

        public RMCPacketResponseEidosService_ClanSetInvitationStatus(ClientInfo client, bool accepting)
        {
            RetVal = (uint)User.SetAcceptClanInvites(client, accepting);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[RetVal: {(EClanError)RetVal}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, RetVal);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanSetInvitationStatus Response]";
        }
    }
}
