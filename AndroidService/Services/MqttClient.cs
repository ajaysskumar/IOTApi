using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using MqttLib;
using System;
//using IoTDemoApp;

public class MqttClient
{
    public string SubscriptionMessage { get; set; }

    public bool ClientConnected { get; set; }

    IMqtt _client;

    public MqttClient(string connectionString, string clientId, string username, string password)
    {
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
        //RegisterOurSubscriptions("relayActionConfirmation/18:FE:34:D4:7F:85");
        ClientConnected = true;
    }

    public void _client_ConnectionLost(object sender, EventArgs e)
    {
        ClientConnected = false;
        throw new Exception("Client Disconnected");
    }

    public void RegisterOurSubscriptions(string subscriptionTopic)
    {
        _client.Subscribe(subscriptionTopic, QoS.OnceAndOnceOnly);
    }

    public void PublishSomething(string topic, string value)
    {
        _client.Publish(topic, value, QoS.OnceAndOnceOnly, false);
    }

    public bool client_PublishArrived(object sender, PublishArrivedArgs e)
    {
        SubscriptionMessage = e.Payload.ToString();
        return true;
    }
}