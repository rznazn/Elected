
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Net;
using System.Threading.Tasks;
using Android.Util;
using Java.Net;

namespace Elected.Droid
{
    [Activity(Label = "Elected!", Name ="com.babykangaroo.ancroid.elected.RepresentativeDetailActivity")]
    public class RepresentativeDetailActivity : Activity
    {
        private TextView name;
        private TextView office;
        private TextView address;
        private ImageView portrait;
        private ListView phoneListView;
        private ListView emailsListView;
        private ListView urlsListView;
        private ListView socialMediaListView;
        Representative currentRep;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RepDetailLayout);

            currentRep = MainActivity.parser.representatives[Intent.GetIntExtra("index", 0)];

            name = FindViewById<TextView>(Resource.Id.textViewName);
            name.Text = currentRep.Name + "(" + currentRep.Party + ")";
            office = FindViewById<TextView>(Resource.Id.textViewOfficeName );
            office.Text = currentRep.OfficeName;
            address = FindViewById<TextView>(Resource.Id.textViewAddress);
            address.Text = currentRep.Address;
            portrait = FindViewById<ImageView>(Resource.Id.imageView1);

            phoneListView = FindViewById<ListView>(Resource.Id.listViewPhone);
            phoneListView.Adapter = new ArrayAdapter(
                this, Android.Resource.Layout.SimpleListItem1, currentRep.PhoneNumbers);
            phoneListView.ItemClick += PhoneListView_ItemClick;
            emailsListView = FindViewById<ListView>(Resource.Id.listViewEmails);
            emailsListView.Adapter = new ArrayAdapter(
                this, Android.Resource.Layout.SimpleListItem1, currentRep.Emails);
            emailsListView.ItemClick += EmailsListView_ItemClick;
            urlsListView = FindViewById<ListView>(Resource.Id.listViewUrls);
            urlsListView.Adapter = new ArrayAdapter(
                this, Android.Resource.Layout.SimpleListItem1, currentRep.Urls);
            urlsListView.ItemClick += UrlsListView_ItemClick;
            socialMediaListView = FindViewById<ListView>(Resource.Id.listViewSocialMedia);
            socialMediaListView.Adapter = new ArrayAdapter(
                this, Android.Resource.Layout.SimpleListItem1, currentRep.Channels);
            socialMediaListView.ItemClick += SocialMediaListView_ItemClick;

            Bitmap imageBitmap = null;
            var imageBytes = currentRep.PhotoBytes;
            var options = new BitmapFactory.Options();
            if (imageBytes != null && imageBytes.Length > 0)
            { 
            if (imageBytes.Length > 100000)
            {
                options.InSampleSize = 8;
            }

            if (imageBytes != null && imageBytes.Length > 0)
            {
                imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length, options);
            }
            portrait.SetImageBitmap(imageBitmap);
            }
        }

        private void SocialMediaListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Channel channel = currentRep.Channels[e.Position];
            string uri;
            if (channel.Type == "GooglePlus")
            {
                uri = "https://plus.google.com/" + channel.Id;
            }else
            {
                uri = "https://www." + channel.Type + ".com/" + channel.Id;
            }
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(uri));
            StartActivity(intent); 
        }

        private void UrlsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string url = currentRep.Urls[e.Position];
            var uri = Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void EmailsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string email = currentRep.Emails[e.Position];
            var emailIntent = new Intent(Android.Content.Intent.ActionSend);
            emailIntent.PutExtra(Android.Content.Intent.ExtraEmail,
            new string[] { email });
            emailIntent.SetType("message/rfc822");
            StartActivity(emailIntent);

        }

        private void PhoneListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string phoneNumber = currentRep.PhoneNumbers[e.Position];
            var uri = Android.Net.Uri.Parse("tel:" + phoneNumber);
            var intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        private void GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                var options = new BitmapFactory.Options();
                if(imageBytes.Length > 100000)
                {
                options.InSampleSize = 8;
                }

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length, options);
                }
            }
            portrait.SetImageBitmap(imageBitmap);
        }
    }
}