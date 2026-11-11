using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_UpdateGathering : RMCPResponse
    {
        public RMCPacketResponseMatchMakingService_UpdateGathering()
        {

        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, 0x10001);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[UpdateGathering Response]";
        }

        public override string PayloadToString()
        {
            return "";
        }
    }
}
