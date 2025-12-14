namespace SmartEnergy.Web.Models
{
    public class FirebaseReading
    {
        public double voltage { get; set; }
        public double current { get; set; }
        public double power { get; set; }
        public string device { get; set; }
        public long timestamp { get; set; } // Unix timestamp in milliseconds
    }
}
