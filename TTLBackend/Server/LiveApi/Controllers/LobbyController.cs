using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using QuazalWV;

namespace TTLBackend
{
    public static class LobbyController
    {
        public static HttpListenerResponse GET(HttpListenerRequest req, HttpListenerResponse res)
        {
            List<Session> sessions = Global.Sessions;
            string platformParam = req.QueryString["platform"];
            if (platformParam == "PC")
                sessions = sessions.Where(c => !c.PS2).ToList();
            else if (platformParam == "PS2")
                sessions = sessions.Where(c => c.PS2).ToList();
            else if (platformParam != null)
            {
                // Invalid platform parameter
                res.StatusCode = 400;
                return res;
            }
            List<Lobby> lobbies = new List<Lobby>();
            // Data mapping
            foreach (Session ses in sessions)
                lobbies.Add(new Lobby(ses));

            string bodyStr = JsonSerializer.Serialize(lobbies);
            byte[] body = Encoding.UTF8.GetBytes(bodyStr);
            res = HttpUtils.WriteBody(res, bodyStr);
            res.ContentType = "application/json";
            res.StatusCode = 200;

            return res;
        }
    }
}
