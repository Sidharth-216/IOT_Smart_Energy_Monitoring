//controller work as a WebAPI , recevices data from device or setup , send to databse and then to website

/*using Microsoft.AspNetCore.Mvc;
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

        // POST: api/energyreading
        [HttpPost]
        public IActionResult AddReading([FromBody] EnergyReading reading)
        {
            if (reading == null)
                return BadRequest("Reading data is missing.");

            // Optionally, add a timestamp here if ESP32 doesn't send it
            reading.Timestamp = DateTime.Now;

            _dataService.AddReading(reading);
            return Ok(new { message = "Reading saved successfully!" });
        }
    }
}*/


using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Models;
using SmartEnergy.Web.Services;

namespace SmartEnergy.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (reading == null)
            {
                return BadRequest("Reading data is null");
            }

            try
            {
                reading.Timestamp = DateTime.Now; // Set timestamp on server side
                _dataService.AddReading(reading);
                return Ok(new { message = "Reading saved successfully!" });
            }
            catch (Exception ex)
            {
                // log exception if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
