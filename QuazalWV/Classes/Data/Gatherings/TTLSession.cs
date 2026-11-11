using System.IO;
using System.Text;

namespace QuazalWV
{
    public class TTLSession : Gathering, IData
    {
        public string LevelName { get; set; }
        public uint LevelIndex { get; set; }
        public uint GameType { get; set; }
        public uint TimeLimit { get; set; }
        public uint ScoreLimit { get; set; }
        public uint TagLimit { get; set; }
        public uint BestOfSeries { get; set; }
        public uint MaxPlayers { get; set; }
        public uint ReservedPlayers { get; set; }
        public uint TotalConnectedPlayers { get; set; }
        public bool IsCustomQuery { get; set; }
        public bool IsRanked { get; set; }
        public bool IsPrivate { get; set; }
        public bool PlayersRespawn { get; set; }
        public bool FriendlyFire { get; set; }
        public bool Apprehension { get; set; }
        public bool ArcadeDamage { get; set; }
        public bool SelfDamage { get; set; }
        public bool AlternatingTeams { get; set; }

        public TTLSession(): base()
        {
            // Data from a sample RegisterGathering request
            // Gathering
            m_uiMinParticipants = 1;
            m_uiMaxParticipants = 16;
            m_uiParticipationPolicy = (uint)PARTICIPATION_POLICY.Unknown2;
            m_uiPolicyArgument = 0;
            m_uiFlags = 0x10;
            m_uiState = 1;
            m_strDescription = "someone's match";

            // TTLSession
            LevelName = "01 Warehouse MP";
            LevelIndex = 0xD;
            GameType = 0;
            TimeLimit = 240;
            ScoreLimit = 50;
            TagLimit = 0;
            BestOfSeries = 0;
            MaxPlayers = 16;
            ReservedPlayers = 0;
            TotalConnectedPlayers = 1;
            IsCustomQuery = false;
            IsRanked = true;
            IsPrivate = false;
            PlayersRespawn = false;
            FriendlyFire = true;
            Apprehension = false;
            ArcadeDamage = false;
            SelfDamage = false;
            AlternatingTeams = false;
        }

        public TTLSession(Stream s)
        {
            FromStream(s);
        }

        public override void ToBuffer(Stream s)
        {
            base.ToBuffer(s);
            Helper.WriteString(s, LevelName);
            Helper.WriteU32(s, LevelIndex);
            Helper.WriteU32(s, GameType);
            Helper.WriteU32(s, TimeLimit);
            Helper.WriteU32(s, ScoreLimit);
            Helper.WriteU32(s, TagLimit);
            Helper.WriteU32(s, BestOfSeries);
            Helper.WriteU32(s, MaxPlayers);
            Helper.WriteU32(s, ReservedPlayers);
            Helper.WriteU32(s, TotalConnectedPlayers);
            Helper.WriteBool(s, IsCustomQuery);
            Helper.WriteBool(s, IsRanked);
            Helper.WriteBool(s, IsPrivate);
            Helper.WriteBool(s, PlayersRespawn);
            Helper.WriteBool(s, FriendlyFire);
            Helper.WriteBool(s, Apprehension);
            Helper.WriteBool(s, ArcadeDamage);
            Helper.WriteBool(s, SelfDamage);
            Helper.WriteBool(s, AlternatingTeams);
        }

        public override void FromStream(Stream s)
        {
            base.FromStream(s);
            LevelName = Helper.ReadString(s);
            LevelIndex = Helper.ReadU32(s);
            GameType = Helper.ReadU32(s);
            TimeLimit = Helper.ReadU32(s);
            ScoreLimit = Helper.ReadU32(s);
            TagLimit = Helper.ReadU32(s);
            BestOfSeries = Helper.ReadU32(s);
            MaxPlayers = Helper.ReadU32(s);
            ReservedPlayers = Helper.ReadU32(s);
            TotalConnectedPlayers = Helper.ReadU32(s);
            IsCustomQuery = Helper.ReadBool(s);
            IsRanked = Helper.ReadBool(s);
            IsPrivate = Helper.ReadBool(s);
            PlayersRespawn = Helper.ReadBool(s);
            FriendlyFire = Helper.ReadBool(s);
            Apprehension = Helper.ReadBool(s);
            ArcadeDamage = Helper.ReadBool(s);
            SelfDamage = Helper.ReadBool(s);
            AlternatingTeams = Helper.ReadBool(s);
        }

        public override uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public override string GetClass()
        {
            return "TTLSession";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[ID: 0x{m_idMyself:X2}]");
            sb.AppendLine($"\t[Host PID: 0x{m_pidHost:X2}]");
            sb.AppendLine($"\t[Session name: {m_strDescription}]");
            sb.AppendLine($"\t[Level name: {LevelName}]");
            sb.AppendLine($"\t[Game type: {GameType}]");
            sb.AppendLine($"\t[Best of: {BestOfSeries}]");
            sb.AppendLine($"\t[Connected players: {TotalConnectedPlayers}]");
            sb.AppendLine($"\t[MaxPlayers: {MaxPlayers}]");
            sb.AppendLine($"\t[IsPrivate: {IsPrivate}]");
            sb.AppendLine($"\t[IsRanked: {IsRanked}]");
            return sb.ToString();
        }
    }
}
