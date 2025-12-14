//this model is for DeviceStatusController.cs
//this model is for Views/DeviceStatus/Index.cshtml

namespace SmartEnergy.Web.Models
{
    public class DeviceStatus
    {
        public string DeviceId { get; set; }
        public bool IsConnected { get; set; }
        public string DeviceType { get; set; }
    }
}
