namespace SmartEnergy.Web.Models
{
    public class ReadingDto
    {
        public string ReadingId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public float Voltage { get; set; }
        public float Current { get; set; }
        public float Power { get; set; }
        public DateTime Timestamp { get; set; }
        public bool HasData { get; set; }
    }
}
