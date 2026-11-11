using QuazalWV;
using System.Net;
using System.Drawing;


namespace TTLBackend
{
    public static class LiveApiController
    {
        public static void HandleRequest(HttpListenerRequest req, HttpListenerResponse res)
        {
            if (req.HttpMethod == "GET")
            {
                string path = req.Url.AbsolutePath;
                string queryPath = req.Url.PathAndQuery;
                Color logColor = Color.Green;
                string logText = $"[Live API] {req.HttpMethod} {queryPath} ";
                // Routes
                switch (path)
                {
                    case "/":
                        res.StatusCode = 404;
                        res.ContentType = "text/html";
                        res = HttpUtils.Write404(res);
                        break;
                    case "/players":
                        res = PlayerController.GET(req, res);
                        break;
                    case "/lobbies":
                        res = LobbyController.GET(req, res);
                        res.StatusCode = 200;
                        break;
                    default:
                        res.StatusCode = 404;
                        break;
                }
                logText += res.StatusCode;
                if (res.StatusCode != 200)
                {
                    logText += $" from {req.RemoteEndPoint.Address}:{req.RemoteEndPoint.Port}";
                    logColor = Color.Red;
                }
                Log.WriteLine(1, logText, logColor);
                res.Close();
            }
            else
            {
                res.StatusCode = 405;
                Log.WriteLine(1, $"[Live API] {req.HttpMethod} {req.Url.PathAndQuery} {res.StatusCode} from {req.RemoteEndPoint.Address}:{req.RemoteEndPoint.Port}", Color.Red);
                res.Close();
            }
        }
    }
}
