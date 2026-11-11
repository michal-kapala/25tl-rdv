using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_FindBySingleID : RMCPRequest
    {
        public uint GathId { get; set; }

        public RMCPacketRequestMatchMakingService_FindBySingleID(Stream s)
        {
            GathId = Helper.ReadU32(s);
        }

        public override string PayloadToString()
        {
            return "";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GathId);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FindBySingleID Request gathId=0x{GathId:X2}]";
        }
    }
}
