using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestNATTraversalService_RequestProbeInitiation : RMCPRequest
    {
        public List<string> StationUrls { get; set; }

        public RMCPacketRequestNATTraversalService_RequestProbeInitiation(Stream s)
        {
            StationUrls = new List<string>();
            StationUrls = Helper.ReadStringList(s);
        }

        public override string ToString()
        {
            return "[RequestProbeInitiation Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            uint count = 1;
            foreach(string url in StationUrls)
            {
                sb.AppendLine($"\t[Url{count}: {url}]");
                count++;
            }
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)StationUrls.Count);
            Helper.WriteStringList(m, StationUrls);
            return m.ToArray();
        }
    }
}
