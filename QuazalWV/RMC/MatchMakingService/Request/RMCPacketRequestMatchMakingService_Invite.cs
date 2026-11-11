using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_Invite : RMCPRequest
    {
        public uint GathId { get; set; }
        public List<uint> Invitees { get; set; }
        public string Message { get; set; }

        public RMCPacketRequestMatchMakingService_Invite(Stream s)
        {
            Invitees = new List<uint>();
            GathId = Helper.ReadU32(s);
            uint count = Helper.ReadU32(s);
            for (uint i = 0; i < count; i++)
                Invitees.Add(Helper.ReadU32(s));
            Message = Helper.ReadStringWithAccents(s);
        }

        public override string ToString()
        {
            return $"[Invite Request gathId=0x{GathId:X2}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[GathId: 0x{GathId:X2}]");
            sb.AppendLine($"\t[Invitee: 0x{Invitees[0]:X2}]");
            sb.AppendLine($"\t[Message: {Message}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, GathId);
            Helper.WriteU32(m, (uint)Invitees.Count);
            foreach (uint pid in Invitees)
                Helper.WriteU32(m, pid);
            Helper.WriteString(m, Message);
            return m.ToArray();
        }

        /// <summary>
        /// Adds invites to 'match_invites' table for all the requested players (a single player for TTL).
        /// </summary>
        /// <param name="inviter"></param>
        /// <returns></returns>
        public bool Save(uint inviter)
        {
            List<MatchInvite> invites = new List<MatchInvite>();
            MatchInvite invite = new MatchInvite
            {
                GathId = GathId,
                Inviter = inviter,
                Message = Message
            };

            foreach(uint pid in Invitees)
            {
                invite.Invitee = pid;
                invites.Add(invite);
            }
            
            return MatchInvite.AddMultiple(invites);
        }
    }
}
