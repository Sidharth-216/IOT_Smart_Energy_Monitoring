using SmartEnergy.Web.Models;
using System.Collections.Generic;
using System.Linq;
using SmartEnergy.Web.Data;  

namespace SmartEnergy.Web.Services
{
    public class AlertService : IAlertService
    {
        private readonly ApplicationDbContext _context;

        public AlertService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Alert> GetAllAlerts()
        {
            return _context.Alerts.OrderByDescending(a => a.Timestamp).ToList();
        }

        public void MarkAlertAsRead(int id)
        {
            var alert = _context.Alerts.FirstOrDefault(a => a.AlertId == id);
            if (alert != null)
            {
                alert.Status = "Read";
                _context.SaveChanges();
            }
        }

        public void ClearAlert(int id)
        {
            var alert = _context.Alerts.FirstOrDefault(a => a.AlertId == id);
            if (alert != null)
            {
                _context.Alerts.Remove(alert);
                _context.SaveChanges();
            }
        }

        public void CreateAlert(Alert alert)
        {
            _context.Alerts.Add(alert);
            _context.SaveChanges();
        }
    }

}