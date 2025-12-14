//old3
using Microsoft.AspNetCore.Mvc;

namespace SmartEnergy.Web.Controllers
{
    public class EnergyController : Controller
    {
        public IActionResult Live()
        {
            return View();
        }
    }
}
