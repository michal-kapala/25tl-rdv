using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseAccountService_UpdateCustomData : RMCPResponse
    {
        public uint resultCode = 0x10001;

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t[Result code : 0x" + resultCode.ToString("X8") + "]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, resultCode);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[UpdateCustomData Response]";
        }
    }
}
