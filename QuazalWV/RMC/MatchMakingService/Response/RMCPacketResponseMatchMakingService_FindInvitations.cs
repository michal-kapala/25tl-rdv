using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_FindInvitations : RMCPResponse
    {
        public uint QueryOffset { get; set; }
        public uint QuerySize { get; set; }

        public List<AnyDataHolder<TTLSession>> Sessions { get; set; }

        public RMCPacketResponseMatchMakingService_FindInvitations(uint offset, uint size)
        {
            QueryOffset = offset;
            QuerySize = size;
            Sessions = new List<AnyDataHolder<TTLSession>>
            {
                new AnyDataHolder<TTLSession>()
            };
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            // Dont send private invitation sessions for now
            Helper.WriteU32(m, 0);
            /*Helper.WriteU32(m, (uint)Sessions.Count);
            foreach (AnyDataHolder<TTLSession> g in Sessions)
                g.ToBuffer(m);*/
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[FindInvitations Response]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
