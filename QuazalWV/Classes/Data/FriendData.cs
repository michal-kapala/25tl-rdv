using System.IO;

namespace QuazalWV
{
    public class FriendData : IData
    {
        public uint Pid { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Defines friend's current status.
        /// </summary>
        /// 0 - incoming invitation request
        /// 1 - outgoing invitation request
        /// 2 - friend
        public byte ByRelationship { get; set; }
        public uint Details { get; set; }
        /// <summary>
        /// Controls 'Match' column status. Add '|' to put the following content in parenthesis, e.g. "25ToLife|PC|ALL".
        /// </summary>
        /// Valid examples:
        /// "25ToLife|PC|ALL"
        /// "Offline"
        public string Status { get; set; }

        public FriendData()
        {

        }

        public FriendData(Stream s)
        {
            FromStream(s);
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, Pid);
            Helper.WriteString(s, Name);
            Helper.WriteU8(s, ByRelationship);
            Helper.WriteU32(s, Details);
            Helper.WriteString(s, Status);
        }

        public void FromStream(Stream s)
        {
            Pid = Helper.ReadU32(s);
            Name = Helper.ReadStringWithAccents(s);
            ByRelationship = Helper.ReadU8(s);
            Details = Helper.ReadU32(s);
            Status = Helper.ReadString(s);
        }

        public uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public string GetClass()
        {
            return "FriendData";
        }
        
    }
}
