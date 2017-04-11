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
    [Activity(MainLauncher =true, Label = "Switch Board", Icon = "@drawable/icon")]
    public class SwitchBoardActivity : Activity
    {
        private ListView _menuListView;
        private List<RelayGroupModel> _relayGroups;
        ProgressBar progressBar;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.IoTMenuView);

            _menuListView = FindViewById<ListView>(Resource.Id.relayGroupListView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.mainMenuProgressBar);

            progressBar.Visibility = ViewStates.Visible;
            progressBar.Progress = 20;

            // Create your application here

            HttpClient client = new HttpClient();

            try
            {
                var response = await client.GetAsync(new Uri("http://ipowersaver-dev.azurewebsites.net/api/RelayGroupApi"));

                progressBar.Progress = 50;
                if (response.IsSuccessStatusCode)
                {
                    progressBar.Progress = 75;
                    var content = await response.Content.ReadAsStringAsync();
                    _relayGroups = JsonConvert.DeserializeObject<List<RelayGroupModel>>(content);

                    _menuListView.Adapter = new MenuListAdapter(this, _relayGroups);

                    _menuListView.ItemClick += Item_OnClick;

                }

                progressBar.Progress = 99;
                progressBar.Visibility = ViewStates.Gone;
            }
            catch (Exception ex)
            {
                progressBar.Visibility = ViewStates.Gone;
                Toast.MakeText(this, String.Format("Error Occured : {0}", ex.Message), ToastLength.Short).Show();
            }

            
        }

        private void Item_OnClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //var relayGroupId = this._menuListView.GetItemIdAtPosition(e.Position);
            var relayGroup = _relayGroups[e.Position];

            Intent relayViewIntent = new Intent(this, typeof(SwitchListActivity));
            
            relayViewIntent.PutExtra("relayGroupId",relayGroup.Id);

            relayViewIntent.PutExtra("relayGroupMac", relayGroup.RelayGroupMac);

            StartActivity(relayViewIntent);
        }
    }
}