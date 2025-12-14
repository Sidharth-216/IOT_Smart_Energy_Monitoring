
// main controller (real api controller) for reading , this controller is for views/Energy/live.cshtml
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SmartEnergy.Web.Models;



namespace SmartEnergy.Web.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class FirebaseStreamController : ControllerBase
    {
        private static readonly HttpClient _client = new HttpClient();

        [HttpGet("stream")]
        public async Task StreamFirebase()
        {
            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            
            //check the database rules , change it to true if it is false
            var firebaseUrl = "https://esp32-testing-aec8b-default-rtdb.firebaseio.com/readings/ESP32-001.json";

            using var request = new HttpRequestMessage(HttpMethod.Get, firebaseUrl);

            using var response =
                await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(line) && line.Contains("{"))
                {
                    await Response.WriteAsync($"data: {line}\n\n");
                    await Response.Body.FlushAsync();
                }
            }
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Question))
                return BadRequest(new { error = "Message cannot be empty" });

            var content = new StringContent(
                JsonConvert.SerializeObject(new { question = request.Question }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync("chat", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json"); // 🔑 send JSON to JS
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeReading([FromBody] EnergyReading reading)
        {
            if (reading == null)
                return BadRequest(new { error = "Invalid reading" });

            // Forward the reading to the Python chatbot
            var content = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    question = $"Based on these readings: Voltage={reading.Voltage}, Current={reading.Current}, Power={reading.Power} kW. Provide energy consumption level and saving tips."
                }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync("chat", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }
        [HttpPost]
        public async Task<IActionResult> AnalyzeReadingAI([FromBody] EnergyReading reading)
        {
            if (reading == null)
                return BadRequest(new { error = "Invalid reading" });

            //  Forward the reading to the Python chatbot
            var content = new StringContent(
                JsonConvert.SerializeObject(new {
                    question = $"Based on these readings: Voltage={reading.Voltage}, Current={reading.Current}, Power={reading.Power} kW. Provide energy consumption level and saving tips."
                }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync("chat", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }
    }
}

