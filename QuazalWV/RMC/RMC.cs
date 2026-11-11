using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Drawing;

namespace QuazalWV
{
    public static class RMC
    {
        public static void HandlePacket(UdpClient udp, QPacket p)
        {
            ClientInfo client = Global.GetClientByIDrecv(p.m_uiSignature);
            if (client == null)
                return;
            client.sessionID = p.m_bySessionID;
            if (p.uiSeqId > client.seqCounter)
                client.seqCounter = p.uiSeqId;
            client.udp = udp;
            if (p.flags.Contains(QPacket.PACKETFLAG.FLAG_ACK))
                return;
            WriteLog(10, "[RMC] Handling packet...", client);

            //check for compression (experimental condition for LZO header)
            if (Helper.IsLzoBuffer(p.payload) && p.usesCompression)
            {
                //decompress
                try
                {
                    LZOCompressor lzo = new LZOCompressor();
                    MemoryStream m = new MemoryStream(lzo.Decompress(p.payload));
                    //read size and trim 0s leftover from decompression
                    uint size = Helper.ReadU32(m);
                    p.payload = Helper.TrimArrayTo(m.ToArray(), size + 4);
                    p.payloadSize = (ushort)p.payload.Length;
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[RMC] Decompression error: {e}", Color.Red, client);
                }

                //log decompressed
                StringBuilder sb = new StringBuilder();
                foreach (byte b in p.payload)
                    sb.Append(b.ToString("X2") + " ");
                WriteLog(10, $"[RMC] Decompressed payload: {sb}", client);
            }
            
            RMCP rmc = new RMCP(p);
            if (rmc.isRequest)
                HandleRequest(client, p, rmc);
            else
                HandleResponse(client, p, rmc);
        }

        public static void HandleResponse(ClientInfo client, QPacket p, RMCP rmc)
        {
            ProcessResponse(client, p, rmc);
            WriteLog(1, "Received Response : " + rmc.ToString());
        }

        public static void ProcessResponse(ClientInfo client, QPacket p, RMCP rmc)
        {
            MemoryStream m = new MemoryStream(p.payload);
            m.Seek(rmc._afterProtocolOffset, 0);
            rmc.success = m.ReadByte() == 1;
            if (rmc.success)
            {
                rmc.callID = Helper.ReadU32(m);
                rmc.methodID = Helper.ReadU32(m);
            }
            else
            {
                rmc.error = Helper.ReadU32(m);
                rmc.callID = Helper.ReadU32(m);
            }
            WriteLog(1, "Got response for Protocol " + rmc.proto + " = " + (rmc.success ? "Success" : "Fail"), client);
        }

        public static void HandleRequest(ClientInfo client, QPacket p, RMCP rmc)
        {
            ProcessRequest(client, p, rmc);
            if (rmc.callID > client.callCounterRMC)
                client.callCounterRMC = rmc.callID;
            WriteLog(1, "Received Request : " + rmc.ToString(), client);
            string payload = rmc.PayLoadToString();
            if (payload != "")
                WriteLog(5, payload);
            switch (rmc.proto)
            {
                case RMCP.PROTOCOL.AuthenticationService:
                    AuthenticationService.HandleAuthenticationServiceRequest(p, rmc, client);
                    break;
                case RMCP.PROTOCOL.SecureService:
                    SecureService.HandleSecureServiceRequest(p, rmc, client);
                    break;
                case RMCP.PROTOCOL.AccountService:
                    AccountService.HandleAccountServiceRequest(p, rmc, client);
                    break;
                case RMCP.PROTOCOL.MatchMakingService:
                    MatchMakingService.HandleMatchMakingServiceRequest(p, rmc, client);
                    break;
                case RMCP.PROTOCOL.CompetitionService:
                    CompetitionService.HandleCompetitionServiceRequest(p, rmc, client);
                    break;
                case RMCP.PROTOCOL.MatchMakingExtService:
                    MatchMakingExtService.HandleMatchMakingExtServiceRequest(p, rmc, client);
                    break;
                case RMCP.PROTOCOL.EidosService:
                    EidosService.HandleEidosServiceRequest(p, rmc, client);
                    break;
                case RMCP.PROTOCOL.NATTraversalRelayProtocol:
                    NATTraversalService.HandleNATTraversalServiceRequest(p, rmc, client);
                    break;
                default:
                    Log.WriteLine(1, "[RMC] No handler implemented for packet protocol " + rmc.proto, Color.Red, client);
                    break;
            }
        }

