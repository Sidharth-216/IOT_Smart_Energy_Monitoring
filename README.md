# Smart Energy Monitoring System (IoT + ASP.NET Core MVC)

An end-to-end smart energy monitoring platform that combines:
- **ESP32 firmware** for voltage/current/power capture,
- **ASP.NET Core MVC (.NET 8)** for dashboards, APIs, and business logic,
- **SQL Server + EF Core** for structured storage,
- **Firebase Realtime Database** for near real-time device stream,
- **Python AI service (Flask/FastAPI)** for chat and energy-saving suggestions.

This repository includes both the web application and supporting scripts/firmware used during development.

---

## 1) What this project is

The project is an **IoT-based Smart Energy Monitoring System** used to:
- ingest power readings from ESP32 devices,
- store and query readings/historical aggregates,
- visualize live and historical energy behavior,
- track alerts and suggestions,
- integrate AI-generated recommendations based on readings.

Core web app project: `SmartEnergy.Web/` (ASP.NET Core MVC).

---

## 2) Complete folder structure

> Notes:
> - `bin/`, `obj/`, `.git/`, and `wwwroot/lib/` are included as directories but their internal generated/vendor files are not expanded.
> - This reflects the current repository layout.

```text
SmartEnergy.Web/
├── backupcodes.txt
├── chatbot_api.py
├── knowledge_from_api_20251016_122504.json
├── localhost.session.sql
├── main_frimware.ino
├── ml_codes.py
├── ollama_test.py
├── readme.md
├── SmartEnergy.sln
├── __pycache__/
└── SmartEnergy.Web/
		├── appsettings.Development.json
		├── appsettings.json
		├── Program.cs
		├── SmartEnergy.Web.csproj
		├── bin/
		├── obj/
		├── Controllers/
		│   ├── AlertsController.cs
		│   ├── DeviceController.cs
		│   ├── DeviceStatusController.cs
		│   ├── EnergyController.cs
		│   ├── EnergyReadingController.cs
		│   ├── EnergyReadingViewController.cs
		│   ├── FirebaseStreamController.cs
		│   ├── HistoricalStatsController.cs
		│   ├── HistoryController.cs
		│   ├── HomeController.cs
		│   ├── LlamaController.cs
		│   └── SuggestionsController.cs
		├── Data/
		│   ├── ApplicationDbContext.cs
		│   └── ApplicationDbContextFactory.cs
		├── esp32-firmware/
		│   ├── energy_monitor.ino
		│   ├── platformio.ini
		│   ├── README.md
		│   ├── data/
		│   ├── lib/
		│   └── test_sketches/
		│       ├── sensor_test.ino
		│       └── wifi_test.ino
		├── Migrations/
		│   ├── 20250803114458_InitialCreate.cs
		│   ├── 20250803114458_InitialCreate.Designer.cs
		│   └── ApplicationDbContextModelSnapshot.cs
		├── Models/
		│   ├── Alert.cs
		│   ├── HistoricalStats.cs
		│   ├── ChatMessageRequest.cs
		│   ├── ChatRequest.cs
		│   ├── ChatResponse.cs
		│   ├── Device.cs
		│   ├── DeviceStatus.cs
		│   ├── EnergyReading.cs
		│   ├── EnergyReadingRequest.cs
		│   ├── ErrorViewModel.cs
		│   ├── FirebaseReading.cs
		│   ├── HistoricalStatsModel.cs
		│   ├── ReadingDto.cs
		│   ├── Suggestion.cs
		│   └── TextSummaryRequest.cs
		├── Properties/
		│   └── launchSettings.json
		├── Services/
		│   ├── AlertService.cs
		│   ├── DeviceService.cs
		│   ├── IAlertService.cs
		│   ├── IDataService.cs
		│   ├── ISuggestionService.cs
		│   └── SuggestionService.cs
		├── Views/
		│   ├── _ViewImports.cshtml
		│   ├── _ViewStart.cshtml
		│   ├── Alerts/
		│   │   ├── Create.cshtml
		│   │   └── Index.cshtml
		│   ├── Device/
		│   │   ├── Dashboard.cshtml
		│   │   ├── Details.cshtml
		│   │   └── Status.cshtml
		│   ├── DeviceStatus/
		│   │   ├── Index.cshtml
		│   │   └── IndexPartial.cshtml
		│   ├── Energy/
		│   │   └── Live.cshtml
		│   ├── EnergyReadingView/
		│   │   └── Index.cshtml
		│   ├── HistoricalStats/
		│   │   └── Index.cshtml
		│   ├── History/
		│   │   └── Index.cshtml
		│   ├── Home/
		│   │   ├── Index.cshtml
		│   │   └── Privacy.cshtml
		│   ├── Llama/
		│   │   └── Index.cshtml
		│   ├── Shared/
		│   │   ├── _Layout.cshtml
		│   │   ├── _Layout.cshtml.css
		│   │   ├── _ValidationScriptsPartial.cshtml
		│   │   └── Error.cshtml
		│   └── Suggestion/
		│       ├── Add.cshtml
		│       └── Index.cshtml
		└── wwwroot/
				├── css/
				│   └── site.css
				├── img/
				├── js/
				│   └── site.js
				└── lib/
```

