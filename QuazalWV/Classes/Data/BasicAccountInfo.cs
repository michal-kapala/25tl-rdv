using System.IO;

namespace QuazalWV
{
    public class BasicAccountInfo : IData
    {
        public uint PID { get; set; }
        public string Name { get; set; }

        public BasicAccountInfo(uint pid, string name)
        {
            PID = pid;
            Name = name;
        }

        public BasicAccountInfo(Stream s)
        {
            FromStream(s);
        }

        public void FromStream(Stream s)
        {
            PID = Helper.ReadU32(s);
            Name = Helper.ReadStringWithAccents(s);
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, PID);
            Helper.WriteString(s, Name);
        }

        public string GetClass()
        {
            return "BasicAccountInfo";
        }

        public uint GetBufferSize()
        {
            return 4 + (uint)Name.Length;
        }
    }
}
