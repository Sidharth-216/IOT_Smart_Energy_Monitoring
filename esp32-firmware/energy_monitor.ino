// energy_monitor.ino
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
