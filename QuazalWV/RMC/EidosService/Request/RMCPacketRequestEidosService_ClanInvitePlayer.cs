using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_ClanInvitePlayer : RMCPRequest
    {
        public string PlayerToInvite { get; set; }

        public RMCPacketRequestEidosService_ClanInvitePlayer(Stream s)
        {
            PlayerToInvite = Helper.ReadStringWithAccents(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Invitee: {PlayerToInvite}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, PlayerToInvite);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[ClanInvitePlayer Request]";
        }

        /// <summary>
        /// Returns the error code.
        /// </summary>
        /// <returns></returns>
        public uint Save(ClientInfo inviter)
        {
            // TODO: errors to be adjusted
            uint error = 1;
            User invitee = User.GetByName(PlayerToInvite);
            if(invitee != null)
            {
                // TODO: clan/clan_members lookup(s) here (clan info + clan rank)

                ClanInvite ci = new ClanInvite
                {
                    InviterPid = inviter.PID,
                    InviterName = inviter.Name,
                    InviteePid = invitee.Pid,
                    InviteeName = invitee.Username,
                    Blocked = false,
                    // TODO
                    ClanId = 0,
                    ClanAbbr = "XYZ",
                    ClanName = "Test Clan Name",
                    InviterClanRank = 0,
                };
                if (ci.Add())
                    error = 0;
            }
            return error;
        }
    }
}
