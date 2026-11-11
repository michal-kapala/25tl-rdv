using System.Drawing;
using System.IO;
using System.Linq;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingExtService_EndParticipation : RMCPResponse
    {
        // To be checked with UpdateGathering's effect
        public RMCPacketResponseMatchMakingExtService_EndParticipation(uint gathId, ClientInfo client)
        {
            Session ses = Global.Sessions.Find(s => s.TtlSession.m_idMyself == gathId);
            Round round = null;
            // If it's not a session end (lobby left), check for round end
            if (ses == null)
                round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == gathId);
            // Error, no gathering to end participation in
            // EndParticipation request might be sent more times than needed and cause this to appear in logs, should probably be ignored
            if (ses == null && round == null)
                Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: gathering 0x{gathId:X2} not found (likely a duplicate)", Color.Gray, client);
            // End participation in a session or round
            else
            {
                // Session end (player has left a lobby)
                if(ses != null)
                {
                    ClientInfo clMatch = ses.Participants.Find(c => c.PID == client.PID);
                    if (clMatch == null)
                        Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: player is not a participant of session:\n{ses}", Color.Red, client);
                    else
                    {
                        // Remove the client from the session
                        --ses.TtlSession.TotalConnectedPlayers;
                        ses.Participants.Remove(client);
                        client.LobbyGathId = 0;
                        client.Host = false;
                        Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: Participant removed from session 0x{gathId:X2}", Color.Orange, client);
                        // If all have left, remove the session
                        if (ses.TtlSession.TotalConnectedPlayers == 0)
                        {
                            //TODO: update database

                            // If the host leaves using 'Quit to Menu' in-game, only the session is removed, remove child rounds if any (there should be max 1)
                            // Find child by session name (should be somehow unique) and remove
                            Round unfinishedRound = Global.Rounds.Find(r => r.TtlRound.m_strDescription == ses.TtlSession.m_strDescription);
                            if(unfinishedRound != null)
                            {
                                Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: unfinished round removed:\n{unfinishedRound}", Color.Orange, client);
                                Global.Rounds.Remove(unfinishedRound);
                            }
                            Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: empty session removed:\n{ses}", Color.Orange, client);
                            Global.Sessions.Remove(ses);
                        }
                        // Host/owner left, set the new host/owner
                        // Currently 'Lost session host' error is sent to all clients if the host leaves and the lobby is deleted
                        else if (ses.TtlSession.m_pidHost == client.PID || ses.TtlSession.m_pidOwner == client.PID)
                        {
                            ClientInfo newHost = ses.Participants.First();
                            ses.TtlSession.m_pidHost = newHost.PID;
                            ses.TtlSession.m_pidOwner = newHost.PID;
                            ses.OwnerPID = newHost.PID;
                            ses.Url = newHost.Urls.First();
                        }
                    }
                }
                // Round end (a match has finished)
                else
                {
                    uint playerPid = round.ParticipantPids.Find(pid => pid == client.PID);
                    // The game happens to send duplicate requests at the end of a round, should be ignored
                    if (playerPid == 0)
                        Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: player is not a participant of round 0x{gathId:X2} (likely a duplicate)", Color.Gray, client);
                    // Remove the client from the round
                    else
                    {
                        round.ParticipantPids.Remove(playerPid);
                        client.RoundGathId = 0;
                        Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: player removed from round 0x{gathId:X2}", Color.Orange, client);
                        // If all have left, remove the round
                        if (round.ParticipantPids.Count == 0)
                        {
                            // All have left the round - we have everyone's stats, calculate new rankings
                            UpdateRankings(round);
                            Log.WriteLine(1, $"[RMC MatchMakingService] EndParticipation: empty round removed:\n{round}", Color.Orange, client);
                            Global.Rounds.Remove(round);
                        }
                        // Host/owner left, set the new host/owner
                        // Currently 'Lost session host' error is sent to all clients if the host leaves and the lobby is deleted
                        else if (round.TtlRound.m_pidHost == client.PID || round.TtlRound.m_pidOwner == client.PID)
                        {
                            uint newHostPid = round.ParticipantPids.First();
                            ClientInfo host = round.Participants.Find(p => p.PID == newHostPid);
                            round.TtlRound.m_pidHost = host.PID;
                            round.TtlRound.m_pidOwner = host.PID;
                        }
                    }
                }
            }
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, 0x10001);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[EndParticipation Response]";
        }

        public override string PayloadToString()
        {
            return "";
        }

        private void UpdateRankings(Round round)
        {
            float[] rankings = new float[round.RankedParticipants.Count];
            uint count = 0;
            foreach (ClientInfo rankedPlayer in round.RankedParticipants)
                rankings[count++] = StatsHelper.NewRanking(rankedPlayer, round);
            // Update the rankings AFTER all the new values are calculated
            count = 0;
            foreach (ClientInfo rankedPlayer in round.RankedParticipants)
            {
                // Cached and db
                rankedPlayer.Rating = rankings[count++];
                StatsHelper.UpdateRanking(rankedPlayer);
                // Clear the round stats
                rankedPlayer.RoundStats.Clear();
            }
        }

    }
}
