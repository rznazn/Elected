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
using Android.Graphics;
using System.Net;

namespace Elected.Droid
{
    public class RepresentativeAdapter : BaseAdapter
    {
        private List<Representative> reps;
        private Activity activity;

        public RepresentativeAdapter(Activity activity, List<Representative> data)
        {
            this.activity = activity;
            this.reps = data;
        }
        public override int Count
        {
            get
            {
                return reps.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            return (long) position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            Representative current = reps[position];
            View rootView = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.ListViewItemReps, parent, false);
            TextView previewTextView = rootView.FindViewById<TextView>(Resource.Id.textView1);
            ImageView photo = rootView.FindViewById<ImageView>(Resource.Id.thumbnail);
            Bitmap imageBitmap = null;
            var imageBytes = current.PhotoBytes;
            var options = new BitmapFactory.Options();
            if (imageBytes != null && imageBytes.Length > 0)
            {
                if (imageBytes.Length > 100000)
                {
                    options.InSampleSize = 20;
                }

                if (imageBytes != null && imageBytes.Length > 0)
                {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length, options);
                }
                photo.SetImageBitmap(imageBitmap);
            }else
            {
                photo.SetImageDrawable(activity.GetDrawable(Resource.Drawable.elected));
            }
            previewTextView.Text = current.OfficeName + "\n" + current.Name + "(" + current.Party + ")";
            return rootView;
        }

        public void setDataSet(List<Representative> data)
        {
            this.reps = data;
            NotifyDataSetChanged();
        }
        
    }
}