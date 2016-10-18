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

int Relay1 = 12;
int Relay2 = 14;
// Host
const char* host = "iotdemo.apexsoftworks.in";

int redLed = 12;                // the pin that the LED is atteched to
int greenLed = 14;                // the pin that the LED is atteched to
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
//  pinMode(greenLed, OUTPUT);      // initalize LED as an output
//  pinMode(redLed, OUTPUT);      // initalize LED as an output
//  pinMode(configLedPin, OUTPUT);
//  pinMode(buttonPin, INPUT);
  pinMode(wifiLedDisconnectPin, OUTPUT);
  pinMode(Relay1, OUTPUT);


  deviceMac = WiFi.macAddress();
  Log("Device MAC " + deviceMac, 1);
  // Serial
  Serial.begin(115200);
}

void postData(RestClient restClient) {

  restClient.setConnectionTimeout(20);

  Serial.println("Create START URL to post...");
  String url = "/api/getrelaygrouprequest?relayGroupMac=" + String(deviceMac);
  Serial.println("Creat FINISH URL to post...\nURL = " + url);
  String response;
  response = "";
  Serial.println("Posting data to service... Waiting for response");

  int statusCode = restClient.get(url.c_str(), &response);

  Serial.println("Status recieved... Going to set frequency...");
  int firstVal;
  int secondVal;
  int thirdVal;
  if (statusCode == 200 && !isnan(statusCode) )
  {
    Serial.println("response recieved : " + response + "\n Converting to int type");

    for (int i = 0; i < response.length(); i++) {
      if (response.substring(i, i + 1) == "-") {
        firstVal = response.substring(0, i).toInt();
        secondVal = response.substring(i + 1, i + 2).toInt();
        thirdVal = response.substring(i + 3).toInt();
        Serial.println("Response values " + String(firstVal) + "--" + String(secondVal) + "--" + String(thirdVal));
        break;
      }
    }

    if(secondVal==1)
    digitalWrite(Relay1, LOW);
    Serial.println("Relay 1 Circuit Close");
    if(secondVal==0)
    digitalWrite(Relay1, HIGH);
    Serial.println("Relay 1 Circuit Open");
    
    int changedStatus = secondVal==1?0:1; 
    
    url = "/api/RequestApi?id=" + String(thirdVal)+"&currentStatus="+String(changedStatus);
    Serial.println("Creat FINISH URL to post...\nURL = " + url);
    Serial.println("Posting data to service... Waiting for response");

    statusCode = restClient.put(url.c_str(), "");

    Serial.print("Status code from server: ");
    Serial.println(statusCode);
    Serial.print("Response body from server: ");
    Serial.println(response);
  } else
  {
    return;
  }
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
  wifiManager.autoConnect();


  //if you get here you have connected to the WiFi
  Serial.println("connected...yeey :)");
  digitalWrite(wifiLedDisconnectPin, LOW);

}

void loop()
{
  deviceMac = WiFi.macAddress();
  Log("Device MAC " + deviceMac, 1);
  Serial.println("Press Config button to go into config mode");
  if (digitalRead(buttonPin) == LOW && digitalRead(buttonPin)==HIGH) {
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
        ////ESP.deepSleep(sleepTimeHotspotUnavailable * 1000000, RF_DEFAULT);
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

      Serial.print("TRY : " + String(count) + "\n" );
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

      return;
    }
    postData(restClient);
  }
}

void Log(String message, int executionMode)
{
  if (executionMode == 1)
    Serial.println(message);
}
