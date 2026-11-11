using System.Drawing;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_Participate : RMCPResponse
    {
        public RMCPacketResponseMatchMakingService_Participate(uint gathId, string msg, ClientInfo client)
        {
            AddParticipant(gathId, msg, client);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, 0x10001);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[Participate Response]";
        }

        public override string PayloadToString()
        {
            return "";
        }

        /// <summary>
        /// Adds participant to the requested Gathering.
        /// </summary>
        /// <param name="gathId"></param>
        /// <param name="msg"></param>
        /// <param name="client"></param>
        private void AddParticipant(uint gathId, string msg, ClientInfo client)
        {
            // Add participant to session
            Session ses = Global.Sessions.Find(s => s.TtlSession.m_idMyself == gathId);
            if(ses != null)
            {
                // Check for duplicates
                ClientInfo dupCheck = ses.Participants.Find(c => c.PID == client.PID);
                if(dupCheck != null)
                {
                    Log.WriteLine(1, $"[RMC MatchmakingService] Participate: player already joined session 0x{gathId:X2}, updating", Color.Gray, client);
                    ses.Participants.Remove(dupCheck);
                    ses.Participants.Add(client);
                }
                // Add if any slots available
                else if(ses.TtlSession.TotalConnectedPlayers < ses.TtlSession.MaxPlayers)
                {
                    client.LobbyGathId = gathId;
                    ses.TtlSession.TotalConnectedPlayers++;
                    ses.Participants.Add(client);
                    Log.WriteLine(1, $"[RMC MatchmakingService] Participate: Added player to session 0x{gathId:X2}", Color.Green, client);
                }
            }
            // Add participant to round
            else
            {   
                Round round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == gathId);
                if (round != null)
                {
                    // Check for duplicates
                    ClientInfo dupCheck = round.Participants.Find(c => c.PID == client.PID);
                    if (dupCheck != null)
                    {
                        Log.WriteLine(1, $"[RMC MatchmakingService] Participate: player already joined round 0x{gathId:X2}, cant add", Color.Gray, client);
                    }
                    // Add if any slots available
                    else if (round.ParticipantPids.Count < round.TtlRound.m_uiMaxParticipants)
                    {
                        client.RoundGathId = gathId;
                        client.StatsUpdated = false;
                        client.RoundStats.Clear();
                        round.Participants.Add(client);
                        round.ParticipantPids.Add(client.PID);
                        Log.WriteLine(1, $"[RMC MatchmakingService] Participate: Added player to round 0x{gathId:X2}", Color.Green, client);
                    }
                }
                else
                {
                    // Add handling here
                    Log.WriteLine(1, $"[RMC MatchmakingService] Participate: cant add player - gathering 0x{gathId:X2} not found", Color.Red, client);
                }
            }
        }
    }
}
