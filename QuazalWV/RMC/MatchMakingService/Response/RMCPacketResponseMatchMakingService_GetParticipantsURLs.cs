using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_GetParticipantsURLs : RMCPResponse
    {
        public List<string> StationUrls { get; set; }

        public RMCPacketResponseMatchMakingService_GetParticipantsURLs(uint gathId, ClientInfo searcher)
        {
            StationUrls = new List<string>();
            Session ses = Global.Sessions.Where(s => s.TtlSession.m_idMyself == gathId).First();
            if (ses != null)
            {
                StationUrl statUrl = new StationUrl(ses.Url)
                {
                    // Mandatory (set both to the searcher's PID)
                    PID = searcher.PID,
                    RVCID = searcher.PID,
                    // Optional (remove CID, stream and type attributes)
                    CID = 0,
                    Stream = 0,
                    Type = 0
                };
                StationUrls.Add(statUrl.ToString());
            }
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteStringList(m, StationUrls);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[RMCPacketResponseMatchMakingService_GetParticipantsURLs]";
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
    }
}
