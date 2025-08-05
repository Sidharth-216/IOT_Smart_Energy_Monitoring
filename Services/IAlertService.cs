using SmartEnergy.Web.Models;
using System.Collections.Generic;

public interface IAlertService
{
    IEnumerable<Alert> GetAllAlerts();
    void MarkAlertAsRead(int id);
    void ClearAlert(int id);
    void CreateAlert(Alert alert);
}
