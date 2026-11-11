using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class Competition : Gathering, IData
    {
        public ECompetitionType Type { get; set; }

        public string CompetitionType { get; set; }
        // Registered ranked rounds?
        public List<Gathering> Gatherings { get; set; }

        /// <summary>
        /// A Ranking instance by default.
        /// </summary>
        public Competition() : base()
        {
            Gatherings = new List<Gathering>();
            Type = ECompetitionType.Ranking;
            m_strDescription = "Nonladder";
            CompetitionType = "NonladderRanking";
            // Add a stub gathering
            //Gatherings.Add(new Gathering());
        }

        public Competition(ECompetitionType t) : base()
        {
            Gatherings = new List<Gathering>();
            Type = t;
            m_strDescription = "Nonladder";
            //to be changed
            switch(t)
            {
                case ECompetitionType.Competition:
                    // To be corrected later
                    CompetitionType = "NonladderRanking";
                    break;
                case ECompetitionType.Ranking:
                    CompetitionType = "NonladderRanking";
                    break;
                case ECompetitionType.Tournament:
                    // To be corrected later
                    CompetitionType = "NonladderRanking";
                    break;
            }
            // Add a stub gathering
            //Gatherings.Add(new Gathering());
        }

        public Competition(Stream s)
        {
            Gatherings = new List<Gathering>();
            FromStream(s);
        }

        public override void FromStream(Stream s)
        {
            base.FromStream(s);
            CompetitionType = Helper.ReadString(s);
            uint count = Helper.ReadU32(s);
            for (uint i = 0; i < count; i++)
                Gatherings.Add(new Gathering(s));
        }

        public override string GetClass()
        {
            return Type.ToString("G");
        }

        public override uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public override void ToBuffer(Stream s)
        {   
            base.ToBuffer(s);
            Helper.WriteString(s, CompetitionType);
            Helper.WriteU32(s, (uint)Gatherings.Count);
            foreach (Gathering g in Gatherings)
                g.ToBuffer(s);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[CompType: {CompetitionType}]");
            sb.AppendLine($"\t[Gath id: 0x{m_idMyself:X2}]");
            sb.AppendLine($"\t[Owner: 0x{m_pidOwner:X2}]");
            sb.AppendLine($"\t[Host: 0x{m_pidHost:X2}]");
            sb.AppendLine($"\t[Gath name: {m_strDescription}]");
            return sb.ToString();
        }
    }
}
