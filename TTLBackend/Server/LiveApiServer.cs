using System;
using System.Threading;
using System.Threading.Tasks;
using QuazalWV;
using System.Configuration;
using System.Net;
using System.Drawing;

namespace TTLBackend
{
    public static class LiveApiServer
    {
        /// <summary>
        /// HTTP server instance.
        /// </summary>
        public static HttpListener Listener { get; private set; }
        /// <summary>
        /// HTTP server IP address, set to RDV's address from configuration file.
        /// </summary>
        public static string Host { get; private set; } = ConfigurationManager.AppSettings["SecureServerAddress"];
        /// <summary>
        /// HTTP server port.
        /// </summary>
        public static ushort Port { get; private set; } = 2525;

        public static readonly object _sync = new object();
        public static bool _exit = false;

        static LiveApiServer()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add($"http://{Host}:{Port}/");
            Listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
            Listener.Prefixes.Add($"http://{Host}:{Port}/players/");
            Listener.Prefixes.Add($"http://127.0.0.1:{Port}/players/");
            Listener.Prefixes.Add($"http://{Host}:{Port}/lobbies/");
            Listener.Prefixes.Add($"http://127.0.0.1:{Port}/lobbies/");
        }

        public static void Start()
        {
            if (Listener.IsListening)
                Listener.Stop();
                
            Listener.Start();
            Log.WriteLine(1, $"[Live API] Server started on {ConfigurationManager.AppSettings["SecureServerAddress"]}:{Port}");
            _exit = false;
            new Thread(MainThread).Start();
        }

        public static void Stop()
        {
            lock (_sync)
            {
                _exit = true;
            }
            if (Listener != null)
                Listener.Stop();
        }

        public static async void MainThread(object obj)
        {
            await ProcessRequest();
        }

        /// <summary>
        /// Handles the incoming request and sends a response back.
        /// </summary>
        /// <returns></returns>
        public static async Task ProcessRequest()
        {
            while (true)
            {
                lock (_sync)
                {
                    if (_exit)
                        break;
                }
                try
                {
                    HttpListenerContext ctx = await Listener.GetContextAsync();
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse res = ctx.Response;

                    LiveApiController.HandleRequest(req, res);
                }
                catch(HttpListenerException)
                {
                    // Listener throws an exception on start/stop, ignore
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[Live API] Server exception:\n{e}", Color.Red);
                }
            }
        }
    }
}
