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
    public class MenuListAdapter : BaseAdapter<RelayGroupModel>
    {
        private Activity _context;
        private List<RelayGroupModel> _relayGroups;
        public MenuListAdapter(Activity context, List<RelayGroupModel> relayGroups)
        {
            this._context = context;
            this._relayGroups = relayGroups;
        }

        public override RelayGroupModel this[int position]
        {
            get
            {
                return _relayGroups[position];
            }
        }

        public override int Count
        {
            get
            {
                return _relayGroups.Count;
            }
        }

        public RelayGroupModel GetCurrentItem(int position)
        {
            return _relayGroups[position];
        }

        //public override Java.Lang.Object GetItem(int position)
        //{
        //    return null;
        //}

        public override long GetItemId(int position)
        {
            return _relayGroups[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var relayGroup = _relayGroups[position];

            //var imageBitmap = ImageHelper.GetImageBitmapFromUrl("http://www.yourenergyblog.com/wp-content/uploads/2013/01/incandescent-bulb1.png");

            if (convertView == null)
            {
                convertView = _context.LayoutInflater.Inflate(Resource.Layout.MenuRowView, null);
            }

            convertView.FindViewById<TextView>(Resource.Id.relayNameTextView).Text = relayGroup.RelayGroupDescription;
            convertView.FindViewById<TextView>(Resource.Id.shortDescriptionTextView).Text = relayGroup.RelayGroupMac;
            convertView.FindViewById<TextView>(Resource.Id.priceTextView).Text = relayGroup.RelayGroupLocation;
            //convertView.FindViewById<ImageView>(Resource.Id.relayImageView).SetImageBitmap(imageBitmap);

            return convertView;
        }
    }
}