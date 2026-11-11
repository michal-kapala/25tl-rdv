using System.Drawing;
using System.IO;
using System.Text;

namespace QuazalWV
{
    public class EidosAccountPublic : IData
    {
        public uint Country { get; set; }

        public EidosAccountPublic()
        {
            Country = (uint)ECountryCode.US;
        }

        public EidosAccountPublic(Stream s)
        {
            FromStream(s);
        }

        /// <summary>
        /// Maps EidosAccountPublic with User model.
        /// </summary>
        /// <param name="model"></param>
        public EidosAccountPublic(User model)
        {
            Country = GetCode(model.Country);
        }

        public void FromStream(Stream s)
        {
            Country = Helper.ReadU32(s);
        }

        public uint GetBufferSize()
        {
            return 4;
        }

        public string GetClass()
        {
            return "EidosAccountPublic";
        }

        public void ToBuffer(Stream s)
        {
            Helper.WriteU32(s, Country);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\t[Country: {(ECountryCode)Country}]");
            return sb.ToString();
        }

        public static string GetCountry(uint code)
        {
            ECountryCode country = (ECountryCode)code;
            switch (country)
            {
                case ECountryCode.CzechRepublic:
                    return "Czech Republic";
                case ECountryCode.NewZealand:
                    return "New Zealand";
                case ECountryCode.SaudiArabia:
                    return "Saudi Arabia";
                case ECountryCode.SouthAfrica:
                    return "South Africa";
                case ECountryCode.SouthKorea:
                    return "South Korea";
                default:
                    return country.ToString("G");
            }
        }

        public static uint GetCode(string country)
        {
            switch(country)
            {
                case "Argentina":
                    return (uint)ECountryCode.Argentina;
                case "Australia":
                    return (uint)ECountryCode.Australia;
                case "Austria":
                    return (uint)ECountryCode.Austria;
                case "Bangladesh":
                    return (uint)ECountryCode.Bangladesh;
                case "Belgium":
                    return (uint)ECountryCode.Belgium;
                case "Brazil":
                    return (uint)ECountryCode.Brazil;
                case "Canada":
                    return (uint)ECountryCode.Canada;
                case "Chile":
                    return (uint)ECountryCode.Chile;
                case "China":
                    return (uint)ECountryCode.China;
                case "Cyprus":
                    return (uint)ECountryCode.Cyprus;
                case "CzechRepublic":
                    return (uint)ECountryCode.CzechRepublic;
                case "Denmark":
                    return (uint)ECountryCode.Denmark;
                case "Finland":
                    return (uint)ECountryCode.Finland;
                case "France":
                    return (uint)ECountryCode.France;
                case "Germany":
                    return (uint)ECountryCode.Germany;
                case "Greece":
                    return (uint)ECountryCode.Greece;
                case "Guatamala":
                    return (uint)ECountryCode.Guatamala;
                case "Hungary":
                    return (uint)ECountryCode.Hungary;
                case "Iceland":
                    return (uint)ECountryCode.Iceland;
                case "India":
                    return (uint)ECountryCode.India;
                case "Indonesia":
                    return (uint)ECountryCode.Indonesia;
                case "Ireland":
                    return (uint)ECountryCode.Ireland;
                case "Israel":
                    return (uint)ECountryCode.Israel;
                case "Italy":
                    return (uint)ECountryCode.Italy;
                case "Japan":
                    return (uint)ECountryCode.Japan;
                case "SouthKorea":
                    return (uint)ECountryCode.SouthKorea;
                case "Lithuania":
                    return (uint)ECountryCode.Lithuania;
                case "Luxembourg":
                    return (uint)ECountryCode.Luxembourg;
                case "Malaysia":
                    return (uint)ECountryCode.Malaysia;
                case "Malta":
                    return (uint)ECountryCode.Malta;
                case "Mexico":
                    return (uint)ECountryCode.Mexico;
                case "Netherlands":
                    return (uint)ECountryCode.Netherlands;
                case "NewZealand":
                    return (uint)ECountryCode.NewZealand;
                case "Norway":
                    return (uint)ECountryCode.Norway;
                case "Philippines":
                    return (uint)ECountryCode.Philippines;
                case "Poland":
                    return (uint)ECountryCode.Poland;
                case "Portugal":
                    return (uint)ECountryCode.Portugal;
                case "Romania":
                    return (uint)ECountryCode.Romania;
                case "Russia":
                    return (uint)ECountryCode.Russia;
                case "SaudiArabia":
                    return (uint)ECountryCode.SaudiArabia;
                case "Singapore":
                    return (uint)ECountryCode.Singapore;
                case "Slovakia":
                    return (uint)ECountryCode.Slovakia;
                case "Slovenia":
                    return (uint)ECountryCode.Slovenia;
                case "SouthAfrica":
                    return (uint)ECountryCode.SouthAfrica;
                case "Spain":
                    return (uint)ECountryCode.Spain;
                case "Sweden":
                    return (uint)ECountryCode.Sweden;
                case "Switzerland":
                    return (uint)ECountryCode.Switzerland;
                case "Taiwan":
                    return (uint)ECountryCode.Taiwan;
                case "Thailand":
                    return (uint)ECountryCode.Thailand;
                case "Turkey":
                    return (uint)ECountryCode.Turkey;
                case "UAE":
                    return (uint)ECountryCode.UAE;
                case "UK":
                    return (uint)ECountryCode.UK;
                case "US":
                    return (uint)ECountryCode.US;
                case "Uzbekistan":
                    return (uint)ECountryCode.Uzbekistan;
                case "Venezuela":
                    return (uint)ECountryCode.Venezuela;
                default:
                    Log.WriteLine(1, $"[RMC Eidos] Unknown country {country}", Color.Red);
                    return (uint)ECountryCode.US;
            }
        }
    }
}