---

## 3) Major features

### Web + API features
- **Live energy ingestion API**: `POST /api/energyreading` stores incoming readings to SQL Server.
- **Latest reading API**: `GET /api/energyreading/latest` returns latest entry.
- **Live dashboard UI**: Voltage/current/power cards + chart updates.
- **Device dashboards**: device status, list, and details pages.
- **History/statistics**:
	- SQL-backed recent history via `HistoryController`,
	- Firebase-backed aggregated/historical analytics via `HistoricalStatsController`.
- **Alert management**: create + view alerts.
- **Suggestion management**: add and list suggestions.
- **AI assistant integration** (`LlamaController`):
	- chat endpoint proxy,
	- reading analysis endpoint for tips.
- **Firebase stream endpoint**: server-sent stream endpoint in `FirebaseStreamController`.

### IoT/firmware features
- ESP32 firmware sends voltage/current/power periodically.
- Includes calibration logic and connectivity flow in `main_frimware.ino`.
- Alternate ESP32 sketch under `SmartEnergy.Web/esp32-firmware/energy_monitor.ino` posts directly to ASP.NET API.

### Data model (SQL Server)
Tables from migration:
- `Devices`
- `EnergyReadings`
- `HistoricalStats`
- `Alerts`
- `Suggestions`

---

## 4) Prerequisites (important)

###+ Required for core web app
- **.NET SDK 8.0+**
- **SQL Server** (Developer/Express/Standard) reachable from app host
- **EF Core CLI tools**

Install EF tool:
```bash
dotnet tool install --global dotnet-ef
```

###+ Required for AI service (if using Llama/Chat pages)
- **Python 3.10+**
- `pip`
- Python packages (minimum):
	- `flask`
	- `google-genai`

Optional (if using FastAPI + Ollama path in comments):
- `fastapi`
- `uvicorn`
- `ollama` Python package + Ollama runtime/model

###+ Required for ESP32 firmware
- **Arduino IDE** or **PlatformIO**
- ESP32 board support packages
- Sensors used in code (e.g., ZMPT101B, ACS712)
- Firebase Realtime Database project (if using Firebase firmware flow)

---

## 5) Configuration setup

### 5.1 ASP.NET app configuration
Edit `SmartEnergy.Web/appsettings.json`:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=<SERVER>;Database=SmartEnergyDb;User Id=<USER>;Password=<PASSWORD>;TrustServerCertificate=True;MultipleActiveResultSets=true;"
	}
}
```

### 5.2 Development URLs
`SmartEnergy.Web/Properties/launchSettings.json` includes:
- HTTP profile on `http://0.0.0.0:5048`
- HTTPS profile on `https://localhost:7071` and `http://localhost:5048`

### 5.3 AI API configuration
In `chatbot_api.py`, configure:
- Gemini API key (`API_KEY`) **or** Ollama setup (commented FastAPI block).

For production/security, use environment variables instead of hardcoded secrets.

