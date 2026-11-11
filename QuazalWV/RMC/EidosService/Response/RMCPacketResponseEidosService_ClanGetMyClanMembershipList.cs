using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanGetMyClanMembershipList : RMCPResponse
    {
        public uint RetVal { get; set; }
        public List<EidosClanMember> Members { get; set; }

        public RMCPacketResponseEidosService_ClanGetMyClanMembershipList()
        {
            RetVal = (uint)EClanError.NoError;
            Members = new List<EidosClanMember>();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, RetVal);
            Helper.WriteU32(m, (uint)Members.Count);
            foreach (EidosClanMember cm in Members)
                cm.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[ClanGetMyClanMembershipList Response count={Members.Count}]";
        }

        public override string PayloadToString()
        {
            return "";
        }
        
    }
}
