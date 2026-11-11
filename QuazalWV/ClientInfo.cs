using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace QuazalWV
{
    public class ClientInfo
    {
        /// <summary>
        /// Player's unique identifier.
        /// </summary>
        public uint PID;
        public uint sPID;
        public ushort sPort;
        public uint IDrecv;
        public uint IDsend;
        public byte sessionID;

        /// <summary>
        /// Only for Kerberos ticket, hardcoded for every user.
        /// </summary>
        public byte[] sessionKey = Global.KerberosSesKey;
        public ushort seqCounter;
        public uint callCounterRMC;

        /// <summary>
        /// Player's nickname.
        /// </summary>
        public string Name;

        /// <summary>
        /// Player's password hash.
        /// </summary>
        public string Pass;
        public IPEndPoint ep;
        public UdpClient udp;
        public bool isLocal = false;

        /// <summary>
        /// Player's available PRUDP URLs (currently only one used).
        /// </summary>
        public List<string> Urls = new List<string>();

        /// <summary>
        /// Live status of the player (online/offline).
        /// </summary>
        public string Status;

        /// <summary>
        /// Gathering ID of player's current lobby, 0 if out of lobby/match.
        /// </summary>
        public uint LobbyGathId = 0;

        /// <summary>
        /// Gathering ID of player's current round, 0 if out of match.
        /// </summary>
        public uint RoundGathId = 0;

        /// <summary>
        /// Indicates if player is currently hosting a lobby of LobbyGathId.
        /// </summary>
        public bool Host = false;

        /// <summary>
        /// Indicates if the game stats for the player are up-to-date to avoid duplicated updates.
        /// </summary>
        public bool StatsUpdated = true;

        /// <summary>
        /// Player's stats from their last finished round.
        /// </summary>
        public List<float> RoundStats = new List<float>();

        /// <summary>
        /// Player's current rating score.
        /// </summary>
        public float Rating;

        /// <summary>
        /// Player's clan tag; tags need to be added manually.
        /// </summary>
        /// <returns></returns>
        public string ClanTag { get; set; }

        /// <summary>
        /// Player's clan invites status.
        /// </summary>
        public bool AcceptClanInvites { get; set; }

        /// <summary>
        /// Player's current game platform (PS2/PC).
        /// </summary>
        /// <returns></returns>
        public bool PS2 { get; set; }

        /// <summary>
        /// Time of receiving the last PRUDP DATA packet.
        /// </summary>
        public DateTime LastPacketTime { get; set; } = DateTime.Now;

        public string EndpointToString()
        {
            StringBuilder sb = new StringBuilder();
            if(ep != null)
                sb.AppendLine($"\t[Client: {ep}]");
            sb.AppendLine($"\t[Name: {Name}]");
            sb.AppendLine($"\t[PID: {PID}]");
            sb.AppendLine($"\t[isLocal: {isLocal}]");
            sb.AppendLine($"\t[Platform: {(PS2 ? "PS2" : "PC")}]");
            return sb.ToString();
        }
    }
}