        public static void ProcessRequest(ClientInfo client, QPacket p, RMCP rmc)
        {
            MemoryStream m = new MemoryStream(p.payload);
            m.Seek(rmc._afterProtocolOffset, 0);
            rmc.callID = Helper.ReadU32(m);
            rmc.methodID = Helper.ReadU32(m);
            switch (rmc.proto)
            {
                case RMCP.PROTOCOL.AuthenticationService:
                    AuthenticationService.ProcessAuthenticationServiceRequest(m, rmc);
                    break;
                case RMCP.PROTOCOL.SecureService:
                    SecureService.ProcessSecureServiceRequest(m, rmc);
                    break;
                case RMCP.PROTOCOL.AccountService:
                    AccountService.ProcessAccountServiceRequest(m, rmc);
                    break;
                case RMCP.PROTOCOL.MatchMakingService:
                    MatchMakingService.ProcessMatchMakingServiceRequest(m, rmc, client);
                    break;
                case RMCP.PROTOCOL.CompetitionService:
                    CompetitionService.ProcessCompetitionServiceRequest(m, rmc, client);
                    break;
                case RMCP.PROTOCOL.MatchMakingExtService:
                    MatchMakingExtService.ProcessMatchMakingExtServiceRequest(m, rmc, client);
                    break;
                case RMCP.PROTOCOL.EidosService:
                    EidosService.ProcessEidosServiceRequest(m, rmc, client);
                    break;
                case RMCP.PROTOCOL.NATTraversalRelayProtocol:
                    NATTraversalService.ProcessNATTraversalServiceRequest(m, rmc);
                    break;
                default:
                    Log.WriteLine(1, "[RMC] No request reader implemented for packet protocol " + rmc.proto, Color.Red, client);
                    break;
            }
        }


        public static void SendResponseWithACK(UdpClient udp, QPacket p, RMCP rmc, ClientInfo client, RMCPResponse reply, bool useCompression = true, uint error = 0)
        {
            WriteLog(2, "Response : " + reply.ToString(), client);
            string payload = reply.PayloadToString();
            if (payload != "")
                WriteLog(5, "Response Data Content : \n" + payload, client);
            SendACK(udp, p, client);
            SendResponsePacket(udp, p, rmc, client, reply, error, useCompression);
        }

        private static void SendACK(UdpClient udp, QPacket p, ClientInfo client)
        {
            QPacket np = new QPacket(p.toBuffer())
            {
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_ACK },
                m_oSourceVPort = p.m_oDestinationVPort,
                m_oDestinationVPort = p.m_oSourceVPort,
                m_uiSignature = client.IDsend,
                payload = new byte[0],
                payloadSize = 0
            };
            WriteLog(10, "sent ACK packet");
            Send(udp, np, client);
        }

