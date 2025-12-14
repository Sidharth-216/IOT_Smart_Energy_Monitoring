
//old1
using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Services;

namespace SmartEnergy.Web.Controllers
{
    public class EnergyReadingViewController : Controller
    {
        private readonly IDataService _dataService;

        public EnergyReadingViewController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // Normal Index (if you still want the table view)
        public IActionResult Index()
        {
            var readings = _dataService.GetLatestReadings();
            return View(readings);
        }

        // New action to return JSON for latest live reading
        [HttpGet]
        public IActionResult Latest()
        {
            var latest = _dataService.GetLatestReading(); // 👉 single record, not a list
            if (latest == null)
                return Json(new { hasData = false });

            return Json(new
            {
                hasData = true,
                readingId = latest.ReadingId,
                deviceId = latest.DeviceId,
                voltage = latest.Voltage,
                current = latest.Current,
                power = latest.Power,
                timestamp = latest.Timestamp
            });
        }
    }
    
}

