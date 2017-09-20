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
using System.Json;
using Android.Preferences;

namespace Elected.Droid
{
    [Activity(Label = "Election Details", Name = "com.babykangaroo.ancroid.elected.ElectionDetailActivity")]
    public class ElectionDetailActivity : Activity
    {
        string address;
        string index;
        private TextView header;
        private JsonParser parser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ElectionDetailLayout);
            header = FindViewById<TextView>(Resource.Id.textViewHeader);
            index = Intent.GetStringExtra("index");
            address = PreferenceManager.GetDefaultSharedPreferences(this).GetString("address", "1600 Pennsylvania AVE NW Washington DC");
            parser = new JsonParser(null);
            loadElectionInfo();

        }

        private async void loadElectionInfo()
        {
            JsonValue jsonResponse = await VoterHttpRequest.FetchElectionInfoAsync(address, index, this);
            
            header.Text = parser.ParseElectionInfoJson(jsonResponse);
        }
    }
}