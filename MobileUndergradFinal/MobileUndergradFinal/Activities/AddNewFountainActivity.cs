using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Motion.Widget;
using AndroidX.RecyclerView.Widget;
using BusinessLogic.Dashboard;
using Communication.SourceVariantDto;
using MobileUndergradFinal.AdapterDto;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.ItemDecorators;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "DashboardActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddNewFountainActivity : TokenAndErrorActivity, IAddNewFountainScreen
    {
        private MotionLayout _selectorLayout;
        private FountainsTypeAdapter _fountainsTypeAdapter;
        private View _selectedView;

        private AddNewFountainLogic _logic;

        public AddNewFountainActivity()
        {
            _logic = new AddNewFountainLogic(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.add_new_fountain);

            _fountainsTypeAdapter = new FountainsTypeAdapter(selectedView =>
            {
                SelectedVariant = selectedView.Id;
                var topText = _selectedView.FindViewById<TextView>(Resource.Id.textView4);
                topText.Text = selectedView.Name;
                var bottomText = _selectedView.FindViewById<TextView>(Resource.Id.textView5);
                bottomText.Text = selectedView.Description;

                _selectorLayout.TransitionToEnd();
            });

            _selectorLayout = FindViewById<MotionLayout>(Resource.Id.motionLayout);

            var fountainType = FindViewById<RecyclerView>(Resource.Id.fountainType);
            fountainType.SetLayoutManager(new LinearLayoutManager(this));
            fountainType.SetAdapter(_fountainsTypeAdapter);

            fountainType.AddItemDecoration(new FountainTypesDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));

            _selectedView = FindViewById(Resource.Id.selected);
            _selectedView.Click += (sender, args) =>
            {
                SelectedVariant = null;
                _selectorLayout.TransitionToStart();
            };
            
            OnCreated();
        }

        private async void OnCreated()
        {
            await OnScreenVisible();
        }

        public Func<Task> OnScreenVisible { get; set; }
        public void SetWaterSourceVariants(List<WaterSourceVariantDto> waterSources)
        {
            _fountainsTypeAdapter.AddItems(waterSources.Select(x => new WaterSourceVariant
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name
            }).ToList());
        }

        public Guid? SelectedVariant { get; private set; }
    }
}