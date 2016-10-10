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

namespace IoTDemoApp.Droid
{
	[Activity (Label = "IoTDemoApp.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.btnSocketBulb);

            TextView textStatus = FindViewById<TextView>(Resource.Id.textViewStatus);

            button.Click += delegate {
                try
                {
                    if (button.Text == "Bulb ON")
                    {

                        if (FetchWeather("http://192.168.100.186/socket1Off"))
                        {
                            button.Text = "Bulb OFF";
                        }
                    }
                    else
                    {
                        if (FetchWeather("http://192.168.100.186/socket1On"))
                        {
                            button.Text = "Bulb ON";
                        }
                        
                        
                    }
                }
                catch (Exception ex)
                {
                    textStatus.Text = ex.Message;
                }
                
			};
		}

        private bool FetchWeather(string url)
        {
            HttpWebResponse response = null;

            try
            {
                HttpWebRequest request =(HttpWebRequest)HttpWebRequest.Create(url); 
                request.Method = "GET";

                response = (HttpWebResponse)request.GetResponse();

                response.Close();

                //StreamReader sr = new StreamReader(response.GetResponseStream());
                //Console.Write(sr.ReadToEnd());
                if (response.StatusCode!=HttpStatusCode.OK )
                {
                    return false;
                }
                return true;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)e.Response;
                    
                    //Console.Write("Errorcode: {0}", (int)response.StatusCode);
                }
                throw e;
            }
        }
    }
}


