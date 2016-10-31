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
using Newtonsoft.Json;
using IoTDemoApp.Model;
using IoTDemoApp.Droid.Adapters;
using IoTDemoApp.Droid.Activities;

namespace IoTDemoApp.Droid
{
    [Activity(MainLauncher =true, Label = "OnActuate IoT", Icon = "@drawable/icon")]
    public class IoTMenuActivity : Activity
    {
        private ListView _menuListView;
        private List<RelayGroupModel> _relayGroups;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.IoTMenuView);

            _menuListView = FindViewById<ListView>(Resource.Id.relayGroupListView);

            // Create your application here

            HttpClient client = new HttpClient();

            var response = await client.GetAsync(new Uri("http://iotdemodev.apexsoftworks.in/api/RelayGroupApi"));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _relayGroups = JsonConvert.DeserializeObject<List<RelayGroupModel>>(content);

                _menuListView.Adapter = new MenuListAdapter(this, _relayGroups);

                _menuListView.ItemClick += Item_OnClick;
                
            }
           
        }

        private void Item_OnClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var relayGroupId = this._menuListView.GetItemIdAtPosition(e.Position);

            Intent relayViewIntent = new Intent(this, typeof(RelayListActivity));
            
            relayViewIntent.PutExtra("relayGroupId",Convert.ToInt32(relayGroupId));
            StartActivity(relayViewIntent);
        }
    }
}