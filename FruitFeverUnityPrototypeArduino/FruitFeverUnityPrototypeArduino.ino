void setup() {
  setup_output();
  setup_input();
}

void loop()
{
  loop_output();
  loop_input();
  //Serial.println(1);
  //Serial.println(0);
  delay(10);
}

// ------------------------------------------------
// -------------------- OUTPUT --------------------
// ------------------------------------------------

#include <Adafruit_NeoPixel.h>
#include <avr/power.h> // Comment out this line for non-AVR boards (Arduino Due, etc.)

#define PIN 18

// Parameter 1 = number of pixels in strip
// Parameter 2 = Arduino pin number (most are valid)
// Parameter 3 = pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
Adafruit_NeoPixel strip = Adafruit_NeoPixel(29, PIN, NEO_GRB + NEO_KHZ800);

// IMPORTANT: To reduce NeoPixel burnout risk, add 1000 uF capacitor across
// pixel power leads, add 300 - 500 Ohm resistor on first pixel's data input
// and minimize distance between Arduino and first pixel.  Avoid connecting
// on a live circuit...if you must, connect GND first.

int PINS_THERMOMETER[] = { 6, 5, 4, 3, 2, 1, 0 };
int PINS_HEART[] = { 15, 16, 17, 18, 7, 8, 9, 10, 11, 12, 13, 14 };
int PINS_STOMACH[] = { 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 };

const int SCALE_OFFSET_TO_EACH_SIDE = 3;
const int SCALE_COUNT = SCALE_OFFSET_TO_EACH_SIDE * 2 + 1;

const int COUNT_THERMOMETER = 7;
const int COUNT_HEART = 12;
const int COUNT_STOMACH = 10;

uint32_t COLOR_EMPTY;
uint32_t COLOR_UNFILLED;
uint32_t COLOR_CORRECT;
uint32_t COLOR_INCORRECT;
uint32_t COLOR_TARGET;

void setup_output() {
  // This is for Trinket 5V 16MHz, you can remove these three lines if you are not using a Trinket
#if defined (__AVR_ATtiny85__)
  if (F_CPU == 16000000) clock_prescale_set(clock_div_1);
#endif
  // End of trinket special code

  COLOR_EMPTY = strip.Color(0, 0, 0);
  COLOR_UNFILLED = strip.Color(5, 5, 5);
  COLOR_CORRECT = strip.Color(0, 255, 0);
  COLOR_INCORRECT = strip.Color(255, 0, 0);
  COLOR_TARGET = strip.Color(255, 255, 0);

  strip.begin();
  strip.show(); // Initialize all pixels to 'off'

  //setValue(1, OFFSET_THERMOMETER, COUNT_THERMOMETER);
  //setValue(2, OFFSET_HEART, COUNT_HEART);
  //setValue(2, OFFSET_STOMACH, COUNT_STOMACH);
  strip.show();
}

void loop_output()
{
  //testCycle();
  
  if ((Serial.available() >= 4) && (Serial.peek() == 100))
  {
    Serial.read(); // throw away the 100
    int value1 = Serial.read();
    int value2 = Serial.read();
    int value3 = Serial.read();
    setValue(value1, COUNT_THERMOMETER, PINS_THERMOMETER);
    setValue(value2, COUNT_HEART, PINS_HEART);
    setValue(value3, COUNT_STOMACH, PINS_STOMACH);
  }
}

void testCycle()
{
  for (int i = 0; i < SCALE_COUNT; i++)
  {
    setValue(i, COUNT_THERMOMETER, PINS_THERMOMETER);
    setValue(i, COUNT_HEART, PINS_HEART);
    setValue(i, COUNT_STOMACH, PINS_STOMACH);
    delay(500);
  }
}

void setValue(int value, int count, int pins[])
{
  bool empty = (value == 99);
  if (empty)
  {
    for (int i = 0; i < count; i++)
    {
      strip.setPixelColor(pins[i], COLOR_EMPTY);
    }
  }
  else
  {
    bool correct = (value - SCALE_OFFSET_TO_EACH_SIDE) == 0;
    uint32_t filledColor = correct ? COLOR_CORRECT : COLOR_INCORRECT;
  
    int lightAmount = 1 + (int)(((float) value / (SCALE_COUNT - 1)) * (count - 1));
    for (int i = 0; i < count; i++)
    {
      uint32_t color = (i < lightAmount) ? filledColor : COLOR_UNFILLED;
      strip.setPixelColor(pins[i], color);
    }
  
    if (!correct)
    {
      strip.setPixelColor(pins[(int)((count - 0.5) / 2.0)], COLOR_TARGET);
    }
  }

  strip.show();
}

// ------------------------------------------------
// -------------------- INPUT ---------------------
// ------------------------------------------------

const int ANALOG_IN = A5;
const int STATE_RESOLUTION = 50;

const int FOOD_COUNT = 4;
const char* FOODS[] = {"None", "Kiwi", "Blueberry", "Cucumber", "Cheese"};
const int RANGES[] = {
                  1020, 1024,
                  900, 1019,
                  600, 899,
                  300, 599,
                  0, 299,
               };

int _testing_food = 0;
int _food_on_fork = -1;
int _state_count = 0;

void setup_input() {
  Serial.begin(9600);
}

void loop_input() {
  int sensorValue = analogRead(ANALOG_IN);

  int current_food = 0;
  for (int i = 0; i <= FOOD_COUNT; i++) {
    int lower = i*2, upper = lower + 1;
    if (sensorValue >= RANGES[lower] && sensorValue <= RANGES[upper]) {
      current_food = i;
      break;
    }
  }
  if (_testing_food == current_food) {
    _state_count++;
  } else {
    _state_count = 0;
    _testing_food = current_food;
  }

  if (_state_count >= STATE_RESOLUTION && _testing_food != _food_on_fork) {
    //this code compensates for occassional false readings when
    //food is removed from the fork
//    if (_food_on_fork != 0 && _testing_food != 0) {
//      _food_on_fork = 0;
//    } else {
      _food_on_fork = _testing_food;
//    }
//    Serial.println(FOODS[_food_on_fork]);
    Serial.println(_food_on_fork);
  }
}
