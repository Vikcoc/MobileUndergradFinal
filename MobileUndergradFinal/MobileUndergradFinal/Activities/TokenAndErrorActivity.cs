using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using BusinessLogic;
using Google.Android.Material.Snackbar;

namespace MobileUndergradFinal.Activities
{
    public class TokenAndErrorActivity : AppCompatActivity, IAuthenticatedScreen
    {
        public string AccessToken
        {
            get
            {
                var preferences = this.GetSharedPreferences(Resources.GetString(Resource.String.auth), FileCreationMode.Private);
                return preferences.GetString(GetString(Resource.String.access_token), "");
            }
            set
            {
                var preferences = this.GetSharedPreferences(Resources.GetString(Resource.String.auth), FileCreationMode.Private);
                preferences.Edit().PutString(Resources.GetString(Resource.String.access_token), value).Apply();
            }
        }

        public void DisplayError(string error)
        {
            Snackbar.Make(Window?.DecorView, error, BaseTransientBottomBar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public void SignOutAndMoveToLogin()
        {
            AccessToken = "";
            var intent = new Intent(this, typeof(LoginRegisterActivity));
            intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            StartActivity(intent);
        }
    }
}