using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Snackbar;
using System.Collections.Generic;
using System.Linq;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "MyMapsActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MyMapsActivity : TokenAndErrorActivity, IOnMapReadyCallback
    {
        private GoogleMap _map;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.map_layout);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            var mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var but = FindViewById<ImageButton>(Resource.Id.currentLoc);
            but.Click += async (sender, args) =>
            {
                if(this.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) != (int)Permission.Granted || this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
                    this.RequestPermissions(new []{ Android.Manifest.Permission.AccessFineLocation, Android.Manifest.Permission.AccessCoarseLocation }, 10);
                else
                {
                    _map.MyLocationEnabled = true;

                    var locationProvider = LocationServices.GetFusedLocationProviderClient(this);
                    var x = await locationProvider.GetLastLocationAsync();

                    _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(x.Latitude, x.Longitude), 15));
                }
            };

            var but2 = FindViewById<ImageButton>(Resource.Id.printLoc);
            but2.Click += async (sender, args) =>
            {
                var geocode = new Geocoder(this);

                var addresses = await geocode.GetFromLocationAsync(_map.CameraPosition.Target.Latitude, _map.CameraPosition.Target.Longitude, 1);

                if (addresses == null || !addresses.Any())
                {
                    var toast = Snackbar.Make(this, (View)sender, "Error: What you're looking at not found", BaseTransientBottomBar.LengthLong);
                    toast?.Show();
                    return;
                }

                var intent = new Intent();
                intent.PutExtra("address", addresses.First().GetAddressLine(0));
                intent.PutExtra("latitude", _map.CameraPosition.Target.Latitude);
                intent.PutExtra("longitude", _map.CameraPosition.Target.Longitude);
                SetResult(Result.Ok, intent);
                Finish();
            };

            var search = FindViewById<AndroidX.AppCompat.Widget.SearchView>(Resource.Id.idSearchView);
            search.QueryTextSubmit += async (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(args.NewText))
                    return;
                var geocode = new Geocoder(this);

                IList<Address> addressList;

                try
                {
                    addressList = await geocode.GetFromLocationNameAsync(args.NewText, 1);
                }
                catch (Java.IO.IOException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    return;
                }
                if (addressList == null || !addressList.Any())
                {
                    var toast = Snackbar.Make(this, (View)sender, "Error: " + args.NewText + " not found", BaseTransientBottomBar.LengthLong);
                    toast?.Show();
                    return;
                }

                _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(addressList.First().Latitude, addressList.First().Longitude), 15));
            };
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults.Any(x => x == Permission.Granted))
            {
                _map.MyLocationEnabled = true;
                var locationProvider = LocationServices.GetFusedLocationProviderClient(this);
                var x = await locationProvider.GetLastLocationAsync();

                _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(x.Latitude, x.Longitude), 15));
            }
        }

        public async void OnMapReady(GoogleMap map)
        {
            _map = map;

            _map.UiSettings.MyLocationButtonEnabled = false;

            if (this.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == (int)Permission.Granted && this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted)
            {
                map.MyLocationEnabled = true;

                var locationProvider = LocationServices.GetFusedLocationProviderClient(this);
                var x = await locationProvider.GetLastLocationAsync();

                map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(x.Latitude, x.Longitude), 15));
            }
        }
    }
}