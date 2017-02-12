#include <WiFiUdp.h>
#include <WiFiServer.h>
#include <WiFiClientSecure.h>
#include <WiFiClient.h>
#include <ESP8266WiFiType.h>
#include <ESP8266WiFiSTA.h>
#include <ESP8266WiFiScan.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266WiFiGeneric.h>
#include <ESP8266WiFiAP.h>
#include <Arduino.h>
#include <ESP8266WiFi.h>          //https://github.com/esp8266/Arduino
//needed for library
#include <DNSServer.h>
#include <ESP8266WebServer.h>
#include <WiFiManager.h>
#include "RestClient.h"
#include "DHT.h"

#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <EEPROM.h>

#define DHTPIN 13     // what pin we're connected to
#define DHTTYPE DHT22   // DHT 22  (AM2302)
//#define fan 4

int maxHum = 60;
int maxTemp = 40;

DHT dht(DHTPIN, DHTTYPE);

// WiFi settings
const char* ssid = "APEX";
const char* password = "apex-wifi";

// Time to sleep (in seconds):
int sleepTimeS = 60;
int dataPostFrequency = 6;
int sleepTimeHotspotUnavailable = 60;
int sensorOutputNotAvailbaleTimeout = 2;
int maxSensorReadTry = 10;
int maxTryCountToConnectWifi = 60;
int wifiConnectingCountStart = 20;
int operationMode = 1;
// Host
const char* host = "oa-iotdemo.azurewebsites.net";

int redLed = 14;                // the pin that the LED is atteched to
int greenLed = 12;                // the pin that the LED is atteched to
int buttonPin = 5;
int configLedPin = 4;
int wifiLedDisconnectPin = 15;
int state = LOW;             // by default, no motion detected
int val = 0;
float analogVal = 0;
int motion = 0;
int count = 0;

float temperature;
float humidity;
String deviceMac;

RestClient restClient = RestClient(host);

void setup()
{
  pinMode(greenLed, OUTPUT);      // initalize LED as an output
  pinMode(redLed, OUTPUT);      // initalize LED as an output
  pinMode(configLedPin, OUTPUT);
  pinMode(buttonPin, INPUT);
  pinMode(wifiLedDisconnectPin, OUTPUT);

  EEPROM.begin(512);
  dht.begin();
  Log("dht initialized...", 1);

  deviceMac = WiFi.macAddress();
  Log("Device MAC " + deviceMac, 1);
  // Serial
  Serial.begin(115200);
}

void postData(RestClient restClient) {

  restClient.setConnectionTimeout(20);

  Serial.println("Create START URL to post...");
  String url = "/api/motionsensor?MotionValue=" + String(temperature) + "&MotionTime=" + String(humidity) + "&DeviceId=" + String(deviceMac);
  Serial.println("Creat FINISH URL to post...\nURL = " + url);
  String response;
  response = "";
  Serial.println("Posting data to service... Waiting for response");

  int statusCode = restClient.post(url.c_str(), "", &response);

  Serial.println("Status recieved... Going to set frequency...");

  if (statusCode == 200 && !isnan(statusCode))
  {
    Serial.println("response recieved : " + response + "\n Converting to int type");
    dataPostFrequency = response.toInt();
  }
  else
  {
    dataPostFrequency = sensorOutputNotAvailbaleTimeout;
  }

  Serial.print("Status code from server: ");
  Serial.println(statusCode);
  Serial.print("Response body from server: ");
  Serial.println(response);
}

void wifiSetup()
{

  //WiFi.begin(ssid, password);
  digitalWrite(wifiLedDisconnectPin, HIGH); // indication of esp going into config mode
  Serial.println("Config button pressed... going to auto connect ");
  WiFiManager wifiManager;
  //reset saved settings
  //wifiManager.resetSettings();

  //set custom ip for portal
  //wifiManager.setAPConfig(IPAddress(10,0,1,1), IPAddress(10,0,1,1), IPAddress(255,255,255,0));

  //fetches ssid and pass from eeprom and tries to connect
  //if it does not connect it starts an access point with the specified name
  //here  "AutoConnectAP"
  //and goes into a blocking loop awaiting configuration
  //wifiManager.autoConnect(String(deviceMac, "123456");
  //or use this for auto generated name ESP + ChipID
  //wifiManager.autoConnect();
  wifiManager.resetSettings();
  wifiManager.autoConnect();

  Serial.println("please switch off the config button else you will need to configure again.");
  delay(5000000);


  //if you get here you have connected to the WiFi
  Serial.println("connected...yeey :)");
  digitalWrite(wifiLedDisconnectPin, LOW);

  //if (WiFi.status() != WL_CONNECTED)
  //{
  //  ESP.restart();
  //}
}

