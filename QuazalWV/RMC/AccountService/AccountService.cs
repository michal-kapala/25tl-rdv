using System.IO;
using System.Drawing;

namespace QuazalWV
{
    public static class AccountService
    {
        public static void ProcessAccountServiceRequest(Stream s, RMCP rmc)
        {
            switch (rmc.methodID)
            {
                case 1:
                    rmc.request = new RMCPacketRequestAccountService_CreateAccount(s);
                    break;
                case 4:
                    rmc.request = new RMCPacketRequestAccountService_ChangePassword(s);
                    break;
                case 0xC:
                    rmc.request = new RMCPacketRequestAccountService_UpdateAccountEmail(s);
                    break;
                case 0xD:
                    rmc.request = new RMCPacketRequestAccountService_UpdateCustomData(s);
                    break;
                case 0xE:
                    rmc.request = new RMCPacketRequestAccountService_FindByNameRegex(s);
                    break;
                case 0x11:
                    rmc.request = new RMCPacketRequestAccountService_UpdateStatus(s);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Account] Error: Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }

        public static void HandleAccountServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 1:
                    RMCPacketRequestAccountService_CreateAccount rCreateAcc = (RMCPacketRequestAccountService_CreateAccount)rmc.request;
                    // Set client's name and password key
                    client.Name = rCreateAcc.Username;
                    client.Pass = rCreateAcc.PassHash;
                    // Add the new user to database
                    User user = new User
                    {
                        Pid = Global.PidCounter++,
                        Username = rCreateAcc.Username,
                        Password = rCreateAcc.PassHash,
                        Groups = rCreateAcc.Groups
                    };
                    client.PID = user.Pid;
                    bool bCreated = User.Add(user);
                    reply = new RMCPacketResponseAccountService_CreateAccount(bCreated);
                    // Timeout if the accout could not be created (the game is dumb and ignores response data)
                    if(bCreated)
                    {
                        // Add clan invite availability flag
                        User.SetAcceptClanInvites(client, true);
                        // Create a stats record
                        StatsHelper.AddDefault(client);
                        RMC.SendResponseWithACK(client.udp, p, rmc, client, reply);
                    }
                    break;
                case 4:
                    RMCPacketRequestAccountService_ChangePassword rChangePass = (RMCPacketRequestAccountService_ChangePassword)rmc.request;
                    // Update user's password hash
                    rChangePass.Save(client.Name);
                    reply = new RMCPacketResponseAccountService_ChangePassword();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply);
                    break;
                case 0xC:
                    RMCPacketRequestAccountService_UpdateAccountEmail rUpdateEmail = (RMCPacketRequestAccountService_UpdateAccountEmail)rmc.request;
                    // Update user's email
                    rUpdateEmail.Save(client);
                    reply = new RMCPacketResponseAccountService_UpdateAccountEmail();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply);
                    break;
                case 0xD:
                    RMCPacketRequestAccountService_UpdateCustomData rCustomData = (RMCPacketRequestAccountService_UpdateCustomData)rmc.request;
                    // Update user's details and preferences
                    rCustomData.Save(client.Name);
                    reply = new RMCPacketResponseAccountService_UpdateCustomData();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply);
                    break;
                case 0xE:
                    RMCPacketRequestAccountService_FindByNameRegex rRegex =(RMCPacketRequestAccountService_FindByNameRegex)rmc.request;
                    // Look the user up for pid
                    reply = new RMCPacketResponseAccountService_FindByNameRegex(rRegex.GetBasicAccountInfos());
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x11:
                    RMCPacketRequestAccountService_UpdateStatus rUpdateStatus = (RMCPacketRequestAccountService_UpdateStatus)rmc.request;
                    // Update user's status
                    rUpdateStatus.Save(client.PID);
                    // Cache ranking on every login and account creation
                    client.Rating = StatsHelper.GetRanking(client);
                    reply = new RMCPResponseEmpty();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Account] Unknown Method 0x" + rmc.methodID.ToString("X"));
                    break;
            }
        }
    }
    
}
