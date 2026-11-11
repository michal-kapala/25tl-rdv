using System.Text.Json.Serialization;
using QuazalWV;

namespace TTLBackend
{
    public class Player
    {
        /// <summary>
        /// Player ID.
        /// </summary>
        [JsonPropertyName("pid")]
        public uint Pid { get; private set; }

        /// <summary>
        /// Player name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Player's online status.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; private set; }

        /// <summary>
        /// Player's current lobby ID, 0 if not in a lobby/round.
        /// </summary>
        [JsonPropertyName("lobby_id")]
        public uint LobbyGathId { get; private set; }

        /// <summary>
        /// Player's current round ID, 0 if not in a round.
        /// </summary>
        [JsonPropertyName("round_id")]
        public uint RoundGathId { get; private set; }

        /// <summary>
        /// Lobby host flag.
        /// </summary>
        [JsonPropertyName("hosting")]
        public bool Host { get; private set; }

        /// <summary>
        /// Player's rating score.
        /// </summary>
        [JsonPropertyName("rating")]
        public uint Rating { get; private set; }

        /// <summary>
        /// Player's clan tag.
        /// </summary>
        [JsonPropertyName("clan_tag")]
        public string ClanTag { get; private set; }

        /// <summary>
        /// Player's current platform.
        /// </summary>
        [JsonPropertyName("platform")]
        public string Platform { get; private set; }

        public Player(ClientInfo client)
        {
            Pid = client.PID;
            Name = client.Name;
            Status = client.Status;
            LobbyGathId = client.LobbyGathId;
            RoundGathId = client.RoundGathId;
            Host = client.Host;
            Rating = (uint)client.Rating;
            ClanTag = client.ClanTag;
            Platform = client.PS2 ? "PS2" : "PC";
        }
    }
}
