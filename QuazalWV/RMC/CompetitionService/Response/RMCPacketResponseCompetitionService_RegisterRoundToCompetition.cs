using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseCompetitionService_RegisterRoundToCompetition : RMCPResponse
    {
        public uint CompetitionId { get; set; }
        public uint RoundId { get; set; }
        public byte Result { get; set; }

        public RMCPacketResponseCompetitionService_RegisterRoundToCompetition(uint competitionId, uint roundId)
        {
            CompetitionId = competitionId;
            RoundId = roundId;
            Result = 1;
            // Find competition
            Competition comp = Global.Competitions.Find(c => c.m_idMyself == competitionId);
            // Find round
            Round round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == roundId);
            // Add round to competition's list (needs rechecking)
            if(round != null && comp != null)
                comp.Gatherings.Add(round.TtlRound);
        }

        public override string PayloadToString()
        {
            return $"\t[Result: {Result}]";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU8(m, Result);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[RegisterRoundToCompetition Response compId=0x{CompetitionId:X2} roundId={RoundId:X2}]";
        }
    }
}
