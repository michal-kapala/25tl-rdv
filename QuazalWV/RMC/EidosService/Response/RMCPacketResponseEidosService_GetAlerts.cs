using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_GetAlerts : RMCPResponse
    {
        public EidosAlerts Alerts { get; set; }

        public RMCPacketResponseEidosService_GetAlerts()
        {
            Alerts = new EidosAlerts();
        }

        public RMCPacketResponseEidosService_GetAlerts(uint pid)
        {
            // Get clan, friend and match invitation alerts (notifications)
            Alerts = new EidosAlerts(pid);
            // Clear player's alerts
            Alert.DeleteAll(pid);
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Alerts.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return "[GetAlerts Response]";
        }

        public override string PayloadToString()
        {
            return "";
        }
    }
}
