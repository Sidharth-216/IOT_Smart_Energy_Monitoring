// sensor_test.ino
void setup() {
  Serial.begin(115200);
  delay(1000);
}

void loop() {
  // Simulated sensor values
  float voltage = 230.0 + random(-5, 5);
  float current = 5.0 + random(-1, 1);
  float power = voltage * current;

  Serial.print("Voltage: "); Serial.println(voltage);
  Serial.print("Current: "); Serial.println(current);
  Serial.print("Power: "); Serial.println(power);
  Serial.println("---");

  delay(2000);
}
