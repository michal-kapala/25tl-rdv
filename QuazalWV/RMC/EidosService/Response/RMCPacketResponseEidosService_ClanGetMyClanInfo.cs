using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_ClanGetMyClanInfo : RMCPResponse
    {
        public uint RetVal { get; set; }
        public EidosClanProps Props { get; set; }
        public EidosClanMember Player { get; set; }

        public RMCPacketResponseEidosService_ClanGetMyClanInfo(ClientInfo client)
        {
            // Success
            RetVal = (uint)EClanError.NoError;
            try
            {
                uint clanId = ClanMember.ClanIdFromPid(client.PID);
                // not a clan member
                if (clanId == 0)
                    Props = new EidosClanProps(client.ClanTag);
                else
                    Props = new Clan(clanId).ToProps();
                // checks if a clan member
                Player = ClanMember.GetEidosClanMember(client.PID) ?? new EidosClanMember(client);
            }
            catch(Exception e)
            {
                Log.WriteLine(1, $"[GetMyClanInfo] Error: {e}", Color.Red);
            }
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, RetVal);
            Props.ToBuffer(m);
            Player.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[ClanGetMyClanInfo Response, retval={RetVal}, clan={Props.ClanName}, player={Player.Player}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[ClanID : {Props.ClanID}]");
            sb.AppendLine($"\t[Clan   : {Props.ClanName}]");
            sb.AppendLine($"\t[PID    : {Player.PlayerID}]");
            sb.AppendLine($"\t[Player : {Player.Player}]");
            return sb.ToString();
        }
    }
}
