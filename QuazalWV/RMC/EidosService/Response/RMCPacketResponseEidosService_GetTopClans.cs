using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_GetTopClans : RMCPResponse
    {
        public List<EidosClanProps> ClansStats { get; set; }

        public RMCPacketResponseEidosService_GetTopClans(uint nbEntries)
        {
            ClansStats = new List<EidosClanProps>();
            /*for(uint i = 1; i <= nbEntries;i++)
                ClansStats.Add(new EidosClanProps("")
                {
                    ClanID = i,
                    ClanName = $"Clan{i}",
                    ClanRank = i,
                    ClanRating = i,
                    ClanKills = i,
                    ClanMemberCount = i,
                });*/
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)ClansStats.Count);
            foreach (EidosClanProps p in ClansStats)
                p.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetTopClans Response count={ClansStats.Count}]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
