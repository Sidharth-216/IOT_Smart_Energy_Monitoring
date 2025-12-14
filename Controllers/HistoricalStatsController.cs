//for Historicalstats/Index.chtml
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    public class HistoricalStatsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        // Update this to match your Firebase URL structure
        // Example: "https://your-project.firebaseio.com/readings/ESP32-001/history"
        private const string FIREBASE_HISTORY_URL = "https://esp32-testing-aec8b-default-rtdb.firebaseio.com/readings/ESP32-001/history";
        
        public HistoricalStatsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoricalData(string startDate, string endDate, string metric = "power")
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{FIREBASE_HISTORY_URL}.json");
                
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Failed to fetch data from Firebase" });
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrEmpty(jsonString) || jsonString == "null")
                {
                    return Json(new { 
                        success = false, 
                        message = "No historical data available yet. Please ensure your ESP32 is running and has time sync enabled. Data will start appearing after a few minutes." 
                    });
                }
                
                // Firebase structure: { "2024-11-27": { "timestamp1": {reading}, "timestamp2": {reading} } }
                var historyData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, EnergyReading2>>>(jsonString);

                if (historyData == null || !historyData.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = "No historical data available. Your ESP32 needs to run for a while to collect data." 
                    });
                }

                // Parse dates
                DateTime start = string.IsNullOrEmpty(startDate) 
                    ? DateTime.Now.AddDays(-7) 
                    : DateTime.Parse(startDate);
                DateTime end = string.IsNullOrEmpty(endDate) 
                    ? DateTime.Now 
                    : DateTime.Parse(endDate);

                // Process and group data by day
                var groupedData = ProcessHistoricalData(historyData, start, end);

                if (!groupedData.Any())
                {
                    return Json(new { 
                        success = false, 
                        message = $"No data found between {start:yyyy-MM-dd} and {end:yyyy-MM-dd}. Try a different date range." 
                    });
                }

                return Json(new { success = true, data = groupedData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDailySummary(string date)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                
                DateTime targetDate = string.IsNullOrEmpty(date) 
                    ? DateTime.Today 
                    : DateTime.Parse(date);
                
                string dateKey = targetDate.ToString("yyyy-MM-dd");
                
                // Fetch specific date's data
                var response = await client.GetAsync($"{FIREBASE_HISTORY_URL}/{dateKey}.json");
                
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "No data found for this date" });
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrEmpty(jsonString) || jsonString == "null")
                {
                    return Json(new { success = false, message = "No readings found for " + dateKey });
                }
                
                var dayData = JsonSerializer.Deserialize<Dictionary<string, EnergyReading2>>(jsonString);

                if (dayData == null || !dayData.Any())
                {
                    return Json(new { success = false, message = "No readings found for " + dateKey });
                }

                var summary = GetDaySummary(dayData, targetDate);

                return Json(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private List<DailyStats> ProcessHistoricalData(
            Dictionary<string, Dictionary<string, EnergyReading2>> historyData,
            DateTime start,
            DateTime end)
        {
            var dailyStats = new List<DailyStats>();

            foreach (var dateEntry in historyData)
            {
                // Parse the date key (format: "2024-11-27")
                if (!DateTime.TryParse(dateEntry.Key, out DateTime date))
                    continue;

                // Filter by date range
                if (date.Date < start.Date || date.Date > end.Date)
                    continue;

                // Get all readings for this date
                var readings = dateEntry.Value.Values.Where(r => r != null).ToList();

                if (!readings.Any())
                    continue;

                var stats = new DailyStats
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    DisplayDate = date.ToString("MMM dd"),
                    TotalPower = CalculateTotal(readings, "power"),
                    AveragePower = CalculateAverage(readings, "power"),
                    TotalCurrent = CalculateTotal(readings, "current"),
                    AverageCurrent = CalculateAverage(readings, "current"),
                    AverageVoltage = CalculateAverage(readings, "voltage"),
                    ReadingCount = readings.Count,
                    MinPower = readings.Min(r => r.Power),
                    MaxPower = readings.Max(r => r.Power),
                    MinCurrent = readings.Min(r => r.Current),
                    MaxCurrent = readings.Max(r => r.Current),
                    MinVoltage = readings.Min(r => r.Voltage),
                    MaxVoltage = readings.Max(r => r.Voltage),
                    EnergyConsumed = CalculateEnergyConsumed(readings)
                };

                dailyStats.Add(stats);
            }

            // Sort by date
            return dailyStats.OrderBy(s => s.Date).ToList();
        }

        private DailySummary GetDaySummary(Dictionary<string, EnergyReading2> dayData, DateTime date)
        {
            var readings = dayData.Values.Where(r => r != null).ToList();
            
            if (!readings.Any())
            {
                return new DailySummary
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    TotalReadings = 0
                };
            }
            
            // Parse timestamps and create hourly breakdown
            var readingsWithTime = new List<(int hour, EnergyReading2 reading)>();
            
            foreach (var entry in dayData)
            {
                if (entry.Value == null) continue;
                
                // Timestamp is the key (in milliseconds)
                if (long.TryParse(entry.Key, out long timestamp))
                {
                    try
                    {
                        var dt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;
                        readingsWithTime.Add((dt.Hour, entry.Value));
                    }
                    catch
                    {
                        // Skip invalid timestamps
                        continue;
                    }
                }
            }

            return new DailySummary
            {
                Date = date.ToString("yyyy-MM-dd"),
                TotalReadings = readings.Count,
                AveragePower = CalculateAverage(readings, "power"),
                PeakPower = readings.Max(r => r.Power),
                MinPower = readings.Min(r => r.Power),
                AverageCurrent = CalculateAverage(readings, "current"),
                PeakCurrent = readings.Max(r => r.Current),
                AverageVoltage = CalculateAverage(readings, "voltage"),
                TotalEnergyConsumed = CalculateEnergyConsumed(readings),
                HourlyData = GetHourlyBreakdown(readingsWithTime)
            };
        }

        private List<HourlyData> GetHourlyBreakdown(List<(int hour, EnergyReading2 reading)> readingsWithTime)
        {
            var hourlyData = new List<HourlyData>();

            if (!readingsWithTime.Any())
                return hourlyData;

            var groupedByHour = readingsWithTime
                .GroupBy(r => r.hour)
                .OrderBy(g => g.Key);

            foreach (var group in groupedByHour)
            {
                var hourReadings = group.Select(x => x.reading).ToList();
                hourlyData.Add(new HourlyData
                {
                    Hour = $"{group.Key:D2}:00",
                    AveragePower = CalculateAverage(hourReadings, "power"),
                    AverageCurrent = CalculateAverage(hourReadings, "current"),
                    AverageVoltage = CalculateAverage(hourReadings, "voltage"),
                    ReadingCount = hourReadings.Count
                });
            }

            return hourlyData;
        }

        private double CalculateTotal(List<EnergyReading2> readings, string metric)
        {
            if (!readings.Any()) return 0;
            
            return metric.ToLower() switch
            {
                "power" => readings.Sum(r => r.Power),
                "current" => readings.Sum(r => r.Current),
                "voltage" => readings.Sum(r => r.Voltage),
                _ => 0
            };
        }

        private double CalculateAverage(List<EnergyReading2> readings, string metric)
        {
            if (!readings.Any()) return 0;

            return metric.ToLower() switch
            {
                "power" => readings.Average(r => r.Power),
                "current" => readings.Average(r => r.Current),
                "voltage" => readings.Average(r => r.Voltage),
                _ => 0
            };
        }

        private double CalculateEnergyConsumed(List<EnergyReading2> readings)
        {
            if (!readings.Any()) return 0;

            // Energy (kWh) = Average Power (W) * Time (hours) / 1000
            // Based on your ESP32 code, readings are sent every 2 seconds when device is connected
            // So: readings per hour = 3600 / 2 = 1800
            double avgPower = readings.Average(r => r.Power);
            double hours = readings.Count / 1800.0; // 1800 readings per hour at 2-second intervals
            return (avgPower * hours) / 1000.0; // Convert to kWh
        }
    }

    // Models matching your Firebase structure
    public class EnergyReading2
    {
        [System.Text.Json.Serialization.JsonPropertyName("current")]
        public double Current { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("power")]
        public double Power { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("voltage")]
        public double Voltage { get; set; }
    }

    public class DailyStats
    {
        public string Date { get; set; }
        public string DisplayDate { get; set; }
        public double TotalPower { get; set; }
        public double AveragePower { get; set; }
        public double TotalCurrent { get; set; }
        public double AverageCurrent { get; set; }
        public double AverageVoltage { get; set; }
        public int ReadingCount { get; set; }
        public double MinPower { get; set; }
        public double MaxPower { get; set; }
        public double MinCurrent { get; set; }
        public double MaxCurrent { get; set; }
        public double MinVoltage { get; set; }
        public double MaxVoltage { get; set; }
        public double EnergyConsumed { get; set; }
    }

    public class DailySummary
    {
        public string Date { get; set; }
        public int TotalReadings { get; set; }
        public double AveragePower { get; set; }
        public double PeakPower { get; set; }
        public double MinPower { get; set; }
        public double AverageCurrent { get; set; }
        public double PeakCurrent { get; set; }
        public double AverageVoltage { get; set; }
        public double TotalEnergyConsumed { get; set; }
        public List<HourlyData> HourlyData { get; set; }
    }

    public class HourlyData
    {
        public string Hour { get; set; }
        public double AveragePower { get; set; }
        public double AverageCurrent { get; set; }
        public double AverageVoltage { get; set; }
        public int ReadingCount { get; set; }
    }
}