using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanGetMyInvitations : RMCPResponse
    {
        public uint Result { get; set; }
        public List<EidosClanInvite> IncomingClanInvites { get; set; }
        public List<EidosClanInvite> OutgoingClanInvites { get; set; }

        public RMCPacketResponseEidosService_ClanGetMyInvitations(ClientInfo player)
        {
            // TODO: get clan invites
            Result = (uint)EClanError.NoError;
            IncomingClanInvites = new List<EidosClanInvite>();
            OutgoingClanInvites = new List<EidosClanInvite>();
        }

        public override string PayloadToString()
        {
            return "";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Result);
            Helper.WriteU32(m, (uint)IncomingClanInvites.Count);
            foreach (EidosClanInvite ci in IncomingClanInvites)
                ci.ToBuffer(m);
            Helper.WriteU32(m, (uint)OutgoingClanInvites.Count);
            foreach (EidosClanInvite ci in OutgoingClanInvites)
                ci.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanGetMyInvitations Response]";
        }
    }
}
