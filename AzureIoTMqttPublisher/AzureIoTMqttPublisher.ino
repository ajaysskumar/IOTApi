

/*
  Basic ESP8266 MQTT example

  This sketch demonstrates the capabilities of the pubsub library in combination
  with the ESP8266 board/library.

  It connects to an MQTT server then:
  - publishes "hello world" to the topic "outTopic" every two seconds
  - subscribes to the topic "inTopic", printing out any messages
  it receives. NB - it assumes the received payloads are strings not binary
  - If the first character of the topic "inTopic" is an 1, switch ON the ESP Led,
  else switch it off

  It will reconnect to the server if the connection is lost using a blocking
  reconnect function. See the 'mqtt_reconnect_nonblocking' example for how to
  achieve the same result without blocking the main loop.

  To install the ESP8266 board, (using Arduino 1.6.4+):
  - Add the following 3rd party board manager under "File -> Preferences -> Additional Boards Manager URLs":
  http://arduino.esp8266.com/stable/package_esp8266com_index.json
  - Open the "Tools -> Board -> Board Manager" and click install for the ESP8266"
  - Select your ESP8266 in "Tools -> Board"

*/
#include <TimeLib.h>
#include <Time.h>
#include <ArduinoJson.h>
#include <DHT.h>
#include <WiFiManager.h>
#include <PubSubClient.h>
#include <ESP8266WiFi.h>
#include "EEPROM.h"
#include <WiFiClientSecure.h> 

// Update these with values suitable for your network.

const char* ssid = "APEX";
const char* password = "apex-wifi";
const char* mqtt_server = "oa-iothub-dev.azure-devices.net";
String publishTopic;
String subscriptionTopic;
String relayGroupStatusTopic;
String statusXML;

int redLed = 12;                // the pin that the LED is atteched to
int greenLed = 14;

#define DHTPIN 13     // what pin we're connected to
#define DHTTYPE DHT22   // DHT 22  (AM2302)

DHT dht(DHTPIN, DHTTYPE);

WiFiClientSecure espClient;
PubSubClient client(espClient);
long lastMsg = 0;
//char msg[50];
int value = 0;

String clientId;

void setup_wifi() {

  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  wifiSetup();
  dht.begin();

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  randomSeed(micros());

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
  Serial.println(clientId);
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");

  const int payloadLength = length;

  client.publish(publishTopic.c_str(), "Hello World from NodeMCU...");
  delay(5000);

}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    Serial.println("Credentials User : " + String(clientId.c_str()));
    // Attempt to connect
    if (client.connect(clientId.c_str(), "oa-iothub-dev.azure-devices.net/18FE34DE278F", "SharedAccessSignature sr=oa-iothub-dev.azure-devices.net%2Fdevices%2F18FE34DE278F&sig=hnQcfXNeQEAZBXL8NYEQd6zObzYbJsEF0RmmxODDqbQ%3D&se=1512156962")) {
      Serial.println("connected");
    
      // ... and resubscribe
      //client.subscribe(subscriptionTopic.c_str());
    }
    else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void setup() {
 
  pinMode(greenLed, OUTPUT);      // initalize LED as an output
  pinMode(redLed, OUTPUT);      // initalize LED as an output

  Serial.begin(115200);
  EEPROM.begin(512);
  clientId = WiFi.macAddress();

  clientId.replace(":","");

  //clientId = "N18FE34DE278F";

  Serial.println("Device Id is : " + clientId);

  publishTopic = "devices/" + clientId + "/messages/events/";
  subscriptionTopic = "relayActionRequest/" + clientId;
  relayGroupStatusTopic = "currentStatusCheck/" + clientId;
  setup_wifi();

  
  client.setServer(mqtt_server, 8883);
  client.setCallback(callback);
}

void loop() {

  int greetValue =1;
  
  if (!client.connected()) {
    reconnect();
  }
  Serial.println("Before client.loop()");
  
  client.loop();

  Serial.println("Publishing on topic : " + String(publishTopic.c_str()));

  String publishContent = "Hello World From NodeMCU"+ String(greetValue);

    client.publish(publishTopic.c_str(), getPublishString().c_str());

  Serial.println("Published on topic : " + String(publishTopic.c_str()));

  Serial.println("After client.loop()");

  greetValue++;

  delay(5000);
}


void wifiSetup()
{
  //WiFi.begin(ssid, password);
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
}

String getPublishString()
{
  float humidity = dht.readHumidity();
  // Read temperature as Celsius
  float temperature = dht.readTemperature();
  int sensorRetryCount = 0;

  String jsonString = "";
  while (isnan(humidity) || isnan(temperature))
  {
    // Read temperature as Celsius
  humidity = dht.readHumidity();
  temperature = dht.readTemperature();
    sensorRetryCount++;
    Serial.println("Sensor Read try " + String(sensorRetryCount));

  jsonString = jsonPublishString(temperature,humidity);

    delay(5000);
  }

  Serial.print("Humidity: ");
  Serial.print(humidity);
  Serial.print(" %\t");
  Serial.print("Temperature: ");
  Serial.print(temperature);
  Serial.println(" *C ");

  jsonString = jsonPublishString(temperature, humidity);

  
  if (temperature >= 27.00)
  {
    Serial.println("Red LED will Blink");
    digitalWrite(redLed, HIGH);
  }
  if (temperature < 27.00)
  {
    Serial.println("Green LED will Blink");
    digitalWrite(greenLed, HIGH);
  }
  digitalWrite(redLed, LOW);
  digitalWrite(greenLed, LOW);

  Serial.println(jsonString);

  return jsonString;
}

String jsonPublishString(float temperature,float humidity)
{
  Serial.println("Inside JSON Publish String ");
  
  StaticJsonBuffer<200> jsonBuffer;

  time_t currentTime = now();

  JsonObject& root = jsonBuffer.createObject();
  root["sensor"] = "t_h_sensor";
  //root["time"] = system_mktime(year(), month(), day(), hour(), minute(), second());
  root["time"] = __TIMESTAMP__;
  root["temperature"] = temperature;
  root["humidity"] = humidity;
  root["deviceId"] = clientId;

  String resultString;

  root.printTo(resultString);
  return resultString;
}


