using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using BusinessLogic;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using MobileUndergradFinal.Fragments;
using MobileUndergradFinal.Fragments.NoAccount;

namespace MobileUndergradFinal
{
    [Activity(Label = "@string/app_name", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity
    {
        private FragmentSignIn _signIn;
        private FragmentSignUp _signUp;

        private SwappableFragment _active;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.fragment_holder);

            _signIn = new FragmentSignIn();
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.fragmentHolder, _signIn).Commit();
            _active = _signIn;
        }

        private void GoTo(SwappableFragment toFragment)
        {
            if(toFragment.Lifecycle.CurrentState != Lifecycle.State.Created)
                SupportFragmentManager.BeginTransaction().Detach(_active).Add(Resource.Id.fragmentHolder, toFragment).Commit();
            else
                SupportFragmentManager.BeginTransaction().Detach(_active).Attach(toFragment).Commit();
            _active = toFragment;
        }

        public void GoToSignIn()
        {
            _signIn ??= new FragmentSignIn();
            GoTo(_signIn);
        }
        
        public void GoToSignUp()
        {
            _signUp ??= new FragmentSignUp();
            GoTo(_signUp);
        }

        public void GoToDashboard()
        {
            //throw new NotImplementedException();
        }

        public void GoBack()
        {
            base.OnBackPressed();
        }

        public override void OnBackPressed()
        {
            _active.OnBackPressed();
        }
    }
}
