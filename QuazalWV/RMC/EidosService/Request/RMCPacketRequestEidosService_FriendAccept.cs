using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_FriendAccept : RMCPRequest
    {
        public uint InviterPid { get; set; }

        public RMCPacketRequestEidosService_FriendAccept(Stream s)
        {
            InviterPid = Helper.ReadU32(s);
        }

        public override string ToString()
        {
            return "[FriendAccept Request]";
        }

        public override string PayloadToString()
        {
            return $"\t[Inviter: 0x{InviterPid:X2}]";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, InviterPid);
            return m.ToArray();
        }

        public bool AddFriendship(uint inviteePid)
        {
            // Add 2 friend records and delete the invites
            return FriendHelper.AddFriendship(InviterPid, inviteePid);
        }
    }
}
