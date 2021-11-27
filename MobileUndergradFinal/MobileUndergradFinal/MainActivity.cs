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
using BusinessLogic;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;

namespace MobileUndergradFinal
{
    [Activity(Label = "@string/app_name", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class MainActivity : AppCompatActivity, ILoginScreen
    {
        private TextInputEditText _username;
        private TextInputEditText _password;

        private TextInputLayout _usernameLayout;
        private TextInputLayout _passwordLayout;

        private Button _submitButton;

        private readonly LoginLogic _loginLogic;

        protected MainActivity(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            _loginLogic = new LoginLogic(this);
        }

        public MainActivity()
        {
            _loginLogic = new LoginLogic(this);
        }

        public MainActivity(int contentLayoutId) : base(contentLayoutId)
        {
            _loginLogic = new LoginLogic(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.sign_in);

            _username = FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
            _password = FindViewById<TextInputEditText>(Resource.Id.textInputEditText2);

            _usernameLayout = FindViewById<TextInputLayout>(Resource.Id.textInputLayout);
            _passwordLayout = FindViewById<TextInputLayout>(Resource.Id.textInputLayout2);

            _submitButton = FindViewById<Button>(Resource.Id.button);

            _username.FocusChange += (sender, args) =>
            {
                OnUsernameTouch?.Invoke();
            };
            _password.FocusChange += (sender, args) =>
            {
                OnPasswordTouch?.Invoke();
            };

            _submitButton.Click += async (sender, args) =>
            {
                var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(_submitButton.WindowToken, 0);
                if (OnSubmitButtonPress != null)
                    await OnSubmitButtonPress.Invoke();
            };
        }

        public string Username => _username.Text;
        public string UsernameError
        {
            get => _usernameLayout.Error;
            set => _usernameLayout.Error = value;
        }
        public Action OnUsernameTouch { get; set; }
        public string Password => _password.Text;
        public string PasswordError 
        { 
            get => _passwordLayout.Error;
            set => _passwordLayout.Error = value;
        }
        public Action OnPasswordTouch { get; set; }
        public Func<Task> OnSubmitButtonPress { get; set; }
        public Action OnGoToSignUpPress { get; set; }
        public Action OnGoBackPress { get; set; }
        
        public void GoToSignUp()
        {
            //throw new NotImplementedException();
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
            this.OnGoBackPress?.Invoke();
        }

        public void StartLoadingState()
        {
            Snackbar.Make(_submitButton, "Started Loading", Snackbar.LengthShort)
                     .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public void EndLoadingState()
        {
            Snackbar.Make(_submitButton, "Stopped Loading", Snackbar.LengthShort)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }
    }
}
