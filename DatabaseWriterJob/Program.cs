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
using System.Configuration;
using System.Threading;
using System.Net.Http;

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
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var queueName = ConfigurationManager.AppSettings["QueueName"];

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            int syncFrequency = Convert.ToInt32(ConfigurationManager.AppSettings["SyncFrequency"]);
            int syncBatchSize = Convert.ToInt32(ConfigurationManager.AppSettings["SyncBatchSize"]);

            while (true)
            {
                try
                {
                    IEnumerable<BrokeredMessage> queueMessages = client.ReceiveBatch(syncBatchSize);

                    List<MotionSensor> datapoints = new List<MotionSensor>();

                    if (queueMessages != null)
                    {
                        foreach (var message in queueMessages)
                        {
                            Stream stream = message.GetBody<Stream>();
                            StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                            string s = reader.ReadToEnd();
                            IoTEventSourceManager.Log.Debug(s, "iot-web-job-queue");

                            var obj = JObject.Parse(s);
                            //var url = (string)obj["data"]["img_url"];

                            MotionSensor datapoint = new MotionSensor()
                            {
                                DeviceId = (string)obj["deviceId"],
                                MotionTime = (decimal)obj["humidity"],
                                MotionValue = (decimal)obj["temperature"],
                                Timestamp = DateTime.UtcNow
                            };

                            datapoints.Add(datapoint);
                            message.Complete();
                        }
                        HttpClient httpClient = new HttpClient();

                        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(datapoints);
                        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = httpClient.PostAsync(ConfigurationManager.AppSettings["RecordsSyncPostUrl"], content).GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine(string.Format("{0} Records Synched. Thread Sleep for {1} Seconds", datapoints.Count, syncFrequency));
                        }
                        else
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync());
                        }
                    }
                }
                catch (Exception ex)
                {
                    IoTEventSourceManager.Log.Error(ex.Message, "iot-web-job-queue");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
