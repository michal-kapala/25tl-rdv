using System.IO;
using System.Linq;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_FindBySingleID : RMCPResponse
    {
        public bool Result { get; set; }
        public Session Session { get; set; }

        public RMCPacketResponseMatchMakingService_FindBySingleID(uint gathId)
        {
            // Find session
            Session = Global.Sessions.Where(s => s.TtlSession.m_idMyself == gathId).First();
            Result = Session != null;
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Result: {Result}]");
            sb.AppendLine($"\t[GathId: 0x{Session.TtlSession.m_idMyself:X2}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, Result);
            AnyDataHolder<Session> h = new AnyDataHolder<Session>(Session);
            h.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FindBySingleID Response]";
        }
    }
}
