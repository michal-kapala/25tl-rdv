using System;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class EidosAccountPrivate : IData
    {
        public byte Flags { get; set; }
        public string Zipcode { get; set; }
        public long Birthday { get; set; }

        public EidosAccountPrivate()
        {
            Flags = 0;
            Zipcode = "Default zipcode, coders bad";
            Birthday = 1010962463;
        }

        public EidosAccountPrivate(Stream s)
        {
            FromStream(s);
        }

        /// <summary>
        /// Maps EidosAccountPrivate with User model.
        /// </summary>
        /// <param name="model"></param>
        public EidosAccountPrivate(User model)
        {
            Flags = 0;
            Flags |= (byte)(model.Sex == "male" ? 1 : 0);
            Flags |= (byte)(model.EidosInfo ? 2 : 0);
            Flags |= (byte)(model.PartnersInfo ? 4 : 0);
            Flags |= (byte)(model.SavePassword ? 8 : 0);
            Zipcode = model.Zipcode;
            Birthday = model.Birthday;
        }

        public void FromStream(Stream s)
        {
            Flags = Helper.ReadU8(s);
            Zipcode = Helper.ReadString(s);
            Birthday = DecodeDateToTimestamp(Helper.ReadU64(s));
        }

        public uint GetBufferSize()
        {
            return 9 + (uint)Zipcode.Length;
        }

        public string GetClass()
        {
            return "EidosAccountPrivate";
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU8(s, Flags);
            Helper.WriteString(s, Zipcode);
            Helper.WriteU64(s, EncodeTimestampToDate(Birthday));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"");
            return sb.ToString();
        }

        public void ReadFlags(User data)
        {
            data.Sex = (Flags & 1) > 0 ? "male" : "female";
            data.EidosInfo = (Flags & 2) > 0;
            data.PartnersInfo = (Flags & 4) > 0;
            data.SavePassword = (Flags & 8) > 0;
        }

        public static long DecodeDateToTimestamp(ulong encoded)
        {
            // Ignore upper DWORD padding if present
            if ((encoded >> 32) == uint.MaxValue)
                encoded &= 0xFFFFFFFFUL;

            uint yearOffset = (uint)(encoded >> 26);
            uint month = (uint)((encoded >> 22) & 0xF);
            uint day = (uint)((encoded >> 17) & 0x1F);
            uint baseYear = (uint)(encoded & 0xFF) * 64;
            int year = (int)(baseYear + yearOffset);
            var date = new DateTime(
                year, (int)month, (int)day,
                0, 0, 0, DateTimeKind.Utc);

            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }

        public static ulong EncodeTimestampToDate(long timestamp)
        {
            var date = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            uint yearOffset = (uint)(date.Year % 64);
            uint baseYear = (uint)(date.Year / 64);

            return ((ulong)yearOffset << 26)
                 | ((ulong)date.Month << 22)
                 | ((ulong)date.Day << 17)
                 // unknown constant sequence
                 | 0x1000
                 | baseYear;
        }
    }
}
