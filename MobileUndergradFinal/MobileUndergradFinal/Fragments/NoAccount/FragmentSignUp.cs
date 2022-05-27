using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Threading.Tasks;
using Android.Views.InputMethods;
using BusinessLogic.LoginRegister;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using MobileUndergradFinal.Activities;

namespace MobileUndergradFinal.Fragments.NoAccount
{
    public class FragmentSignUp : SwappableFragment, ISignUpScreen
    {
        private TextInputEditText _email;
        private TextInputEditText _username;
        private TextInputEditText _password;

        private TextInputLayout _emailLayout;
        private TextInputLayout _usernameLayout;
        private TextInputLayout _passwordLayout;

        private Button _submitButton;

        private SignUpLogic _signUp;
        public FragmentSignUp() : base(Resource.Layout.sign_up)
        {
            _signUp = new SignUpLogic(this);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _email = view.FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
            _username = view.FindViewById<TextInputEditText>(Resource.Id.textInputEditText2);
            _password = view.FindViewById<TextInputEditText>(Resource.Id.textInputEditText3);

            _emailLayout = view.FindViewById<TextInputLayout>(Resource.Id.textInputLayout);
            _usernameLayout = view.FindViewById<TextInputLayout>(Resource.Id.textInputLayout2);
            _passwordLayout = view.FindViewById<TextInputLayout>(Resource.Id.textInputLayout3);

            _submitButton = view.FindViewById<Button>(Resource.Id.button);

            _email.Click += (sender, args) =>
            {
                _emailLayout.Error = "";
            };
            _username.Click += (sender, args) =>
            {
                _usernameLayout.Error = "";
            };
            _password.Click += (sender, args) =>
            {
                _passwordLayout.Error = "";
            };

            _email.FocusChange += (sender, args) =>
            {
                if (args.HasFocus)
                    _emailLayout.Error = "";
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
                OnGoToSignInPress?.Invoke();
            };
        }

        public override void OnBackPressed()
        {
            OnGoBackPress?.Invoke();
        }

        public string Email => _email.Text;
        public string EmailError
        {
            get => _emailLayout.Error;
            set => _emailLayout.Error = value;
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
        public Action OnGoToSignInPress { get; set; }
        public Action OnGoBackPress { get; set; }
        public void GoToDashboard()
        {
            ((LoginRegisterActivity)Activity).GoToDashboard();
        }

        public void GoBack()
        {
            ((LoginRegisterActivity)Activity).GoToSignIn();
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

        public string ErrorForEmail => Resources.GetString(Resource.String.sign_up_email_error);
        public string ErrorForUsername => Resources.GetString(Resource.String.sign_up_username_error);
        public string ErrorForPassword => Resources.GetString(Resource.String.sign_up_password_error);

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