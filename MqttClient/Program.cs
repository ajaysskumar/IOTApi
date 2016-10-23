using System;
using System.Collections.Generic;
using System.Text;

using MqttLib;

namespace SamplePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {

                Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0]
                    + " cloudmqtt_connectionString ClientId cloudmqtt_username cloudmqtt_password");
                Console.WriteLine("Enter the four parameters separated by white space\n");
                string conString = "";

                args = conString.Split(' ');

                //return;
            }

            Console.WriteLine("Starting MqttDotNet sample program.");
            Console.WriteLine("Press any key to stop\n");

            //Program prog = new Program(args[0], args[1], args[2], args[3]);
            Program prog = new Program("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");

            
            prog.Start();


            //Console.ReadKey();
            //prog.Stop();
        }

        IMqtt _client;

        Program(string connectionString, string clientId, string username, string password)
        {
            // Instantiate client using MqttClientFactor

            _client = MqttClientFactory.CreateClient(connectionString, clientId, username, password);

            // Setup some useful client delegate callbacks
            _client.Connected += new ConnectionDelegate(client_Connected);
            _client.ConnectionLost += new ConnectionDelegate(_client_ConnectionLost);
            _client.PublishArrived += new PublishArrivedDelegate(client_PublishArrived);
        }

        void Start()
        {
            // Connect to broker in 'CleanStart' mode
            Console.WriteLine("Client connecting\n");
            _client.Connect(true);
        }

        void Stop()
        {
            if (_client.IsConnected)
            {
                Console.WriteLine("Client disconnecting\n");
                _client.Disconnect();
                Console.WriteLine("Client disconnected\n");
            }
        }

        void client_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Client connected\n");
            RegisterOurSubscriptions();
            PublishSomething();
        }

        void _client_ConnectionLost(object sender, EventArgs e)
        {
            Console.WriteLine("Client connection lost\n");
        }

        void RegisterOurSubscriptions()
        {
            Console.WriteLine("Subscribing to mqttdotnet/subtest/#\n");
            _client.Subscribe("mqttdotnet/pubtest", QoS.BestEfforts);
        }

        void PublishSomething()
        {
            Console.WriteLine("Publishing on mqttdotnet/pubtest\n");
            _client.Publish("mqttdotnet/pubtest", "Hello MQTT World : " + Guid.NewGuid().ToString(), QoS.BestEfforts, false);
        }

        bool client_PublishArrived(object sender, PublishArrivedArgs e)
        {
            Console.WriteLine("Received Message");
            Console.WriteLine("Topic: " + e.Topic);
            Console.WriteLine("Payload: " + e.Payload);
            Console.WriteLine();
            return true;
        }

        //bool MessageArrived()
        //{
        //    if (_client.)
        //    {

        //    }
        //}
    }
}