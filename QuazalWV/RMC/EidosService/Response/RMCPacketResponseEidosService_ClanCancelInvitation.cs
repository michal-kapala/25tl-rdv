using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanCancelInvitation : RMCPResponse
    {
        public uint RetVal { get; set; }

        public RMCPacketResponseEidosService_ClanCancelInvitation(ClientInfo client, string player)
        {
            RetVal = (uint)ClanInvite.Cancel(client, player);
        }

        public override string PayloadToString()
        {
            return "";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, RetVal);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanCancelInvitation Response]";
        }
    }
}
