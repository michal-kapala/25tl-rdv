using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_GetMessageOfTheDay : RMCPRequest
    {
        public string Language { get; set; }

        public RMCPacketRequestEidosService_GetMessageOfTheDay(Stream s)
        {
            Language = Helper.ReadString(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, Language);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetMessageOfTheDay Request lang={Language}]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
