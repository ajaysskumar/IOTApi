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
	[Activity (Label = "OnActuate IoT", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		//int count = 1;
        HttpClient client;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.btnSocketBulb);

            button.Text = "Bulb ON";

            TextView textStatus = FindViewById<TextView>(Resource.Id.textViewStatus);

            client = new HttpClient();

            button.Click += delegate {
                try
                {
                    //ProgressDialog mDialog = new ProgressDialog(this);
                    //mDialog.SetMessage("Loading data...");
                    //mDialog.SetCancelable(false);
                    //mDialog.Show();

                    //Task.Run(() => {
                    if (button.Text == "Bulb ON")
                    {

                        if (FetchWeather("http://iotdemo.apexsoftworks.in/api/RequestApi?relayId=2&opCode="+0+"&msgId="+Guid.NewGuid()))
                            {
                                button.Text = "Bulb OFF";
                            }
                        }
                        else
                        {
                            if (FetchWeather("http://iotdemo.apexsoftworks.in/api/RequestApi?relayId=2&opCode=" + 1 + "&msgId=" + Guid.NewGuid()))
                            {
                                button.Text = "Bulb ON";
                            }


                        }
                    //    //Your Logic Here.
                    //    mDialog.Dismiss();
                    //});

                    
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


