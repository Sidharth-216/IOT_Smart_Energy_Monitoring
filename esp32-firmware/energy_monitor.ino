// energy_monitor.ino(main code)(it is a RESTApi)
#include <WiFi.h>
#include <HTTPClient.h>

// Replace with your WiFi credentials
const char* ssid = "YOUR_SSID";
const char* password = "YOUR_PASSWORD";

// Your backend API endpoint or the link between the Ardunio IDE to the .net mvc
const char* serverUrl = "http://192.168.29.127:5048/api/energyreading";
;

// ADC pins
const int VOLT_PIN = 35; // ZMPT101B OUT pin connected here
const int CURR_PIN = 34; // ACS712 OUT pin connected here

// Calibration constants (you'll adjust these later experimentally)
float voltageCalibration = 220.0; // adjust after real test
float currentCalibration = 30.0;  // adjust after real test

void setup() {
  Serial.begin(115200);
  delay(1000);

  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected!");
}

void loop() {
  // Read raw ADC values
  int rawV = analogRead(VOLT_PIN);
  int rawC = analogRead(CURR_PIN);

  // ESP32 ADC: 12-bit (0–4095), reference voltage ~3.3V
  float voltageSample = (rawV / 4095.0) * 3.3;
  float currentSample = (rawC / 4095.0) * 3.3;

  // Convert to real-world units using calibration factors
  float mainsVoltage = voltageSample * voltageCalibration; // e.g., ~220V
  float mainsCurrent = currentSample * currentCalibration; // e.g., ~1–10A

  // Calculate power
  float power = mainsVoltage * mainsCurrent;

  // Debug output
  Serial.print("Voltage: "); Serial.print(mainsVoltage, 2); Serial.print(" V, ");
  Serial.print("Current: "); Serial.print(mainsCurrent, 2); Serial.print(" A, ");
  Serial.print("Power: "); Serial.print(power, 2); Serial.println(" W");

  // Send to backend if WiFi is connected
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;

    String jsonPayload = "{\"DeviceId\":1, \"Voltage\":" + String(mainsVoltage,2) +
                        ", \"Current\":" + String(mainsCurrent,2) +
                        ", \"Power\":" + String(power,2) + "}";

    http.begin(serverUrl);
    http.addHeader("Content-Type", "application/json");

    int httpResponseCode = http.POST(jsonPayload);

    if (httpResponseCode > 0) {
      String response = http.getString();
      Serial.print("POST response code: ");
      Serial.println(httpResponseCode);
      Serial.println("Server response: " + response);
    } else {
      Serial.print("POST failed, error: ");
      Serial.println(http.errorToString(httpResponseCode));
    }

    http.end();

  } else {
    Serial.println("WiFi not connected!");
  }

  delay(5000); // repeat every 5 seconds
}


/*
// energy_monitor.ino
#include <WiFi.h>
#include <HTTPClient.h>
#include <WiFiClientSecure.h>  // Needed for HTTPS (optional)

// ===== WiFi Credentials =====
const char* ssid = "YOUR_SSID";
const char* password = "YOUR_PASSWORD";

// ===== API Endpoint =====
// For HTTP (current)
const char* serverUrl = "http://192.168.29.127:5048/api/energyreading";

// For HTTPS (future) -> enable this after you configure SSL in ASP.NET Core
// const char* serverUrl = "https://192.168.29.127:7048/api/energyreading";

// Optional Root CA (for HTTPS)
// static const char* root_ca PROGMEM = R"EOF(
// -----BEGIN CERTIFICATE-----
// YOUR_SERVER_CERTIFICATE_HERE
// -----END CERTIFICATE-----
// )EOF";

// ===== Device Info =====
int deviceId = 1; // configurable if you have multiple devices

// ===== Sensor Pins =====
const int VOLT_PIN = 35; // ZMPT101B OUT pin
const int CURR_PIN = 34; // ACS712 OUT pin

// ===== Calibration Factors (tune these experimentally) =====
float voltageCalibration = 220.0;  // e.g., multiply ADC result to map ~220V
float currentCalibration = 30.0;   // e.g., multiply ADC result to match real current

// ===== WiFi Connection Helper =====
void connectWiFi() {
  if (WiFi.status() != WL_CONNECTED) {
    WiFi.begin(ssid, password);
    Serial.print("Connecting to WiFi");
    unsigned long start = millis();
    while (WiFi.status() != WL_CONNECTED && millis() - start < 20000) {
      delay(500);
      Serial.print(".");
    }
    if (WiFi.status() == WL_CONNECTED) {
      Serial.println("\n✅ WiFi Connected!");
      Serial.print("IP Address: ");
      Serial.println(WiFi.localIP());
    } else {
      Serial.println("\n❌ WiFi Connection Failed!");
    }
  }
}

void setup() {
  Serial.begin(115200);
  delay(1000);
  connectWiFi();
}

void loop() {
  connectWiFi(); // ensure always connected

  // ===== Read Sensor Data =====
  int rawV = analogRead(VOLT_PIN);
  int rawC = analogRead(CURR_PIN);

  // Convert ADC to voltage (ESP32 ADC = 0–4095 for 0–3.3V)
  float voltageSample = (rawV / 4095.0) * 3.3;
  float currentSample = (rawC / 4095.0) * 3.3;

  // Apply calibration
  float mainsVoltage = voltageSample * voltageCalibration;
  float mainsCurrent = currentSample * currentCalibration;
  float power = mainsVoltage * mainsCurrent;

  // Debug
  Serial.printf("Voltage: %.2f V, Current: %.2f A, Power: %.2f W\n",
                mainsVoltage, mainsCurrent, power);

  // ===== Send Data to Backend =====
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;

    // If HTTPS:
    // WiFiClientSecure client;
    // client.setInsecure(); // ⚠️ use only for testing without cert
    // client.setCACert(root_ca);
    // http.begin(client, serverUrl);

    http.begin(serverUrl); // HTTP
    http.addHeader("Content-Type", "application/json");

    String jsonPayload = "{\"DeviceId\":" + String(deviceId) +
                         ",\"Voltage\":" + String(mainsVoltage, 2) +
                         ",\"Current\":" + String(mainsCurrent, 2) +
                         ",\"Power\":" + String(power, 2) + "}";

    int httpResponseCode = http.POST(jsonPayload);

    if (httpResponseCode > 0) {
      Serial.printf("✅ POST Success! Code: %d\n", httpResponseCode);
      Serial.println("Server response: " + http.getString());
    } else {
      Serial.printf("❌ POST Failed. Error: %s\n",
                    http.errorToString(httpResponseCode).c_str());
    }

    http.end();
  } else {
    Serial.println("⚠️ WiFi not connected, skipping POST");
  }

  delay(5000); // send every 5 sec
}

*/