using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Services;

namespace SmartEnergy.Web.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDataService _dataService;

        public DeviceController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Status()
        {
            var devices = _dataService.GetAllDevices();
            return View(devices);
        }
        //public IActionResult Dashboard()
        //{
           // var readings = _dataService.GetLatestReadings(50); // or any number
            //return View(readings); // ✅ Pass the readings to the view
            // }
        public IActionResult Dashboard()
        {
            var devices = _dataService.GetAllDevices();
            return View(devices);
        }


        public IActionResult Details(int id)
        {
            var device = _dataService.GetDeviceById(id);
            if (device == null)
                return NotFound();

            return View(device);
        }
    }
}
