
//old2
using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Models;
using SmartEnergy.Web.Services;

namespace SmartEnergy.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyReadingController : ControllerBase
    {
        private readonly IDataService _dataService;

        public EnergyReadingController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public IActionResult PostReading([FromBody] EnergyReading reading)
        {
            if (reading == null) return BadRequest();

            reading.Timestamp = DateTime.UtcNow; // Set server-side timestamp
            _dataService.AddReading(reading);

            return Ok(new { status = "success", reading });
        }

        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            var latest = _dataService.GetLatestReading();
            return Ok(latest);
        }
    }
}
