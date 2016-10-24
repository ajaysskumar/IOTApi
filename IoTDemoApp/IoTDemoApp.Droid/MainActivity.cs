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
    [Activity(Label = "OnActuate IoT", MainLauncher = true, Icon = "@drawable/icon")]
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
            Button button = FindViewById<Button>(Resource.Id.btnSocketBulb);

            button.Text = "Bulb ON";

            TextView textStatus = FindViewById<TextView>(Resource.Id.txtSubMessage);

            client = new HttpClient();

            bool flag = false;

            string subscriptionMessage = "";

            MqttClient _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");
            _mqttClient.Start();

            button.Click += delegate
            {
                if (flag)
                {
                    button.Text = "Bulb ON";
                    textStatus.Text = subscriptionMessage;
                }
                else
                {
                    textStatus.Text = subscriptionMessage;
                    button.Text = "Bulb OFF";
                }

                try
                {
                    ProgressDialog mDialog = new ProgressDialog(this);
                    mDialog.SetMessage("Loading data...");
                    mDialog.SetCancelable(false);
                    mDialog.Show();

                    Task.Run(() =>
                    {
                        string msgId = Guid.NewGuid().ToString();

                        if (_mqttClient.ClientConnected)
                        {
                            _mqttClient.PublishSomething("1", flag==false?"1":"0",msgId);
                        }

                        while (_mqttClient.ClientConnected && _mqttClient.SubscriptionMessage != msgId)
                        {
                            subscriptionMessage = _mqttClient.SubscriptionMessage;
                        }
                        
                        if (button.Text == "Bulb ON")
                        {
                            flag = false;
                        }
                        else
                        {
                            flag = true;
                        }
                        //Your Logic Here.
                        mDialog.Dismiss();
                    });


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


