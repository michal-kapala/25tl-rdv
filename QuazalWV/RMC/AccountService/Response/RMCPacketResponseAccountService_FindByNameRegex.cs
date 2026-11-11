using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseAccountService_FindByNameRegex : RMCPResponse
    {
        List<BasicAccountInfo> BasicAccInfos { get; set; }

        public RMCPacketResponseAccountService_FindByNameRegex(List<BasicAccountInfo> infos)
        {
            BasicAccInfos = infos;
        }

        public override string PayloadToString()
        {
            return $"\t[Result count: {BasicAccInfos.Count}]";
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)BasicAccInfos.Count);
            foreach (BasicAccountInfo info in BasicAccInfos)
                info.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[FindByNameRegex Response]";
        }
    }
}
