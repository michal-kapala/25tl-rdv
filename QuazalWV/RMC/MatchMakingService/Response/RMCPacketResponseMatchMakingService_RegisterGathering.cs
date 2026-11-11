using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_RegisterGathering : RMCPResponse
    {
        public Gathering Gath { get; set; }
        public bool Hijack { get; set; }

        public RMCPacketResponseMatchMakingService_RegisterGathering(Gathering g, string gathClass)
        {
            Gath = g;
            Hijack = false;
            // handle null TTLSessions (hijacks)
            if(gathClass == "TTLSession")
            {
                TTLSession ses = (TTLSession)g;
                if (ses == null)
                    Hijack = true;
            }
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            // doesnt assign gathering ID to spoofed lobbies
            if(!Hijack)
                Helper.WriteU32(m, Gath.m_idMyself);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[RegisterGathering Response]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            if(!Hijack)
            {
                sb.AppendLine($"\t[Gath ID: {Gath.m_idMyself}]");
                sb.AppendLine($"\t[Gath name: {Gath.m_strDescription}]");
            }
            return sb.ToString();
        }
    }
}
