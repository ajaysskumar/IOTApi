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

namespace IoTDemoApp.Droid.Activities
{
    [Activity(Label = "RelayListActivity")]
    public class RelayListActivity : Activity
    {
        private List<RelayModel> _relays;
        private ListView _relayListView;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.RelayMenu);

            _relayListView = FindViewById<ListView>(Resource.Id.relayListView);

            // Create your application here

            int relayGroupId = Intent.GetIntExtra("relayGroupId", 0);

            HttpClient client = new HttpClient();

            var response = await client.GetAsync(new Uri("http://iotdemodev.apexsoftworks.in/api/RelayApi/"+ relayGroupId));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _relays = JsonConvert.DeserializeObject<List<RelayModel>>(content);

                _relayListView.Adapter = new Adapters.RelayListAdapter(this, _relays);

                _relayListView.ItemClick += Item_OnClick;
               
            }
        }

        private void Item_OnClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}