using MqttLib;
using System;
using IoTDemoApp;

public class MqttClient
{
    static void Main(string[] args)
    {
        if (args.Length != 4)
        {

            //Console.WriteLine("Usage: " + Environment.GetCommandLineArgs()[0]
                //+ " cloudmqtt_connectionString ClientId cloudmqtt_username cloudmqtt_password");
            //Console.WriteLine("Enter the four parameters separated by white space\n");
            string conString = "";

            args = conString.Split(' ');

            //return;
        }

        //Console.WriteLine("Starting MqttDotNet sample program.");
        //Console.WriteLine("Press any key to stop\n");

        //Program prog = new Program(args[0], args[1], args[2], args[3]);
        MqttClient prog = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");

        string doContinue = "Y";
        while (doContinue == "Y" || doContinue == "y")
        {

        }
        prog.Start();
        //Console.WriteLine("Press Y to publish again\n");
        //doContinue = Console.ReadLine();
        prog.Stop();
    }

    public string SubscriptionMessage { get; set; }

    public bool ClientConnected { get; set; }
    //public bool ConnectionLost { get; set; }

    IMqtt _client;

    public MqttClient(string connectionString, string clientId, string username, string password)
    {
        // Instantiate client using MqttClientFactor

        _client = MqttClientFactory.CreateClient(connectionString, clientId, username, password);

        // Setup some useful client delegate callbacks
        _client.Connected += new ConnectionDelegate(client_Connected);
        _client.ConnectionLost += new ConnectionDelegate(_client_ConnectionLost);
        _client.PublishArrived += new PublishArrivedDelegate(client_PublishArrived);
    }

    public void Start()
    {
        _client.Connect(true);
    }

    public void Stop()
    {
        if (_client.IsConnected)
        {
            _client.Disconnect();
        }
    }

    public void client_Connected(object sender, EventArgs e)
    {
        RegisterOurSubscriptions("relayActionConfirmation/18:FE:34:D4:7F:85");
        ClientConnected = true;
    }

    public void _client_ConnectionLost(object sender, EventArgs e)
    {
        ClientConnected = false;
        throw new Exception(Helper.ClientDisconnected);
    }

    public void RegisterOurSubscriptions(string subscriptionTopic)
    {
        _client.Subscribe(subscriptionTopic, QoS.BestEfforts);
    }

    public void PublishSomething(string relayIdString,string currentStatus, string msgId,string publishTopic= "relayActionRequest/18:FE:34:D4:7F:85")
    {
        _client.Publish(publishTopic,string.Format("{0}={1}={2}",relayIdString,currentStatus,msgId), QoS.BestEfforts, false);
    }

    public bool client_PublishArrived(object sender, PublishArrivedArgs e)
    {
        SubscriptionMessage = e.Payload.ToString();
        return true;
    }
}