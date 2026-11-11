using System.IO;
using System.Text;

namespace QuazalWV
{
    public class TTLRound : Gathering, IData
    {
        public bool IsPrivate { get; set; }
        public uint LevelIndex { get; set; }
        public uint GameType { get; set; }
        public uint TimeLimit { get; set; }
        public string LevelName { get; set; }
        /// <summary>
        /// Used as round's average ranking at the beginning of the round.
        /// </summary>
        public uint Rating { get; set; }

        public TTLRound() : base()
        {
            //Gathering
            m_strDescription = "someone's round";
            m_uiMinParticipants = 1;
            m_uiMaxParticipants = 16;
            m_uiParticipationPolicy = (uint)PARTICIPATION_POLICY.Unknown2;
            m_uiPolicyArgument = 0;
            m_uiFlags = 0x10;

            //TTLRound
            IsPrivate = false;
            LevelIndex = 0xD;
            GameType = 0;
            TimeLimit = 240;
            LevelName = "01 Warehouse MP";
            Rating = 1;
        }

        public TTLRound(Stream s) 
        {
            FromStream(s);
        }

        public override void ToBuffer(Stream s)
        {
            base.ToBuffer(s);
            Helper.WriteBool(s, IsPrivate);
            Helper.WriteU32(s, LevelIndex);
            Helper.WriteU32(s, GameType);
            Helper.WriteU32(s, TimeLimit);
            Helper.WriteString(s, LevelName);
            Helper.WriteU32(s, Rating);
        }

        public override void FromStream(Stream s)
        {
            base.FromStream(s);
            IsPrivate = Helper.ReadBool(s);
            LevelIndex = Helper.ReadU32(s);
            GameType = Helper.ReadU32(s);
            TimeLimit = Helper.ReadU32(s);
            LevelName = Helper.ReadString(s);
            Rating = Helper.ReadU32(s);
        }

        public override uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public override string GetClass()
        {
            return "TTLRound";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[ID: 0x{m_idMyself:X2}]");
            sb.AppendLine($"\t[Session name: {m_strDescription}]");
            sb.AppendLine($"\t[Level name: {LevelName}]");
            sb.AppendLine($"\t[Level index: {LevelIndex}]");
            sb.AppendLine($"\t[Game type: {GameType}]");
            sb.AppendLine($"\t[IsPrivate: {IsPrivate}]");
            sb.AppendLine($"\t[Time limit: {TimeLimit}]");
            sb.AppendLine($"\t[Ranking: {Rating}]");
            return sb.ToString();
        }
    }
}
