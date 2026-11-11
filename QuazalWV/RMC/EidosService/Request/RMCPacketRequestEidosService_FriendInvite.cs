using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_FriendInvite : RMCPRequest
    {
        public string Player { get; set; }

        public RMCPacketRequestEidosService_FriendInvite(Stream s)
        {
            Player = Helper.ReadStringWithAccents(s);
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Invitee: {Player}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, Player);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[FriendInvite Request]";
        }

        /// <summary>
        /// Adds a friend invitation to SQLite database.
        /// </summary>
        /// <param name="inviter"></param>
        /// <returns></returns>
        public bool Save(ClientInfo inviter)
        {
            // Get invitee's pid
            User invitee = User.GetByName(Player);
            if(invitee != null)
            {
                FriendInvite fi = new FriendInvite
                {
                    InviterPid = inviter.PID,
                    InviterName = inviter.Name,
                    InviteePid = invitee.Pid,
                    InviteeName = invitee.Username,
                    Blocked = false
                };
                return FriendInvite.Add(fi);
            }
            Log.WriteLine(1, $"[RMC Eidos] FriendInvite: Invited player {Player} doesn't exist", Color.Orange);
            return false;
        }
    }
}
