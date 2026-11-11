using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestEidosService_AddStatsPersonal : RMCPRequest
    {
        public string Player { get; set; }
        public List<float> Stats { get; set; }
        
        public RMCPacketRequestEidosService_AddStatsPersonal(Stream s, ClientInfo client)
        {
            Stats = new List<float>();
            Player = client.Name;
            uint count = Helper.ReadU32(s);
            for (uint i = 0; i < count; i++)
                Stats.Add(Helper.ReadFloat(s));
        }

        public override string ToString()
        {
            return $"[AddStatsPersonal Request player={Player}]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Player: {Player}]");
            sb.AppendLine($"\t[Nb of stats: {Stats.Count}]");
            uint count = 1;
            foreach (float stat in Stats)
            {
                switch(count)
                {
                    case 2:
                        sb.AppendLine($"\t\t[Victory as law: {stat}]");
                        break;
                    case 3:
                        sb.AppendLine($"\t\t[Defeat as law: {stat}]");
                        break;
                    case 4:
                        sb.AppendLine($"\t\t[Draw as law: {stat}]");
                        break;
                    case 5:
                        sb.AppendLine($"\t\t[Victory as criminal: {stat}]");
                        break;
                    case 6:
                        sb.AppendLine($"\t\t[Defeat as criminal: {stat}]");
                        break;
                    case 7:
                        sb.AppendLine($"\t\t[Draw as criminal: {stat}]");
                        break;
                    case 8:
                        sb.AppendLine($"\t\t[Kills as law: {stat}]");
                        break;
                    case 9:
                        sb.AppendLine($"\t\t[Kills as criminal: {stat}]");
                        break;
                    case 10:
                        sb.AppendLine($"\t\t[Deaths as law: {stat}]");
                        break;
                    case 11:
                        sb.AppendLine($"\t\t[Deaths as criminal: {stat}]");
                        break;
                    case 12:
                        sb.AppendLine($"\t\t[Suicides as law: {stat}]");
                        break;
                    case 13:
                        sb.AppendLine($"\t\t[Suicides as criminal: {stat}]");
                        break;
                    case 14:
                        sb.AppendLine($"\t\t[Headshots as law: {stat}]");
                        break;
                    case 15:
                        sb.AppendLine($"\t\t[Headshots as criminal: {stat}]");
                        break;
                    case 16:
                        sb.AppendLine($"\t\t[Teamkills as law: {stat}]");
                        break;
                    case 17:
                        sb.AppendLine($"\t\t[Teamkills as criminal: {stat}]");
                        break;
                    case 18:
                        sb.AppendLine($"\t\t[Team deaths as law: {stat}]");
                        break;
                    case 19:
                        sb.AppendLine($"\t\t[Team deaths as criminal: {stat}]");
                        break;
                    case 20:
                        sb.AppendLine($"\t\t[Killed hostages as law: {stat}]");
                        break;
                    case 21:
                        sb.AppendLine($"\t\t[Killed hostages as criminal: {stat}]");
                        break;
                    case 22:
                        sb.AppendLine($"\t\t[Apprehensions: {stat}]");
                        break;
                    case 23:
                        sb.AppendLine($"\t\t[Apprehended: {stat}]");
                        break;
                    case 24:
                        sb.AppendLine($"\t\t[Shots fired as law: {stat}]");
                        break;
                    case 25:
                        sb.AppendLine($"\t\t[Shots fired as criminal: {stat}]");
                        break;
                    case 26:
                        sb.AppendLine($"\t\t[Shots hit as law: {stat}]");
                        break;
                    case 27:
                        sb.AppendLine($"\t\t[Shots hit as criminal: {stat}]");
                        break;
                    case 28:
                        sb.AppendLine($"\t\t[Time as law: {stat}]");
                        break;
                    case 29:
                        sb.AppendLine($"\t\t[Time as criminal: {stat}]");
                        break;
                    case 30:
                        sb.AppendLine($"\t\t[Goods: {stat}]");
                        break;
                    case 31:
                        sb.AppendLine($"\t\t[Evidence: {stat}]");
                        break;
                    case 32:
                        sb.AppendLine($"\t\t[Tags: {stat}]");
                        break;
                    case 33:
                        sb.AppendLine($"\t\t[Melee kills as law: {stat}]");
                        break;
                    case 34:
                        sb.AppendLine($"\t\t[Grenade kills as law: {stat}]");
                        break;
                    case 35:
                        sb.AppendLine($"\t\t[Non-auto kills as law: {stat}]");
                        break;
                    case 36:
                        sb.AppendLine($"\t\t[MP kills as law: {stat}]");
                        break;
                    case 37:
                        sb.AppendLine($"\t\t[Assault rifle kills as law: {stat}]");
                        break;
                    case 38:
                        sb.AppendLine($"\t\t[Shotgun kills as law: {stat}]");
                        break;
                    case 39:
                        sb.AppendLine($"\t\t[Melee kills as criminal: {stat}]");
                        break;
                    case 40:
                        sb.AppendLine($"\t\t[Grenade kills as criminal: {stat}]");
                        break;
                    case 41:
                        sb.AppendLine($"\t\t[Non-auto kills as criminal: {stat}]");
                        break;
                    case 42:
                        sb.AppendLine($"\t\t[MP kills as criminal: {stat}]");
                        break;
                    case 43:
                        sb.AppendLine($"\t\t[Assault rifle kills as criminal: {stat}]");
                        break;
                    case 44:
                        sb.AppendLine($"\t\t[Shotgun kills as criminal: {stat}]");
                        break;
                    case 45:
                        sb.AppendLine($"\t\t[Deaths holding melee as law: {stat}]");
                        break;
                    case 46:
                        sb.AppendLine($"\t\t[Deaths holding grenade as law: {stat}]");
                        break;
                    case 47:
                        sb.AppendLine($"\t\t[Deaths holding non-auto as law: {stat}]");
                        break;
                    case 48:
                        sb.AppendLine($"\t\t[Deaths holding MP as law: {stat}]");
                        break;
                    case 49:
                        sb.AppendLine($"\t\t[Deaths holding AR as law: {stat}]");
                        break;
                    case 50:
                        sb.AppendLine($"\t\t[Deaths holding shotgun as law: {stat}]");
                        break;
                    case 51:
                        sb.AppendLine($"\t\t[Deaths holding melee as criminal: {stat}]");
                        break;
                    case 52:
                        sb.AppendLine($"\t\t[Deaths holding grenade as criminal: {stat}]");
                        break;
                    case 53:
                        sb.AppendLine($"\t\t[Deaths holding non-auto as criminal: {stat}]");
                        break;
                    case 54:
                        sb.AppendLine($"\t\t[Deaths holding MP as criminal: {stat}]");
                        break;
                    case 55:
                        sb.AppendLine($"\t\t[Deaths holding AR as criminal: {stat}]");
                        break;
                    case 56:
                        sb.AppendLine($"\t\t[Deaths holding shotgun as criminal: {stat}]");
                        break;
                    case 57:
                        sb.AppendLine($"\t\t[Melee deaths as law: {stat}]");
                        break;
                    case 58:
                        sb.AppendLine($"\t\t[Grenade deaths as law: {stat}]");
                        break;
                    case 59:
                        sb.AppendLine($"\t\t[Non-auto deaths as law: {stat}]");
                        break;
                    case 60:
                        sb.AppendLine($"\t\t[MP deaths as law: {stat}]");
                        break;
                    case 61:
                        sb.AppendLine($"\t\t[Assault rifle deaths as law: {stat}]");
                        break;
                    case 62:
                        sb.AppendLine($"\t\t[Shotgun deaths as law: {stat}]");
                        break;
                    case 63:
                        sb.AppendLine($"\t\t[Melee deaths as criminal: {stat}]");
                        break;
                    case 64:
                        sb.AppendLine($"\t\t[Grenade deaths as criminal: {stat}]");
                        break;
                    case 65:
                        sb.AppendLine($"\t\t[Non-auto deaths as criminal: {stat}]");
                        break;
                    case 66:
                        sb.AppendLine($"\t\t[MP deaths as criminal: {stat}]");
                        break;
                    case 67:
                        sb.AppendLine($"\t\t[Assault rifle deaths as criminal: {stat}]");
                        break;
                    case 68:
                        sb.AppendLine($"\t\t[Shotgun deaths as criminal: {stat}]");
                        break;
                    case 69:
                        sb.AppendLine($"\t\t[Tag MVP: {stat}]");
                        break;
                    case 70:
                        sb.AppendLine($"\t\t[Tag mode: {stat}]");
                        break;
                    case 71:
                        sb.AppendLine($"\t\t[Tag round time: {stat}]");
                        break;
                    case 72:
                        sb.AppendLine($"\t\t[War MVP: {stat}]");
                        break;
                    case 73:
                        sb.AppendLine($"\t\t[War mode: {stat}]");
                        break;
                    case 74:
                        sb.AppendLine($"\t\t[War round time: {stat}]");
                        break;
                    case 75:
                        sb.AppendLine($"\t\t[Robbery MVP: {stat}]");
                        break;
                    case 76:
                        sb.AppendLine($"\t\t[Robbery mode: {stat}]");
                        break;
                    case 77:
                        sb.AppendLine($"\t\t[Robbery round time: {stat}]");
                        break;
                    case 78:
                        sb.AppendLine($"\t\t[Raid MVP: {stat}]");
                        break;
                    case 79:
                        sb.AppendLine($"\t\t[Raid mode: {stat}]");
                        break;
                    case 80:
                        sb.AppendLine($"\t\t[Raid round time: {stat}]");
                        break;
                    default:
                        sb.AppendLine($"\t\t[Stat{count}: {stat}]");
                        break;
                }
                count++;
            }
                
            return sb.ToString();
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)Stats.Count);
            foreach (float s in Stats)
                Helper.WriteFloat(m, s);
            return m.ToArray();
        }
    }
}
