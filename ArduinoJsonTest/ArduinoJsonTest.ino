

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
#include <RestClient.h>
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
char* mqtt_server = "oa-iothub-dev.azure-devices.net";
char* baseServer = "oa-th-dev.azurewebsites.net";
String iotHubUsername;
String iotHubPassword;
int datapointFrequency = 20;
String publishTopic;
String subscriptionTopic;
String relayGroupStatusTopic;
String statusXML;
int maxSensorReadRetryCount = 10;

int redLed = 12;                // the pin that the LED is atteched to
int greenLed = 14;

int configButtonPin = 5;

#define DHTPIN 13     // what pin we're connected to
#define DHTTYPE DHT22   // DHT 22  (AM2302)

DHT dht(DHTPIN, DHTTYPE);

WiFiClientSecure espClient;
PubSubClient client(espClient);
long lastMsg = 0;
//char msg[50];
int value = 0;

String clientId;

void setup() {

  pinMode(greenLed, OUTPUT);      // initalize LED as an output
  pinMode(redLed, OUTPUT);      // initalize LED as an output

  Serial.begin(115200);
  EEPROM.begin(512);
  dht.begin();
  Serial.println("DHT initialized");
  
  clientId = WiFi.macAddress();

  clientId.replace(":", "");

  Serial.println("Device Id is : " + clientId);

  publishTopic = "devices/" + clientId + "/messages/events/";
  subscriptionTopic = "relayActionRequest/" + clientId;
  relayGroupStatusTopic = "currentStatusCheck/" + clientId;
  if (digitalRead(configButtonPin) == LOW)
  {
    Serial.println("Config Button Pressed LOW. Continue with existing connection settings... ");
  }
  else
  {
    Serial.println("Config Button Pressed HIGH. Going to Configuration Mode...");
    WiFi.disconnect();
    setup_wifi();
  }

  loadConfiguration();

  Serial.print("printing mqtt_server on setup");
  Serial.print(mqtt_server);

  client.setServer(mqtt_server, 8883);
  client.setCallback(callback);
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }

  client.loop();

  Serial.println("Publishing on topic : " + String(publishTopic.c_str()));

  String publishString = getPublishString();
  if (publishString=="")
  {
    Serial.println("Invalid inputs. Device will retry after sometime.");
    delay(30000);
    return;
  }
  client.publish(publishTopic.c_str(), getPublishString().c_str());

  Serial.println("Published on topic : " + String(publishTopic.c_str()));

  delay(45000);
}

void loadConfiguration() {
  Serial.println("Loading Configuration");
  RestClient restClient = RestClient(baseServer);

  String settingsGetUsername = "/api/getdeviceUsername?deviceId=" + clientId;
  String settingsGetSASKey = "/api/getsaskey?deviceId=" + clientId + "&ttlValue=365";
  String settingsGetInterval = "/api/getdatapointfrequency?deviceId=" + clientId;
  String settingsResponse;
  
  int statusCode = restClient.get(settingsGetUsername.c_str(), &settingsResponse);

  if (statusCode == 200)
  {
    Serial.println("success");
    String response = settingsResponse;
    Serial.println(response);
  iotHubUsername = response;
  Serial.println("IotHubUsername\n");
  Serial.println(iotHubUsername);
  }
  else
  {
    Serial.println("failed returning");
    return;
  }
  //For SAS Key
  statusCode = restClient.get(settingsGetSASKey.c_str(), &settingsResponse);

  if (statusCode == 200)
  {
    Serial.println("success");
    String response = settingsResponse;
    Serial.println(response);
    iotHubPassword = response;
    Serial.println("IotHubPassword\n");
    Serial.println(iotHubPassword);
  }
  else
  {
    Serial.println("failed returning");
    return;
  }

  //For datapoint frequency 
  statusCode = restClient.get(settingsGetInterval.c_str(), &settingsResponse);

  if (statusCode == 200)
  {
    Serial.println("success");
    String response = settingsResponse;
    Serial.println(response);
    datapointFrequency = response.toInt();
    Serial.println("Frequency\n");
    Serial.println(iotHubPassword);
    return;
  }
  else
  {
    Serial.println("failed returning");
    return;
  }
}

void setup_wifi() {

  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  wifiSetup();

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

  char iotUsername[100];
  Serial.print("printing mqtt_server");
  Serial.print(mqtt_server);

  strcpy(iotUsername, mqtt_server); // copy to result array 
  strcat(iotUsername, "/"); // concat to result array
  strcat(iotUsername, clientId.c_str()); // concat to result array

  char iotPassword[200];

  char * str1 = "SharedAccessSignature sr=oa-iothub-dev.azure-devices.net%2Fdevices%2F";
  char * str2 = "&sig=G6ew9igp1CEMbo5v8Pu3SbJ1BCnuuEqu3MYpTgQxwy8%3D&se=1518096109";

  strcpy(iotPassword, str1); // copy to result array 
  strcat(iotPassword, clientId.c_str()); // concat to result array
  strcat(iotPassword, str2); // concat to result array

  Serial.print(iotUsername);
  Serial.print(iotPassword);

  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    Serial.println("Credentials User : " + String(clientId.c_str()));
    // Attempt to connect
    if (client.connect(clientId.c_str(), iotHubUsername.c_str(), iotHubPassword.c_str())) {
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

void wifiSetup()
{
  //WiFi.begin(ssid, password);
  Serial.println("Config button pressed... going to auto connect ");
  WiFiManager wifiManager;

  wifiManager.startConfigPortal(clientId.c_str(), "nodemcuv2");

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
  while ((isnan(humidity) || isnan(temperature)) && sensorRetryCount < maxSensorReadRetryCount)
  {
    // Read temperature as Celsius
    humidity = dht.readHumidity();
    temperature = dht.readTemperature();
    sensorRetryCount++;
    Serial.println("Sensor Read try " + String(sensorRetryCount));
    delay(5000);
  }

  if (isnan(humidity) || isnan(temperature))
  {
    return "";
  }
  else
  {
    jsonString = jsonPublishString(temperature, humidity);
  }
  

  Serial.print("Humidity: ");
  Serial.print(humidity);
  Serial.print(" %\t");
  Serial.print("Temperature: ");
  Serial.print(temperature);
  Serial.println(" *C ");

  jsonString = jsonPublishString(temperature, humidity);

  Serial.println(jsonString);

  return jsonString;
}

String jsonPublishString(float temperature, float humidity)
{
  Serial.println("Inside JSON Publish String ");

  StaticJsonBuffer<200> jsonBuffer;

  time_t currentTime = now();

  JsonObject& root = jsonBuffer.createObject();
  root["sensor"] = "t_h_sensor";
  root["time"] = __TIMESTAMP__;
  root["temperature"] = temperature;
  root["humidity"] = humidity;
  root["deviceId"] = clientId;

  String resultString;

  root.printTo(resultString);
  return resultString;
}




