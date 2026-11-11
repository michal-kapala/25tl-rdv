using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_FilterText : RMCPResponse
    {
        public bool RetVal { get; set; }

        public RMCPacketResponseEidosService_FilterText()
        {
            RetVal = true;
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, RetVal);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[FilterText Response]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
