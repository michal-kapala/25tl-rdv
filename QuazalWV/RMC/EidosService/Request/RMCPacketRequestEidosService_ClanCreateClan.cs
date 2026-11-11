using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanCreateClan : RMCPRequest
    {
        public string ClanName { get; set; }
        public string ClanAbbr { get; set; }

        public RMCPacketRequestEidosService_ClanCreateClan(Stream s)
        {
            ClanName = Helper.ReadStringWithAccents(s);
            ClanAbbr = Helper.ReadString(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Clan name: {ClanName}]");
            sb.AppendLine($"\t[Clan abbr: {ClanAbbr}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, ClanName);
            Helper.WriteString(m, ClanAbbr);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[ClanCreateClan Request clan={ClanAbbr}]";
        }
    }
}
