/*********
  Rui Santos
  Complete project details at http://randomnerdtutorials.com  
*********/

#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>

MDNSResponder mdns;

// Replace with your network credentials
const char* ssid = "APEX";
const char* password = "apex-wifi";

ESP8266WebServer server(80);

String webPage = "";

int gpio0_pin = 12;
int gpio2_pin = 14;

const int numberOfPins = 2;

int pins[numberOfPins] = {gpio0_pin,gpio2_pin};

void setup(void){
  webPage += "<h1>ESP8266 Web Server</h1><p>Socket #1 <a href=\"socket1On\"><button>ON</button></a>&nbsp;<a href=\"socket1Off\"><button>OFF</button></a></p>";
  webPage += "<p>Socket #2 <a href=\"socket2On\"><button>ON</button></a>&nbsp;<a href=\"socket2Off\"><button>OFF</button></a></p>";

  webPage = "";
  
  // preparing GPIOs
  pinMode(gpio0_pin, OUTPUT);
  digitalWrite(gpio0_pin, LOW);
  pinMode(gpio2_pin, OUTPUT);
  digitalWrite(gpio2_pin, LOW);
  
  delay(1000);
  Serial.begin(115200);
  WiFi.begin(ssid, password);
  Serial.println("");

  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
  
  if (mdns.begin("esp8266", WiFi.localIP())) {
    Serial.println("MDNS responder started");
  }
  
  server.on("/", [](){
    server.send(200, "application/xml", Status(pins));
  });
  server.on("/socket1On", [](){
    server.send(200, "application/json", "Socket1 on");
    digitalWrite(gpio0_pin, HIGH);
    Serial.println("Something happened");
    delay(1000);
  });
  server.on("/socket1Off", [](){
    server.send(200, "application/json", "Socket1 off");
    digitalWrite(gpio0_pin, LOW);
    Serial.println("Something happened");
    delay(1000); 
  });
  server.on("/socket2On", [](){
    server.send(200, "application/json", "Socket2 on");
    digitalWrite(gpio2_pin, HIGH);
    Serial.println("Something happened");
    delay(1000);
  });
  server.on("/socket2Off", [](){
    server.send(200, "application/json", "Socket2 off");
    digitalWrite(gpio2_pin, LOW);
    Serial.println("Something happened");
    delay(1000); 
  });
  server.begin();
  Serial.println("HTTP server started");
}
 
void loop(void){
  server.handleClient();
} 

class PinStatus
{
  public:
    String Relay1Status;
    String Relay2Status;
};

String Status(int relays[])
{
  //int arrSize = sizeof(relays)/sizeof(int);
  String xmlString = "";
  for(int i=0;i<numberOfPins;i++)
  {
    //Serial.println(arrSize);
    
    if(digitalRead(relays[i])==LOW)
    xmlString =String(xmlString) +String("<Relay><RelayName>Relay"+String(i+1)+"</RelayName><RelayStatus>CKT_CLOSE</RelayStatus></Relay>");
    else
    xmlString =String(xmlString) +String("<Relay><RelayName>Relay"+String(i+1)+"</RelayName><RelayStatus>CKT_OPEN</RelayStatus></Relay>");
  }
    return "<Relays>"+xmlString+"</Relays>";
}
