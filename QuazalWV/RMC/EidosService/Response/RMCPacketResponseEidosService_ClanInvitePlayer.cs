using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanInvitePlayer : RMCPResponse
    {
        /// <summary>
        /// Can return errors, 0 for success.
        /// </summary>
        public uint Result { get; set; }

        public RMCPacketResponseEidosService_ClanInvitePlayer(uint result)
        {
            Result = result;
        }

        public override string ToString()
        {
            return "[ClanInvitePlayer Response]";
        }

        public override string PayloadToString()
        {
            return $"\t[Result: {Result}]";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Result);
            return m.ToArray();
        }
    }
}
