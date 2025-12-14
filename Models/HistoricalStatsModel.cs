//for firebase database
//for the controller HistoricalStatsController.cs
//for Views/HistoricalStats/Index.cshtml

using System;

namespace SmartEnergy.Web.Models
{
    public class HistoricalStatsModel
    {
        // Changed Date from DateTime to string for device labels
        public string Date { get; set; }  

        public double TotalPower { get; set; }
        public double AvgVoltage { get; set; }
        public double AvgCurrent { get; set; }
    }
}

