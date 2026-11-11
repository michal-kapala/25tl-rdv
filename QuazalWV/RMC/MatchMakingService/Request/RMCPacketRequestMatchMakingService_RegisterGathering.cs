using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_RegisterGathering : RMCPRequest
    {
        public AnyDataHeader Header { get; set; }
        public Gathering Gathering { get; set; }

        public RMCPacketRequestMatchMakingService_RegisterGathering(Stream s, ClientInfo client)
        {
            Header = new AnyDataHeader(s);
            ProcessData(s, client);
        }

        private void ProcessData(Stream s, ClientInfo client)
        {
            // Specific processing
            switch (Header.DataClass)
            {
                case "Ranking":
                    // Read and assign ids (self-hosted)
                    Competition c = new Competition(s)
                    {
                        m_idMyself = Global.GathIdCounter++,
                        m_pidOwner = client.PID,
                        m_pidHost = client.PID
                    };
                    // Add a ranking
                    Log.WriteLine(1, $"[RMC MatchMakingService] Ranking registered:\n{c}", Color.Blue, client);
                    Global.Competitions.Add(c);
                    Gathering = c;
                    break;
                case "TTLSession":
                    // Read and assign ids (self-hosted)
                    TTLSession ses = new TTLSession(s)
                    {
                        m_idMyself = Global.GathIdCounter++,
                        m_pidOwner = client.PID,
                        m_pidHost = client.PID,
                        // Leave the game's policy 7
                        //m_uiParticipationPolicy = (uint)PARTICIPATION_POLICY.Open,
                        // Increased on Participate response
                        TotalConnectedPlayers = 0
                    };
                    // The game happens to send the request for the same session twice, check if the session has been already registered
                    Session dupCheckSes = Global.Sessions.Find(sess => sess.TtlSession.m_strDescription == ses.m_strDescription);
                    // if the owner recreates the lobby, update the old one
                    if(dupCheckSes != null && client.PID == dupCheckSes.TtlSession.m_pidOwner)
                    {
                        Log.WriteLine(1, $"[RMC MatchMakingService] RegisterGathering: session request is duplicate of 0x{dupCheckSes.TtlSession.m_idMyself:X2}, updating:\n{ses}", Color.Gray, client);
                        // Sends an already sent id, game's behaviour to be checked
                        //ses = dupCheckSes.TtlSession;

                        // Update the old session with new data
                        dupCheckSes.TtlSession = ses;
                        dupCheckSes.OwnerPID = ses.m_pidOwner;
                        dupCheckSes.Participants.Clear();
                        dupCheckSes.Url = "";
                        Gathering = ses;
                    }
                    // Session hijack attempt
                    else if (dupCheckSes != null && client.PID != dupCheckSes.TtlSession.m_pidOwner)
                    {
                        // Null value processed in response handler
                        Log.WriteLine(1, $"[RMC MatchMakingService] RegisterGathering: lobby {dupCheckSes.TtlSession.m_strDescription} (0x{dupCheckSes.TtlSession.m_idMyself:X2}) hijack attempt - the owner is {dupCheckSes.TtlSession.m_pidOwner}:\n{ses}", Color.Red, client);
                        Gathering = null;
                    }
                    // Not a duplicate, add a session
                    else
                    {
                        Log.WriteLine(1, $"[RMC MatchMakingService] Session registered:\n{ses}", Color.Blue, client);
                        Global.Sessions.Add(new Session(ses, client));
                        // Set client's participation data (for APIs)
                        client.LobbyGathId = ses.m_idMyself;
                        client.Host = true;
                        Gathering = ses;
                    }
                    break;
                case "TTLRound":
                    // Read and assign ids (self-hosted)
                    TTLRound ttlRound = new TTLRound(s)
                    {
                        m_idMyself = Global.GathIdCounter++,
                        m_pidOwner = client.PID,
                        m_pidHost = client.PID,
                        // Leave the game's policy 7
                        //m_uiParticipationPolicy = (uint)PARTICIPATION_POLICY.Open,
                    };
                    Round round = new Round(ttlRound, client);
                    // Assign participants and round data from the parent session (lobby) by name
                    // TODO: ensure session names are unique (probably eidos' flop, no parent session id is sent)
                    Session parentSession = Global.Sessions.Find(pSes => pSes.TtlSession.m_strDescription == ttlRound.m_strDescription);
                    if (parentSession != null)
                    {
                        round.TtlRound.LevelName = parentSession.TtlSession.LevelName;
                        round.TtlRound.LevelIndex = parentSession.TtlSession.LevelIndex;
                        round.TtlRound.GameType = parentSession.TtlSession.GameType;
                        round.TtlRound.IsPrivate = parentSession.TtlSession.IsPrivate;
                        round.TtlRound.TimeLimit = parentSession.TtlSession.TimeLimit;
                        round.TtlRound.m_uiMinParticipants = parentSession.TtlSession.m_uiMinParticipants;
                        round.TtlRound.m_uiMaxParticipants = parentSession.TtlSession.m_uiMaxParticipants;
                        // TODO: Ranking value to be assigned to the round
                    }
                    // Add the round
                    Log.WriteLine(1, $"[RMC MatchMakingService] Round registered:\n{ttlRound}", Color.Blue, client);
                    Global.Rounds.Add(round);
                    Gathering = ttlRound;
                    break;
                // havent received yet
                case "Competition":
                case "Tournament":
                default:
                    Log.WriteLine(1, $"[RMC MatchMakingService] Unknown gathering registered: {Header.DataClass}", Color.Red, client);
                    break;
            }
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Header.ToBuffer(m);
            Gathering.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[RegisterGathering Request dataClass={Header.DataClass}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Data class: {Header.DataClass}]");
            sb.AppendLine($"\t[Gathering desc: {(Gathering.m_strDescription)}]");
            return sb.ToString();
        }
    }
}
