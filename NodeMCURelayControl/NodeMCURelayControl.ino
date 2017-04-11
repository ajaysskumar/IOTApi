#include <TimeLib.h>
#include <Time.h>
//#include <AzureIoTHubClient.h>
//#include <AzureIoTHub.h>
#include <WiFiManager.h>
#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <WiFiClient.h>
#include <WiFiServer.h>
#include <WiFiUdp.h>
#include <PubSubClient.h>
#include <EEPROM.h>

// Update these with values suitable for your network.

const char* ssid = "APEX";
const char* password = "Apex-Wifi2017";
const char* mqtt_server = "m13.cloudmqtt.com";
String publishTopic;
String subscriptionTopic;
String relayGroupStatusTopic;
String statusXML;
int wifiConfigModePin = 15;

WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
//char msg[50];
int value = 0;
int switch1 = 16;
int switch2 = 4;
int switch3 = 5;
int switch4 = 12;
int switch5 = 13;
int switch6 = 14;

int switchGroup[] = { switch1, switch2, switch3, switch4, switch5, switch6 };

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

	if (currentRelay != 0)
	{
		// Switch on the LED if an 1 was received as first character
		if (currentRelayState == 0) {
			digitalWrite(currentRelay, LOW);   // Turn the LED on (Note that LOW is the voltage level
			Serial.print(String(currentRelay) + " : LOW");
			EEPROM.write(currentRelay, currentRelayState);
			EEPROM.commit();

			int value = (int)EEPROM.read(currentRelay);

			Serial.println("Written in disk value : " + String(value));
			// but actually the LED is on; this is because
			// it is acive low on the ESP-01)
		}
		else {
			digitalWrite(currentRelay, HIGH);  // Turn the LED off by making the voltage HIGH
			Serial.print(String(currentRelay) + " : HIGH");
			EEPROM.write(currentRelay, currentRelayState);
			EEPROM.commit();

			int value = (int)EEPROM.read(currentRelay);

			Serial.println("Written in disk value : " + String(value));
		}
	}

	statusXML = "<s><d>" + clientId + "</d><m>" + String(msg) + "</m><rl><r><rn>1</rn><rs>" + String(digitalRead(16)) + "</rs></r><r><rn>2</rn><rs>" + String(digitalRead(4)) + "</rs></r><r><rn>3</rn><rs>" + String(digitalRead(5)) + "</rs></r><r><rn>4</rn><rs>" + String(digitalRead(12)) + "</rs></r><r><rn>5</rn><rs>" + String(digitalRead(13)) + "</rs></r><r><rn>6</rn><rs>" + String(digitalRead(14)) + "</rs></r></rl></s>";
	Serial.print("Sending Ack:" + statusXML);
	sendAck(statusXML);
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
	//pinMode(switch1, OUTPUT);     // Initialize the switch1 pin as an output

	for (int i = 0; i < 6; i++)
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

//	if (digitalRead(15)== LOW)
//	{
//    Serial.print("Entering Setup : " + String(digitalRead(15)));
//		wifiSetup();
//    Serial.print("Exit Setup");
//	}

	if (!client.connected()) {
		reconnect();
	}
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
	for (int i = 0; i < 6; i++)
	{
		int value = (int)EEPROM.read(switchGroup[i]);
		if (value == 0) {
			digitalWrite(switchGroup[i], LOW);   // Turn the LED on (Note that LOW is the voltage level
			Serial.print(String(switchGroup[i]) + " : LOW");
			// but actually the LED is on; this is because
			// it is acive low on the ESP-01)
		}
		else {
			digitalWrite(switchGroup[i], HIGH);  // Turn the LED off by making the voltage HIGH
			Serial.print(String(switchGroup[i]) + " : HIGH");
		}
	}
}

void wifiSetup()
{
	//WiFi.begin(ssid, password);
	//digitalWrite(wifiConfigModePin, HIGH); // indication of esp going into config mode
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
	//digitalWrite(wifiLedDisconnectPin, LOW);

}
