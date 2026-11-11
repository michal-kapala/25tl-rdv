using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseSecureService_RequestUrls : RMCPResponse
    {
        public bool Result { get; set; }
        public List<string> Urls { get; set; }

        public RMCPacketResponseSecureService_RequestUrls(uint pid)
        {
            Urls = new List<string>();
            Result = true;
            // Find session owner's URLs
            ClientInfo owner = Global.Clients.Find(c => c.PID == pid);
            if (owner.Urls.Count == 0)
                Log.WriteLine(1, $"[RMC Secure] RequestUrls: empty URL list of 0x{pid:X2}", Color.Red);
            /*// Set p2p port 4659
            List<string> peerUrls = new List<string>();
            foreach(string url in owner.urls)
            {
                StationUrl sUrl = new StationUrl(url) { Port = 4659 };
                peerUrls.Add(sUrl.ToString());
            }*/
            Urls = owner.Urls;
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            uint count = 1;
            foreach(string url in Urls)
            {
                sb.AppendLine($"\t[Url{count}: {url}]");
                count++;
            }
            sb.AppendLine($"\t[Count: {count-1}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, Result);
            Helper.WriteStringList(m, Urls);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[RequestUrls Response]";
        }
    }
}
