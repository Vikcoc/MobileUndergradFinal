using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using BusinessLogic.Dashboard;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.ItemDecorators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Communication.SourceContributionDto;
using MobileUndergradFinal.AdapterDto;
using MobileUndergradFinal.Helper;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "DashboardActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DashboardActivity : TokenAndErrorActivity, IDashboardScreen
    {

        private readonly DashboardLogic _dashboard;

        private TextView _welcome;

        private ContributionListingAdapter _contributionListingAdapter;
        public DashboardActivity()
        {
            _dashboard = new DashboardLogic(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.dashboard);

            var nearYou = FindViewById<RecyclerView>(Resource.Id.nearYou);
            nearYou.SetLayoutManager(new LinearLayoutManager(this));
            nearYou.SetAdapter(new FountainsAdapter());
            nearYou.AddItemDecoration(new FountainsDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            var yourContributions = FindViewById<RecyclerView>(Resource.Id.yourContributions);
            yourContributions.SetLayoutManager(new LinearLayoutManager(this));
            _contributionListingAdapter = new ContributionListingAdapter();
            yourContributions.SetAdapter(_contributionListingAdapter);
            yourContributions.AddItemDecoration(new FountainsDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            var addNew = FindViewById<Button>(Resource.Id.addNew);
            addNew.Click += (sender, args) => OnAddNewFountainPress?.Invoke();

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
                        //Picture = x.WaterSourcePlace.,
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
            var bitmap = BitmapHelper.GetOfScale(Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                image);
            _contributionListingAdapter.AddPicture(contributionId, bitmap);
        }
    }
}