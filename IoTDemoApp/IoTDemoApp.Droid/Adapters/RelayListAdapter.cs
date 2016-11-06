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
using Java.Lang;
using IoTDemoApp.Model;
using IoTDemoApp.Droid.Utility;

namespace IoTDemoApp.Droid.Adapters
{
    public class RelayListAdapter : BaseAdapter
    {
        private Activity _context;
        private List<RelayModel> _relays;
        public RelayListAdapter(Activity context, List<RelayModel> relays)
        {
            this._context = context;
            this._relays = relays;
        }
       
        public override int Count
        {
            get
            {
                return _relays.Count;
            }
        }

        public RelayModel GetCurrentItem(int position)
        {
            return _relays[position];
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            return _relays[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var relay = _relays[position];

            if (convertView == null)
            {
                convertView = _context.LayoutInflater.Inflate(Resource.Layout.RelayListView, null);
            }

            convertView.FindViewById<TextView>(Resource.Id.shortDescriptionTextView).Text = relay.RelayDescription;
            convertView.FindViewById<TextView>(Resource.Id.relayNumberTextView).Text = relay.RelayNumber.ToString();
            //convertView.FindViewById<TextView>(Resource.Id.relayStatus).Text = relay.RelayState.ToString();
            //convertView.FindViewById<ImageView>(Resource.Id.relayImageView).SetImageBitmap(imageBitmap);
            convertView.FindViewById<Switch>(Resource.Id.switchRelayStatus).Activated = !relay.RelayState;
            convertView.FindViewById<Switch>(Resource.Id.switchRelayStatus).Toggle();

            return convertView;
        }
    }
}