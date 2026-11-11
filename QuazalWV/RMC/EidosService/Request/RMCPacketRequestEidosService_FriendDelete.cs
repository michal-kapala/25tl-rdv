using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_FriendDelete : RMCPRequest
    {
        public uint Pid { get; set; }

        public RMCPacketRequestEidosService_FriendDelete(Stream s)
        {
            Pid = Helper.ReadU32(s);
        }

        public override string ToString()
        {
            return "[FriendDelete Request]";
        }

        public override string PayloadToString()
        {
            return $"\t[Pid: 0x{Pid:X2}]";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Pid);
            return m.ToArray();
        }

        public bool DeleteFriendOrInvite(uint selfPid)
        {
            return FriendHelper.DeleteFriendOrInvite(selfPid, Pid);
        }
    }
}
