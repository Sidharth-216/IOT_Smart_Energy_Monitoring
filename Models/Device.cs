namespace SmartEnergy.Web.Models
{
    public class Device
    {
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } // Online/Offline
        public DateTime LastActive { get; set; }
        public string FirmwareVersion { get; set; }
        public string IPAddress { get; set; }
        public string SSID { get; set; }
    }

}
