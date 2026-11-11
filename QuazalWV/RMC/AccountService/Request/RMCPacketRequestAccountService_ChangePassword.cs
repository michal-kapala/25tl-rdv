using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestAccountService_ChangePassword : RMCPRequest
    {
        /// <summary>
        /// New or unchanged prehashed password (key) to save.
        /// </summary>
        public string Hash { get; set; }

        public RMCPacketRequestAccountService_ChangePassword(Stream s)
        {
            Hash = Helper.ReadString(s);
        }

        public override string ToString()
        {
            return "[ChangePassword Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Hash: {Hash}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, Hash);
            return m.ToArray();
        }

        /// <summary>
        /// Updates the user's password.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool Save(string username)
        {
            return User.UpdatePassword(Hash, username);
        }

    }
}
