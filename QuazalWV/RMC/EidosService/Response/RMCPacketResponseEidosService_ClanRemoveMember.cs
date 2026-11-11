using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanRemoveMember : RMCPResponse
    {
        public uint RetVal { get; set; }

        public RMCPacketResponseEidosService_ClanRemoveMember(ClientInfo client, uint pid)
        {
            RetVal = (uint)ClanMember.Remove(client, pid);
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
            return "[ClanRemoveMember Response]";
        }
    }
}
