//chatbot using api key
using Microsoft.AspNetCore.Mvc;
using SmartEnergy. Web.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartEnergy.Web.Controllers
{
    public class LlamaController : Controller
    {
        private readonly HttpClient _httpClient;

        public LlamaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:5001/");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // This will render Views/Llama/Index.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return Json(new { error = "Message cannot be empty" });

            var content = new StringContent(
                JsonConvert.SerializeObject(new { question = request.Question }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("chat", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json"); // 🔑 send JSON to JS
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeReading([FromBody] EnergyReading reading)
        {
            if (reading == null)
                return Json(new { error = "Invalid reading" });

            // 🔑 Forward the reading to the Python chatbot
            var content = new StringContent(
                JsonConvert.SerializeObject(new {
                    question = $"Based on these readings: Voltage={reading.Voltage}, Current={reading.Current}, Power={reading.Power} kW. Provide energy consumption level and saving tips."
                }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("chat", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }


    }
}





/*using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Models; // Make sure your models are in this namespace
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartEnergy.Web.Controllers
{
    public class LlamaController : Controller
    {
        private readonly HttpClient _httpClient;

        public LlamaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:5001/");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Renders Views/Llama/Index.cshtml
        }

        // -------------------------------
        // CHAT ENDPOINT
        // -------------------------------
        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return Json(new { error = "Message cannot be empty" });

            var content = new StringContent(
                JsonConvert.SerializeObject(new { question = request.Question }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("chat", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json"); // Send JSON back to JS
        }

        // -------------------------------
        // ENERGY READING ANALYSIS
        // -------------------------------
        [HttpPost]
        public async Task<IActionResult> AnalyzeReading([FromBody] EnergyReadingRequest reading)
        {
            if (reading == null)
                return Json(new { error = "Invalid reading" });

            var content = new StringContent(
                JsonConvert.SerializeObject(new
                {
                    voltage = reading.Voltage,
                    current = reading.Current,
                    power = reading.Power
                }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("analyze_reading", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        // -------------------------------
        // SUMMARIZATION ENDPOINT
        // -------------------------------
        [HttpPost]
        public async Task<IActionResult> Summarize([FromBody] TextSummaryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return Json(new { error = "Text cannot be empty" });

            var content = new StringContent(
                JsonConvert.SerializeObject(new { text = request.Text }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("summarize", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }
    }
}

/*
//offline llm , using ollama
using Microsoft.AspNetCore.Mvc;
using SmartEnergy.Web.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartEnergy.Web.Controllers
{
    public class LlamaController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = "sid_api_key"; // SAME as FastAPI

        public LlamaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();

            // Base URL of FastAPI server
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:5001/");

            // Add API KEY header
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", API_KEY);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // -------------------------------
        // CHAT ENDPOINT
        // -------------------------------
        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return Json(new { error = "Message cannot be empty" });

            var jsonBody = JsonConvert.SerializeObject(new { question = request.Question });

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        // -------------------------------
        // ENERGY READING ANALYSIS
        // -------------------------------
        [HttpPost]
        public async Task<IActionResult> AnalyzeReading([FromBody] EnergyReadingRequest reading)
        {
            if (reading == null)
                return Json(new { error = "Invalid reading" });

            var jsonBody = JsonConvert.SerializeObject(new
            {
                voltage = reading.Voltage,
                current = reading.Current,
                power = reading.Power
            });

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("analyze_reading", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        // -------------------------------
        // SUMMARIZATION ENDPOINT
        // -------------------------------
        [HttpPost]
        public async Task<IActionResult> Summarize([FromBody] TextSummaryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return Json(new { error = "Text cannot be empty" });

            var jsonBody = JsonConvert.SerializeObject(new { text = request.Text });

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("summarize", content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }
    }
}
*/