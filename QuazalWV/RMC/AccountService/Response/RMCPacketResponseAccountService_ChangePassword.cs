using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseAccountService_ChangePassword : RMCPResponse
    {
        public bool Result { get; set; }
        public RMCPacketResponseAccountService_ChangePassword()
        {
            Result = true;
        }

        public override string ToString()
        {
            return "[ChangePassword Response]";
        }

        public override string PayloadToString()
        {
            return $"\t[Result: {Result}]";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, Result);
            return m.ToArray();
        }
    }
}
