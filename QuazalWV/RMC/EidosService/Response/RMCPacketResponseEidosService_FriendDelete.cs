using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_FriendDelete : RMCPResponse
    {
        public bool Result { get; set; }

        public RMCPacketResponseEidosService_FriendDelete(bool result)
        {
            Result = result;
        }

        public override string ToString()
        {
            return "[FriendDelete Response]";
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
