using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Communication.AccountDto;
using Network;

namespace BusinessLogic.LoginRegister
{
    public class LoginLogic
    {
        private readonly ILoginScreen _loginScreen;
        private readonly NetworkService _networkService;

        public LoginLogic(ILoginScreen loginScreen)
        {
            _loginScreen = loginScreen;

            _loginScreen.OnGoBackPress = GoBack;
            _loginScreen.OnSubmitButtonPress = SignIn;
            _loginScreen.OnPasswordTouch = RemoveError;
            _loginScreen.OnUsernameTouch = RemoveError;
            _loginScreen.OnGoToSignUpPress = GoToSignUp;

            _networkService = new NetworkService("http://192.168.0.152:5000/api");
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

        private async Task SignIn()
        {
            var makeRequest = true;
            if (string.IsNullOrWhiteSpace(_loginScreen.Username))
            {
                makeRequest = false;
                _loginScreen.UsernameError = _loginScreen.ErrorForUsernameEmpty;
            }

            if (string.IsNullOrWhiteSpace(_loginScreen.Password))
            {
                makeRequest = false;
                _loginScreen.PasswordError = _loginScreen.ErrorForPasswordEmpty;
            }

            if (makeRequest)
            {
                _loginScreen.StartLoadingState();

                var signIn = new UserSignInDto
                {
                    Email = _loginScreen.Username,
                    Password = _loginScreen.Password
                };

                await _networkService.PostAsync<string>(RequestPaths.SignIn, signIn, a =>
                {
                    _loginScreen.AccessToken = a;
                    _loginScreen.GoToDashboard();

                }, a =>
                {
                    Debug.WriteLine(a.Aggregate((x, y) => x + '\n' + y));
                    _loginScreen.UsernameError = _loginScreen.ErrorForUsername;
                    _loginScreen.PasswordError = _loginScreen.ErrorForPassword;
                });

                _loginScreen.EndLoadingState();

            }
        }
    }
}
