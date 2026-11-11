using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_Participate : RMCPRequest
    {
        public uint GathId { get; set; }
        public string Message { get; set; }

        public RMCPacketRequestMatchMakingService_Participate(Stream s)
        {
            GathId = Helper.ReadU32(s);
            Message = Helper.ReadStringWithAccents(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GathId);
            Helper.WriteString(m, Message);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[Participate Request gathId=0x{GathId:X2}, msg={Message}]";
        }

        public override string PayloadToString()
        {
            return "";
        }
        
    }
}
