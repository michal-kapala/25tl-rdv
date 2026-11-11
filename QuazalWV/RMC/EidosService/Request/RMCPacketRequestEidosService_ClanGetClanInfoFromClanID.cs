using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanGetClanInfoFromClanID : RMCPRequest
    {
        public uint ClanId { get; set; }

        public RMCPacketRequestEidosService_ClanGetClanInfoFromClanID(Stream s)
        {
            ClanId = Helper.ReadU32(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[ClanId: {ClanId}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, ClanId);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanGetClanInfoFromClanID Request]";
        }
    }
}
