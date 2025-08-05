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
