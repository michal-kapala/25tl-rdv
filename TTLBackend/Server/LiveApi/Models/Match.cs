using System.Text.Json.Serialization;
using QuazalWV;

namespace TTLBackend
{
    public class Match
    {
        [JsonPropertyName("id")]
        public uint Id { get; private set; }
        [JsonPropertyName("name")]
        public string Name { get; private set; }
        [JsonPropertyName("owner")]
        public string Owner { get; private set; }
        [JsonPropertyName("private")]
        public bool IsPrivate { get; private set; }
        [JsonPropertyName("game_mode")]
        public string GameType { get; private set; }
        [JsonPropertyName("time_limit")]
        public uint TimeLimit { get; private set; }
        [JsonPropertyName("map")]
        public string LevelName { get; private set; }
        [JsonPropertyName("min_players")]
        public uint MinPlayers { get; private set; }
        [JsonPropertyName("max_players")]
        public uint MaxPlayers { get; private set; }
        [JsonPropertyName("mmr")]
        public uint Rating { get; private set; }

        public Match(Round round)
        {
            Id = round.TtlRound.m_idMyself;
            Name = round.TtlRound.m_strDescription;
            ClientInfo owner = Global.Clients.Find(c => c.PID == round.TtlRound.m_pidOwner);
            Owner = owner.Name;
            IsPrivate = round.TtlRound.IsPrivate;
            GameType = ((EGameType)round.TtlRound.GameType).ToString();
            TimeLimit = round.TtlRound.TimeLimit;
            LevelName = round.TtlRound.LevelName;
            MinPlayers = round.TtlRound.m_uiMinParticipants;
            MaxPlayers = round.TtlRound.m_uiMaxParticipants;
            Rating = round.TtlRound.Rating;
        }
    }
}
