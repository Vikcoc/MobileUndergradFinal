using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Communication.AccountDto;
using Network;
using Network.Response;

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

            _networkService = new NetworkService();
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

                var res = await _networkService.PostAsync<string>(RequestPaths.SignIn, signIn);

                _loginScreen.EndLoadingState();

                switch (res.ErrorType)
                {
                    case ErrorType.None:
                    {
                        _loginScreen.AccessToken = res.Data;
                        _loginScreen.GoToDashboard();
                        break;
                    }
                    case ErrorType.Actionable:
                    {
                        _loginScreen.UsernameError = _loginScreen.ErrorForUsername;
                        _loginScreen.PasswordError = _loginScreen.ErrorForPassword;
                        break;
                    }
                    case ErrorType.NonActionable:
                        _loginScreen.DisplayError(res.Error);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }
    }
}
