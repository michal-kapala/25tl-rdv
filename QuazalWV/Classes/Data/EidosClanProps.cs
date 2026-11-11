using System.IO;

namespace QuazalWV
{
    public class EidosClanProps : IData
    {
        public uint ClanID { get; set; }
        /// <summary>
        /// Clan tag which appears in-game.
        /// </summary>
        public string ClanAbbr { get; set; }
        public string ClanName { get; set; }
        public uint ClanRank { get; set; }
        public uint ClanRating { get; set; }
        public uint ClanKills { get; set; }
        public uint ClanMemberCount { get; set; }
        public string ClanLeader { get; set; }

        public EidosClanProps()
        { 

        }

        public EidosClanProps(string clanTag)
        {
            ClanID = 0;
            ClanAbbr = clanTag;
            ClanName = "";
            ClanRank = 0;
            ClanRating = 0;
            ClanKills = 0;
            ClanMemberCount = 0;
            ClanLeader = "";
        }

        public EidosClanProps(uint clanId)
        {
            ClanID = clanId;
            ClanAbbr = "";
            ClanName = "";
            ClanRank = 0;
            ClanRating = 0;
            ClanKills = 0;
            ClanMemberCount = 0;
            ClanLeader = "";
        }

        public EidosClanProps(Stream s)
        {
            FromStream(s);
        }

        public void FromStream(Stream s)
        {
            ClanID = Helper.ReadU32(s);
            ClanAbbr = Helper.ReadString(s);
            ClanName = Helper.ReadStringWithAccents(s);
            ClanRank = Helper.ReadU32(s);
            ClanRating = Helper.ReadU32(s);
            ClanKills = Helper.ReadU32(s);
            ClanMemberCount = Helper.ReadU32(s);
            ClanLeader = Helper.ReadStringWithAccents(s);
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, ClanID);
            Helper.WriteString(s, ClanAbbr);
            Helper.WriteString(s, ClanName);
            Helper.WriteU32(s, ClanRank);
            Helper.WriteU32(s, ClanRating);
            Helper.WriteU32(s, ClanKills);
            Helper.WriteU32(s, ClanMemberCount);
            Helper.WriteString(s, ClanLeader);
        }

        public uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public string GetClass()
        {
            return "EidosClanProps";
        }

    }
}
