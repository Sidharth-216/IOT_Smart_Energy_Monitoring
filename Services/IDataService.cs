using SmartEnergy.Web.Models;
using System;
using System.Collections.Generic;

namespace SmartEnergy.Web.Services
{
    public interface IDataService
    {
        EnergyReading GetLatestReading();
        float GetTotalEnergyToday();
        IEnumerable<Device> GetAllDevices();
        Device GetDeviceById(int id);
        IEnumerable<HistoricalStat> GetHistoricalStatsLastDays(int days);
        IEnumerable<HistoricalStat> GetHistoricalStatsByDateRange(DateTime start, DateTime end);
        void AddReading(EnergyReading reading);
    }
}
