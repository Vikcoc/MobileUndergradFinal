using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using BusinessLogic.Dashboard;
using Communication.SourceContributionDto;
using Communication.SourcePlaceDto;
using MobileUndergradFinal.AdapterDto;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.Helper;
using MobileUndergradFinal.ItemDecorators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "DashboardActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DashboardActivity : TokenAndErrorActivity, IDashboardScreen, IOnMapReadyCallback
    {

        private readonly DashboardLogic _dashboard;

        private TextView _welcome;

        private FountainsAdapter _fountainsAdapter;
        private ContributionListingAdapter _contributionListingAdapter;

        private GoogleMap _map;

        public DashboardActivity()
        {
            _dashboard = new DashboardLogic(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.dashboard);

            var mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var nearYou = FindViewById<RecyclerView>(Resource.Id.nearYou);
            nearYou.SetLayoutManager(new LinearLayoutManager(this));
            _fountainsAdapter = new FountainsAdapter(OnPlaceSelected);
            nearYou.SetAdapter(_fountainsAdapter);
            nearYou.AddItemDecoration(new FountainsDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            var yourContributions = FindViewById<RecyclerView>(Resource.Id.yourContributions);
            yourContributions.SetLayoutManager(new LinearLayoutManager(this));
            _contributionListingAdapter = new ContributionListingAdapter();
            yourContributions.SetAdapter(_contributionListingAdapter);
            yourContributions.AddItemDecoration(new FountainsDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            var addNew = FindViewById<Button>(Resource.Id.addNew);
            addNew.Click += (sender, args) => OnAddNewFountainPress?.Invoke();


            var seeAll = FindViewById<Button>(Resource.Id.seeAll);
            seeAll.Click += (sender, args) => OnSeeAllPlacesPress?.Invoke();

            var view = FindViewById(Resource.Id.signOut);
            view.Click += (sender, args) => OnSignOutPress?.Invoke();

            _welcome = FindViewById<TextView>(Resource.Id.textView3);

            OnCreated();
        }

        private async void OnCreated()
        {
            await OnScreenVisible();
        }

        public void MoveToAddNewFountain()
        {
            StartActivity(typeof(AddNewPlaceActivity));
        }

        public Action OnSignOutPress { get; set; }
        public Action OnAddNewFountainPress { get; set; }
        public Func<Task> OnScreenVisible { get; set; }
        public string Welcome
        {
            set => _welcome.Text = value;
        }
        public string WelcomeText => Resources.GetString(Resource.String.dashboard_welcome);

        public List<WaterSourceContributionWithPlaceDto> WaterContributions
        {
            set
            {
                _contributionListingAdapter.AddItems(value.Select(x => new WaterSourceContributionWithPlace
                {
                    Id = x.Id,
                    WaterSourcePlaceId = x.WaterSourcePlaceId,
                    WaterSourcePlace = new WaterSourcePlaceListing
                    {
                        Nickname = x.WaterSourcePlace.Nickname,
                        Latitude = x.WaterSourcePlace.Latitude,
                        Address = x.WaterSourcePlace.Address,
                        Id = x.WaterSourcePlace.Id,
                        Longitude = x.WaterSourcePlace.Longitude,
                        WaterSourceVariantId = x.WaterSourcePlace.WaterSourceVariantId,
                    },
                    ContributionType = x.ContributionType,
                    Details = x.Details,
                    RelatedContributionId = x.RelatedContributionId,
                    WaterUserId = x.WaterUserId,
                    
                }).ToList());
            }
        }

        public void AddContributionPicture(Guid contributionId, Stream image)
        {
            var bitmap = image.GetOfScale(Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                Resources.GetDimensionPixelSize(Resource.Dimension.image_size));
            _contributionListingAdapter.AddPicture(contributionId, bitmap);
        }

        public decimal MapLeft => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Southwest.Longitude);
        public decimal MapBot => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Southwest.Latitude);
        public decimal MapRight => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Northeast.Longitude);
        public decimal MapTop => Convert.ToDecimal(_map.Projection.VisibleRegion.LatLngBounds.Northeast.Latitude);

        public List<WaterSourcePlaceListingWithContributionDto> WaterPlaces
        {
            set
            {
                _fountainsAdapter.AddItems(value.Take(5).Select(x => new WaterSourcePlaceListingWithContribution
                {
                    Id = x.Id,
                    Latitude = x.Latitude,
                    Address = x.Address,
                    Contribution = new WaterSourceContribution
                    {
                        Id = x.Contribution.Id,
                        WaterSourcePlaceId = x.Contribution.WaterSourcePlaceId,
                        ContributionType = x.Contribution.ContributionType,
                        Details = x.Contribution.Details,
                        RelatedContributionId = x.Contribution.RelatedContributionId,
                        WaterUserId = x.Contribution.WaterUserId,
                    },
                    Longitude = x.Longitude,
                    Nickname = x.Nickname,

                }).ToList());

                foreach (var dto in value)
                {
                    _map.AddMarker(new MarkerOptions()
                        .SetPosition(new LatLng(Convert.ToDouble(dto.Latitude), Convert.ToDouble(dto.Longitude)))
                        .SetTitle(dto.Nickname));
                }
            }
        }
        public void AddPlacePicture(Guid placeId, Stream image)
        {
            var bitmap = image.GetOfScale(Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                Resources.GetDimensionPixelSize(Resource.Dimension.image_size));
            _fountainsAdapter.AddPicture(placeId, bitmap);
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

            _map.MapClick += (sender, args) =>
            {
                OnMapPress?.Invoke();
            };

            _map.MarkerClick += (sender, args) =>
            {
                OnMapPress?.Invoke();
            };

            await _dashboard.GetPlacesAroundMap();
        }

        public void MoveToMap(Guid? placeId = null)
        {
            var intent = new Intent(this, typeof(FountainsOnMapActivity));
            if (placeId.HasValue)
                intent.PutExtra("placeId", placeId.Value.ToString());
            StartActivity(intent);
        }

        public Action OnMapPress { get; set; }
        public Action OnSeeAllPlacesPress { get; set; }
        public Action<Guid> OnPlaceSelected { get; set; }
    }
}