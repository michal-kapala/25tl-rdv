using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_FriendsQuery : RMCPResponse
    {
        public List<EidosFriend> Friends { get; set; }
        /// <summary>
        /// Dont even ask me why invitations are friends, it's true.
        /// </summary>
        public List<EidosFriend> IncomingInvites { get; set; }
        /// <summary>
        /// Dont even ask me why invitations are friends, it's true.
        /// </summary>
        public List<EidosFriend> OutgoingInvites { get; set; }

        public RMCPacketResponseEidosService_FriendsQuery(string lang, uint pid)
        {
            Friends = new List<EidosFriend>();
            IncomingInvites = new List<EidosFriend>();
            OutgoingInvites = new List<EidosFriend>();
            ProcessLangauge(lang, pid);
        }

        // Language is probably sent for Status string
        private void ProcessLangauge(string lang, uint pid)
        {
            switch (lang)
            {
                case "english":
                    Friends = FriendHelper.GetFriends(pid);
                    IncomingInvites = FriendInvite.GetAllIncoming(pid);
                    OutgoingInvites = FriendInvite.GetAllOutgoing(pid);
                    // Append invites to friend list (invites are friends)
                    if(IncomingInvites != null)
                        Friends.AddRange(IncomingInvites);
                    if(OutgoingInvites != null)
                        Friends.AddRange(OutgoingInvites);
                    break;
                default:
                    break;
            }
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)Friends.Count);
            foreach (EidosFriend f in Friends)
                f.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FriendsQuery Response count={Friends.Count}]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
