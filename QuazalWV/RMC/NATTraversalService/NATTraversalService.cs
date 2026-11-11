using System.IO;

namespace QuazalWV
{
    public static class NATTraversalService
    {
        public static void ProcessNATTraversalServiceRequest(Stream s, RMCP rmc)
        {
            switch (rmc.methodID)
            {
                case 1:
                    rmc.request = new RMCPacketRequestNATTraversalService_RequestProbeInitiation(s);
                    break;
                default:
                    Log.WriteLine(1, "[RMC NAT] Error: Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }

        public static void HandleNATTraversalServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 1:
                    RMCPacketRequestNATTraversalService_RequestProbeInitiation rProbeInit = (RMCPacketRequestNATTraversalService_RequestProbeInitiation)rmc.request;
                    Log.WriteLine(2, $"[RMC NAT] RequestProbeInitiation request payload:\n{rProbeInit.PayloadToString()}");
                    reply = new RMCPResponseEmpty();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply);
                    break;
                default:
                    Log.WriteLine(1, "[RMC NAT] Error: Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }
    }
}
