using System.IO;

namespace QuazalWV
{
    public class GatheringEx : IData
    {
        public Gathering Gathering { get; set; }
        public string Type { get; set; }

        public GatheringEx()
        {
            // Ranking gathering by default
            Gathering = new Gathering();
            Type = "Ranking";
        }

        public GatheringEx(string type = "Ranking")
        {
            // Ranking gathering by default
            Gathering = new Gathering();
            Type = type;
        }

        public GatheringEx(Stream s)
        {
            FromStream(s);
        }

        public void ToBuffer(Stream s)
        {
            Gathering.ToBuffer(s);
            Helper.WriteString(s, Type);
        }

        public void FromStream(Stream s)
        {
            Gathering.FromStream(s);
            Type = Helper.ReadString(s);
        }

        public uint GetBufferSize()
        {
            return Gathering.GetBufferSize() + (uint)Type.Length + 2;
        }

        public string GetClass()
        {
            // A stub
            return "NonladderRanking";
        }
        
    }
}
