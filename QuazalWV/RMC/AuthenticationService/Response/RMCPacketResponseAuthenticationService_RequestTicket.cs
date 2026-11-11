using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseAuthenticationService_RequestTicket : RMCPResponse
    {
        public uint resultCode = 0x10001;
        public byte[] ticketBuffer;
        public KerberosTicket ticket;

        public RMCPacketResponseAuthenticationService_RequestTicket(ClientInfo client)
        {
            ticket = new KerberosTicket(client);
            ticketBuffer = ticket.toBuffer();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, resultCode);
            Helper.WriteU32(m, (uint)ticketBuffer.Length);
            foreach (byte b in ticketBuffer)
                Helper.WriteU8(m, b);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[RequestTicket Response]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t[Result Code   : 0x" + resultCode.ToString("X8") + "]");
            sb.Append("\t[Ticket Buffer : { ");
            foreach (byte b in ticketBuffer)
                sb.Append(b.ToString("X2") + " ");
            sb.AppendLine("}]");
            return sb.ToString();
        }
    }
}
