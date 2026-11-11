using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanGetClanIDListForPlayer : RMCPResponse
    {
        public uint RetVal { get; set; }
        public List<uint> ClanIds { get; set; }

        public RMCPacketResponseEidosService_ClanGetClanIDListForPlayer(uint pid)
        {
            RetVal = (uint)EClanError.NoError;
            ClanIds = ClanMember.GetClanIds(pid);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Clan count: {ClanIds.Count}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, RetVal);
            Helper.WriteU32(m, (uint)ClanIds.Count);
            foreach(uint id in ClanIds)
                Helper.WriteU32(m, id);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanGetClanIDListForPlayer Response]";
        }
    }
}
