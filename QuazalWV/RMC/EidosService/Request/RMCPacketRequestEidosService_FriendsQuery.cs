using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_FriendsQuery : RMCPRequest
    {
        public string Locale { get; set; }

        public RMCPacketRequestEidosService_FriendsQuery(Stream s)
        {
            Locale = Helper.ReadString(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, Locale);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FriendsQuery Request]";
        }

        public override string PayloadToString()
        {
            return $"\t[Locale: {Locale}]";
        }

    }
}
