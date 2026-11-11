using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_GetMessageOfTheDay : RMCPResponse
    {
        // Assumed
        public string MessageOfTheDay { get; set; }

        public RMCPacketResponseEidosService_GetMessageOfTheDay(string lang)
        {
            MessageOfTheDay = new MessageOfTheDay(lang).Message;
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteString(m, MessageOfTheDay);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetMessageOfTheDay Response msg={MessageOfTheDay}]";
        }

        public override string PayloadToString()
        {
            return "";
        }
        
    }
}
