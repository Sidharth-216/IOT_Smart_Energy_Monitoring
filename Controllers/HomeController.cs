using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Models;
using SmartEnergy.Web.Services;

namespace SmartEnergy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataService _dataService;

        public HomeController(ILogger<HomeController> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // 👇 New API for Chart.js
        [HttpGet]
        public IActionResult GetChartData()
        {
            try
            {
                var readings = _dataService.GetLatestReadings();
                if (readings == null || !readings.Any())
                    return Json(new { hasData = false });

                var labels = readings.Select(r => r.Timestamp.ToString("HH:mm:ss")).ToArray();
                var values = readings.Select(r => r.Voltage).ToArray();

                return Json(new
                {
                    hasData = true,
                    labels = labels,
                    series = new[]
                    {
                        new {
                            deviceId = 1,
                            device = "Device",
                            values = values
                        }
                    },
                    thresholds = new[] { 200, 240 } // optional threshold lines
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
