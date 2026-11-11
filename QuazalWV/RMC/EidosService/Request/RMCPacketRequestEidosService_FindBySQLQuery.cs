using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_FindBySQLQuery : RMCPRequest
    {
        /// <summary>
        /// WHERE clause for a Gathering type.
        /// </summary>
        public string SqlQuery { get; set; }
        // Result Range
        public uint Offset { get; set; }
        public uint Length { get; set; }

        public RMCPacketRequestEidosService_FindBySQLQuery(Stream s)
        {
            SqlQuery = Helper.ReadString(s);
            Offset = Helper.ReadU32(s);
            Length = Helper.ReadU32(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, SqlQuery);
            Helper.WriteU32(m, Offset);
            Helper.WriteU32(m, Length);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[FindBySQLQuery Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[SQL    : {SqlQuery}]");
            sb.AppendLine($"\t[Offset : {Offset}]");
            sb.AppendLine($"\t[Length : {Length}]");
            return sb.ToString();
        }
        
    }
}
