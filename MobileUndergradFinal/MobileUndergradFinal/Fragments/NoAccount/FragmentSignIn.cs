using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using BusinessLogic.LoginRegister;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using System;
using System.Threading.Tasks;
using MobileUndergradFinal.Activities;

namespace MobileUndergradFinal.Fragments.NoAccount
{
    public class FragmentSignIn : SwappableFragment, ILoginScreen
    {
        private TextInputEditText _username;
        private TextInputEditText _password;

        private TextInputLayout _usernameLayout;
        private TextInputLayout _passwordLayout;

        private Button _submitButton;

        private LoginLogic _loginLogic;

        public FragmentSignIn() : base(Resource.Layout.sign_in)
        {
            _loginLogic = new LoginLogic(this);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _username = view.FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
            _password = view.FindViewById<TextInputEditText>(Resource.Id.textInputEditText2);

            _usernameLayout = view.FindViewById<TextInputLayout>(Resource.Id.textInputLayout);
            _passwordLayout = view.FindViewById<TextInputLayout>(Resource.Id.textInputLayout2);

            _submitButton = view.FindViewById<Button>(Resource.Id.button);

            _username.Click += (sender, args) =>
            {
                _usernameLayout.Error = "";
            };
            _password.Click += (sender, args) =>
            {
                _passwordLayout.Error = "";
            };

            _username.FocusChange += (sender, args) =>
            {
                if (args.HasFocus)
                    _usernameLayout.Error = "";
            };
            _password.FocusChange += (sender, args) =>
            {
                if (args.HasFocus)
                    _passwordLayout.Error = "";
            };

            _submitButton.Click += async (sender, args) =>
            {
                var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(_submitButton.WindowToken, 0);
                if (OnSubmitButtonPress != null)
                    await OnSubmitButtonPress.Invoke();
            };

            var signUp = view.FindViewById<TextView>(Resource.Id.textView2);
            signUp.Click += (sender, args) =>
            {
                OnGoToSignUpPress?.Invoke();
            };
        }

        public override void OnBackPressed()
        {
            OnGoBackPress?.Invoke();
        }

        public string Username => _username.Text;
        public string UsernameError
        {
            get => _usernameLayout.Error;
            set => _usernameLayout.Error = value;
        }
        public string Password => _password.Text;
        public string PasswordError
        {
            get => _passwordLayout.Error;
            set => _passwordLayout.Error = value;
        }
        public Func<Task> OnSubmitButtonPress { get; set; }
        public Action OnGoToSignUpPress { get; set; }
        public Action OnGoBackPress { get; set; }

        public void GoToSignUp()
        {
            ((LoginRegisterActivity)Activity).GoToSignUp();
        }

        public void GoToDashboard()
        {
            ((LoginRegisterActivity)Activity).GoToDashboard();
        }

        public void GoBack()
        {
            ((LoginRegisterActivity) Activity).GoBack();
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

        public string ErrorForUsername => Resources.GetString(Resource.String.sign_in_username_error);
        public string ErrorForUsernameEmpty => Resources.GetString(Resource.String.sign_in_username_error_empty);
        public string ErrorForPassword => Resources.GetString(Resource.String.sign_in_password_error);
        public string ErrorForPasswordEmpty => Resources.GetString(Resource.String.sign_in_password_error_empty);

        public string AccessToken
        {
            set => ((TokenAndErrorActivity)Activity).AccessToken = value;
        }

        public void DisplayError(string error)
        {
            ((TokenAndErrorActivity)Activity).DisplayError(error);
        }
    }
}