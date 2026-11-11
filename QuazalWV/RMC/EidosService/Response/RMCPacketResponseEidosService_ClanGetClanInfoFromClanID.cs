using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanGetClanInfoFromClanID : RMCPResponse
    {
        public uint RetVal { get; set; }
        public EidosClanProps ClanProps { get; set; }

        public RMCPacketResponseEidosService_ClanGetClanInfoFromClanID(uint clanId)
        {
            RetVal = (uint)EClanError.NoError;
            ClanProps = new Clan(clanId).ToProps();
        }

        public override string PayloadToString()
        {
            return "";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, RetVal);
            ClanProps.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanGetClanInfoFromClanID Response]";
        }
    }
}
