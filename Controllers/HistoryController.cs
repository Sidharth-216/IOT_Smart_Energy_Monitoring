using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Services;

public class HistoryController : Controller
{
    private readonly IDataService _dataService;

    public HistoryController(IDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        // Default: show past 7 days
        var data = _dataService.GetHistoricalStatsLastDays(7);
        return View(data);
    }

    [HttpPost]
    public IActionResult Filter(DateTime startDate, DateTime endDate)
    {
        var data = _dataService.GetHistoricalStatsByDateRange(startDate, endDate);
        return View("Index", data);
    }
}

/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEnergy.Web.Data;
using SmartEnergy.Web.Models;
using SmartEnergy.Web.ViewModels;

namespace SmartEnergy.Web.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /History
        public async Task<IActionResult> Index(int? deviceId, DateTime? startDate, DateTime? endDate)
        {
            // default date range: last 7 days (inclusive)
            var end = endDate?.Date ?? DateTime.UtcNow.Date;
            var start = startDate?.Date ?? end.AddDays(-7);

            var vm = new HistoryViewModel
            {
                SelectedDeviceId = deviceId,
                StartDate = start,
                EndDate = end
            };

            vm.Devices = await _context.Devices.AsNoTracking().OrderBy(d => d.Name).ToListAsync();

            var query = _context.HistoricalStats
                                .AsNoTracking()
                                .Include(h => h.Device)
                                .Where(h => h.Date.Date >= start && h.Date.Date <= end);

            if (deviceId.HasValue)
                query = query.Where(h => h.DeviceId == deviceId.Value);

            vm.Stats = await query
                            .OrderBy(h => h.Date)
                            .ToListAsync();

            return View(vm);
        }

        // JSON endpoint used for AJAX (optional)
        [HttpGet]
        public async Task<IActionResult> ChartData(int? deviceId, DateTime? startDate, DateTime? endDate)
        {
            var end = endDate?.Date ?? DateTime.UtcNow.Date;
            var start = startDate?.Date ?? end.AddDays(-7);

            var query = _context.HistoricalStats
                                .AsNoTracking()
                                .Where(h => h.Date.Date >= start && h.Date.Date <= end);

            if (deviceId.HasValue)
                query = query.Where(h => h.DeviceId == deviceId.Value);

            var stats = await query.OrderBy(h => h.Date).ToListAsync();

            var labels = stats.Select(s => s.Date.ToString("MMM d")).ToArray();
            var avg = stats.Select(s => s.AvgPower).ToArray();
            var peak = stats.Select(s => s.PeakPower).ToArray();
            var total = stats.Select(s => s.TotalEnergy).ToArray();

            return Json(new
            {
                labels,
                avg,
                peak,
                total
            });
        }
    }
}*/
