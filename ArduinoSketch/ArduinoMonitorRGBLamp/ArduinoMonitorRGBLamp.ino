#include <Arduino_APDS9960.h>
#include <Adafruit_NeoPixel.h>
#include <ArduinoBLE.h>

BLEService colorChangeService("180C");                                     // User defined service
BLEStringCharacteristic colorChangeCharacteristic("2A56",                  // standard 16-bit characteristic UUID
                                                  BLERead | BLEWrite, 3);  // remote clients will only be able to read this

#define LED_PIN 2
#define NUM_LEDS 120
#define saveColorAddressStart 0

struct StripColor {
  uint8_t r;
  uint8_t g;
  uint8_t b;
};

Adafruit_NeoPixel pixels(NUM_LEDS, LED_PIN, NEO_GRBW + NEO_KHZ800);

void initializeLights() {
  pixels.begin();  // INITIALIZE NeoPixel strip object (REQUIRED)
  pixels.setBrightness(100);
  pixels.clear();
  pixels.show();
}

void initializeGestureSensor() {
  if (APDS.begin()) {
    digitalWrite(LEDR, HIGH);
  } else {
    digitalWrite(LEDR, LOW);
  }
  APDS.setGestureSensitivity(80);
}

void initializeBLE() {
  if (!BLE.begin()) {  // initialize BLE
    while (1)
      ;
  }
  BLE.setLocalName("MonitorRGBLamp");                               // Set name for connection
  BLE.setAdvertisedService(colorChangeService);                     // Advertise service
  colorChangeService.addCharacteristic(colorChangeCharacteristic);  // Add characteristic to service
  BLE.addService(colorChangeService);                               // Add service

  BLE.advertise();  // Start advertising
}

void setup() {
  //Serial.begin(115200);
  //while (!Serial);
  // put your setup code here, to run once:
  pinMode(LEDR, OUTPUT);
  pinMode(LEDB, OUTPUT);  // initialize the blue LED pin
  digitalWrite(LEDR, LOW);
  digitalWrite(LEDB, HIGH);
  initializeLights();
  initializeGestureSensor();
  initializeBLE();

  //Serial.println("BLE Central - LED control");
}

void toggleLights(StripColor c = { 0, 0, 0 }) {
  uint32_t colorStatus = pixels.getPixelColor(0);
  if (colorStatus == 0) {
    if ((c.r == 0) && (c.g == 0) && (c.b == 0)) {
      c.r = 255;
      c.b = 255;
    }
    pixels.fill(pixels.Color(c.r, c.g, c.b, 0), 0, 120);
  } else {
    pixels.fill(pixels.Color(0, 0, 0, 0), 0, 120);
  }
  pixels.show();
}

void readGesture() {
  if (APDS.gestureAvailable()) {
    // a gesture was detected, read and print to Serial Monitor
    int gesture = APDS.readGesture();

    switch (gesture) {
      case GESTURE_UP:
        // Serial.println("Detected UP gesture");
        // break;

      case GESTURE_DOWN:
        // Serial.println("Detected DOWN gesture");
        // break;

      case GESTURE_LEFT:
        // Serial.println("Detected LEFT gesture");
        // break;

      case GESTURE_RIGHT:
        //Serial.println("Detected gesture");
        toggleLights();
        break;

      default:
        // ignore
        break;
    }
  }
}

void tryReadBLE() {
  BLEDevice central = BLE.central();  // Wait for a BLE central to connect

  if (central) {

    //Serial.print("Connected to central MAC: ");
    // print the central's BT address:
    //Serial.println(central.address());
    // turn on the LED to indicate the connection:
    digitalWrite(LEDB, LOW);

    while (central.connected()) {
      readGesture();
      if (colorChangeCharacteristic.written()) {
        uint8_t value[3];
        colorChangeCharacteristic.readValue(value, 3);
        StripColor c = { value[0], value[1], value[2] };
        // Serial.println("I received: ");
        // Serial.println(value[0]); //red
        // Serial.println(value[1]); //green
        // Serial.println(value[2]); //blue


        toggleLights(c);
      }
    }  // keep looping while connected

    //Serial.print("Disconnected....");
    // when the central disconnects, turn off the LED:
    digitalWrite(LEDB, HIGH);
  }
}

void loop() {
  readGesture();
  tryReadBLE();
}