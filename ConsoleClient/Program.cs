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
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            EmailClient client = new EmailClient();

            //client.SendEmail();

            var response = httpClient.GetAsync("http://iotdemo.apexsoftworks.in/api/getdatapointspartial?top=2&lastRecord=84166&sensorId=18FE34DE278F").Result;
            var contents = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
              
                List<MotionSensor> resultingMessage = JsonConvert.DeserializeObject<List<MotionSensor>>(contents);
                
            }

            else throw new Exception("Request Failed");
        }
    }
}
