using System;
using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class TTLStats : IData, IComparable<TTLStats>
    {
        public string Player { get; set; }
        public string ClanAbbr { get; set; }
        public float ClanRating { get; set; }
        public float ClanKills { get; set; }
        public uint PidPlayer { get; set; }
        public List<float> Values { get; set; }

        public TTLStats()
        {
            Player = "mimak";
            ClanAbbr = "";
            ClanRating = 1.0f;
            ClanKills = 1.0f;
            PidPlayer = 0x1234;
            Values = new List<float>
            {
                1.0f,
                1.0f
            };
        }

        public TTLStats(ClientInfo client)
        {
            Player = client.Name;
            ClanAbbr = "";
            ClanRating = 0.0f;
            ClanKills = 0.0f;
            PidPlayer = client.PID;
            // Gets the player's stats for leaderboards
            Values = StatsHelper.Get(client).ToList();
        }

        public TTLStats(Stream s)
        {
            FromStream(s);
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteString(s, Player);
            Helper.WriteString(s, ClanAbbr);
            Helper.WriteFloat(s, ClanRating);
            Helper.WriteFloat(s, ClanKills);
            Helper.WriteU32(s, PidPlayer);
            Helper.WriteU32(s, (uint)Values.Count);
            foreach (float f in Values)
                Helper.WriteFloat(s, f);
        }

        public void FromStream(Stream s)
        {
            Player = Helper.ReadStringWithAccents(s);
            ClanAbbr = Helper.ReadString(s);
            ClanRating = Helper.ReadFloat(s);
            ClanKills = Helper.ReadFloat(s);
            PidPlayer = Helper.ReadU32(s);
            uint count = Helper.ReadU32(s);
            float f;
            for (int i = 0; i < count; i++)
            {
                f = Helper.ReadFloat(s);
                Values.Add(f);
            }
        }

        public uint GetBufferSize()
        {
            MemoryStream m = new MemoryStream();
            ToBuffer(m);
            return (uint)m.Length;
        }

        public string GetClass()
        {
            return "TTLStats";
        }

        /// <summary>
        /// Compares by ranking value, needed for List<TTLStats>.Sort().
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TTLStats other)
        {
            if (other == null)
                return 1;
            else return Values[0].CompareTo(other.Values[0]);
        }
    }
}
