using Android.App;
using Android.Content;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using BusinessLogic.Startup;
using System;
using System.Threading.Tasks;

namespace MobileUndergradFinal
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Layouts.Splash", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class SplashActivity : AppCompatActivity, ISplashScreen
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

        public string AccessToken
        {
            get
            {
                var preferences = this.GetSharedPreferences(Resources.GetString(Resource.String.auth), FileCreationMode.Private);
                return preferences.GetString(Resources.GetString(Resource.String.access_token), null);
            }
        }

        public void MoveToDashboard()
        {
            StartActivity(new Intent(this, typeof(DashboardActivity)));
        }

        public void MoveToLogin()
        {
            StartActivity(new Intent(this, typeof(LoginRegister)));
        }

        public Action AfterInitialization { get; set; }
    }
}