using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using IoT.Common.Logging;


namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "iothub-homeautomation-dev.azure-devices.net";
        static string deviceKey = "yhYrHVtdHko4PNITZmurwpgixxK+qsCupLsn5Rswo1g=";

        static void Main(string[] args)
        {
            LoggingManager.InitializeLogger("IoTEventSourceManager",System.Diagnostics.Tracing.EventLevel.LogAlways);
            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("18FE34DE278F", deviceKey), TransportType.Mqtt);

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();

        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = "18FE34DE278F",
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                string levelValue;

                if (rand.NextDouble() > 0.7)
                {
                    messageString = "This is a critical message";
                    levelValue = "critical";
                }
                else
                {
                    levelValue = "normal";
                }

                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("level", levelValue);

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sent message: {1}", DateTime.Now, messageString);

                IoTEventSourceManager.Log.Debug(messageString,"simulated-device-log");

                await Task.Delay(1000);
            }

        }

    }
}
