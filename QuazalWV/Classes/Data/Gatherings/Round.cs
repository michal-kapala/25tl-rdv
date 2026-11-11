using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class Round : IData
    {
        public TTLRound TtlRound { get; set; }
        /// <summary>
        /// The list of all participants of the round.
        /// </summary>
        public List<ClientInfo> Participants { get; set; }
        /// <summary>
        /// The list of the participants at the end of the round (qualified for ranking update).
        /// </summary>
        public List<ClientInfo> RankedParticipants { get; set; }
        /// <summary>
        /// The list of current participant ids of the round.
        /// </summary>
        public List<uint> ParticipantPids { get; set; }
        public uint OwnerPID { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// Round's platform (PS2/PC).
        /// </summary>
        public bool PS2 { get; set; }

        public Round()
        {
            TtlRound = new TTLRound();
            Participants = new List<ClientInfo>();
            RankedParticipants = new List<ClientInfo>();
            ParticipantPids = new List<uint>();
            OwnerPID = 0;
            Url = "";
        }

        public Round(TTLRound round, ClientInfo owner)
        {
            TtlRound = round;
            Participants = new List<ClientInfo>();
            RankedParticipants = new List<ClientInfo>();
            ParticipantPids = new List<uint>();
            OwnerPID = owner.PID;
            Url = "";
            PS2 = owner.PS2;
        }

        public Round(Stream s)
        {
            FromStream(s);
            Participants = new List<ClientInfo>();
            RankedParticipants = new List<ClientInfo>();
            ParticipantPids = new List<uint>();
            OwnerPID = 0;
            Url = "";
        }

        public void FromStream(Stream s)
        {
            TtlRound = new TTLRound(s);
        }

        public void ToBuffer(Stream s)
        {
            TtlRound.ToBuffer(s);
        }

        public string GetClass()
        {
            return "TTLRound";
        }

        public uint GetBufferSize()
        {
            return TtlRound.GetBufferSize();
        }

        public override string ToString()
        {
            return TtlRound.ToString();
        }
    }
}
