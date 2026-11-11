using System.IO;

namespace QuazalWV
{
    public static class MatchMakingExtService
    {
        public static void ProcessMatchMakingExtServiceRequest(Stream s, RMCP rmc, ClientInfo client)
        {
            switch (rmc.methodID)
            {
                case 1:
                    rmc.request = new RMCPacketRequestMatchMakingExtService_EndParticipation(s);
                    break;
                default:
                    Log.WriteLine(1, "[RMC MatchMakingExtService] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), client);
                    break;
            }
        }

        public static void HandleMatchMakingExtServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 1:
                    RMCPacketRequestMatchMakingExtService_EndParticipation rEndPart = (RMCPacketRequestMatchMakingExtService_EndParticipation)rmc.request;
                    reply = new RMCPacketResponseMatchMakingExtService_EndParticipation(rEndPart.GatheringId, client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                default:
                    Log.WriteLine(1, "[RMC MatchMakingExtService] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), client);  
                    break;
            }
        }
    }
}
