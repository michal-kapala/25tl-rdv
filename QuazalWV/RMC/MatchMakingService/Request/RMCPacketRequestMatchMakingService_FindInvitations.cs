using System.IO;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_FindInvitations : RMCPRequest
    {
        //Result Range
        public uint Offset { get; set; }
        public uint Length { get; set; }

        public RMCPacketRequestMatchMakingService_FindInvitations(Stream s)
        {
            Offset = Helper.ReadU32(s);
            Length = Helper.ReadU32(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Offset);
            Helper.WriteU32(m, Length);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[FindInvitations Request]";
        }

        public override string PayloadToString()
        {
            return "";
        }
        
    }
}
