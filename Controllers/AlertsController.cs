
using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Data;
using SmartEnergy.Web.Models;
using System.Threading.Tasks;
using System.Linq;

namespace SmartEnergy.Web.Controllers
{
    public class AlertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Alert/
        public IActionResult Index()
        {
            var alerts = _context.Alerts.OrderByDescending(a => a.Timestamp).ToList();
            return View(alerts);
        }

        // GET: /Alert/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Alert/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Alert alert)
        {
            if (ModelState.IsValid)
            {
                alert.Timestamp = DateTime.Now;
                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(alert);
        }
    }
}


