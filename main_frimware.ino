/******************  UNIVERSAL SMART ENERGY MONITORING - CALIBRATED (Firebase)  ******************/
#include <WiFi.h>
#include <HTTPClient.h>
#include <WiFiClient.h>
#include <math.h>

// ---------- USER SETTINGS ----------
#define DEVICE_ID "ESP32-001"

// === WiFi / Firebase URL ===
const char* WIFI_SSID = "Sidhu";
const char* WIFI_PASS = "12345678";
const char* FIREBASE_URL = "https://esp32-testing-aec8b-default-rtdb.firebaseio.com"; // Replace with your DB URL

// Analog Pins
const int voltagePin = 34;   // ZMPT101B output
const int currentPin = 35;   // ACS712-30A output

// CALIBRATED VALUES
float voltageCalibration = 250.0;
float currentCalibration = 15.15;

// Detection thresholds
const float MIN_VOLTAGE_THRESHOLD = 50.0;
const float MIN_CURRENT_THRESHOLD = 0.015;
const float NOISE_CURRENT_THRESHOLD = 0.010;
const int SAMPLES_FOR_DETECTION = 12;
const int CONSECUTIVE_READINGS = 3;

// Variables
unsigned long readingID = 0;
float energy_Wh = 0.0;
unsigned long lastMillis = 0;
bool deviceConnected = false;
bool lastDeviceState = false;
int consecutiveDetections = 0;
int consecutiveNoDetections = 0;

// Auto-ranging
String deviceType = "Unknown";
float avgPower = 0;
int powerReadings = 0;

// Calibration offsets
float voltageOffset = 0;
float currentOffset = 0;

// ---------- WiFi Connect ----------
void connectWiFi() {
  Serial.print("📶 Connecting to WiFi ");
  WiFi.begin(WIFI_SSID, WIFI_PASS);
  unsigned long start = millis();
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    if (millis() - start > 15000) {
      Serial.println("\n⚠ WiFi connect timeout. Retrying...");
      WiFi.disconnect();
      delay(1000);
      WiFi.begin(WIFI_SSID, WIFI_PASS);
      start = millis();
    }
  }
  Serial.println("\n✓ WiFi connected: " + WiFi.localIP().toString());
}

// ---------- Setup ----------
void setup() {
  Serial.begin(115200);
  delay(1000);

  analogReadResolution(12);
  analogSetAttenuation(ADC_11db);

  Serial.println("\n╔════════ UNIVERSAL SMART ENERGY MONITOR ════════╗");
  Serial.println("Device ID: " + String(DEVICE_ID));
  Serial.println("⚙ Initializing sensors...");

  connectWiFi();
  delay(1000);
  calibrateSensors();

  Serial.println("\n✓ System Ready!\n⏳ Waiting for device connection...");
}

// ---------- Sensor Calibration ----------
void calibrateSensors() {
  Serial.println("📍 Calibrating sensors...");
  delay(2000);

  float vSum = 0;
  for (int i = 0; i < 1000; i++) {
    int raw = analogRead(voltagePin);
    vSum += (raw / 4095.0) * 3.3;
    delayMicroseconds(50);
  }
  voltageOffset = vSum / 1000.0;

  float cSum = 0;
  for (int i = 0; i < 1000; i++) {
    int raw = analogRead(currentPin);
    cSum += (raw / 4095.0) * 3.3;
    delayMicroseconds(50);
  }
  currentOffset = cSum / 1000.0;

  Serial.print("Voltage Offset: "); Serial.println(voltageOffset, 4);
  Serial.print("Current Offset: "); Serial.println(currentOffset, 4);
}

// ---------- RMS Calculations ----------
float getVoltageRMS() {
  const int numSamples = 1000;
  float sumSquares = 0;
  for (int i = 0; i < numSamples; i++) {
    float centered = ((analogRead(voltagePin) / 4095.0) * 3.3) - voltageOffset;
    sumSquares += centered * centered;
    delayMicroseconds(20);
  }
  float rms = sqrt(sumSquares / numSamples);
  float actualVoltage = rms * voltageCalibration;
  if (actualVoltage < 0 || actualVoltage > 280) actualVoltage = 0;
  return actualVoltage;
}

float getCurrentRMS() {
  const int numSamples = 1000;
  float sumSquares = 0;
  for (int i = 0; i < numSamples; i++) {
    float centered = ((analogRead(currentPin) / 4095.0) * 3.3) - currentOffset;
    sumSquares += centered * centered;
    delayMicroseconds(20);
  }
  float rms = sqrt(sumSquares / numSamples);
  float current = rms * currentCalibration;
  if (current < 0.008 || current > 32) current = 0;
  return current;
}

