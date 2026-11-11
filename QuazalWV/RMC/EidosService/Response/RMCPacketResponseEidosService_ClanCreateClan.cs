using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanCreateClan : RMCPResponse
    {
        public uint RetVal { get; set; }

        public RMCPacketResponseEidosService_ClanCreateClan(ClientInfo client, string clanName, string clanAbbr)
        {
            // DB access
            Clan clan = new Clan()
            { 
                Abbr = clanAbbr,
                Name = clanName,
                Leader = client.Name
            };
            RetVal = (uint)clan.Add(client);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[RetVal: {(EClanError)RetVal}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, RetVal);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[ClanCreateClan Response RetVal={(EClanError)RetVal}]";
        }

    }
}