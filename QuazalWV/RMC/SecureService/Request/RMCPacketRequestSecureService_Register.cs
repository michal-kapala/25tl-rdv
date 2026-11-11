using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestSecureService_Register : RMCPRequest
    {
        public List<string> StationUrls { get; set; }

        public RMCPacketRequestSecureService_Register(Stream s)
        {
            StationUrls = Helper.ReadStringList(s);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream result = new MemoryStream();
            Helper.WriteStringList(result, StationUrls);
            return result.ToArray();
        }

        public override string ToString()
        {
            return "[Register Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t[Station List :]");
            foreach (string s in StationUrls)
                sb.AppendLine("\t\t\t[\"" + s + "\"]");
            return sb.ToString();
        }

        public void SetUrls(ClientInfo client)
        {
            List<string> updatedUrls = new List<string>();
            // Update local endpoints to as-seen by the server (the same/public ones for LAN/online server)
            foreach(string url in StationUrls)
            {
                StationUrl sUrl = new StationUrl(url)
                {
                    Address = client.ep.Address.ToString(),
                    // Port remains 4659 for p2p, right now the game needs it forwarded to connect to the host over inet
                };
                // Add to the updated list
                updatedUrls.Add(sUrl.ToString());
            }
            if (updatedUrls.Count == 0)
                Log.WriteLine(1, $"[RMC Secure] Register: assigned empty URL list", Color.Red, client);
            client.Urls = updatedUrls;
            // Just-in-case update for the global list
            ClientInfo listClient = Global.Clients.Find(c => c.PID == client.PID);
            if(listClient == null)
            {
                Log.WriteLine(1, "[RMC Secure] Register: the client wasnt found in global list, adding", Color.Red, client);
                Global.Clients.Add(client);
            }
            else if (listClient.Urls.Count == 0)
            {
                Log.WriteLine(1, "[RMC Secure] Register: the client has empty url list in global list, updating", Color.Orange, client);
                listClient.Urls = updatedUrls;
            }
        }
    }
}