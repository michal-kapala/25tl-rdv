using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_FindBySQLQuery : RMCPResponse
    {
        public bool PrivateSearch { get; set; }
        public bool SessionSearch { get; set; }
        public List<AnyDataHolder<Session>> Sessions { get; set; }

        /// <summary>
        /// Processes the WHERE-clause from the query.
        /// </summary>
        /// <param name="query"></param>
        public RMCPacketResponseEidosService_FindBySQLQuery(string query, ClientInfo client)
        {
            Sessions = new List<AnyDataHolder<Session>>();
            SessionSearch = query.IndexOf("Gatherings.type = 'TTLSession'") > 0;
            PrivateSearch = query.IndexOf("TTLSessions.isPrivate = 1") > 0;
            // Read the map name
            int conditionPos = query.IndexOf("TTLSessions.levelName = '");
            string levelName = "";
            int valuePos = -1;
            if (conditionPos > 0)
            {   
                valuePos = conditionPos + 24;
                levelName = query.Substring(valuePos);
                string[] tokens = levelName.Split('\'');
                levelName = tokens[1];
            }
            conditionPos = query.IndexOf("TTLSessions.gameType = ");
            // Read the game mode
            string gameTypeStr = "";
            if (conditionPos > 0)
            {
                valuePos = conditionPos + 23;
                gameTypeStr = query.Substring(valuePos, 1);
            }

            List<Session> sessions = new List<Session>();
            // Filter by visibility and platform
            if (SessionSearch)
                sessions = Global.Sessions.Where(s => s.TtlSession.IsPrivate == PrivateSearch && s.PS2 == client.PS2).ToList();
            // Filter by map
            if (levelName != "")
                sessions = sessions.Where(s => s.TtlSession.LevelName == levelName).ToList();
            // Filter by game mode
            if (gameTypeStr != "")
                sessions = sessions.Where(s => s.TtlSession.GameType == Convert.ToUInt32(gameTypeStr)).ToList();
            // Add to result list
            foreach(Session s in sessions)
                Sessions.Add(new AnyDataHolder<Session>(s));
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)Sessions.Count);
            foreach (AnyDataHolder<Session> ses in Sessions)
                ses.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[FindBySQLQuery Response count={Sessions.Count}]";
        }

        public override string PayloadToString()
        {
            return "";
        }
        
    }
}
