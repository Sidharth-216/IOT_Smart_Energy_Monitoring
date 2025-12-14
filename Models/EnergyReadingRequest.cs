namespace SmartEnergy.Web.Models
{
    public class EnergyReadingRequest
    {
        public float Voltage { get; set; }
        public float Current { get; set; }
        public float Power { get; set; }
    }

}