void loop()
{
  Serial.println("Press Config button to go into config mode");
  if (digitalRead(buttonPin) == LOW) {
    wifiSetup();
  }
  else
  {
    Serial.println("ESP8266 in normal mode");

    // Connect to WiFi
    while (WiFi.status() != WL_CONNECTED) {
      if (count == maxTryCountToConnectWifi)
      {
        Serial.println("ESP8266 will retry connecting after 60 seconds");
        //ESP.deepSleep(sleepTimeHotspotUnavailable * 1000000, RF_DEFAULT);
        delay(5 * 1000);
      }
      if (digitalRead(wifiLedDisconnectPin) != HIGH && count > wifiConnectingCountStart) {
        Serial.println("Wifi Disconnect LED should blink");
        digitalWrite(wifiLedDisconnectPin, HIGH);
        delay(1000);
      }

      count++;
      if (digitalRead(wifiLedDisconnectPin) == HIGH) {
        digitalWrite(wifiLedDisconnectPin, LOW);
        delay(1000);
      }

      if (digitalRead(buttonPin) == LOW)
      {
        wifiSetup();
      }

      Serial.print("TRY : " + String(count) + "\n");
      delay(1000);
    }
    Serial.println("");
    Serial.println("WiFi connected");

    // Print the IP address
    Serial.println(WiFi.localIP());
    digitalWrite(configLedPin, LOW);

    // Logging data to cloud
    Serial.print("Connecting to ");
    Serial.println(host);

    // Use WiFiClient class to create TCP connections
    WiFiClient client;
    const int httpPort = 80;
    if (!client.connect(host, httpPort)) {
      Serial.println("connection failed");
      digitalWrite(configLedPin, HIGH);
      delay(2000);
      digitalWrite(configLedPin, LOW);
      Serial.println("ESP8266 will retry connecting to site after 60 seconds");
      //ESP.deepSleep(sleepTimeHotspotUnavailable * 1000000, RF_DEFAULT);
      delay(2000);
      //return;
    }

    //temperature = getTemperature(sensorPin);

    humidity = dht.readHumidity();
    // Read temperature as Celsius
    temperature = dht.readTemperature();
    int sensorRetryCount = 0;
    while ((isnan(humidity) || isnan(temperature)) && sensorRetryCount < maxSensorReadTry)
    {
      humidity = dht.readHumidity();
      temperature = dht.readTemperature();
      sensorRetryCount++;
      Serial.println("Sensor Read try " + String(sensorRetryCount));
      delay(1000);
    }

    Serial.print("Humidity: ");
    Serial.print(humidity);
    Serial.print(" %\t");
    Serial.print("Temperature: ");
    Serial.print(temperature);
    Serial.println(" *C ");

    // Check if any reads failed and exit early (to try again).
    if (isnan(humidity) || isnan(temperature)) {
      Serial.println("Failed to read from DHT sensor!... ESP will try reconnecting after 60 seconds");
      //ESP.deepSleep(sensorOutputNotAvailbaleTimeout * 1000000, RF_DEFAULT);
      delay(2000);
    }

    if (temperature > 33.00)
    {
      Serial.println("Red LED will Blink");
      digitalWrite(redLed, HIGH);
    }
    if (temperature <= 33.00)
    {
      Serial.println("Green LED will Blink");
      digitalWrite(greenLed, HIGH);
    }
    postData(restClient);
    digitalWrite(redLed, LOW);
    digitalWrite(greenLed, LOW);
    Serial.println("going to sleep");

    //ESP.deepSleep(dataPostFrequency * 1000000, RF_DEFAULT);
    delay(1000 * dataPostFrequency);
    //delay(dataPostFrequency*1000);
    // Read all the lines of the reply from server and print them to Serial
    //  while (client.available()) {
    //    String line = client.readStringUntil('\r');
    //    Serial.print(line);
    //  }

    //Serial.println();
    //Serial.println("closing connection");
  }
}

void Log(String message, int executionMode)
{
  if (executionMode == 1)
    Serial.println(message);
}




