using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestAccountService_UpdateAccountEmail : RMCPRequest
    {
        public string email;

        public RMCPacketRequestAccountService_UpdateAccountEmail(Stream s)
        {
            email = Helper.ReadString(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream result = new MemoryStream();
            Helper.WriteString(result, email);
            return result.ToArray();
        }

        public override string ToString()
        {
            return $"[UpdateAccountEmail Request : email={email}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Email : {email}]");
            return sb.ToString();
        }

        public bool Save(ClientInfo client)
        {
            return User.UpdateEmail(email, client.Name);
        }
    }
}
