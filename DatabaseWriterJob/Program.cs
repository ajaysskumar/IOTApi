using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using IoT.Common.Logging;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using IoT.Common.Model.Models;
using Newtonsoft.Json.Linq;

namespace DatabaseWriterJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            LoggingManager.InitializeLogger("IoTEventSourceManager", System.Diagnostics.Tracing.EventLevel.LogAlways);

            //JobHostConfiguration config = new JobHostConfiguration();
            //config.UseServiceBus();
            //JobHost host = new JobHost(config);
            //host.RunAndBlock();

            Console.WriteLine("Receive critical messages. Ctrl-C to exit.\n");
            var connectionString = "Endpoint=sb://sb-homeautomation-dev.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=kuxMpAEfg7EBdRK6l5Pv4/afc1lhIhY4j82NGOmBERA=";
            var queueName = "sensor-input-queue";

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            try
            {
                client.OnMessage(message =>
                   {
                       Stream stream = message.GetBody<Stream>();
                       StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                       string s = reader.ReadToEnd();
                       IoTEventSourceManager.Log.Debug(s, "iot-web-job-queue");

                       using (ApplicationDbContext context = new ApplicationDbContext())
                       {
                           var obj = JObject.Parse(s);
                           //var url = (string)obj["data"]["img_url"];

                           MotionSensor datapoint = new MotionSensor()
                           {
                               DeviceId = (string)obj["deviceId"],
                               MotionTime = (string)obj["humidity"],
                               MotionValue = (string)obj["temperature"],
                               Timestamp = DateTime.UtcNow
                           };
                           context.MotionsSensor.Add(datapoint);
                           context.SaveChanges();
                       }
                   });

            }
            catch (Exception ex)
            {
                IoTEventSourceManager.Log.Error(ex.Message, "iot-web-job-queue");
            }

            Console.ReadLine();
        }
    }
}
