using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    /// <summary>
    /// The game session object used for matchmaking by the server. Serialized like TTLSession.
    /// </summary>
    public class Session : IData
    {
        public TTLSession TtlSession { get; set; }
        public List<ClientInfo> Participants { get; set; }
        public uint OwnerPID { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// Session's platform (PS2/PC).
        /// </summary>
        public bool PS2 { get; set; }

        public Session()
        {
            TtlSession = new TTLSession();
            Participants = new List<ClientInfo>();
            OwnerPID = 0;
            Url = "";
        }

        public Session(TTLSession ttlSes, ClientInfo owner)
        {
            TtlSession = ttlSes;
            Participants = new List<ClientInfo>();
            OwnerPID = owner.PID;
            Url = "";
            PS2 = owner.PS2;
        }

        public Session(Stream s)
        {
            FromStream(s);
            Participants = new List<ClientInfo>();
            OwnerPID = 0;
            Url = "";
        }        

        public void FromStream(Stream s)
        {
            TtlSession = new TTLSession(s);
        }

        public void ToBuffer(Stream s)
        {
            TtlSession.ToBuffer(s);
        }

        public string GetClass()
        {
            return "TTLSession";
        }

        public uint GetBufferSize()
        {
            return TtlSession.GetBufferSize();
        }

        public override string ToString()
        {
            return TtlSession.ToString();
        }
    }
}
