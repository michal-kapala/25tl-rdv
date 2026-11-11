using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestAccountService_FindByNameRegex : RMCPRequest
    {
        public uint Groups { get; set; }
        /// <summary>
        /// Simply a player name, not an actual regex pattern.
        /// </summary>
        public string Regex { get; set; }
        public ResultRange ResRange { get; set; }


        public RMCPacketRequestAccountService_FindByNameRegex(Stream s)
        {
            Groups = Helper.ReadU32(s);
            Regex = Helper.ReadStringWithAccents(s);
            ResRange = new ResultRange(s);
        }

        public override string ToString()
        {
            return "[FindByNameRegex Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Groups: {Groups}]");
            sb.AppendLine($"\t[Regex: {Regex}]");
            sb.AppendLine($"\t[Offset: {ResRange.Offset}]");
            sb.AppendLine($"\t[Size: {ResRange.Size}]");
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, Groups);
            Helper.WriteString(m, Regex);
            ResRange.ToBuffer(m);
            return m.ToArray();
        }

        /// <summary>
        /// Returns the regex query indicated by ResultRange.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<BasicAccountInfo> GetBasicAccountInfos()
        {
            return User.GetBasicAccountInfos(Regex)
                .Skip((int)ResRange.Offset)
                .Take((int)ResRange.Size)
                .ToList();
        }
    }
}
