using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_GetTopClans : RMCPRequest
    {
        public uint Count { get; set; }

        public RMCPacketRequestEidosService_GetTopClans(Stream s)
        {
            Count = Helper.ReadU32(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Count);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[GetTopClans Request]";
        }

        public override string PayloadToString()
        {
            return "";
        }
    }
}
