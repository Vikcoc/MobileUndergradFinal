using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Motion.Widget;
using AndroidX.RecyclerView.Widget;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.ItemDecorators;

namespace MobileUndergradFinal
{
    [Activity(Label = "DashboardActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddNewFountainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.add_new_fountain);

            var nearYou = FindViewById<RecyclerView>(Resource.Id.nearYou);
            nearYou.SetLayoutManager(new LinearLayoutManager(this));
            nearYou.SetAdapter(new FountainsTypeAdapter(() =>
            {
                var x = FindViewById<MotionLayout>(Resource.Id.motionLayout);
                if (x.CurrentState == x.StartState)
                    x.TransitionToEnd();
                else
                    x.TransitionToStart();
            }));
            nearYou.AddItemDecoration(new FountainsDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            var select = FindViewById(Resource.Id.selected);
            select.Click += (sender, args) =>
            {
                var x = FindViewById<MotionLayout>(Resource.Id.motionLayout);
                if (x.CurrentState == x.StartState)
                    x.TransitionToEnd();
                else
                    x.TransitionToStart();
            };
        }
    }
}