        private static void SendResponsePacket(UdpClient udp, QPacket p, RMCP rmc, ClientInfo client, RMCPResponse reply, uint error, bool useCompression)
        {
            MemoryStream m = new MemoryStream();
            if ((ushort)rmc.proto < 0x7F)
            {
                Helper.WriteU8(m, (byte)rmc.proto);
            }
            else
            {
                Helper.WriteU8(m, 0x7F);
                Helper.WriteU16(m, (ushort)rmc.proto);
            }
            byte[] buff;
            if (error == 0)
            {
                Helper.WriteU8(m, 0x1);
                Helper.WriteU32(m, rmc.callID);
                Helper.WriteU32(m, rmc.methodID | 0x8000);
                buff = reply.ToBuffer();
                if(buff != null) m.Write(buff, 0, buff.Length);                
            }
            else
            {
                Helper.WriteU8(m, 0);
                Helper.WriteU32(m, error);
                Helper.WriteU32(m, rmc.callID);
            } 
            buff = m.ToArray();
            m = new MemoryStream();
            Helper.WriteU32(m, (uint)buff.Length);
            m.Write(buff, 0, buff.Length);
            byte[] responseBuff = m.ToArray();
            m.SetLength(0);

            //experimental condition for compression, the threshold may be higher
            if (responseBuff.Length > 23 && useCompression)
            {
                WriteLog(10, $"[RMC] Uncompressed response payload: {Helper.BufferToString(responseBuff)}", client);

                WriteLog(10, $"[RMC] Compressing response...", client);
                try
                {
                    LZOCompressor lzo = new LZOCompressor();
                    responseBuff = lzo.Compress(responseBuff);
                    responseBuff = Helper.DowngradeLzoCompressedBuffer(responseBuff);

                    // WriteLog(10, $"[RMC] Compressed payload: {Helper.BufferToString(responseBuff)}", client);
                }
                catch (Exception e)
                {
                    Log.WriteLine(1, $"[RMC] Compression error: {e}", Color.Red, client);
                }
            }

            QPacket np = new QPacket(p.toBuffer())
            {
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_NEED_ACK },
                m_oSourceVPort = p.m_oDestinationVPort,
                m_oDestinationVPort = p.m_oSourceVPort,
                m_uiSignature = client.IDsend
            };
            MakeAndSend(client, np, responseBuff);
        }
        
        public static void SendRequestPacket(UdpClient udp, QPacket p, RMCP rmc, ClientInfo client, RMCPResponse packet, bool useCompression, uint error)
        {
            MemoryStream m = new MemoryStream();
            if ((ushort)rmc.proto < 0x7F)
                Helper.WriteU8(m, (byte)((byte)rmc.proto | 0x80));
            else
            {
                Helper.WriteU8(m, 0xFF);
                Helper.WriteU16(m, (ushort)rmc.proto);
            }
            byte[] buff;
            if (error == 0)
            {
                Helper.WriteU32(m, rmc.callID);
                Helper.WriteU32(m, rmc.methodID);
                buff = packet.ToBuffer();
                m.Write(buff, 0, buff.Length);
            }
            else
            {
                Helper.WriteU32(m, error);
                Helper.WriteU32(m, rmc.callID);
            }
            buff = m.ToArray();
            m = new MemoryStream();
            Helper.WriteU32(m, (uint)buff.Length);
            m.Write(buff, 0, buff.Length);
            QPacket np = new QPacket(p.toBuffer())
            {
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_NEED_ACK },
                m_uiSignature = client.IDsend
            };
            MakeAndSend(client, np, m.ToArray());
        }

        public static void MakeAndSend(ClientInfo client, QPacket np, byte[] data)
        {
            MemoryStream m = new MemoryStream(data);
            if (data.Length < 0x3C3)
            {
                np.uiSeqId++;
                np.payload = data;
                np.payloadSize = (ushort)np.payload.Length;
                WriteLog(10, "sent packet");
                Send(client.udp, np, client);
            }
            else
            {
                np.flags.Add(QPacket.PACKETFLAG.FLAG_HAS_SIZE);
                int pos = 0;
                m.Seek(0, 0);
                np.m_byPartNumber = 0;
                while (pos < data.Length)
                {
                    np.uiSeqId++;
                    bool isLast = false;
                    int len = 0x3C3;
                    if (len + pos >= data.Length)
                    {
                        len = data.Length - pos;
                        isLast = true;
                    }
                    if (!isLast)
                        np.m_byPartNumber++;
                    else
                        np.m_byPartNumber = 0;
                    byte[] buff = new byte[len];
                    m.Read(buff, 0, len);
                    np.payload = buff;
                    np.payloadSize = (ushort)np.payload.Length;
                    Send(client.udp, np, client);
                    pos += len;
                }
                WriteLog(10, "sent packets");
            }
        }

        public static void Send(UdpClient udp, QPacket p, ClientInfo client)
        {
            byte[] data = p.toBuffer();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
                sb.Append(b.ToString("X2") + " ");
            WriteLog(5, "sent : " + p.ToStringShort());
            WriteLog(10, "sent : " + sb.ToString());
            WriteLog(10, "sent : " + p.ToStringDetailed());
            udp.Send(data, data.Length, client.ep);
            Log.LogPacket(true, data);
        }

        public static void SendNotification(ClientInfo client, uint source, uint type, uint subType, uint param1, uint param2, uint param3, string paramStr)
        {
            WriteLog(1, "Send Notification: [" + source.ToString("X8") + " " 
                                         + type.ToString("X8") + " "
                                         + subType.ToString("X8") + " " 
                                         + param1.ToString("X8") + " "
                                         + param2.ToString("X8") + " "
                                         + param3.ToString("X8") + " \""
                                         + paramStr + "\"]", client);
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, source);
            Helper.WriteU32(m, type * 1000 + subType);
            Helper.WriteU32(m, param1);
            Helper.WriteU32(m, param2);
            Helper.WriteU16(m, (ushort)(paramStr.Length + 1));
            foreach (char c in paramStr)
                m.WriteByte((byte)c);
            m.WriteByte(0);
            Helper.WriteU32(m, param3);
            byte[] payload = m.ToArray();
            QPacket q = new QPacket
            {
                m_oSourceVPort = new QPacket.VPort(0x31),
                m_oDestinationVPort = new QPacket.VPort(0x3f),
                type = QPacket.PACKETTYPE.DATA,
                flags = new List<QPacket.PACKETFLAG>(),
                payload = new byte[0],
                uiSeqId = (ushort)(++client.seqCounter),
                m_bySessionID = client.sessionID
            };
            RMCP rmc = new RMCP
            {
                proto = RMCP.PROTOCOL.GlobalNotificationEventProtocol,
                methodID = 1,
                callID = ++client.callCounterRMC
            };
            RMCPCustom reply = new RMCPCustom
            {
                buffer = payload
            };
            SendRequestPacket(client.udp, q, rmc, client, reply, true, 0);
        }

        private static void WriteLog(int priority, string s, ClientInfo client = null)
        {
            Log.WriteLine(priority, "[RMC] " + s, null, client);
        }

    }
}
