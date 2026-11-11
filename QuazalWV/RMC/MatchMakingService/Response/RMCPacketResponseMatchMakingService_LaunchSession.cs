using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_LaunchSession : RMCPResponse
    {
        public string Url { get; set; }
        public uint Result { get; set; }

        public RMCPacketResponseMatchMakingService_LaunchSession(uint gathId, string url, uint owner)
        {
            Url = url;
            Result = 0x10001;
            UpdateSession(gathId, url, owner);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Result);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[LaunchSession Response]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Result: 0x{Result:X2}]");
            sb.AppendLine($"\t[Session url: {Url}]");
            return sb.ToString();
        }

        /// <summary>
        /// Updates the session with host's url.
        /// </summary>
        /// <param name="gathId"></param>
        /// <param name="url"></param>
        private void UpdateSession(uint gathId, string url, uint owner)
        {
            // Try to find a session
            Session ses = Global.Sessions.Find(s => s.TtlSession.m_idMyself == gathId);
            if(ses != null)
            {
                ses.OwnerPID = owner;
                ses.Url = url;
            }
            else
            {
                // Try to find a round
                Round round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == gathId);
                if(round != null)
                {
                    round.OwnerPID = owner;
                    round.Url = url;
                }
                else
                {
                    // Add handling here
                    Log.WriteLine(1, $"[RMC MatchmakingService] LaunchSession: no session or round found with gathId 0x{gathId:X2}", Color.Red);
                }
            }
            
        }
    }
}
