using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Services;

public class AlertsController : Controller
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    public IActionResult Index()
    {
        var alerts = _alertService.GetAllAlerts();
        return View(alerts);
    }

    public IActionResult MarkAsRead(int id)
    {
        _alertService.MarkAlertAsRead(id);
        return RedirectToAction("Index");
    }

    public IActionResult Clear(int id)
    {
        _alertService.ClearAlert(id);
        return RedirectToAction("Index");
    }
}
