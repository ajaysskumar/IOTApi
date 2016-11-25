using AndroidService.Services;
using IoT.Common.Logging;
using IoT.Common.Model.Models;
using IoT.Core.Email;
using IoTOperations.Sensors;
using IoTOperations.ServiceHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //HttpClient httpClient = new HttpClient();
            //EmailClient client = new EmailClient();

            //client.SendEmail();
            //LoggingManager.InitializeLogger("IoTEventSourceManager", System.Diagnostics.Tracing.EventLevel.LogAlways);
            //IoTEventSourceManager.Log.Debug("askkdnksadnknkas", "asjdbjksbakjdbkjasbkjd");

            ////var response = httpClient.GetAsync("http://iotdemo.apexsoftworks.in/api/getdatapointspartial?top=2&lastRecord=84166&sensorId=18FE34DE278F").Result;
            //var contents = response.Content.ReadAsStringAsync().Result;

            //if (response.IsSuccessStatusCode)
            //{

            //    List<MotionSensor> resultingMessage = JsonConvert.DeserializeObject<List<MotionSensor>>(contents);

            //}

            //else throw new Exception("Request Failed");

            MqttClient _mqttClient = null;

            Location loc = new Location()
            {
                Longitude = 77.085998,
                Latitude = 28.491740,
                Altitude = 120
            };

            if (_mqttClient == null)
            {
                _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");
                _mqttClient.Start();
            }


            for (int i = 0; i < 100000; i++)
            {
                loc = new Location()
                {
                    Longitude = 77.085998,
                    Latitude = 28.491740,
                    Altitude = 120
                };

                Random random = new Random();

                double num = random.NextDouble();

                loc.Latitude += num;

                loc.Longitude += num;

                string publishString = JsonConvert.SerializeObject(loc);
                _mqttClient.PublishSomething("locationData", publishString);
                Console.WriteLine(publishString);
                Thread.Sleep(2000);
            }
            
        }
    }
}
