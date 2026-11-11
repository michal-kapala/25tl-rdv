using System.Net.Sockets;
using System.Net;
using System.Threading;
using QuazalWV;
using System.Configuration;
using System;

namespace TTLBackend
{
    public static class PCAuthServer
    {
        public static readonly object _sync = new object();
        public static bool _exit = false;
        public static ushort listenPort = 30125;
        private static UdpClient listener;
        public static ushort _skipNextNAT = 0xFFFF;

        public static void Start()
        {
            _exit = false;
            new Thread(tMainThread).Start();
        }

        public static void Stop()
        {
            lock (_sync)
            {
                _exit = true;
            }
            if (listener != null)
                listener.Close();
        }

        public static void tMainThread(object obj)
        {
            WriteLog(1, $"Server started, RDV address: {ConfigurationManager.AppSettings["SecureServerAddress"]}");
            listener = new UdpClient(listenPort);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                lock (_sync)
                {
                    if (_exit)
                        break;
                }
                try
                {
                    byte[] bytes = listener.Receive(ref ep);
                    ProcessPacket(bytes, ep);
                }
                catch(Exception e)
                {
                    Log.WriteLine(1, $"[PC Auth] Uncaught error:\n{e}");
                }
            }
            WriteLog(1, "Server stopped");
        }

        public static void ProcessPacket(byte[] data, IPEndPoint ep)
        {
            QPacketHandler.ProcessPacket("PC Auth", data, ep, listener, RDVServer.serverPID, RDVServer.listenPort);
        }

        private static void WriteLog(int priority, string s)
        {
            Log.WriteLine(priority, "[PC Auth] " + s);
        }
    }
}