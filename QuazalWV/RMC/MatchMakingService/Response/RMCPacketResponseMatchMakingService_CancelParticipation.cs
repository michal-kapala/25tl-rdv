using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_CancelParticipation : RMCPResponse
    {
        public uint GathId { get; set; }
        public uint Player { get; set; }
        public bool Result { get; set; }

        public RMCPacketResponseMatchMakingService_CancelParticipation(uint gathId, ClientInfo client)
        {
            Result = true;
            GathId = gathId;
            Player = client.PID;
            UpdateGathering(gathId, client);
        }

        public override string ToString()
        {
            return $"[CancelParticipation Response gathId=0x{GathId:X2}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[GathId: 0x{GathId:X2}]");
            sb.AppendLine($"\t[PID: 0x{Player:X2}]");
            sb.AppendLine($"\t[Result: {Result}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteBool(m, Result);
            return m.ToArray();
        }

        /// <summary>
        /// Removes participant from the requested Gathering.
        /// </summary>
        /// <param name="gathId"></param>
        /// <param name="client"></param>
        private void UpdateGathering(uint gathId, ClientInfo client)
        {
            // Try to find a session
            Session ses = Global.Sessions.Find(s => s.TtlSession.m_idMyself == gathId);
            if (ses != null)
            {
                // Session participants should only be removed once, by EndParticipation!
                Log.WriteLine(1, $"[RMC MatchMakingService] CancelParticipation: player leaving session 0x{gathId:X2}, waiting for EndParticipation", Color.Gray, client);
                // I'd like to believe Quazal takes care of gathering ownership elsewhere
            }
            else
            {
                // Try to find a round
                Round round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == gathId);
                if (round != null)
                {
                    // Round participation should be removed here, in CancelParticipation
                    // Only the pid is removed to keep track of the current participants
                    round.ParticipantPids.Remove(client.PID);
                    Log.WriteLine(1, $"[RMC MatchMakingService] CancelParticipation: Player removed from round 0x{gathId:X2}", Color.Orange, client);
                    // I'd like to believe Quazal takes care of gathering ownership elsewhere
                }
                else
                {
                    // Add handling here
                    Log.WriteLine(1, $"[RMC MatchmakingService] CancelParticipation: no session or round found with gathId 0x{gathId:X2}", Color.Red, client);
                }
            }
        }
    }
}
