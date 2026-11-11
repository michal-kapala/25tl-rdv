using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_GetEidosAccountInfo : RMCPResponse
    {
        public uint Pid { get; set; }
        public EidosAccountPrivate EidosAccPrivate { get; set; }
        public EidosAccountPublic EidosAccPublic { get; set; }
        public string Email { get; set; }

        public RMCPacketResponseEidosService_GetEidosAccountInfo(uint pid)
        {
            Pid = pid;
            ReadAccountData();
        }

        public override string PayloadToString()
        {
            string result = EidosAccPrivate.ToString();
            result += "\n" + EidosAccPublic.ToString();
            result += "\n" + $"\t[Email: {Email}]";
            return result;
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            EidosAccPrivate.ToBuffer(m);
            EidosAccPublic.ToBuffer(m);
            Helper.WriteString(m, Email);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetEidosAccountInfo Response pid=0x{Pid:X2}]";
        }

        /// <summary>
        /// Reads account data from 'users' DB table.
        /// </summary>
        private void ReadAccountData()
        {
            User data = User.GetByPid(Pid);
            if(data != null)
            {
                EidosAccPrivate = new EidosAccountPrivate(data);
                EidosAccPublic = new EidosAccountPublic(data);
                Email = data.Email;
            }
        }
    }
}
