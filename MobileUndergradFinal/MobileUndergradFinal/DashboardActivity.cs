using Android.App;
using Android.OS;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.ItemDecorators;

namespace MobileUndergradFinal
{
    [Activity(Label = "DashboardActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class DashboardActivity : AppCompatActivity
    {
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
        }
    }
}