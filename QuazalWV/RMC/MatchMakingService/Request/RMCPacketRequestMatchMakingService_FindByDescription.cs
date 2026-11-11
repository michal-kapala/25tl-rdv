using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestMatchMakingService_FindByDescription : RMCPRequest
    {
        public string Description { get; set; }
        public ResultRange ResRange { get; set; }

        public RMCPacketRequestMatchMakingService_FindByDescription(Stream s)
        {
            Description = Helper.ReadStringWithAccents(s);
            ResRange = new ResultRange(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, Description);
            ResRange.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FindByDescription Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Description : {Description}]");
            sb.AppendLine($"\t[Range offset: {ResRange.Offset}]");
            sb.AppendLine($"\t[Range size  : {ResRange.Size}]");
            return sb.ToString();
        }
    }
}
