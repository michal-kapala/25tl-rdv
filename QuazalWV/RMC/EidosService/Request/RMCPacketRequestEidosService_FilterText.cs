using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_FilterText : RMCPRequest
    {
        public string TextToFilter { get; set; }

        public RMCPacketRequestEidosService_FilterText(Stream s)
        {
            TextToFilter = Helper.ReadString(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, TextToFilter);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FilterText Request text='{TextToFilter}']";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
