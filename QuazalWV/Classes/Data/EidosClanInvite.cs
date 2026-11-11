using System.IO;

namespace QuazalWV
{
    public class EidosClanInvite : IData
    {
        public uint InviterClanID { get; set; }
        public string InviterClanAbbr { get; set; }
        public string InviterClanName { get; set; }
        public string InviterPlayer { get; set; }
        public uint InviterClanRank { get; set; }
        public uint InviterClanMembers { get; set; }
        public string InviteePlayer { get; set; }
        public bool IsNew { get; set; }

        public EidosClanInvite() { 
            
        }

        public EidosClanInvite(ClientInfo inviter, ClientInfo invitee)
        {

        }

        public EidosClanInvite(Stream s)
        {
            FromStream(s);
        }

        public void FromStream(Stream s)
        {
            InviterClanID = Helper.ReadU32(s);
            InviterClanAbbr = Helper.ReadString(s);
            InviterClanName = Helper.ReadString(s);
            InviterPlayer = Helper.ReadStringWithAccents(s);
            InviterClanRank = Helper.ReadU32(s);
            InviterClanMembers = Helper.ReadU32(s);
            InviteePlayer = Helper.ReadStringWithAccents(s);
            IsNew = Helper.ReadBool(s);
        }

        public uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public string GetClass()
        {
            return "EidosClanInvite";
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, InviterClanID);
            Helper.WriteString(s, InviterClanAbbr);
            Helper.WriteString(s, InviterClanName);
            Helper.WriteString(s, InviterPlayer);
            Helper.WriteU32(s, InviterClanRank);
            Helper.WriteU32(s, InviterClanMembers);
            Helper.WriteString(s, InviteePlayer);
            Helper.WriteBool(s, IsNew);
        }
    }
}
