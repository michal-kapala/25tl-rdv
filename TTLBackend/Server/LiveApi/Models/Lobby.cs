using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using QuazalWV;

namespace TTLBackend
{
    public class Lobby
    {
        [JsonPropertyName("id")]
        public uint Id { get; private set; }
        [JsonPropertyName("name")]
        public string Name { get; private set; }
        [JsonPropertyName("owner")]
        public string Owner { get; private set; }
        [JsonPropertyName("round")]
        public Match CurrentRound { get; private set; }
        [JsonPropertyName("map")]
        public string LevelName { get; private set; }
        [JsonPropertyName("game_mode")]
        public string GameType { get; private set; }
        [JsonPropertyName("time_limit")]
        public uint TimeLimit { get; private set; }
        [JsonPropertyName("score_limit")]
        public uint ScoreLimit { get; private set; }
        [JsonPropertyName("tag_limit")]
        public uint TagLimit { get; private set; }
        [JsonPropertyName("best_of")]
        public uint BestOfSeries { get; private set; }
        [JsonPropertyName("max_players")]
        public uint MaxPlayers { get; private set; }
        [JsonPropertyName("reserved_slots")]
        public uint ReservedPlayers { get; private set; }
        [JsonPropertyName("connected_players")]
        public uint TotalConnectedPlayers { get; private set; }
        [JsonPropertyName("custom_query")]
        public bool IsCustomQuery { get; private set; }
        [JsonPropertyName("ranked")]
        public bool IsRanked { get; private set; }
        [JsonPropertyName("private")]
        public bool IsPrivate { get; private set; }
        [JsonPropertyName("respawn")]
        public bool PlayersRespawn { get; private set; }
        [JsonPropertyName("friendly_fire")]
        public bool FriendlyFire { get; private set; }
        [JsonPropertyName("apprehensions")]
        public bool Apprehension { get; private set; }
        [JsonPropertyName("arcade_damage")]
        public bool ArcadeDamage { get; private set; }
        [JsonPropertyName("self_damage")]
        public bool SelfDamage { get; private set; }
        [JsonPropertyName("alternating_teams")]
        public bool AlternatingTeams { get; private set; }
        [JsonPropertyName("platform")]
        public string Platform { get; private set; }
        /// <summary>
        /// Average player rating in the lobby.
        /// </summary>
        [JsonPropertyName("mmr")]
        public uint Rating { get; private set; }

        public Lobby(Session ses)
        {
            Id = ses.TtlSession.m_idMyself;
            Name = ses.TtlSession.m_strDescription;
            ClientInfo owner = Global.Clients.Find(c => c.PID == ses.TtlSession.m_pidOwner);
            Owner = owner.Name;
            Round round = Global.Rounds.Find(r => r.TtlRound.m_strDescription == Name);
            // null value to be inspected in terms of serialization exceptions (NullReferenceException: Object reference not set to an instance of an object.)
            if (round == null)
                CurrentRound = null;
            else
                CurrentRound = new Match(round);
            LevelName = ses.TtlSession.LevelName.Remove(0, 3).Replace(" MP", "");
            GameType = ((EGameType)ses.TtlSession.GameType).ToString();
            TimeLimit = ses.TtlSession.TimeLimit;
            ScoreLimit = ses.TtlSession.ScoreLimit;
            TagLimit = ses.TtlSession.TagLimit;
            BestOfSeries = ses.TtlSession.BestOfSeries;
            MaxPlayers = ses.TtlSession.MaxPlayers;
            ReservedPlayers = ses.TtlSession.ReservedPlayers;
            TotalConnectedPlayers = ses.TtlSession.TotalConnectedPlayers;
            IsCustomQuery = ses.TtlSession.IsCustomQuery;
            IsRanked = ses.TtlSession.IsRanked;
            IsPrivate = ses.TtlSession.IsPrivate;
            PlayersRespawn = ses.TtlSession.PlayersRespawn;
            FriendlyFire = ses.TtlSession.FriendlyFire;
            Apprehension = ses.TtlSession.Apprehension;
            ArcadeDamage = ses.TtlSession.ArcadeDamage;
            SelfDamage = ses.TtlSession.SelfDamage;
            AlternatingTeams = ses.TtlSession.AlternatingTeams;
            Platform = ses.PS2 ? "PS2" : "PC";
            Rating = Mmr();
        }

        /// <summary>
        /// Calculates the lobby MMR.
        /// </summary>
        /// <returns></returns>
        private uint Mmr()
        {
            List<ClientInfo> participants = Global.Clients.Where(c => c.LobbyGathId == Id).ToList();
            uint mmr = 0;
            foreach (ClientInfo p in participants)
                mmr += (uint)p.Rating;
            return (uint)Math.Round((double)(mmr / participants.Count));
        }
    }
}
