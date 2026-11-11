using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestAccountService_CreateAccount : RMCPRequest
    {
        public string Username { get; set; }
        public string PassHash { get; set; }
        public uint Groups { get; set; }

        public RMCPacketRequestAccountService_CreateAccount(Stream s)
        {
            Username = Helper.ReadStringWithAccents(s);
            PassHash = Helper.ReadString(s);
            Groups = Helper.ReadU32(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream result = new MemoryStream();
            Helper.WriteString(result, Username);
            Helper.WriteString(result, PassHash);
            Helper.WriteU32(result, Groups);
            return result.ToArray();
        }

        public override string ToString()
        {
            return $"[CreateAccount Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Username: {Username}]");
            sb.AppendLine($"\t[Pass hash: {PassHash}]");
            sb.AppendLine($"\t[Groups: {Groups}]");
            return sb.ToString();
        }
    }
}
