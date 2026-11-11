using System.IO;
using System.Text;

namespace QuazalWV
{
    public class RMCPacketRequestAccountService_UpdateCustomData : RMCPRequest
    {
        public User UserCustomData { get; set; }
        public AnyDataHolder<EidosAccountPublic> EidosAccPublic { get; set; }
        public AnyDataHolder<EidosAccountPrivate> EidosAccPrivate { get; set; }

        public RMCPacketRequestAccountService_UpdateCustomData(Stream s)
        {
            UserCustomData = new User();
            EidosAccPublic = new AnyDataHolder<EidosAccountPublic>(s);
            EidosAccPrivate = new AnyDataHolder<EidosAccountPrivate>(s);
            // Public data
            UserCustomData.Country = EidosAccountPublic.GetCountry(EidosAccPublic.Data.Country);
            // Private data
            EidosAccPrivate.Data.ReadFlags(UserCustomData);
            UserCustomData.Zipcode = EidosAccPrivate.Data.Zipcode;
            UserCustomData.Birthday = EidosAccPrivate.Data.Birthday;
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            EidosAccPublic.ToBuffer(m);
            EidosAccPrivate.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[UpdateCustomData Request]";
        }

        public override string PayloadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Country      : {UserCustomData.Country}]");
            sb.AppendLine($"\t[Sex          : {UserCustomData.Sex}]");
            sb.AppendLine($"\t[Eidos info   : {(UserCustomData.EidosInfo ? "yes" : "no")}]");
            sb.AppendLine($"\t[Partners info: {(UserCustomData.PartnersInfo ? "yes" : "no")}]");
            sb.AppendLine($"\t[Save password: {(UserCustomData.SavePassword ? "yes" : "no")}]");
            sb.AppendLine($"\t[Zipcode      : {UserCustomData.Zipcode}]");
            return sb.ToString();
        }

        /// <summary>
        /// Updates the account with user's form data (except for email).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Save(string name)
        {
            UserCustomData.Username = name;
            return User.UpdateCustomData(UserCustomData);
        }
    }
}
