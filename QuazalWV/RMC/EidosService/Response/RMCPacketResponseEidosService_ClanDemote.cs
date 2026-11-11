using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanDemote : RMCPResponse
    {
        public uint RetVal { get; set; }

        public RMCPacketResponseEidosService_ClanDemote(uint pid)
        {
            RetVal = (uint)ClanMember.Demote(pid);
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
            return "[ClanDemote Response]";
        }
    }
}
