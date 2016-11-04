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

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            MqttClient _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");

            SetContentView(Resource.Layout.RelayMenu);

            _relayListView = FindViewById<ListView>(Resource.Id.relayListView);

            ProgressBar relayListProgressBar = FindViewById<ProgressBar>(Resource.Id.relayListProgressBar);

            relayListProgressBar.Visibility = ViewStates.Visible;

            // Create your application here

            relayGroupId = Intent.GetIntExtra("relayGroupId", 0);
            relayGroupMac = Intent.GetStringExtra("relayGroupMac");

            client = new HttpClient();

            var response = await client.GetAsync(new Uri("http://iotdemodev.apexsoftworks.in/api/RelayApi/"+ relayGroupId));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _relays = JsonConvert.DeserializeObject<List<RelayModel>>(content);

                _relayListView.Adapter = new Adapters.RelayListAdapter(this, _relays);

                _relayListView.ItemClick += Item_OnClick;
               
            }
            
            relayListProgressBar.Visibility = ViewStates.Gone;
        }

        private async void Item_OnClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var relay = _relays[e.Position];

            bool flag = false;
            string switchStatus = "1";

            string subscriptionMessage = "";
            
            _mqttClient.Start();

            _mqttClient.RegisterOurSubscriptions("relayActionConfirmation/" +relayGroupMac);

            try
                {
                    //flag = !swtich1.Checked;
                    if (flag == true)
                        switchStatus = "1";
                    else
                        switchStatus = "0";

                    ProgressDialog mDialog = new ProgressDialog(this);
                    mDialog.SetMessage("Loading data...");
                    mDialog.SetCancelable(false);
                    mDialog.Show();

                    await Task.Run(() =>
                    {
                        string msgId = Guid.NewGuid().ToString();

                        if (_mqttClient.ClientConnected)
                        {
                            _mqttClient.PublishSomething(relay.RelayNumber.ToString(), switchStatus, msgId,string.Format("relayActionRequest/{0}",relayGroupMac));
                        }

                        while (_mqttClient.ClientConnected && _mqttClient.SubscriptionMessage != msgId)
                        {
                            subscriptionMessage = _mqttClient.SubscriptionMessage;
                        }
                        //Your Logic Here.
                        mDialog.Dismiss();
                    });


                }
                catch (Exception ex)
                {
                    //textStatus.Text = ex.Message;
                }

        }
    }
}