using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using IoT.Common.Logging;

namespace IoT.Core.MessageProcessing.Services
{
    partial class QueueReader : ServiceBase
    {
        public QueueReader()
        {
            InitializeComponent();
            LoggingManager.InitializeLogger("IoTEventSourceManager",System.Diagnostics.Tracing.EventLevel.LogAlways);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                IoTEventSourceManager.Log.Debug("Receive critical messages. Ctrl-C to exit.", "azure-queue-reader");

                var connectionString = System.Configuration.ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
                var queueName = "q-iot-dev";

                var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

                client.OnMessage(message =>
                {
                    Stream stream = message.GetBody<Stream>();
                    StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                    string s = reader.ReadToEnd();
                //Console.WriteLine(String.Format("Message body: {0}", s));
                IoTEventSourceManager.Log.Debug(s, "azure-queue-message");
                });

                //Console.ReadLine();
            }
            catch ( Exception ex)
            {
                IoTEventSourceManager.Log.Debug(ex.Message, "azure-queue-reader");
            }
        }

        protected override void OnStop()
        {
            this.Stop();
        }
    }
}