### 5.4 ESP32 configuration
Update Wi-Fi and target URL in firmware:
- SSID/password
- API endpoint (`/api/energyreading`) or Firebase URL
- calibration constants for your sensor setup

---

## 6) Setup to run the project (step-by-step)

### Step 1: Restore .NET dependencies
From repo root:
```bash
dotnet restore SmartEnergy.sln
```

### Step 2: Create/update database schema
From web project folder:
```bash
cd SmartEnergy.Web
dotnet ef database update
```

If EF command fails, ensure `dotnet-ef` is installed globally and SQL connection is valid.

### Step 3: Run ASP.NET Core app
From `SmartEnergy.Web`:
```bash
dotnet run
```

Open:
- `http://localhost:5048` (or your configured launch URL)

### Step 4: Run AI service (optional but recommended)
From repo root:
```bash
python3 -m pip install flask google-genai
python3 chatbot_api.py
```

Expected AI service URL:
- `http://127.0.0.1:5001`

### Step 5: Flash and run ESP32 firmware
- Open either:
	- `main_frimware.ino` (advanced Firebase/history variant), or
	- `SmartEnergy.Web/esp32-firmware/energy_monitor.ino` (direct API post variant)
- Set Wi-Fi and backend/Firebase target
- Flash to ESP32
- Verify serial logs and incoming readings in web dashboard/API

---

## 7) Deployment setup

###+ A) Deploy ASP.NET app on Linux (recommended pattern)

1. Publish build:
```bash
cd SmartEnergy.Web
dotnet publish -c Release -o ./publish
```

2. Copy publish folder to server (example path):
`/opt/smartenergy-web/`

3. Create systemd service (`/etc/systemd/system/smartenergy-web.service`):
```ini
[Unit]
Description=SmartEnergy ASP.NET Core App
After=network.target

[Service]
WorkingDirectory=/opt/smartenergy-web
ExecStart=/usr/bin/dotnet /opt/smartenergy-web/SmartEnergy.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=smartenergy-web
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5048

[Install]
WantedBy=multi-user.target
```

4. Enable + start service:
```bash
sudo systemctl daemon-reload
sudo systemctl enable smartenergy-web
sudo systemctl start smartenergy-web
sudo systemctl status smartenergy-web
```

5. (Optional but recommended) Put Nginx in front for TLS and reverse proxy to `:5048`.

###+ B) Deploy AI Python service

1. Create venv and install deps:
```bash
python3 -m venv .venv
source .venv/bin/activate
pip install flask google-genai
```

2. Run service:
```bash
python chatbot_api.py
```

3. In production, run via process manager (systemd/supervisor) and expose behind Nginx.

###+ C) Deploy with SQL Server
- Ensure production SQL Server is reachable.
- Set production connection string via environment or secure config.
- Run migrations on target DB before traffic cutover:
```bash
dotnet ef database update
```

###+ D) ESP32 deployment
- Program device with production Wi-Fi + endpoint/Firebase URL.
- Ensure firewall rules allow device to reach backend.
- Validate device posts and dashboard updates before scaling to multiple devices.

---

## 8) Useful commands

```bash
# Build
dotnet build SmartEnergy.sln

# Run web app
cd SmartEnergy.Web && dotnet run

# Apply DB migration
cd SmartEnergy.Web && dotnet ef database update

# Publish
cd SmartEnergy.Web && dotnet publish -c Release -o ./publish
```

---

## 9) Important notes

- Current code includes hardcoded secrets/keys in some files (`appsettings.json`, `chatbot_api.py`, firmware). Move them to secure secret management before production use.
- CORS policy is currently open (`AllowAnyOrigin/Method/Header`). Restrict in production.
- Keep Firebase security rules locked down for production workloads.
- AI integration depends on Python service availability at `127.0.0.1:5001` as configured by controller logic.

---

## 10) Quick health checklist

- [ ] SQL Server running and connection string valid
- [ ] Migrations applied successfully
- [ ] ASP.NET app reachable on configured port
- [ ] AI service running on port `5001`
- [ ] ESP32 connected to Wi-Fi and posting telemetry
- [ ] Dashboard receiving live updates

