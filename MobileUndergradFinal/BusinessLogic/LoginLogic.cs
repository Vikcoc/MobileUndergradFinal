using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Communication.AccountDto;
using Network;

namespace BusinessLogic
{
    public class LoginLogic
    {
        private readonly ILoginScreen _loginScreen;

        public LoginLogic(ILoginScreen loginScreen)
        {
            _loginScreen = loginScreen;

            _loginScreen.OnGoBackPress = GoBack;
            _loginScreen.OnSubmitButtonPress = SignUp;
            _loginScreen.OnPasswordTouch = RemoveError;
            _loginScreen.OnUsernameTouch = RemoveError;
            _loginScreen.OnGoToSignUpPress = GoToSignUp;
        }

        private void GoToSignUp()
        {
            _loginScreen.GoToSignUp();
        }

        private void RemoveError()
        {
            _loginScreen.UsernameError = "";
            _loginScreen.PasswordError = "";
        }

        private void GoBack()
        {
            _loginScreen.GoBack();
        }

        private async Task SignUp()
        {
            var makeRequest = true;
            if (string.IsNullOrWhiteSpace(_loginScreen.Username))
            {
                makeRequest = false;
                _loginScreen.UsernameError = "Please fill username";
            }

            if (string.IsNullOrWhiteSpace(_loginScreen.Password))
            {
                makeRequest = false;
                _loginScreen.PasswordError = "Please fill password";
            }

            if (makeRequest)
            {
                _loginScreen.StartLoadingState();

                var net = new NetworkService("http://192.168.0.152:5000/api", a => Debug.WriteLine(a.FirstOrDefault()));

                var signIn = new UserSignInDto
                {
                    Email = _loginScreen.Username,
                    Password = _loginScreen.Password
                };

                await net.PostAsync<string>("Account/sign_in", signIn, a =>
                {
                    Debug.WriteLine(a);
                    _loginScreen.GoToDashboard();

                });

                _loginScreen.EndLoadingState();

            }
        }
    }
}
