using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanGetMyNotices : RMCPResponse
    {
        public uint RetVal { get; set; }
        public List<EidosClanNotice> Notices { get; set; }

        public RMCPacketResponseEidosService_ClanGetMyNotices(ClientInfo client)
        {
            RetVal = (uint)EClanError.NoError;
            Notices = Clan.GetNotices(client.PID);
        }

        public override string ToString()
        {
            return "[ClanGetMyNotices Response]";
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
            Helper.WriteU32(m, (uint)Notices.Count);
            foreach (EidosClanNotice n in Notices)
                n.ToBuffer(m);
            return m.ToArray();
        }
    }
}
