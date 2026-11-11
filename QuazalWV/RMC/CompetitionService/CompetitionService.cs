using System.IO;

namespace QuazalWV
{
    public static class CompetitionService
    {
        public static void ProcessCompetitionServiceRequest(Stream s, RMCP rmc, ClientInfo client)
        {
            switch (rmc.methodID)
            {
                case 2:
                    rmc.request = new RMCPacketRequestCompetitionService_RegisterRoundToCompetition(s);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Competition] Error: Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }

        public static void HandleCompetitionServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 2:
                    RMCPacketRequestCompetitionService_RegisterRoundToCompetition rRegRound = (RMCPacketRequestCompetitionService_RegisterRoundToCompetition)rmc.request;
                    reply = new RMCPacketResponseCompetitionService_RegisterRoundToCompetition(rRegRound.CompetitionId, rRegRound.RoundId);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Competition] Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }
    }
}
