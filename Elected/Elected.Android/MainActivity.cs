using System;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Views;
using Android.Preferences;
using System.Collections.Generic;
using Android.Widget;
using Android.OS;
using System.Json;
using Android.Gms.Location.Places.UI;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;

namespace Elected.Droid
{
	[Activity (Label = "Elected!", MainLauncher = true, Icon = "@drawable/elected")]
	public class MainActivity : Activity, JsonParser.IPhotosLoadedListner
	{
        private ISharedPreferences mPreferences;
        private string address;
        private TextView addressView;
        private ListView listViewReps;
        private TextView listViewEmptyView;
        private TextView listViewEmptyViewElecs;
        private ListView listViewElecs;
        private RepresentativeAdapter adapterReps;
        private BaseAdapter adapterElecs;
        public static JsonParser parser;
        public static List<Representative> Representatives = new List<Representative>();
        public static List<Election> Elections = new List<Election>();
        private Button showReps;
        private Button showElecs;

        private readonly int PLACE_PICKER_ID = 9999;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
            checkPermissions();
            ShowDisclaimer();
            parser = new JsonParser(this);
            mPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            address = mPreferences.GetString("address", "1600 Pennsylvania AVE NW Washington DC");

			// Get our button from the layout resource,
			// and attach an event to it
            Button buttonSetAddress = FindViewById<Button>(Resource.Id.buttonChangeAddress);
            addressView = FindViewById<TextView>(Resource.Id.textViewAddress);
            listViewReps = FindViewById<ListView>(Resource.Id.listView1);
            adapterReps = new RepresentativeAdapter(this, Representatives);
            listViewReps.Adapter = adapterReps;
            adapterElecs = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Elections);
            listViewEmptyView = FindViewById<TextView>(Resource.Id.listViewEmptyView);
            listViewReps.EmptyView = listViewEmptyView;
            addressView.Text = address;

            listViewElecs = FindViewById<ListView>(Resource.Id.listView2);
            listViewEmptyViewElecs = FindViewById<TextView>(Resource.Id.listViewEmptyViewElecs);


            listViewReps.ItemClick += RepsListView_ItemClick;
            listViewElecs.ItemClick += ListViewElecs_ItemClick;
            buttonSetAddress.Click += delegate
            {
                var builder = new PlacePicker.IntentBuilder();
                StartActivityForResult(builder.Build(this), PLACE_PICKER_ID);
            };
            showReps = FindViewById<Button>(Resource.Id.buttonShowReps);
            showReps.Click += delegate
            {
                showRepData();
            };
            showElecs = FindViewById<Button>(Resource.Id.buttonShowElecs);
            showElecs.Click += delegate
            {
                showElectionData();
            };
        }

        private void ListViewElecs_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int index = e.Position;
            string electionId = Elections[index].id;
            if (!string.IsNullOrEmpty(electionId))
            { 
            Intent intent = new Intent(this, typeof(ElectionDetailActivity));
            intent.PutExtra("index", electionId);
            StartActivity(intent);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == PLACE_PICKER_ID && resultCode == Result.Ok)
            {
                var placePicked = PlacePicker.GetPlace(data, this);
                address = placePicked.AddressFormatted.ToString();
                addressView.Text = address;
                ISharedPreferencesEditor editor = mPreferences.Edit();
                editor.PutString("address", address);
                editor.Commit();

                loadRepData();
                loadElectionData();
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void RepsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int index = e.Position;
            Intent intent = new Intent(this, typeof(RepresentativeDetailActivity));
            intent.PutExtra("index", index);
            StartActivity(intent);
        }

        private async void loadRepData()
        {
            JsonValue jsonResponse = await VoterHttpRequest.FetchRepresentativesAsync(address, this);
            if (jsonResponse != null)
            {
                Representatives = parser.ParseRepresentativeJson(jsonResponse);
            } else
            {
                Toast.MakeText(this, "no result retrieved", ToastLength.Long).Show();
            }
            adapterReps.setDataSet(Representatives);
            showRepData();
        }

        private void showRepData()
        {
            showReps.SetBackgroundColor(Android.Graphics.Color.CadetBlue);
            showElecs.SetBackgroundColor(Android.Graphics.Color.Black);
            listViewReps.Visibility = Android.Views.ViewStates.Visible;
            listViewElecs.Visibility = ViewStates.Gone;
            
        }

        private async void loadElectionData()
        {
            JsonValue jsonResponse = await VoterHttpRequest.FetchVoterInfoAsync(address, this);
            if (jsonResponse != null)
            {
                Elections = parser.ParseElectionJson(jsonResponse);
                adapterElecs = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Elections);
                listViewElecs.Adapter = adapterElecs;
                listViewElecs.Visibility = ViewStates.Gone;
            } 
            else
            {
                Elections = new List<Election> { new Election { name = "no election data found", date = "election info may not be available more than 2-4 weeks prior" } };
                adapterElecs = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Elections);
                listViewElecs.Adapter = adapterElecs;
                listViewElecs.Visibility = ViewStates.Gone;
            }
        }

        private void showElectionData()
        {
            showReps.SetBackgroundColor(Android.Graphics.Color.Black);
            showElecs.SetBackgroundColor(Android.Graphics.Color.CadetBlue);

            listViewReps.Visibility = Android.Views.ViewStates.Gone;
            listViewElecs.Visibility = ViewStates.Visible;
        }

        public void OnPhotosLoaded(List<Representative> data)
        {
            Representatives = data;
            adapterReps.setDataSet(Representatives);
        }

        private void checkPermissions()
        {
            { 
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessNetworkState,
                    Manifest.Permission.WriteExternalStorage }, 9999);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (CheckSelfPermission(Manifest.Permission.AccessNetworkState) == (int)Permission.Granted &&
                CheckSelfPermission(Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                loadRepData();
                loadElectionData();

            }
                //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void ShowDisclaimer()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage("Elected! displays data from an online resource. Elected! does not create, edit, maintain, or check this data. " +
                "This data is provide on a volunteer basis by individual districts. Some areas may require the address of a registered voter be used. " +
                "Thank you for checking out Elected! Please share with your friends and use your power to vote.");
            builder.SetPositiveButton("Dismiss", (s,e) => { });

            AlertDialog ad = builder.Create();
            ad.Show();
        }
    }
}