// ---------- Firebase POST ----------
bool sendToFirebase(float voltage, float current, float power) {
  if (WiFi.status() != WL_CONNECTED) {
    connectWiFi();
    if (WiFi.status() != WL_CONNECTED) return false;
  }

  HTTPClient http;
  String url = String(FIREBASE_URL) + "/readings/" + DEVICE_ID + ".json";
  http.begin(url);
  http.addHeader("Content-Type", "application/json");

  String payload = "{";
  payload += "\"voltage\":" + String(voltage, 1);
  payload += ",\"current\":" + String(current, 4);
  payload += ",\"power\":" + String(power, 6);
  payload += ",\"timestamp\":" + String(millis());
  payload += "}";

  int code = http.PUT(payload);
  if (code > 0) {
    Serial.print("Firebase Response code: "); Serial.println(code);
    http.end();
    return (code >= 200 && code < 300);
  } else {
    Serial.print("Firebase POST failed: "); Serial.println(code);
    http.end();
    return false;
  }
}

// ---------- Device Detection ----------
bool detectDevice() {
  float voltageSum = 0, currentSum = 0;
  int validReadings = 0;
  for (int i = 0; i < SAMPLES_FOR_DETECTION; i++) {
    float v = getVoltageRMS();
    float c = getCurrentRMS();
    if (v > MIN_VOLTAGE_THRESHOLD && c > NOISE_CURRENT_THRESHOLD) {
      voltageSum += v;
      currentSum += c;
      validReadings++;
    }
    delay(80);
  }
  float avgCurrent = (validReadings > 0) ? (currentSum / validReadings) : 0;
  return (validReadings >= (SAMPLES_FOR_DETECTION * 0.4) && avgCurrent > MIN_CURRENT_THRESHOLD);
}

// ---------- Device Classification ----------
void classifyDevice(float power) {
  if (power < 0.5) deviceType = "Standby/Idle";
  else if (power < 15) deviceType = "LED Bulb";
  else if (power < 60) deviceType = "CFL / Small Appliance";
  else if (power < 200) deviceType = "Fan / TV / Electronics";
  else if (power < 500) deviceType = "Small Heater / Iron";
  else if (power < 1500) deviceType = "AC / Heater / Kettle";
  else if (power < 2500) deviceType = "High Power Heater";
  else deviceType = "Very High Power Device";
}

// ---------- Main Loop ----------
void loop() {
  if (WiFi.status() != WL_CONNECTED) connectWiFi();

  bool currentDetection = detectDevice();
  if (currentDetection) {
    consecutiveDetections++; consecutiveNoDetections = 0;
    if (consecutiveDetections >= CONSECUTIVE_READINGS && !deviceConnected) deviceConnected = true;
  } else {
    consecutiveNoDetections++; consecutiveDetections = 0;
    if (consecutiveNoDetections >= CONSECUTIVE_READINGS && deviceConnected) deviceConnected = false;
  }

  if (deviceConnected != lastDeviceState) {
    if (deviceConnected) {
      Serial.println("\n✓ DEVICE CONNECTED\n");
      energy_Wh = 0; readingID = 0; avgPower = 0; powerReadings = 0; lastMillis = millis(); deviceType = "Unknown";
    } else {
      Serial.println("\n✗ DEVICE DISCONNECTED\n"); lastMillis = 0; deviceType = "Unknown";
    }
    lastDeviceState = deviceConnected;
  }

  if (deviceConnected) {
    float voltage = getVoltageRMS();
    float current = getCurrentRMS();
    float power = voltage * current; // Watts

    powerReadings++;
    avgPower = ((avgPower * (powerReadings - 1)) + power) / powerReadings;
    classifyDevice(avgPower);

    unsigned long now = millis();
    if (lastMillis > 0) energy_Wh += power * ((now - lastMillis) / 3600000.0);
    lastMillis = now;

    readingID++;
    Serial.printf("Reading #%lu | Voltage: %.1f V | Current: %.4f A | Power: %.2f W | Device: %s\n",
                  readingID, voltage, current, power, deviceType.c_str());

    // Send to Firebase
    if (!sendToFirebase(voltage, current, power)) {
      delay(1000);
      sendToFirebase(voltage, current, power);
    }
    delay(2000);
  } else {
    delay(2000);
  }
}