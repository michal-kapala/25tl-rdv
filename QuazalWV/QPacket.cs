using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace QuazalWV
{
    public class QPacket
    {
        public enum STREAMTYPE
        {
            Unused,
            DO,
            RV,
            OldRVSec,
            SBMGMT,
            NAT,
            SessionDiscovery,
            NATEcho,
            Routing,
            Game,
            RVSecure,
            Relay
        }

        public enum PACKETFLAG
        {
            FLAG_ACK = 1,       //0x8
            FLAG_RELIABLE = 2,  //0x10
            FLAG_NEED_ACK = 4,  //0x20
            FLAG_HAS_SIZE = 8,  //0x40
            FLAG_UNKNOWN = 16   //0x80
        }

        public enum PACKETTYPE
        {
            SYN,
            CONNECT,
            DATA,
            DISCONNECT,
            PING,
            NATPING,
            ACK,
            RESET
        }

        public class VPort
        {
            public STREAMTYPE type;
            public byte port;
            public VPort(byte b)
            {
                type = (STREAMTYPE)(b >> 4);
                port = (byte)(b & 0xF);
            }

            public override string ToString()
            {
                return "VPort[port=" + port.ToString("D2") + " type=" + type + "]";
            }

            public byte toByte()
            {
                byte result = port;
                result |= (byte)((byte)type << 4);
                return result;
            }
        }

        public VPort m_oSourceVPort;
        public VPort m_oDestinationVPort;
        public byte m_byPacketTypeFlags;
        public PACKETTYPE type;
        public List<PACKETFLAG> flags;
        public byte m_bySessionID;
        public uint m_uiSignature;
        public ushort uiSeqId;
        public uint m_uiConnectionSignature;
        public byte m_byPartNumber;
        public ushort payloadSize;
        public byte[] payload;
        public byte checkSum;
        public bool usesCompression = true;
        public uint realSize;

        public QPacket()
        {
        }

        public QPacket(byte[] data)
        {
            MemoryStream m = new MemoryStream(data);
            m_oSourceVPort = new VPort(Helper.ReadU8(m));
            m_oDestinationVPort = new VPort(Helper.ReadU8(m));
            m_byPacketTypeFlags = Helper.ReadU8(m);
            type = (PACKETTYPE)(m_byPacketTypeFlags & 0x7);
            flags = new List<PACKETFLAG>();
            ExtractFlags();
            m_bySessionID = Helper.ReadU8(m);
            m_uiSignature = Helper.ReadU32(m);
            uiSeqId = Helper.ReadU16(m);
            if (type == PACKETTYPE.SYN || type == PACKETTYPE.CONNECT)
                m_uiConnectionSignature = Helper.ReadU32(m);
            if (type == PACKETTYPE.DATA)
                m_byPartNumber = Helper.ReadU8(m);
            if (flags.Contains(PACKETFLAG.FLAG_HAS_SIZE))
                payloadSize = Helper.ReadU16(m);
            else
                payloadSize = (ushort)(m.Length - m.Position - 1);
            MemoryStream pl = new MemoryStream();
            if (payloadSize != 0)
                for (int i = 0; i < payloadSize; i++)
                    pl.WriteByte(Helper.ReadU8(m));
            payload = pl.ToArray();
            if (payload != null && payload.Length > 0 && type != PACKETTYPE.SYN && m_oSourceVPort.type != STREAMTYPE.NAT)
            {
                //ttl doesn't use packet encryption
                /*if (m_oSourceVPort.type == STREAMTYPE.OldRVSec)
                    payload = Helper.Decrypt(Global.keyDATA, payload);*/
                usesCompression = payload[0] != 0;
                MemoryStream m2 = new MemoryStream();
                m2.Write(payload, 1, payload.Length - 1);
                payload = m2.ToArray();
                payloadSize = (ushort)payload.Length;
            }
            checkSum = Helper.ReadU8(m);
            realSize = (uint)m.Position;
        }

        public byte[] toBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU8(m, m_oSourceVPort.toByte());
            Helper.WriteU8(m, m_oDestinationVPort.toByte());
            byte typeFlag = (byte)type;
            foreach (PACKETFLAG flag in flags)
                typeFlag |= (byte)((byte)flag << 3);
            Helper.WriteU8(m, typeFlag);
            Helper.WriteU8(m, m_bySessionID);
            Helper.WriteU32(m, m_uiSignature);
            Helper.WriteU16(m, uiSeqId);
            if (type == PACKETTYPE.SYN || type == PACKETTYPE.CONNECT)
                Helper.WriteU32(m, m_uiConnectionSignature);
            if(type == PACKETTYPE.DATA)
                Helper.WriteU8(m, m_byPartNumber);
            byte[] tmpPayload = payload;
            if (tmpPayload != null && tmpPayload.Length > 0 && type != PACKETTYPE.SYN && m_oSourceVPort.type != STREAMTYPE.NAT)
            {
                MemoryStream m2 = new MemoryStream();
                m2.WriteByte(0);
                m2.Write(tmpPayload, 0, tmpPayload.Length);
                tmpPayload = m2.ToArray();
                // no encryption
                /*if (m_oSourceVPort.type == STREAMTYPE.OldRVSec)
                    tmpPayload = Helper.Encrypt(Global.keyDATA, tmpPayload);*/
            }
            if (flags.Contains(PACKETFLAG.FLAG_HAS_SIZE))
                Helper.WriteU16(m, (ushort)tmpPayload.Length);
            m.Write(tmpPayload, 0, tmpPayload.Length);
            return AddCheckSum(m.ToArray());
        }

        private byte[] AddCheckSum(byte[] buff)
        {
            byte[] result = new byte[buff.Length + 1];
            for (int i = 0; i < buff.Length; i++)
                result[i] = buff[i];
            result[buff.Length] = checkSum = MakeChecksum(buff);
            return result;
        }

        public static byte MakeChecksum(byte[] data, byte setting = 0xFF)
        {
            byte result = 0;

            uint tmp1 = 0;
            byte tmp2 = setting;
            int count1 = 0, pos = 0, length = data.Length;
            for (count1 = 0; count1 < length / 4; count1++)
            {
                tmp1 += BitConverter.ToUInt32(data, pos);
                pos += 4;
            }

            bool shouldMake2ndSum = (length & 3) != 0;
            if (shouldMake2ndSum)
            {
                int count2 = 0;
                do
                    tmp2 += data[pos + count2++];
                while (count2 < (length & 3));
            }

            result = (byte)((byte)(tmp1 >> 24) +
                     (byte)(tmp1 >> 16) +
                     (byte)(tmp1 >> 8) +
                     (byte)(tmp1) +
                     tmp2);

            return result;
        }

        private void ExtractFlags()
        {
            byte v = (byte)(m_byPacketTypeFlags >> 3);
            int[] values = (int[])Enum.GetValues(typeof(PACKETFLAG));
            for (int i = 0; i < values.Length; i++)
                if ((v & values[i]) != 0)
                    flags.Add((PACKETFLAG)values[i]);
        }

        public string GetFlagsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PACKETFLAG flag in flags)
                sb.Append("[" + flag.ToString().Replace("FLAG_","") + "]");
            return sb.ToString();
        }

        public string ToStringDetailed()
        {
            StringBuilder sb = new StringBuilder();            
            sb.AppendLine("UDPPacket {");
            sb.AppendLine("\tFrom         : " + m_oSourceVPort);
            sb.AppendLine("\tTo           : " + m_oDestinationVPort);
            sb.AppendLine("\tFlags        : " + GetFlagsString());
            sb.AppendLine("\tType         : " + type);
            sb.AppendLine("\tSession ID   : 0x" + m_bySessionID.ToString("X2"));
            sb.AppendLine("\tSignature    : 0x" + m_uiSignature.ToString("X8"));
            sb.AppendLine("\tSequence ID  : 0x" + uiSeqId.ToString("X4"));
            if (type == PACKETTYPE.SYN || type == PACKETTYPE.CONNECT)
                sb.AppendLine("\tConn. Sig.   : 0x" + m_uiConnectionSignature.ToString("X8"));
            if (type == PACKETTYPE.DATA)
                sb.AppendLine("\tPart Number  : 0x" + m_byPartNumber.ToString("X2"));
            if (flags.Contains(PACKETFLAG.FLAG_HAS_SIZE))
                sb.AppendLine("\tPayload Size : 0x" + payloadSize.ToString("X4"));
            sb.Append("\tPayLoad      : ");
            foreach (byte b in payload)
                sb.Append(b.ToString("X2") + " ");
            sb.AppendLine();
            sb.AppendLine("\tChecksum     : 0x" + checkSum.ToString("X2"));
            sb.AppendLine("}");
            return sb.ToString();     
        }

        public string ToStringShort()
        {
            return "UDPPacket { " + type + " ( " + GetFlagStringShort() + " )}";
        }

        private string GetFlagStringShort()
        {
            string s = "";
            s += flags.Contains(PACKETFLAG.FLAG_RELIABLE) ? "R" : " ";
            s += flags.Contains(PACKETFLAG.FLAG_ACK) ? "A" : " ";
            s += flags.Contains(PACKETFLAG.FLAG_NEED_ACK) ? "W" : " ";
            s += flags.Contains(PACKETFLAG.FLAG_HAS_SIZE) ? "S" : " ";
            return s;
        }
    }
}
