using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_GetTopPlayers : RMCPRequest
    {
        public uint NbEntries { get; set; }
        

        public RMCPacketRequestEidosService_GetTopPlayers(Stream s)
        {
            NbEntries = Helper.ReadU32(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, NbEntries);            
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[GetTopPlayers Request]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
