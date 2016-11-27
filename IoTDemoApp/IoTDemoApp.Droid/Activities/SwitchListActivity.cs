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
using System.Net.Http;
using IoTDemoApp.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using IoT.Core.AppManager.Helpers;
using IoT.Core.AppManager.Models;
using Android.Database;
using IoTDemoApp.Droid.Adapters;

namespace IoTDemoApp.Droid.Activities
{
    [Activity(Label = "Switch List Activity")]
    public class SwitchListActivity : Activity
    {
        private List<RelayModel> _relays;
        private ListView _relayListView;
        int relayGroupId = 0;
        string relayGroupMac;
        HttpClient client;
        MqttClient _mqttClient;
        ProgressBar relayListProgressBar;
        RelayListAdapter adapter;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.RelayMenu);

            _relayListView = FindViewById<ListView>(Resource.Id.relayListView);

            relayListProgressBar = FindViewById<ProgressBar>(Resource.Id.relayListProgressBar);

            relayListProgressBar.Visibility = ViewStates.Visible;

            // Create your application here

            relayGroupId = Intent.GetIntExtra("relayGroupId", 0);

            relayGroupMac = Intent.GetStringExtra("relayGroupMac");

            client = new HttpClient();

            //----MQTT Initialize section---//
            _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");

            _mqttClient.Start();

            _mqttClient.RegisterOurSubscriptions("relayActionConfirmation/" + relayGroupMac);
            //----MQTT Initialize section---//

            RunOnUiThread(async () =>
            {
                try
                {
                    var response = await client.GetAsync(new Uri("http://iotdemo.apexsoftworks.in/api/RelayApi/" + relayGroupId));
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        _relays = JsonConvert.DeserializeObject<List<RelayModel>>(content);

                        Status status = _mqttClient.GetRelayGroupStatus(relayGroupMac);

                        foreach (var rel in _relays)
                        {
                            rel.RelayState = status.Relays.Relay.FirstOrDefault(x => x.RelayName == rel.RelayNumber.ToString()).RelayStatus == "1" ? false : true;
                        }

                        adapter = new RelayListAdapter(this,_relays);

                        adapter.RegisterDataSetObserver(new MyDatasetObserver());

                        _relayListView.Adapter = adapter;

                        _relayListView.ItemClick += Item_OnClick;

                    }

                    relayListProgressBar.Visibility = ViewStates.Gone;
                }
                catch (Exception ex)
                {
                    relayListProgressBar.Visibility = ViewStates.Gone;
                    Toast.MakeText(this, String.Format("Error Occured : {0}", ex.Message), ToastLength.Short).Show();
                }

            });

            
        }

        private void Item_OnClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            relayListProgressBar.Visibility = ViewStates.Visible;
            var relay = _relays[e.Position];

            string subscriptionMessage = "";

            //if (_mqttClient == null)
            //{
            //    _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");
            //    _mqttClient.Start();
            //}

            

            try
            {
                string switchStatus = AppHelper.GetStatus(relay.RelayState);
                int ackWaitTime = 0;
                bool ackRecived = false;
                Status status = null;

                string msgId = Guid.NewGuid().ToString();

                if (_mqttClient.ClientConnected)
                {
                    _mqttClient.PublishSomething(AppHelper.GetNodeMCUPin( relay.RelayNumber).ToString(), switchStatus, msgId, string.Format("relayActionRequest/{0}", relayGroupMac));

                    while (_mqttClient.ClientConnected && 
                           ackWaitTime < 20 && 
                           !ackRecived
                           )
                    {
                        ackWaitTime++;
                        //Thread.Sleep(1000);
                        if (_mqttClient.SubscriptionMessage!=null)
                        {
                            status = XmlHelper<Status>.ConvertToObject(_mqttClient.SubscriptionMessage);
                            subscriptionMessage = status.MsgId;
                        }

                        if (subscriptionMessage == msgId)
                        {
                            ackRecived = true;
                            //_relays.Where(x => x.Id == relay.Id).FirstOrDefault().RelayState = relay.RelayState;

                            foreach (var rel in _relays)
                            {
                                rel.RelayState = status.Relays.Relay.FirstOrDefault(x => x.RelayName == rel.RelayNumber.ToString()).RelayStatus == "1" ? false:true ;
                            }

                            relayListProgressBar.Visibility = ViewStates.Gone;
                            adapter.NotifyDataSetChanged();
                        }
                        else
                        {
                            Thread.Sleep(200);
                        }
                    }
                }

                if (!ackRecived)
                {
                    relayListProgressBar.Visibility = ViewStates.Gone;
                    Toast.MakeText(this,"Something went wrong. Please refresh cannot perform operation.",ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                relayListProgressBar.Visibility = ViewStates.Gone;
                Toast.MakeText(this, String.Format("Error Occured : {0}", ex.Message), ToastLength.Short).Show();
            }
        }
    }

    public class MyDatasetObserver: DataSetObserver
    {
        public MyDatasetObserver()
        {
              
        }

        public override void OnChanged()
        {
            base.OnChanged();
        }
    }
}