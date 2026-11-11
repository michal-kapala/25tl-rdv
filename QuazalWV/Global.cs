using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Drawing;
using System;
using System.Timers;

namespace QuazalWV
{
    public static class Global
    {
        /// <summary>
        /// Hardcoded key for RC4 encryption/decryption.
        /// </summary>
        public static readonly string KeyDATA = "CD&ML";
        /// <summary>
        /// PRUDP endpoint signature.
        /// </summary>
        public static uint IdCounter = 0xCDCDCDCD;
        /// <summary>
        /// Set to the highest db PID + 1 on server startup, incremented at runtime.
        /// </summary>
        public static uint PidCounter = User.GetHighestPid() + 1;
        /// <summary>
        /// Gathering ID for the next gathering registration.
        /// </summary>
        public static uint GathIdCounter = 0x2000;
        /// <summary>
        /// Only for Kerberos ticket, hardcoded for every user.
        /// </summary>
        public static byte[] KerberosSesKey = new byte[] { 0x9C, 0xB0, 0x1D, 0x7A, 0x2C, 0x5A, 0x6C, 0x5B, 0xED, 0x12, 0x68, 0x45, 0x69, 0xAE, 0x09, 0x0D };
        /// <summary>
        /// The list of connected clients.
        /// </summary>
        public static List<ClientInfo> Clients = new List<ClientInfo>();
        /// <summary>
        /// The list of live competitions.
        /// </summary>
        public static List<Competition> Competitions = new List<Competition>();
        /// <summary>
        /// The list of live game sessions.
        /// </summary>
        public static List<Session> Sessions = new List<Session>();
        /// <summary>
        /// The list of live game rounds.
        /// </summary>
        public static List<Round> Rounds = new List<Round>();
        /// <summary>
        /// Total server uptime.
        /// </summary>
        public static Stopwatch Uptime = new Stopwatch();
        /// <summary>
        /// Timeout-checking timer (1 min interval).
        /// </summary>
        public static Timer TimeoutTimer = new Timer(60000);

        /// <summary>
        /// Timeout threshold (1 minute).
        /// </summary>
        private static TimeSpan Timeout = new TimeSpan(0, 1, 0);

        static Global()
        {
            TimeoutTimer.Elapsed += CheckTimeout;
            TimeoutTimer.AutoReset = true;
            TimeoutTimer.Start();
        }

        private static void CheckTimeout(object sender, EventArgs args)
        {
            // Mark the timed-out client
            foreach (ClientInfo c in Clients)
            {
                if(DateTime.Now - c.LastPacketTime > Timeout)
                {
                    string log = $"[Global] Timeout check: {c.Name}";
                    c.Status = "Timeout";

                    // Remove the hosted lobby if exists
                    Session lobby = Sessions.Find(s => s.TtlSession.m_pidOwner == c.PID);
                    if(lobby != null)
                    {
                        Sessions.Remove(lobby);
                        log += $" and session {lobby.TtlSession.m_strDescription}";
                        // Remove the round if exists
                        Round round = Rounds.Find(s => s.TtlRound.m_strDescription == lobby.TtlSession.m_strDescription);
                        if (round != null)
                            Rounds.Remove(round);
                    }
                    Log.WriteLine(1, log + " removed", Color.Gray);
                }
            }

            // Remove the timed-out clients
            Clients.RemoveAll(c => c.Status == "Timeout"); 
        }

        public static void UpdateLastPacketTime(uint pid)
        {
            foreach (ClientInfo c in Clients)
            {
                if (c.PID == pid)
                {
                    c.LastPacketTime = DateTime.Now;
                    break;
                }
            }
        }

        public static ClientInfo GetClientByEndPoint(IPEndPoint ep)
        {
            foreach (ClientInfo c in Clients)
                if (c.ep.Address.ToString() == ep.Address.ToString() && c.ep.Port == ep.Port)
                    return c;
            WriteLog(1, "Cant find client for end point : " + ep.ToString());
            return null;
        }

        public static ClientInfo GetClientByIDsend(uint id)
        {
            foreach (ClientInfo c in Clients)
                if (c.IDsend == id)
                    return c;
            WriteLog(2, "Cant find client for id : 0x" + id.ToString("X8"));
            return null;
        }

        public static ClientInfo GetClientByIDrecv(uint id)
        {
            foreach (ClientInfo c in Clients)
                if (c.IDrecv == id)
                    return c;
            WriteLog(2, "Cant find client for id : 0x" + id.ToString("X8"));
            return null;
        }

        private static void WriteLog(int priority, string s)
        {
            Log.WriteLine(priority, "[Global] " + s, Color.Orange);
        }
    }
}
