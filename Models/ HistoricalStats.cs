namespace SmartEnergy.Web.Models
{
    public class HistoricalStat
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public DateTime Date { get; set; }
        public float AvgPower { get; set; }
        public float PeakPower { get; set; }
        public float TotalEnergy { get; set; }
    }

}
