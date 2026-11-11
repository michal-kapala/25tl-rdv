using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestAccountService_UpdateStatus: RMCPRequest
    {
        public string Status { get; set; }

        public RMCPacketRequestAccountService_UpdateStatus(Stream s)
        {
            Status = Helper.ReadString(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, Status);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[UpdateStatus Request : status={Status}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Status : {Status}]");
            return sb.ToString();
        }

        public bool Save(uint pid)
        {
            ClientInfo client = Global.Clients.Find(c => c.PID == pid);
            if (client != null)
            {
                client.Status = Status;
                return true;
            }
            Log.WriteLine(1, $"[RMC Account] UpdateStatus: player 0x{pid:X2} not found", Color.Red);
            return false;
        }
    }
}
