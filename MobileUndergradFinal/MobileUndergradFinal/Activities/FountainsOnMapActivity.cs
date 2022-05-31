using Android.App;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.RecyclerView.Widget;
using BusinessLogic.Dashboard;
using Communication.SourceContributionDto;
using Communication.SourcePlaceDto;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.Helper;
using MobileUndergradFinal.ItemDecorators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "FountainsOnMapActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FountainsOnMapActivity : TokenAndErrorActivity, IOnMapReadyCallback, IFountainsOnMapScreen
    {
        private GoogleMap _map;
        private ContributionOfPlacesAdapter _contributionsAdapter;
        private readonly FountainsOnMapLogic _logic;
        private Guid? _pinSelected;
        private ConstraintLayout _sheet;

        public FountainsOnMapActivity()
        {
            _logic = new FountainsOnMapLogic(this);
        }

        public decimal MapLeft => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Southwest.Longitude);
        public decimal MapBot => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude);
        public decimal MapRight => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Northeast.Longitude);
        public decimal MapTop => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);

        public WaterSourcePlaceListingDto SelectedPlace
        {
            set
            {
                _sheet.FindViewById<TextView>(Resource.Id.textView4).Text = value.Nickname;
                _sheet.FindViewById<TextView>(Resource.Id.textView5).Text = value.Address;
            }
    }

        public List<WaterSourceContributionDto> SelectedContributions
        {
            set => _contributionsAdapter.SetItems(value);
        }
        public List<WaterSourcePlaceListingDto> WaterPlaces
        {
            set
            {
                var selected = Intent.GetStringExtra("placeId");
                if (!string.IsNullOrWhiteSpace(selected))
                    _pinSelected = Guid.Parse(selected);
                foreach (var dto in value)
                {
                    var marker = _map.AddMarker(new MarkerOptions()
                        .SetPosition(new LatLng(Convert.ToDouble(dto.Latitude), Convert.ToDouble(dto.Longitude)))
                        .SetTitle(dto.Nickname));
                    marker.Tag = dto.Id.ToString();
                    if (_pinSelected.HasValue && _pinSelected.Value == dto.Id)
                        MarkerClicked(marker);

                }
            }
        }

        public void AddSelectedPlacePicture(Stream image)
        {
            var bitmap = BitmapHelper.GetOfScale(Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                image);
            _sheet.FindViewById<ImageView>(Resource.Id.imageView2).SetImageBitmap(bitmap);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.map_with_fountains);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            var mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            _sheet = FindViewById<ConstraintLayout>(Resource.Id.standardBottomSheet);
            ((_sheet.LayoutParameters as CoordinatorLayout.LayoutParams).Behavior as BottomSheetBehavior).Hideable = true;
            ((_sheet.LayoutParameters as CoordinatorLayout.LayoutParams).Behavior as BottomSheetBehavior).State = BottomSheetBehavior.StateHidden;


            var recycler = FindViewById<RecyclerView>(Resource.Id.contributions);
            recycler.SetLayoutManager(new LinearLayoutManager(this));
            _contributionsAdapter = new ContributionOfPlacesAdapter();
            recycler.SetAdapter(_contributionsAdapter);
            recycler.AddItemDecoration(new FountainsDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            var but = FindViewById<FloatingActionButton>(Resource.Id.fab_locate);
            but.Click += async (sender, args) =>
            {
                if (this.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) != (int)Permission.Granted || this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
                    this.RequestPermissions(new[] { Android.Manifest.Permission.AccessFineLocation, Android.Manifest.Permission.AccessCoarseLocation }, 10);
                else
                {
                    _map.MyLocationEnabled = true;

                    var locationProvider = LocationServices.GetFusedLocationProviderClient(this);
                    var x = await locationProvider.GetLastLocationAsync();

                    _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(x.Latitude, x.Longitude), 15));
                }
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

            var sheet = _sheet.LayoutParameters as CoordinatorLayout.LayoutParams;

            _map.MarkerClick += (sender, args) => MarkerClicked(args.Marker);

            _map.CameraMoveStarted += (sender, args) =>
            {
                if(_pinSelected.HasValue)
                    (sheet.Behavior as BottomSheetBehavior).State = BottomSheetBehavior.StateCollapsed;
            };

            _map.MapClick += (sender, args) =>
            {
                (sheet.Behavior as BottomSheetBehavior).Hideable = true;
                (sheet.Behavior as BottomSheetBehavior).State = BottomSheetBehavior.StateHidden;
                _pinSelected = null;
                _contributionsAdapter.SetItems(new List<WaterSourceContributionDto>());
            };


            await _logic.GetPlacesAroundMap();
        }

        private async void MarkerClicked(Marker marker)
        {
            var id = Guid.Parse((string)marker.Tag);
            _pinSelected = id;
            marker.ShowInfoWindow();
            var sheet = _sheet.LayoutParameters as CoordinatorLayout.LayoutParams;
            (sheet.Behavior as BottomSheetBehavior).State = BottomSheetBehavior.StateCollapsed;
            (sheet.Behavior as BottomSheetBehavior).Hideable = false;
            await _logic.GetPlace(id);
        }
    }
}