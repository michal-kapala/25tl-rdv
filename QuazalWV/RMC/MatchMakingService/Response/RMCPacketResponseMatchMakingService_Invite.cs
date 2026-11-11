using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_Invite : RMCPResponse
    {
        public bool Result { get; set; }

        public RMCPacketResponseMatchMakingService_Invite()
        {
            Result = true;
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

        public override string ToString()
        {
            return "[Invite Response]";
        }
    }
}
