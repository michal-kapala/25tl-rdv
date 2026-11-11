using System;
using System.Drawing;
using System.IO;

namespace QuazalWV
{
    public static class MatchMakingService
    {
        public static void ProcessMatchMakingServiceRequest(Stream s, RMCP rmc, ClientInfo client)
        {
            switch(rmc.methodID)
            {
                case 1:
                    rmc.request = new RMCPacketRequestMatchMakingService_RegisterGathering(s, client);
                    break;
                case 4:
                    rmc.request = new RMCPacketRequestMatchMakingService_UpdateGathering(s, client);
                    break;
                case 5:
                    rmc.request = new RMCPacketRequestMatchMakingService_Invite(s);
                    break;
                case 0xB:
                    rmc.request = new RMCPacketRequestMatchMakingService_Participate(s);
                    break;
                case 0xC:
                    rmc.request = new RMCPacketRequestMatchMakingService_CancelParticipation(s);
                    break;
                case 0x10:
                    rmc.request = new RMCPacketRequestMatchMakingService_GetParticipantsURLs(s);
                    break;
                case 0x12:
                    rmc.request = new RMCPacketRequestMatchMakingService_FindByDescription(s);
                    break;
                case 0x15:
                    rmc.request = new RMCPacketRequestMatchMakingService_FindBySingleID(s);
                    break;
                case 0x18:
                    rmc.request = new RMCPacketRequestMatchMakingService_FindInvitations(s);
                    break;
                case 0x1A:
                    rmc.request = new RMCPacketRequestMatchMakingService_LaunchSession(s);
                    break;
                case 0x1B:
                    rmc.request = new RMCPacketRequestMatchMakingService_UpdateSessionURL(s);
                    break;
                case 0x1E:
                    rmc.request = new RMCPacketRequestMatchMakingService_SetState(s);
                    break;
                case 0x1F:
                    rmc.request = new RMCPacketRequestMatchMakingService_ReportStats(s);
                    break;
                default:
                    Log.WriteLine(1, "[RMC MatchMakingService] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), Color.Red, client);
                    break;
            }
        }

        public static void HandleMatchMakingServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 1:
                    RMCPacketRequestMatchMakingService_RegisterGathering rRegGath = (RMCPacketRequestMatchMakingService_RegisterGathering)rmc.request;
                    // handles lobby hijacks (accidental or not)
                    // doesnt assign Gath ID on hijack attempts, results in 'Failed to request participation in match' error
                    reply = new RMCPacketResponseMatchMakingService_RegisterGathering(rRegGath.Gathering, rRegGath.Header.DataClass);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 4:
                    // TODO: Updates to be rechecked (session not found still happens sporadically)
                    RMCPacketRequestMatchMakingService_UpdateGathering rUpGath = (RMCPacketRequestMatchMakingService_UpdateGathering)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_UpdateGathering();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 5:
                    RMCPacketRequestMatchMakingService_Invite rInvite = (RMCPacketRequestMatchMakingService_Invite)rmc.request;
                    // Add invite to database to be polled into EidosAlerts
                    rInvite.Save(client.PID);
                    reply = new RMCPacketResponseMatchMakingService_Invite();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0xB:
                    RMCPacketRequestMatchMakingService_Participate rParticipate = (RMCPacketRequestMatchMakingService_Participate)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_Participate(rParticipate.GathId, rParticipate.Message, client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0xC:
                    RMCPacketRequestMatchMakingService_CancelParticipation rCancel = (RMCPacketRequestMatchMakingService_CancelParticipation)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_CancelParticipation(rCancel.GathId, client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x10:
                    RMCPacketRequestMatchMakingService_GetParticipantsURLs rGetUrls = (RMCPacketRequestMatchMakingService_GetParticipantsURLs)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_GetParticipantsURLs(rGetUrls.GathId, client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x12:
                    RMCPacketRequestMatchMakingService_FindByDescription rFindDesc = (RMCPacketRequestMatchMakingService_FindByDescription)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_FindByDescription(rFindDesc);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x15:
                    RMCPacketRequestMatchMakingService_FindBySingleID rSingleId = (RMCPacketRequestMatchMakingService_FindBySingleID)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_FindBySingleID(rSingleId.GathId);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x18:
                    RMCPacketRequestMatchMakingService_FindInvitations rFindInv = (RMCPacketRequestMatchMakingService_FindInvitations)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_FindInvitations(rFindInv.Offset, rFindInv.Length);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1A:
                    RMCPacketRequestMatchMakingService_LaunchSession rLaunchSes = (RMCPacketRequestMatchMakingService_LaunchSession)rmc.request;
                    reply = new RMCPacketResponseMatchMakingService_LaunchSession(rLaunchSes.GatheringId, rLaunchSes.SessionUrl, client.PID);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1B:
                    RMCPacketRequestMatchMakingService_UpdateSessionURL rUpdateSesUrl = (RMCPacketRequestMatchMakingService_UpdateSessionURL)rmc.request;
                    // Host migration
                    // The host migration updates are perfored on EndParticipation response or without it after a host crash/critical connection error
                    reply = new RMCPacketResponseMatchMakingService_UpdateSessionURL();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1E:
                    RMCPacketRequestMatchMakingService_SetState rSetState = (RMCPacketRequestMatchMakingService_SetState)rmc.request;
                    // Set round's average ranking
                    Round round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == rSetState.GathId);
                    float roundElo = 0;
                    foreach(ClientInfo player in round.Participants)
                        roundElo += player.Rating;
                    round.TtlRound.Rating = Convert.ToUInt32(roundElo / round.Participants.Count);
                    reply = new RMCPacketResponseMatchMakingService_SetState(rSetState.GathId, rSetState.State);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1F:
                    RMCPacketRequestMatchMakingService_ReportStats rRepStats = (RMCPacketRequestMatchMakingService_ReportStats)rmc.request;
                    // Do something with the stats (what are they even?)
                    Log.WriteLine(2, "[RMC MatchMaking] ReportStats payload:\n" + rRepStats.PayloadToString(), null, client);
                    reply = new RMCPacketResponseMatchMakingService_ReportStats(rRepStats.GathId, rRepStats.GathStats);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                default:
                    Log.WriteLine(1, "[RMC MatchMakingService] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), Color.Red, client);
                    break;
            }
        }
    }
}
