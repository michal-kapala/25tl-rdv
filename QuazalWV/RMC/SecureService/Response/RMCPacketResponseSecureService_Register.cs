using System.IO;
using System.Linq;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseSecureService_Register : RMCPResponse
    {
        public uint ResultCode { get; set; }
        public uint ConnectionId { get; set; }
        public string ClientUrl { get; set; }

        public RMCPacketResponseSecureService_Register(ClientInfo client)
        {
            // Dont touch
            ResultCode = 0x10001;
            ConnectionId = 78;
            // All the urls have the ISP's endpoint set already
            StationUrl url = new StationUrl(client.Urls.First())
            {
                SID = 15,
                PID = client.PID,
                CID = 1,
                Stream = 3,
                Type = 2
            };
            ClientUrl = url.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, ResultCode);
            Helper.WriteU32(m, ConnectionId);
            Helper.WriteString(m, ClientUrl);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[Register Response]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t[Result Code   : 0x" + ResultCode.ToString("X8") + "]");
            sb.AppendLine("\t[Connection Id : 0x" + ConnectionId.ToString("X8") + "]");
            sb.AppendLine("\t[Client Url    : " + ClientUrl + "]");
            return sb.ToString();
        }
    }
}