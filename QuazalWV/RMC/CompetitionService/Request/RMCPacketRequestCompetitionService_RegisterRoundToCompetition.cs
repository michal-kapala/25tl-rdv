using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestCompetitionService_RegisterRoundToCompetition : RMCPRequest
    {
        public uint CompetitionId { get; set; }
        public uint RoundId { get; set; }

        public RMCPacketRequestCompetitionService_RegisterRoundToCompetition(Stream s)
        {
            CompetitionId = Helper.ReadU32(s);
            RoundId = Helper.ReadU32(s);
        }

        public override string ToString()
        {
            return "[RegisterRoundToCompetition Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Competition ID: 0x{CompetitionId:X2}]");
            sb.AppendLine($"\t[Round ID: 0x{RoundId:X2}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, CompetitionId);
            Helper.WriteU32(m, RoundId);
            return m.ToArray();
        }
    }
}
