using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_SetState : RMCPResponse
    {
        public bool Result { get; set; }

        public RMCPacketResponseMatchMakingService_SetState(uint gathId, uint state)
        {
            // Set result
            Result = true;
            // Try to find a session
            Session ses = Global.Sessions.Find(s => s.TtlSession.m_idMyself == gathId);
            if (ses != null)
                ses.TtlSession.m_uiState = state;
            else
            {
                // Try to find a round
                Round round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == gathId);
                if (round != null)
                    round.TtlRound.m_uiState = state;
                else
                {
                    // Add handling here
                    Log.WriteLine(1, $"[RMC MatchmakingService] SetState: gathering 0x{gathId:X2} not found", Color.Red);
                }
            }
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Result: {Result}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, Result);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[SetState Response]";
        }
    }
}
