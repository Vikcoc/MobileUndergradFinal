using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using BusinessLogic.Dashboard;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.ItemDecorators;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "DashboardActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DashboardActivity : TokenAndErrorActivity, IDashboardScreen
    {

        private readonly DashboardLogic _dashboard;
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
            yourContributions.SetAdapter(new FountainsAdapter());
            yourContributions.AddItemDecoration(new FountainsDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            var addNew = FindViewById<Button>(Resource.Id.addNew);
            addNew.Click += (sender, args) => OnAddNewFountainPress?.Invoke();

            var view = FindViewById(Resource.Id.signOut);
            view.Click += (sender, args) => OnSignOutPress?.Invoke();
        }

        public void MoveToAddNewFountain()
        {
            StartActivity(typeof(AddNewFountainActivity));
        }

        public Action OnSignOutPress { get; set; }
        public Action OnAddNewFountainPress { get; set; }
    }
}