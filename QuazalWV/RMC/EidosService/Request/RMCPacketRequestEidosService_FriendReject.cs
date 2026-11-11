using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_FriendReject : RMCPRequest
    {
        public uint InviterPid { get; set; }
        public bool Block { get; set; }

        public RMCPacketRequestEidosService_FriendReject(Stream s)
        {
            InviterPid = Helper.ReadU32(s);
            Block = Helper.ReadBool(s);
        }

        public override string ToString()
        {
            return "[FriendReject Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Inviter: 0x{InviterPid:X2}]");
            sb.AppendLine($"\t[Block: {Block}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, InviterPid);
            Helper.WriteBool(m, Block);
            return m.ToArray();
        }

        public bool RejectInvite(uint inviteePid)
        {
            FriendInvite fi = new FriendInvite
            {
                InviterPid = InviterPid,
                InviteePid = inviteePid,
                Blocked = Block
            };
            return FriendInvite.RejectOrBlock(fi);
        }
    }
}
