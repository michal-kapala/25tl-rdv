using System.IO;

namespace QuazalWV
{
    public class ResultRange : IData
    {
        public uint Offset { get; set; }
        public uint Size { get; set; }

        /// <summary>
        /// Default TTL stat page size.
        /// </summary>
        public ResultRange()
        {
            Offset = 0;
            Size = 25;
        }

        public ResultRange(Stream s)
        {
            FromStream(s);
        }

        public void FromStream(Stream s)
        {
            Offset = Helper.ReadU32(s);
            Size = Helper.ReadU32(s);
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, Offset);
            Helper.WriteU32(s, Size);
        }

        public string GetClass()
        {
            return "ResultRange";
        }

        public uint GetBufferSize()
        {
            return 8;
        }
    }
}
