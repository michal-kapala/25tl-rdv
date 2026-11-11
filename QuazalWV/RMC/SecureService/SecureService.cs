using System.IO;

namespace QuazalWV
{
    public static class SecureService
    {
        public static void ProcessSecureServiceRequest(Stream s, RMCP rmc)
        {
            switch (rmc.methodID)
            {
                case 1:
                    rmc.request = new RMCPacketRequestSecureService_Register(s);
                    break;
                case 3:
                    rmc.request = new RMCPacketRequestSecureService_RequestUrls(s);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Secure] Error: Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }


        public static void HandleSecureServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 1:
                    RMCPacketRequestSecureService_Register rReg = (RMCPacketRequestSecureService_Register)rmc.request;
                    // Change ip addresses to external and save urls
                    rReg.SetUrls(client);
                    // Response is the same as in RegisterEx
                    reply = new RMCPacketResponseSecureService_Register(client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 3:
                    RMCPacketRequestSecureService_RequestUrls rReqUrls = (RMCPacketRequestSecureService_RequestUrls)rmc.request;
                    reply = new RMCPacketResponseSecureService_RequestUrls(rReqUrls.PID);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Secure] Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }
    }
}
