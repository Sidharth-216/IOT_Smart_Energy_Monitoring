using SmartEnergy.Web.Data;
using SmartEnergy.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartEnergy.Web.Services
{
    public class DeviceService : IDataService
    {
        private readonly ApplicationDbContext _context;

        public DeviceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get latest reading for real‑time display
        public EnergyReading GetLatestReading()
        {
            return _context.EnergyReadings
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();
        }

        // Total energy consumed today
        public float GetTotalEnergyToday()
        {
            var today = DateTime.Today;
            return _context.EnergyReadings
                .Where(r => r.Timestamp.Date == today)
                .Sum(r => r.Power); // adjust if needed
        }

        // All registered devices
        public IEnumerable<Device> GetAllDevices()
        {
            return _context.Devices.ToList();
        }

        // Single device by ID
        public Device GetDeviceById(int id)
        {
            return _context.Devices.FirstOrDefault(d => d.DeviceId == id);
        }

        // Historical stats for last N days
        public IEnumerable<HistoricalStat> GetHistoricalStatsLastDays(int days)
        {
            var start = DateTime.Today.AddDays(-days);
            return _context.HistoricalStats
                .Where(s => s.Date >= start)
                .OrderBy(s => s.Date)
                .ToList();
        }

        // Historical stats for custom date range
        public IEnumerable<HistoricalStat> GetHistoricalStatsByDateRange(DateTime start, DateTime end)
        {
            return _context.HistoricalStats
                .Where(s => s.Date >= start && s.Date <= end)
                .OrderBy(s => s.Date)
                .ToList();
        }

        // Add new reading (from ESP32 or API)
        public void AddReading(EnergyReading reading)
        {
            _context.EnergyReadings.Add(reading);

            var device = _context.Devices.FirstOrDefault(d => d.DeviceId == reading.DeviceId);
            if (device != null)
            {
                device.LastActive = DateTime.Now;
            }

            _context.SaveChanges();
        }

        // ✅ Get latest N energy readings (for live table)
        public IEnumerable<EnergyReading> GetLatestReadings(int count = 50)
        {
            return _context.EnergyReadings
                .OrderByDescending(r => r.Timestamp)
                .Take(count)
                .ToList();
        }
        public void GenerateDailyHistoricalStats()
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);

            var grouped = _context.EnergyReadings
                .Where(r => r.Timestamp.Date == yesterday)
                .GroupBy(r => r.DeviceId)
                .Select(g => new HistoricalStat
                {
                    DeviceId = g.Key,
                    Date = yesterday,
                    AvgPower = g.Average(r => r.Power),
                    PeakPower = g.Max(r => r.Power),
                    // Assumption: 1-second sampling → Wh → kWh
                    TotalEnergy = g.Sum(r => (r.Power / 3600.0f))  
                })
                .ToList();

            if (grouped.Any())
            {
                _context.HistoricalStats.AddRange(grouped);
                _context.SaveChanges();
            }
        }


    }
}
