using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanPostNotice : RMCPRequest
    {
        public string NewNotice { get; set; }

        public RMCPacketRequestEidosService_ClanPostNotice(Stream s)
        {
            NewNotice = Helper.ReadString(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Notice: {NewNotice}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, NewNotice);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanPostNotice Request]";
        }
    }
}
