using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class GatheringStats : IData
    {
        public uint PID { get; set; }
        public uint Flags { get; set; }
        public List<float> Values { get; set; }

        public GatheringStats(uint pid)
        {
            PID = pid;
            Flags = 1;
            Values = new List<float>();
        }

        public GatheringStats(Stream s)
        {
            Values = new List<float>();
            FromStream(s);
        }

        public void FromStream(Stream s)
        {
            PID = Helper.ReadU32(s);
            Flags = Helper.ReadU32(s);
            uint count = Helper.ReadU32(s);
            for (uint i = 0; i < count; i++)
                Values.Add(Helper.ReadFloat(s));
        }

        public uint GetBufferSize()
        {
            return 8 + 4 * (uint)Values.Count;
        }

        public string GetClass()
        {
            return "GatheringStats";
        }

        public void ToBuffer(Stream s)
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, PID);
            Helper.WriteU32(m, Flags);
            Helper.WriteU32(m, (uint)Values.Count);
            foreach (float v in Values)
                Helper.WriteFloat(m, v);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[PID: 0x{PID:X2}]");
            sb.AppendLine($"\t[Flags: 0x{Flags:X2}]");
            uint count = 1;
            foreach(float f in Values)
                sb.AppendLine($"\t[Stat{count++}: {f}]");
            return sb.ToString();
        }
    }
}
