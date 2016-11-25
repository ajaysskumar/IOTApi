using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Util;
using Newtonsoft.Json;

namespace AndroidService
{
    [Activity(Label = "AndroidService", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ILocationListener
    {
        int count = 1;
        LocationManager locMgr;
        MqttClient _mqttClient;

        public void OnLocationChanged(Location location)
        {
            Services.Location loc = new Services.Location() {
                Longitude = location.Longitude,
                Latitude = location.Latitude,
                Altitude = location.Altitude
            };
            if (_mqttClient==null)
            {
                _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");
                _mqttClient.Start();
            }
            _mqttClient.PublishSomething("locationData", JsonConvert.SerializeObject(loc));
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            
        }

        protected override void OnCreate(Bundle bundle)
        {
            
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);


            button.Click += delegate
            {
                if (_mqttClient==null)
                {
                    _mqttClient = new MqttClient("TCP://m13.cloudmqtt.com:19334", "SFD-GBL-PER-16102016-11-50-19-153445", "cbaeasea", "KiYFQP0Q1gbe");
                    _mqttClient.Start();
                }
                
                button.Text = string.Format("Location Service on");
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            string Provider = LocationManager.GpsProvider;

            if (locMgr.IsProviderEnabled(Provider))
            {
                locMgr.RequestLocationUpdates(Provider, 2000, 1, this);
            }
            else
            {
                Log.Info("Log", Provider + " is not available. Does the device have location services enabled?");
            }
        }
    }
}

