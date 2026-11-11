using System.Drawing;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_UpdateGathering : RMCPRequest
    {
        public uint GathId { get; set; }
        public AnyDataHeader Header { get; set; }

        public RMCPacketRequestMatchMakingService_UpdateGathering(Stream s, ClientInfo client)
        {
            Header = new AnyDataHeader(s);
            ProcessData(s, client);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Header.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[UpdateGathering Request gathId=0x{GathId:X2}]";
        }

        public override string PayloadToString()
        {
            return $"\t[Class: {Header.DataClass}]";
        }

        private void ProcessData(Stream stream, ClientInfo client)
        {
            switch(Header.DataClass)
            {
                case "TTLSession":
                    TTLSession newSes = new TTLSession(stream);
                    GathId = newSes.m_idMyself;
                    Session ses = Global.Sessions.Find(sess => sess.TtlSession.m_idMyself == newSes.m_idMyself);
                    if (ses != null)
                    {
                        // Preserve ids bcs the game is dumb and sends zeroes in updates
                        // Host migrations are only done when the host leaves - see EndParticipation response
                        uint sesId = ses.TtlSession.m_idMyself;
                        uint owner = ses.TtlSession.m_pidOwner;
                        uint host = ses.TtlSession.m_pidHost;
                        ses.TtlSession = newSes;
                        ses.TtlSession.m_idMyself = sesId;
                        ses.TtlSession.m_pidOwner = owner;
                        ses.TtlSession.m_pidHost = host;
                    }
                    else
                        Log.WriteLine(1, $"[RMC MatchMakingService] UpdateGathering: session not found: 0x{GathId:X2}", Color.Red, client);
                    break;
                default:
                    Log.WriteLine(1, $"[RMC MatchMakingService] UpdateGathering: unknown gathering update: {Header.DataClass}", Color.Red, client);
                    break;
            }
        }

    }
}
