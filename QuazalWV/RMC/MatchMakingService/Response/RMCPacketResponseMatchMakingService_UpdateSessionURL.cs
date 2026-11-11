using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_UpdateSessionURL : RMCPResponse
    {
        public bool Result { get; set; }

        public RMCPacketResponseMatchMakingService_UpdateSessionURL()
        {
            Result = true;
        }

        public override string ToString()
        {
            return "[RMCPacketResponseMatchMakingService_UpdateSessionURL]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Result: {Result}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, Result);
            return m.ToArray();
        }
    }
}
