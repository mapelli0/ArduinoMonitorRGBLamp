#include <Arduino_APDS9960.h>
#include <Adafruit_NeoPixel.h>
#define LED_PIN 2
#define NUM_LEDS 120


Adafruit_NeoPixel pixels(NUM_LEDS, LED_PIN, NEO_GRBW + NEO_KHZ800);

void initializeLights(){
  pixels.begin(); // INITIALIZE NeoPixel strip object (REQUIRED)
  pixels.setBrightness(100);
  pixels.clear();  
  pixels.show();
}

void initializeGestureSensor(){
  //Serial.begin(9600);
  //while (!Serial);
  if (APDS.begin()){
    digitalWrite(LEDR, HIGH);
  } else {
    digitalWrite(LEDR, LOW);
  }
  APDS.setGestureSensitivity(80);
  //Serial.println("Detecting gestures ...");
}

void setup() {
  // put your setup code here, to run once:
  pinMode(LEDR, OUTPUT);
  initializeLights();
  initializeGestureSensor();  
}

void toggleLights(){
  uint32_t colorStatus 	= pixels.getPixelColor(0);  
  if(colorStatus == 0){    
    pixels.fill(pixels.Color(255, 0 ,255 , 0), 0, 120);
  } else {    
    pixels.fill(pixels.Color(0, 0 ,0 , 0), 0, 120);    
  }
  pixels.show();
}

void loop() {  
      // turn the LED off   
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
