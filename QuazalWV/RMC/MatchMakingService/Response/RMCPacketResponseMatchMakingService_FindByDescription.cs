using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseMatchMakingService_FindByDescription : RMCPResponse
    {
        public List<AnyDataHolder<Competition>> Competitions { get; set; }
        public string QueryDescription { get; set; }
        public ResultRange QueryRange { get; set; }

        public RMCPacketResponseMatchMakingService_FindByDescription(RMCPacketRequestMatchMakingService_FindByDescription request)
        {
            Competitions = new List<AnyDataHolder<Competition>>(); 
            QueryDescription = request.Description;
            QueryRange = new ResultRange()
            {
                Offset = request.ResRange.Offset,
                Size = request.ResRange.Size
            };
            // Find clan gatherings by description
            // TODO: AND client's pid
            switch (QueryDescription)
            {
                // Rankings
                case "Nonladder":
                    // Add Ranking
                    Competition g = new Competition();
                    AnyDataHolder<Competition> gathGata = new AnyDataHolder<Competition>(g);
                    Competitions.Add(gathGata);
                    break;
                default:
                    Log.WriteLine(1, $"[RMC MatchMakingService] Unknown gathering description: {QueryDescription}", Color.Red);
                    break;
            }
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)Competitions.Count);
            foreach (AnyDataHolder<Competition> c in Competitions)
                c.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FindByDescription Response]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Count: {Competitions.Count}]");
            return sb.ToString();
        }
    }
}
