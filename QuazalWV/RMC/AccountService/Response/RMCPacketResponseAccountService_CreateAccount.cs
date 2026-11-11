using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseAccountService_CreateAccount : RMCPResponse
    {
        public uint ResultCode { get; set; }

        public RMCPacketResponseAccountService_CreateAccount(bool bCreated)
        {
            ResultCode = bCreated ? (uint)0x10001 : 0;
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t[Result code : 0x" + ResultCode.ToString("X8") + "]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            // Result code is ignored by the game
            Helper.WriteU32(m, ResultCode);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[CreateAccount Response]";
        }
    }
}
