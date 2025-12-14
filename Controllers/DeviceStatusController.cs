//this controller is for DeviceStatus.cs modeland 
// this controller is for Views/DeviceStatus/Index.cshtml
// Controllers/DeviceStatusController.cs

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SmartEnergy.Web.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEnergy.Web.Controllers
{
    public class DeviceStatusController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Store last reading and last change time
        private static double? lastCurrentReading = null;
        private static DateTime lastChangeTime = DateTime.MinValue;

        // Set stable duration before marking disconnected (in seconds)
        private const double StableDuration = 10.0;

        public DeviceStatusController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetDeviceStatus();
            return View(model);
        }

        public async Task<IActionResult> IndexPartial()
        {
            var model = await GetDeviceStatus();
            return PartialView(model);
        }

        private async Task<DeviceStatus> GetDeviceStatus()
        {
            string liveUrl = "https://esp32-testing-aec8b-default-rtdb.firebaseio.com/readings/ESP32-001.json";
            var client = _httpClientFactory.CreateClient();

            string deviceType = "Unknown";
            bool isConnected = false;
            double currentReading = 0;

            try
            {
                var liveResp = await client.GetAsync(liveUrl);
                if (liveResp.IsSuccessStatusCode)
                {
                    var liveJson = await liveResp.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(liveJson) && liveJson != "null")
                    {
                        var liveObj = JObject.Parse(liveJson);

                        deviceType = liveObj["deviceType"]?.ToString() ?? "Unknown";
                        currentReading = liveObj["current"]?.ToObject<double>() ?? 0;

                        var now = DateTime.UtcNow;

                        if (!lastCurrentReading.HasValue || currentReading != lastCurrentReading.Value)
                        {
                            // New reading → Connected
                            isConnected = true;
                            lastChangeTime = now;
                            lastCurrentReading = currentReading;
                        }
                        else
                        {
                            // No new reading
                            if ((now - lastChangeTime).TotalSeconds <= StableDuration)
                            {
                                // within stable duration → keep Connected
                                isConnected = true;
                            }
                            else
                            {
                                // stable duration exceeded → mark Not Connected
                                isConnected = false;
                            }
                        }
                    }
                }
            }
            catch { }

            return new DeviceStatus
            {
                DeviceId = "ESP32-001",
                DeviceType = deviceType,
                IsConnected = isConnected
            };
        }
    }
}
