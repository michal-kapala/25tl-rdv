using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestSecureService_RequestUrls : RMCPRequest
    {
        public uint CID { get; set; }
        public uint PID { get; set; }

        public RMCPacketRequestSecureService_RequestUrls(Stream s)
        {
            CID = Helper.ReadU32(s);
            PID = Helper.ReadU32(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[CID: {CID}]");
            sb.AppendLine($"\t[PID: {PID}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, CID);
            Helper.WriteU32(m, PID);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[RequestUrls Request]";
        }
    }
}
