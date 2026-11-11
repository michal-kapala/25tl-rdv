using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class EidosClanMember : IData
    {
        public uint PlayerID { get; set; }
        public string Player { get; set; }
        public string ClanAbbr { get; set; }
        public bool ClanIsLeader { get; set; }
        public bool ClanIsOfficer { get; set; }
        public bool IsAcceptingInvites { get; set; }
        public List<AnyDataHolder<Gathering>> NetLobbyMatch { get; set; }
        public bool IsFriend { get; set; }
        public bool IsOnline { get; set; }

        public EidosClanMember(string name, uint pid)
        {
            PlayerID = pid;
            Player = name;
            ClanAbbr = "";
            ClanIsLeader = false;
            ClanIsOfficer = false;
            IsAcceptingInvites = false;
            // Should be taken from a global lobby list, stub added here
            NetLobbyMatch = new List<AnyDataHolder<Gathering>>();
            AnyDataHolder<Gathering> lobby = new AnyDataHolder<Gathering>();
            //NetLobbyMatch.Add(lobby);
            // TODO
            IsFriend = false;
            IsOnline = false;
        }

        /// <summary>
        /// Called for non-members only (PC and non-member PS2).
        /// </summary>
        /// <param name="client"></param>
        public EidosClanMember(ClientInfo client)
        {
            PlayerID = client.PID;
            Player = client.Name;
            ClanAbbr = client.ClanTag;
            ClanIsLeader = false;
            ClanIsOfficer = false;
            IsAcceptingInvites = client.AcceptClanInvites;
            // Should be taken from a global lobby list, stub added here
            NetLobbyMatch = new List<AnyDataHolder<Gathering>>();
            AnyDataHolder<Gathering> lobby = new AnyDataHolder<Gathering>();
            //NetLobbyMatch.Add(lobby);
            // TODO
            IsFriend = false;
            IsOnline = false;
        }

        public EidosClanMember(Stream s)
        {
            NetLobbyMatch = new List<AnyDataHolder<Gathering>>();
            FromStream(s);
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, PlayerID);
            Helper.WriteString(s, Player);
            Helper.WriteString(s, ClanAbbr);
            Helper.WriteBool(s, ClanIsLeader);
            Helper.WriteBool(s, ClanIsOfficer);
            Helper.WriteBool(s, IsAcceptingInvites);
            Helper.WriteU32(s, (uint)NetLobbyMatch.Count);
            foreach (AnyDataHolder<Gathering> g in NetLobbyMatch)
                g.ToBuffer(s);
            Helper.WriteBool(s, IsFriend);
            Helper.WriteBool(s, IsOnline);
        }

        public void FromStream(Stream s)
        {
            PlayerID = Helper.ReadU32(s);
            Player = Helper.ReadStringWithAccents(s);
            ClanAbbr = Helper.ReadString(s);
            ClanIsLeader = Helper.ReadBool(s);
            ClanIsOfficer = Helper.ReadBool(s);
            IsAcceptingInvites = Helper.ReadBool(s);
            uint count = Helper.ReadU32(s);
            AnyDataHolder<Gathering> g = new AnyDataHolder<Gathering>();
            for(int i = 0; i < count; i++)
            {
                g.FromStream(s);
                NetLobbyMatch.Add(g);
            }
            IsFriend = Helper.ReadBool(s);
            IsOnline = Helper.ReadBool(s);
        }

        public uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public string GetClass()
        {
            return "EidosClanMember";
        }

    }
}
