using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class EidosFriend : IData
    {
        public FriendData FriendData { get; set; }
        public string ClanAbbr { get; set; }
        public List<GatheringEx> Gatherings { get; set; }
        public bool IsLookingToPlay { get; set; }

        public EidosFriend()
        {
            FriendData = new FriendData();
            ClanAbbr = "";
            Gatherings = new List<GatheringEx>();
            IsLookingToPlay = true;
        }

        public EidosFriend(Stream s)
        {
            FromStream(s);
        }

        public void ToBuffer(Stream s)
        {
            FriendData.ToBuffer(s);
            Helper.WriteString(s, ClanAbbr);
            Helper.WriteU32(s, (uint)Gatherings.Count);
            foreach (GatheringEx g in Gatherings)
                g.ToBuffer(s);
            Helper.WriteBool(s, IsLookingToPlay);
        }

        public void FromStream(Stream s)
        {
            FriendData = new FriendData(s);
            ClanAbbr = Helper.ReadString(s);
            uint gathCount = Helper.ReadU32(s);
            for(uint i = 0; i < gathCount; i++)
                Gatherings.Add(new GatheringEx(s));
            IsLookingToPlay = Helper.ReadBool(s);
        }

        public uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public string GetClass()
        {
            return "EidosFriend";
        }
        
    }
}
