using System.Drawing;
using System.IO;

namespace QuazalWV
{
    public static class EidosService
    {
        public static void ProcessEidosServiceRequest(Stream s, RMCP rmc, ClientInfo client)
        {
            switch(rmc.methodID)
            {
                case 2:
                    rmc.request = new RMCPacketRequestEidosService_FindBySQLQuery(s);
                    break;
                case 5:
                    rmc.request = new RMCPacketRequestEidosService_GetTopPlayers(s);
                    break;
                case 6:
                    rmc.request = new RMCPacketRequestEidosService_GetStatsForPlayers(s);
                    break;
                case 7:
                    rmc.request = new RMCPacketRequestEidosService_GetLadderStats(s);
                    break;
                // Empty GetAlerts request
                case 8:
                    break;
                case 9:
                    rmc.request = new RMCPacketRequestEidosService_AddStatsPersonal(s, client);
                    break;
                case 0xA:
                    rmc.request = new RMCPacketRequestEidosService_FilterText(s);
                    break;
                case 0xB:
                    rmc.request = new RMCPacketRequestEidosService_FriendInvite(s);
                    break;
                case 0xD:
                    rmc.request = new RMCPacketRequestEidosService_FriendAccept(s);
                    break;
                case 0xE:
                    rmc.request = new RMCPacketRequestEidosService_FriendReject(s);
                    break;
                case 0xF:
                    rmc.request = new RMCPacketRequestEidosService_FriendDelete(s);
                    break;
                case 0x10:
                    rmc.request = new RMCPacketRequestEidosService_FriendsQuery(s);
                    break;
                // Empty ClanGetMyClanInfo request
                case 0x11:
                    break;
                case 0x12:
                    rmc.request = new RMCPacketRequestEidosService_ClanCreateClan(s);
                    break;
                // Empty ClanGetMyClanMembershipList request
                case 0x13:
                    break;
                case 0x14:
                    rmc.request = new RMCPacketRequestEidosService_ClanGetClanIDListForPlayer(s);
                    break;
                case 0x15:
                    rmc.request = new RMCPacketRequestEidosService_ClanGetClanInfoFromClanID(s);
                    break;
                // Empty ClanGetMyInvitations request
                case 0x16:
                    break;
                case 0x17:
                    rmc.request = new RMCPacketRequestEidosService_ClanInvitePlayer(s);
                    break;
                case 0x18:
                    rmc.request = new RMCPacketRequestEidosService_ClanCancelInvitation(s);
                    break;
                case 0x19:
                    rmc.request = new RMCPacketRequestEidosService_ClanAcceptInvitation(s);
                    break;
                case 0x1A:
                    rmc.request = new RMCPacketRequestEidosService_ClanRejectInvitation(s);
                    break;
                case 0x1B:
                    rmc.request = new RMCPacketRequestEidosService_ClanBlockInvitation(s);
                    break;
                case 0x1C:
                    rmc.request = new RMCPacketRequestEidosService_ClanSetInvitationStatus(s);
                    break;
                case 0x1D:
                    rmc.request = new RMCPacketRequestEidosService_ClanRemoveMember(s);
                    break;
                case 0x1E:
                    // Empty ClanAbandonClan request
                    break;
                case 0x1F:
                    rmc.request = new RMCPacketRequestEidosService_ClanPromote(s);
                    break;
                case 0x20:
                    rmc.request = new RMCPacketRequestEidosService_ClanDemote(s);
                    break;
                case 0x21:
                    rmc.request = new RMCPacketRequestEidosService_ClanNewLeader(s);
                    break;
                case 0x22:
                    rmc.request = new RMCPacketRequestEidosService_ClanPostNotice(s);
                    break;
                case 0x23:
                    // Empty ClanGetMyNotices request
                    break;
                case 0x24:
                    // Empty ClanMarkAlertKickedRead request
                    break;
                case 0x25:
                    rmc.request = new RMCPacketRequestEidosService_GetTopClans(s);
                    break;
                case 0x26:
                    rmc.request = new RMCPacketRequestEidosService_GetMessageOfTheDay(s);
                    break;
                case 0x27:
                    // Empty GetEidosAccountInfo request
                    break;
                default:
                    Log.WriteLine(1, "[RMC Eidos] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), Color.Red, client);
                    break;
            }
        }

        public static void HandleEidosServiceRequest(QPacket p, RMCP rmc, ClientInfo client)
        {
            RMCPResponse reply;
            switch (rmc.methodID)
            {
                case 2:
                    RMCPacketRequestEidosService_FindBySQLQuery rQuerySes = (RMCPacketRequestEidosService_FindBySQLQuery)rmc.request;
                    reply = new RMCPacketResponseEidosService_FindBySQLQuery(rQuerySes.SqlQuery, client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 5:
                    RMCPacketRequestEidosService_GetTopPlayers rTopStats = (RMCPacketRequestEidosService_GetTopPlayers)rmc.request;
                    reply = new RMCPacketResponseEidosService_GetTopPlayers(client, rTopStats.NbEntries);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 6:
                    RMCPacketRequestEidosService_GetStatsForPlayers rGetStats = (RMCPacketRequestEidosService_GetStatsForPlayers)rmc.request;
                    reply = new RMCPacketResponseEidosService_GetStatsForPlayers(client, rGetStats.Pids);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 7:
                    RMCPacketRequestEidosService_GetLadderStats rSpecStats = (RMCPacketRequestEidosService_GetLadderStats)rmc.request;
                    reply = new RMCPacketResponseEidosService_GetLadderStats(client, rSpecStats.Count);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 8:
                    reply = new RMCPacketResponseEidosService_GetAlerts(client.PID);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 9:
                    RMCPacketRequestEidosService_AddStatsPersonal rAddStats = (RMCPacketRequestEidosService_AddStatsPersonal)rmc.request;
                    Log.WriteLine(2, $"[RMC Eidos] AddStatsPersonal stats payload:\n" + rAddStats.PayloadToString(), null, client);
                    // Update if it's the first update for this player
                    Session currentSession = Global.Sessions.Find(ses => ses.TtlSession.m_idMyself == client.LobbyGathId);
                    // People who joined the progressing round will send empty stat lists, ignored
                    if(rAddStats.Stats.Count == 0)
                        Log.WriteLine(1, $"[RMC Eidos] Empty stat request - player hotjoined, ignored", Color.Gray, client);
                    else if (!client.StatsUpdated)
                    {
                        // Save stats if the session is ranked (just in case, players from an unranked session don't send stats)
                        if (currentSession.TtlSession.IsRanked)
                        {
                            client.RoundStats = rAddStats.Stats;
                            StatsHelper.UpdateLifetimeStats(client, rAddStats.Stats);
                        }
                        // Subscribe for ranking update at the end of the round
                        Round round = Global.Rounds.Find(r => r.TtlRound.m_idMyself == client.RoundGathId);
                        round.RankedParticipants.Add(client);
                        client.StatsUpdated = true;
                    }
                    // Ignore duplicates until the next round begins (Participate request needed)
                    else
                        Log.WriteLine(1, $"[RMC Eidos] Stats duplicate, ignored", Color.Gray, client);
                    reply = new RMCPResponseEmpty();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0xA:
                    RMCPacketRequestEidosService_FilterText rCreateMatch = (RMCPacketRequestEidosService_FilterText)rmc.request;
                    reply = new RMCPacketResponseEidosService_FilterText();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0xB:
                    RMCPacketRequestEidosService_FriendInvite rFriendInvite = (RMCPacketRequestEidosService_FriendInvite)rmc.request;
                    // Add the invitation, false for a non-existent invitee
                    bool validFrInvite = rFriendInvite.Save(client);
                    reply = new RMCPacketResponseEidosService_FriendInvite(validFrInvite);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0xD:
                    RMCPacketRequestEidosService_FriendAccept rFriendAccept = (RMCPacketRequestEidosService_FriendAccept)rmc.request;
                    // Add friendship, delete invites
                    bool validFrAccept = rFriendAccept.AddFriendship(client.PID);
                    reply = new RMCPacketResponseEidosService_FriendAccept(validFrAccept);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0xE:
                    RMCPacketRequestEidosService_FriendReject rFriendReject = (RMCPacketRequestEidosService_FriendReject)rmc.request;
                    // Delete the invitation or mark as blocked
                    bool validFrReject = rFriendReject.RejectInvite(client.PID);
                    reply = new RMCPacketResponseEidosService_FriendReject(validFrReject);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0xF:
                    RMCPacketRequestEidosService_FriendDelete rFriendDelete = (RMCPacketRequestEidosService_FriendDelete)rmc.request;
                    // Delete a friend or cancel an outgoing friend invite
                    bool validDelete = rFriendDelete.DeleteFriendOrInvite(client.PID);
                    reply = new RMCPacketResponseEidosService_FriendDelete(validDelete);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x10:
                    RMCPacketRequestEidosService_FriendsQuery rFriends = (RMCPacketRequestEidosService_FriendsQuery)rmc.request;
                    reply = new RMCPacketResponseEidosService_FriendsQuery(rFriends.Locale, client.PID);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x11:
                    reply = new RMCPacketResponseEidosService_ClanGetMyClanInfo(client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x12:
                    // Add the clan
                    RMCPacketRequestEidosService_ClanCreateClan rCreateClan = (RMCPacketRequestEidosService_ClanCreateClan)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanCreateClan(client, rCreateClan.ClanName, rCreateClan.ClanAbbr);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x13:
                    reply = new RMCPacketResponseEidosService_ClanGetMyClanMembershipList();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x14:
                    RMCPacketRequestEidosService_ClanGetClanIDListForPlayer rGetClanIds = (RMCPacketRequestEidosService_ClanGetClanIDListForPlayer)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanGetClanIDListForPlayer(rGetClanIds.Pid);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x15:
                    RMCPacketRequestEidosService_ClanGetClanInfoFromClanID rGetClanInfoFromId = (RMCPacketRequestEidosService_ClanGetClanInfoFromClanID)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanGetClanInfoFromClanID(rGetClanInfoFromId.ClanId);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x16:
                    reply = new RMCPacketResponseEidosService_ClanGetMyInvitations(client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x17:
                    RMCPacketRequestEidosService_ClanInvitePlayer rClanInvite = (RMCPacketRequestEidosService_ClanInvitePlayer)rmc.request;
                    // Add the invitation
                    uint clanInviteResult = rClanInvite.Save(client);
                    reply = new RMCPacketResponseEidosService_ClanInvitePlayer(clanInviteResult);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x18:
                    RMCPacketRequestEidosService_ClanCancelInvitation rCancelClanInvite = (RMCPacketRequestEidosService_ClanCancelInvitation)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanCancelInvitation(client, rCancelClanInvite.PlayerToUninvite);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x19:
                    RMCPacketRequestEidosService_ClanAcceptInvitation rAcceptClanInvite = (RMCPacketRequestEidosService_ClanAcceptInvitation)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanAcceptInvitation(client, rAcceptClanInvite.ClanId, rAcceptClanInvite.Inviter);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1A:
                    RMCPacketRequestEidosService_ClanRejectInvitation rRejectClanInvite = (RMCPacketRequestEidosService_ClanRejectInvitation)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanRejectInvitation(client, rRejectClanInvite.ClanId, rRejectClanInvite.Inviter);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1B:
                    RMCPacketRequestEidosService_ClanBlockInvitation rBlockClanInvite = (RMCPacketRequestEidosService_ClanBlockInvitation)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanBlockInvitation(client, rBlockClanInvite.ClanId, rBlockClanInvite.Inviter);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1C:
                    RMCPacketRequestEidosService_ClanSetInvitationStatus rSetInviteStatus = (RMCPacketRequestEidosService_ClanSetInvitationStatus)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanSetInvitationStatus(client, rSetInviteStatus.IsAcceptingInvites);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1D:
                    RMCPacketRequestEidosService_ClanRemoveMember rRemoveClanMember = (RMCPacketRequestEidosService_ClanRemoveMember)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanRemoveMember(client, rRemoveClanMember.Pid);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1E:
                    reply = new RMCPacketResponseEidosService_ClanAbandonClan(client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x1F:
                    RMCPacketRequestEidosService_ClanPromote rClanPromote = (RMCPacketRequestEidosService_ClanPromote)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanPromote(rClanPromote.Pid);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x20:
                    RMCPacketRequestEidosService_ClanDemote rClanDemote = (RMCPacketRequestEidosService_ClanDemote)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanDemote(rClanDemote.Pid);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x21:
                    RMCPacketRequestEidosService_ClanNewLeader rClanNewLead = (RMCPacketRequestEidosService_ClanNewLeader)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanNewLeader(rClanNewLead.Pid);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x22:
                    RMCPacketRequestEidosService_ClanPostNotice rPostNotice = (RMCPacketRequestEidosService_ClanPostNotice)rmc.request;
                    reply = new RMCPacketResponseEidosService_ClanPostNotice(client, rPostNotice.NewNotice);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x23:
                    reply = new RMCPacketResponseEidosService_ClanGetMyNotices(client);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x24:
                    reply = new RMCPacketResponseEidosService_ClanMarkAlertKickedRead();
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x25:
                    RMCPacketRequestEidosService_GetTopClans rTopClans = (RMCPacketRequestEidosService_GetTopClans)rmc.request;
                    reply = new RMCPacketResponseEidosService_GetTopClans(rTopClans.Count);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x26:
                    RMCPacketRequestEidosService_GetMessageOfTheDay rGetMessageOfTheDay = (RMCPacketRequestEidosService_GetMessageOfTheDay)rmc.request;
                    reply = new RMCPacketResponseEidosService_GetMessageOfTheDay(rGetMessageOfTheDay.Language);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                case 0x27:
                    reply = new RMCPacketResponseEidosService_GetEidosAccountInfo(client.PID);
                    RMC.SendResponseWithACK(client.udp, p, rmc, client, reply, useCompression: false);
                    break;
                default:
                    Log.WriteLine(1, "[RMC Eidos] Error: Unknown Method 0x" + rmc.methodID.ToString("X"), Color.Red, client);
                    break;
            }
        }
    }
}
