using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QuazalWV
{
    public class KerberosTicket
    {
        public uint userPID;
        public byte[] sessionKey = Global.KerberosSesKey;
        public uint serverPID;
        public byte[] ticket = new byte[] { 0x76, 0x21, 0x4B, 0xA6, 0x21, 0x96, 0xD3, 0xF3, 0x9A, 0x8C, 0x7A, 0x27, 0x0D, 0xD9, 0xB3, 0xFA, 0x21, 0x0E, 0xED, 0xAF, 0x42, 0x63, 0x92, 0x95, 0xC1, 0x16, 0x54, 0x08, 0xEE, 0x6E, 0x69, 0x17, 0x35, 0x78, 0x2E, 0x6E };
        public byte[] userPassHash = null;

        public KerberosTicket(ClientInfo client)
        {
            userPID = client.PID;
            serverPID = client.sPID;
            // A registered account
            if (client.Pass != null)
                userPassHash = DeriveKey(userPID, client.Pass);
            // Guest account
            else
                userPassHash = DeriveKey(userPID);
        }

        /// <summary>
        /// Pass user's password as input
        /// For account creation, the following credentials are used:
        /// Username: Guest
        /// Password: h7fyctiuucf
        /// In other cases, the hash received from Account Service CreateAccount is used
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] DeriveKey(uint pid, string password = "h7fyctiuucf")
        {
            byte[] buff;
            uint count = password == "h7fyctiuucf" ? 65000 + (pid % 1024) : (pid % 1024);
            MD5 md5 = MD5.Create();
            // Post-hashing for registered user
            if (count < 1024)
            {
                buff = Helper.HexStringToByteArray(password);
                for (uint i = 0; i < count; i++)
                    buff = md5.ComputeHash(buff);
            }
            // Full hashing for Guest
            else
            {
                buff = Encoding.ASCII.GetBytes(password);
                for (uint i = 0; i < count; i++)
                    buff = md5.ComputeHash(buff);
            }
            
            return buff;
        }

        public byte[] toBuffer()
        {
            MemoryStream m = new MemoryStream();
            m.Write(sessionKey, 0, 16);
            Helper.WriteU32(m, serverPID);
            Helper.WriteU32(m, (uint)ticket.Length);
            m.Write(ticket, 0, ticket.Length);
            byte[] buff = m.ToArray();
            byte[] key = userPassHash;
            buff = Helper.Encrypt(key, buff);
            byte[] hmac = Helper.MakeHMAC(key, buff);
            m = new MemoryStream();
            m.Write(buff, 0, buff.Length);
            m.Write(hmac, 0, hmac.Length);
            return m.ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t[Kerberos Ticket]");
            sb.Append("\t\t[Session Key : { ");
            foreach (byte b in sessionKey)
                sb.Append(b.ToString("X2") + " ");
            sb.AppendLine("}]");
            sb.AppendLine("\t\t[Server PID : 0x" + serverPID.ToString("X8"));
            sb.Append("\t\t[Ticket Data : { ");
            foreach (byte b in ticket)
                sb.Append(b.ToString("X2") + " ");
            sb.AppendLine("}]");
            return sb.ToString();
        }
    }
}
