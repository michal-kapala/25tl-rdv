using System.Drawing;
using System.IO;

namespace QuazalWV
{
    public static class AuthenticationService
    {
        public static void ProcessAuthenticationServiceRequest(Stream s, RMCP rmc)
        {
            switch (rmc.methodID)
            {
                case 1:
                    rmc.request = new RMCPacketRequestAuthenticationService_Login(s);
                    break;
                case 3:
                    rmc.request = new RMCPacketRequestAuthenticationService_RequestTicket(s);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Authentication] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), Color.Red);
                    break;
            }
        }

        public static void HandleAuthenticationServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 1:
                    RMCPacketRequestAuthenticationService_Login userQuery = (RMCPacketRequestAuthenticationService_Login)rmc.request;
                    User queryResult = User.GetByName(userQuery.username);
                    // If the account exists, set user data
                    if(queryResult != null)
                    {
                        client.PID = queryResult.Pid;
                        client.Name = queryResult.Username;
                        client.Pass = queryResult.Password;
                        client.ClanTag = queryResult.ClanTag;
                        client.AcceptClanInvites = queryResult.AcceptingClanInvites;
                    }
                    // Guest account
                    else
                    {
                        client.Name = ((RMCPacketRequestAuthenticationService_Login)rmc.request).username;
                    }
                    
                    reply = new RMCPacketResponseAuthenticationService_Login(client);
                    client.sessionKey = ((RMCPacketResponseAuthenticationService_Login)reply).ticket.sessionKey;
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 3:
                    reply = new RMCPacketResponseAuthenticationService_RequestTicket(client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Authentication] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), Color.Red, client);
                    break;
            }
        }
    }
}
