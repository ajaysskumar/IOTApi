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
String mqtt_server = "";
char* baseServer = "ipowersaver-dev.azurewebsites.net";
String iotHubUsername;
String iotHubPassword;
int datapointFrequency = 20;
String publishTopic;
String subscriptionTopic;
String relayGroupStatusTopic;
String statusXML;
int maxSensorReadRetryCount = 10;
int loadStatus = 0;

int LED1 = 12;  //RED               // the pin that the LED is atteched to
int LED2 = 14;  //GREEN
int LED3 = 4; //ORANGE

int configButtonPin = 5;

#define DHTPIN 13     // what pin we're connected to
#define DHTTYPE DHT22   // DHT 22  (AM2302)

DHT dht(DHTPIN, DHTTYPE);

WiFiClientSecure espClient;
PubSubClient client(espClient);
WiFiManager wifiManager;
long lastMsg = 0;
//char msg[50];
int value = 0;

String clientId;

void setup() {

  pinMode(LED1, OUTPUT);      // initalize LED as an output
  pinMode(LED2, OUTPUT);      // initalize LED as an output
  pinMode(LED3, OUTPUT);      // initalize LED as an output

  Serial.begin(115200);
  EEPROM.begin(512);
  dht.begin();
  Serial.println("DHT initialized");

  //check wifi connectivity

  String ssid = wifiManager.readSavedAP();
  String password = wifiManager.readSavedPassword();

  Serial.println("After EEPROM : " + ssid);
  //Serial.println("After EEPROM : " + password);

   clientId = WiFi.macAddress();

  clientId.replace(":", "");

  Serial.println("Device Id is : " + clientId);

  wifiManager.connectWifi(ssid, password);

  if (WiFi.status() != WL_CONNECTED)
  {
    wifiSetup();
  }
  Serial.println(WiFi.localIP());

  publishTopic = "devices/" + clientId + "/messages/events/";
  subscriptionTopic = "relayActionRequest/" + clientId;
  relayGroupStatusTopic = "currentStatusCheck/" + clientId;
  if (digitalRead(configButtonPin) == LOW)
  {

    Serial.println("Config Button Pressed LOW. Going to Configuration Mode...");
    WiFi.disconnect();
  wifiSetup();
  }
  else
  {
    Serial.println("Config Button Pressed HIGH. Continue with existing connection settings... ");
  }

  loadConfiguration();

  Serial.print("Configuration Load Complete");

  digitalWrite(LED1,HIGH);
  delay(2000);
  digitalWrite(LED1,LOW);

  digitalWrite(LED2,HIGH);
  delay(2000);
  digitalWrite(LED2,LOW);

  digitalWrite(LED3,HIGH);
  delay(2000);
  digitalWrite(LED3,LOW);

  Serial.print("printing mqtt_server on setup");
  Serial.print(mqtt_server);

  client.setServer(mqtt_server.c_str(), 8883);
  client.setCallback(callback);
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }

  client.loop();

  Serial.println("Publishing on topic : " + String(publishTopic.c_str()));

  String publishString = getPublishString();
  if (publishString == "")
  {
    Serial.println("Invalid inputs. Device will retry after sometime.");
    delay(datapointFrequency);
    return;
  }
  client.publish(publishTopic.c_str(), publishString.c_str());

  Serial.println("Published on topic : " + String(publishTopic.c_str()));

#pragma region Success post indication
  digitalWrite(LED2, HIGH);
  delay(2000);
  digitalWrite(LED2, LOW);
#pragma endregion


  Serial.println("Next Reading will be sent after : " + String(datapointFrequency));

  delay(datapointFrequency);
}

void loadConfiguration() {
  Serial.println("Loading Configuration");
  RestClient restClient = RestClient(baseServer);

  String settingsGetUsername = "/api/getdeviceUsername?deviceId=" + clientId;
  String settingsGetSASKey = "/api/getsaskey?deviceId=" + clientId + "&ttlValue=365";
  String settingsGetInterval = "/api/getdatapointfrequency?deviceId=" + clientId;
  String settingsGetHostName = "/api/gethubservername?deviceId=" + clientId;
  String settingsResponse;

  int statusCode = restClient.get(settingsGetHostName.c_str(), &settingsResponse);

  if (statusCode == 200)
  {
    Serial.println("success");
    String response = settingsResponse;
    Serial.println(response);
    mqtt_server = rectify( response);
    Serial.println("IotHubUsername\n");
    Serial.println(iotHubUsername);
  loadStatus = 1;
  }
  else
  {
    Serial.println("failed returning");
    return;
  }
  //For SAS Key
  String sasKeyResponse;
  statusCode = restClient.get(settingsGetSASKey.c_str(), &sasKeyResponse);

  if (statusCode == 200)
  {
    Serial.println("success");
    String response = sasKeyResponse;
    Serial.println(response);
    iotHubPassword = rectify(response);
    Serial.println("IotHubPassword\n");
    Serial.println(iotHubPassword);
  loadStatus = 2;
  }
  else
  {
    Serial.println("failed returning");
    return;
  }

  //For datapoint frequency
  String frequencyResponse;
  statusCode = restClient.get(settingsGetInterval.c_str(), &frequencyResponse);

  if (statusCode == 200)
  {
    Serial.println("success");
    String response = frequencyResponse;
    Serial.println(response);
    datapointFrequency = response.toInt();
    Serial.println("Frequency\n");
    Serial.println(iotHubPassword);
  loadStatus = 3;
    return;
  }
  else
  {
    Serial.println("failed returning");
    return;
  }
}

//void setup_wifi() {
//
//  delay(10);
//  // We start by connecting to a WiFi network
//  Serial.println();
//  Serial.print("Connecting to ");
//  Serial.println(ssid);
//
//  wifiSetup();
//
//  while (WiFi.status() != WL_CONNECTED) {
//    delay(500);
//    Serial.print(".");
//  }
//
//  randomSeed(micros());
//
//  Serial.println("");
//  Serial.println("WiFi connected");
//  Serial.println("IP address: ");
//  Serial.println(WiFi.localIP());
//  Serial.println(clientId);
//}

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
  Serial.print("printing mqtt_server \n");
  Serial.print(mqtt_server);

  strcpy(iotUsername, mqtt_server.c_str()); // copy to result array
  strcat(iotUsername, "/"); // concat to result array
  strcat(iotUsername, clientId.c_str()); // concat to result array

  Serial.print(iotUsername);
  Serial.println();
  Serial.print(iotHubPassword.c_str());
  Serial.println();

  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...\n");

    // Attempt to connect
    if (client.connect(clientId.c_str(), iotUsername, iotHubPassword.c_str())) {
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
  Serial.println("going to configuration Mode \n");
  digitalWrite(LED1, HIGH);

  wifiManager.startConfigPortal(clientId.c_str(), "nodemcuv2");

  //if you get here you have connected to the WiFi
  Serial.println("connected...yeey :)");
  digitalWrite(LED1, HIGH);
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
    Serial.print("Humidity: ");
    Serial.print(humidity);
    Serial.print(" %\t");
    Serial.print("Temperature: ");
    Serial.print(temperature);
    Serial.println(" *C ");

    jsonString = jsonPublishString(temperature, humidity);

    Serial.println(jsonString);
  }
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

String rectify(String mString) {
  mString.replace("\"", "");
  return mString;
}



