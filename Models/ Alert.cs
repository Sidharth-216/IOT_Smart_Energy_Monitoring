namespace SmartEnergy.Web.Models
{
    public class Alert
    {
        public int AlertId { get; set; }
        public int? DeviceId { get; set; } // nullable
        public Device Device { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; } // Info, Warning, Critical
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } // New, Read, Cleared
    }

}
