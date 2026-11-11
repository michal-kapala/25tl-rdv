using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class EidosAlerts : IData
    {
        // Friend pids
        public List<uint> FriendInvitations { get; set; }
        // Clan ids
        public List<uint> ClanInvitations { get; set; }
        // Match ids
        public List<uint> MatchInvitations { get; set; }

        public EidosAlerts()
        {
            FriendInvitations = new List<uint>();
            ClanInvitations = new List<uint>();
            MatchInvitations = new List<uint>();
        }

        public EidosAlerts(Stream s)
        {
            FromStream(s);
        }

        public EidosAlerts(uint pid)
        {
            // Get friend invite alerts
            FriendInvitations = new List<uint>();
            List<Alert> alerts = Alert.GetAllOfType(pid, "F");
            foreach(Alert a in alerts)
                FriendInvitations.Add(a.TargetId);

            // Get clan invite alerts
            ClanInvitations = new List<uint>();
            alerts = Alert.GetAllOfType(pid, "C");
            foreach (Alert a in alerts)
                ClanInvitations.Add(a.TargetId);

            // Get match invite alerts
            MatchInvitations = new List<uint>();
            alerts = Alert.GetAllOfType(pid, "M");
            foreach (Alert a in alerts)
                MatchInvitations.Add(a.TargetId);
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, (uint)FriendInvitations.Count);
            foreach (uint fi in FriendInvitations)
                Helper.WriteU32(s, fi);
            Helper.WriteU32(s, (uint)ClanInvitations.Count);
            foreach (uint ci in ClanInvitations)
                Helper.WriteU32(s, ci);
            Helper.WriteU32(s, (uint)MatchInvitations.Count);
            foreach (uint mi in MatchInvitations)
                Helper.WriteU32(s, mi);
        }

        public void FromStream(Stream s)
        {
            uint count = Helper.ReadU32(s);
            for(uint i = 0; i < count; i++)
                FriendInvitations.Add(Helper.ReadU32(s));

            count = Helper.ReadU32(s);
            for (uint i = 0; i < count; i++)
                ClanInvitations.Add(Helper.ReadU32(s));

            count = Helper.ReadU32(s);
            for (uint i = 0; i < count; i++)
                MatchInvitations.Add(Helper.ReadU32(s));
        }

        public uint GetBufferSize()
        {
            return (uint)(FriendInvitations.Count + ClanInvitations.Count + MatchInvitations.Count) * 4 + 12;
        }

        public string GetClass()
        {
            return "EidosAlerts";
        }

    }
}
