using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using BusinessLogic.Startup;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Layouts.Splash", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class SplashActivity : TokenAndErrorActivity, ISplashScreen
    {
        private readonly StartupLogic _logic;
        public SplashActivity()
        {
            _logic = new StartupLogic(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var startup = new Task(SimulateStartup);
            startup.Start();
        }

        private void SimulateStartup()
        {
            AfterInitialization?.Invoke();
        }

        public void MoveToDashboard()
        {
            StartActivity(new Intent(this, typeof(FountainsOnMapActivity)));
        }

        public void MoveToLogin()
        {
            StartActivity(new Intent(this, typeof(LoginRegisterActivity)));
        }

        public Action AfterInitialization { get; set; }
    }
}