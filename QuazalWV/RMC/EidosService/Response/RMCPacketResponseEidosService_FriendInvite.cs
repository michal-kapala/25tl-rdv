using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_FriendInvite : RMCPResponse
    {
        public bool Result { get; set; }

        public RMCPacketResponseEidosService_FriendInvite(bool result)
        {
            Result = result;
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
            return "[FriendInvite Response]";
        }
    }
}
