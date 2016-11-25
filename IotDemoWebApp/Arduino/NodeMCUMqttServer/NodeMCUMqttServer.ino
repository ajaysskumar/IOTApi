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

#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include <EEPROM.h>

// Update these with values suitable for your network.

const char* ssid = "APEX";
const char* password = "apex-wifi";
const char* mqtt_server = "m13.cloudmqtt.com";
String publishTopic;
String subscriptionTopic;
String relayGroupStatusTopic;

WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
//char msg[50];
int value = 0;
int switch1 = LED_BUILTIN;
int switch2 = 4;
int switch3 = 5;
int switch4 = 12;
int switch5 = 13;
int switch6 = 14;

int switchGroup[] = {switch1, switch2, switch3, switch4, switch5, switch6};

String clientId;

void setup_wifi() {

  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

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
  char payloadArray[60];
  int relayState;

  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
    payloadArray[i] = (char)payload[i];
  }

  int currentRelay;
  int currentRelayState;
  String msg;

  String payloadString(payloadArray);

  for (int i = 0; i < payloadString.length(); i++) {
    if (payloadString.substring(i, i + 1) == "=") {
      currentRelay = payloadString.substring(0, i).toInt();
      currentRelayState = payloadString.substring(i + 1, i + 2).toInt();
      msg = payloadString.substring(i + 3, i + 39);
      Serial.println("Response values " + String(currentRelay) + "--" + String(currentRelayState) + "--" + msg);
      break;
    }
  }

  char msgId[36];
  //
  //  for(int i=0;i<36;i++)
  //  {
  //    msgId[i] = msg[i];
  //  }

  Serial.println();

  // Switch on the LED if an 1 was received as first character
  if (currentRelayState == 0) {
    digitalWrite(currentRelay, LOW);   // Turn the LED on (Note that LOW is the voltage level
    Serial.print(String(currentRelay)+" : LOW");
    EEPROM.write(currentRelay, currentRelayState);
    EEPROM.commit();

    int value = (int)EEPROM.read(currentRelay);

    Serial.println("Written in disk value : " + String(value));
    // but actually the LED is on; this is because
    // it is acive low on the ESP-01)
  } else {
    digitalWrite(currentRelay, HIGH);  // Turn the LED off by making the voltage HIGH
    Serial.print(String(currentRelay)+" : HIGH");
    EEPROM.write(currentRelay, currentRelayState);
    EEPROM.commit();

    int value = (int)EEPROM.read(currentRelay);

    Serial.println("Written in disk value : " + String(value));
  }
  Serial.print("Sending Ack:" + msg);
  sendAck(msg);
}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID

    // Attempt to connect
    if (client.connect(clientId.c_str(), "cbaeasea", "KiYFQP0Q1gbe")) {
      Serial.println("connected");
      // Once connected, publish an announcement...

      client.publish("heartBeatCheck", "Connected");
      // ... and resubscribe
      client.subscribe(subscriptionTopic.c_str());
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void setup() {
  //pinMode(switch1, OUTPUT);     // Initialize the switch1 pin as an output
  
  for (int i = 0; i < 6;i++)
  {
   pinMode(switchGroup[i], OUTPUT);
  }
  
  Serial.begin(115200);
  EEPROM.begin(512);
  restoreSwitchState();
  clientId = WiFi.macAddress();

  publishTopic = "relayActionConfirmation/" + clientId;
  subscriptionTopic = "relayActionRequest/" + clientId;
  relayGroupStatusTopic = "currentStatusCheck/" + clientId;
  setup_wifi();
  client.setServer(mqtt_server, 19334);
  client.setCallback(callback);
}

void loop() {

  if (!client.connected()) {
    reconnect();
  }
  //  Serial.println("Publishing status...");
  //  String statusXML = "<Relays><Relay><RelayNumber>1</RelayNumber>"+String(digitalRead(switch1))+"<RelayStatus></RelayStatus></Relay></Relays>";
  //  client.publish("currentStatusCheck", statusXML.c_str());
  //  Serial.println("Published : "+relayGroupStatusTopic+String(statusXML));
  //  //delay(200);
  client.loop();
}

void sendAck(String msg)
{
  Serial.print("Publish message: ");
  Serial.println(msg.c_str());
  client.publish(publishTopic.c_str(), msg.c_str());
}

void restoreSwitchState()
{
  for (int i = 0; i < 6;i++)
  {
    int value = (int)EEPROM.read(switchGroup[i]);
    if (value == 0) {
      digitalWrite(switchGroup[i], LOW);   // Turn the LED on (Note that LOW is the voltage level
      Serial.print(String(switchGroup[i])+" : LOW");
      // but actually the LED is on; this is because
      // it is acive low on the ESP-01)
    } else {
      digitalWrite(switchGroup[i], HIGH);  // Turn the LED off by making the voltage HIGH
      Serial.print(String(switchGroup[i])+" : HIGH");
    }
  }
}

