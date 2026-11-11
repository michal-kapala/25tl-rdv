using System.IO;

namespace QuazalWV
{
    public class EidosClanNotice : IData
    {
        public string From { get; set; }
        public string Body { get; set; }

        public EidosClanNotice()
        {
            
        }

        public EidosClanNotice(Stream s)
        {
            FromStream(s);
        }

        public void FromStream(Stream s)
        {
            From = Helper.ReadStringWithAccents(s);
            Body = Helper.ReadString(s);
        }

        public uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public string GetClass()
        {
            return "EidosClanNotice";
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteString(s, From);
            Helper.WriteString(s, Body);
        }
    }
}
