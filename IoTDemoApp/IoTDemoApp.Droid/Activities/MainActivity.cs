using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;

namespace IoTDemoApp.Droid
{
    [Activity(Label = "OnActuate IoT", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //int count = 1;
        HttpClient client;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.btnSocketBulb);

            //button.Text = "Bulb ON";

            TextView textStatus = FindViewById<TextView>(Resource.Id.txtSubMessage);
            Switch swtich1 = FindViewById<Switch>(Resource.Id.swtich1);
            

            client = new HttpClient();

            bool flag = false;
            string switchStatus = "1";

            string subscriptionMessage = "";

            MqttClient _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");
            _mqttClient.Start();



            swtich1.Click += delegate
            {
                try
                {
                    flag = !swtich1.Checked;
                    if (flag == true)
                        switchStatus = "1";
                    else
                        switchStatus = "0";

                    ProgressDialog mDialog = new ProgressDialog(this);
                    mDialog.SetMessage("Loading data...");
                    mDialog.SetCancelable(false);
                    mDialog.Show();

                    Task.Run(() =>
                    {
                        string msgId = Guid.NewGuid().ToString();

                        if (_mqttClient.ClientConnected)
                        {
                            _mqttClient.PublishSomething("1", switchStatus,msgId);
                        }

                        while (_mqttClient.ClientConnected && _mqttClient.SubscriptionMessage != msgId)
                        {
                            subscriptionMessage = _mqttClient.SubscriptionMessage;
                        }
                        //Your Logic Here.
                        mDialog.Dismiss();
                    });

                    textStatus.Text = subscriptionMessage;

                }
                catch (Exception ex)
                {
                    textStatus.Text = ex.Message;
                }

            };
        }

        private bool FetchWeather(string url)
        {
            try
            {

                var response = client.PostAsync(url, new StringContent("")).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                    //Items = JsonConvert.DeserializeObject<List<TodoItem>>(content);
                }

                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}


