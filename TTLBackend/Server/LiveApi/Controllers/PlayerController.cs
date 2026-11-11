using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using QuazalWV;

namespace TTLBackend
{
    public static class PlayerController
    {
        public static HttpListenerResponse GET(HttpListenerRequest req, HttpListenerResponse res)
        {
            List<ClientInfo> clients = Global.Clients;
            string platformParam = req.QueryString["platform"];
            if (platformParam == "PC")
                clients = clients.Where(c => !c.PS2 && c.Name != null && c.Name != "Guest").ToList();
            else if(platformParam == "PS2")
                clients = clients.Where(c => c.PS2 && c.Name != null && c.Name != "Guest").ToList();
            else if (platformParam != null)
            {
                // Invalid platform parameter
                res.StatusCode = 400;
                return res;
            }
            List<Player> players = new List<Player>();
            // Data mapping
            foreach(ClientInfo client in clients)
                players.Add(new Player(client));

            string bodyStr = JsonSerializer.Serialize(players);
            byte[] body = Encoding.UTF8.GetBytes(bodyStr);
            res = HttpUtils.WriteBody(res, bodyStr);
            res.ContentType = "application/json";
            res.StatusCode = 200;

            return res;
        }
    }
}
