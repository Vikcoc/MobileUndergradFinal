using Communication.AccountDto;
using Network;
using Network.Response;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogic.LoginRegister
{
    public class SignUpLogic
    {
        private readonly ISignUpScreen _signUpScreen;
        private readonly NetworkService _networkService;

        public SignUpLogic(ISignUpScreen signUpScreen)
        {
            _signUpScreen = signUpScreen;


            _networkService = new NetworkService();
            _signUpScreen.OnSubmitButtonPress += SignUp;
            _signUpScreen.OnGoToSignInPress += GoBack;
            _signUpScreen.OnGoBackPress += GoBack;
        }

        private void GoBack()
        {
            _signUpScreen.GoBack();
        }

        private async Task SignUp()
        {
            var makeRequest = true;
            var regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            if (!regex.IsMatch(_signUpScreen.Email))
            {
                makeRequest = false;
                _signUpScreen.EmailError = _signUpScreen.ErrorForEmail;
            }

            var regex2 = new Regex("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@_$!%*?&-])[A-Za-z\\d@_$!%*?&-]{8,}");
            if (!regex2.IsMatch(_signUpScreen.Password))
            {
                makeRequest = false;
                _signUpScreen.PasswordError = _signUpScreen.ErrorForPassword;
            }

            if (string.IsNullOrWhiteSpace(_signUpScreen.Username))
            {
                makeRequest = false;
                _signUpScreen.UsernameError = _signUpScreen.ErrorForUsername;
            }

            if (makeRequest)
            {
                _signUpScreen.StartLoadingState();

                var signUp = new UserSignUpDto
                {
                    Email = _signUpScreen.Email,
                    Password = _signUpScreen.Password,
                    UserName = _signUpScreen.Username
                };

                var res = await _networkService.PostAsync<string>(RequestPaths.SignUp, signUp);

                _signUpScreen.EndLoadingState();

                switch (res.ErrorType)
                {
                    case ErrorType.None:
                        _signUpScreen.AccessToken = res.Data;
                        _signUpScreen.GoToDashboard();
                        break;
                    case ErrorType.Actionable:
                        _signUpScreen.DisplayError(res.Error);
                        break;
                    case ErrorType.NonActionable:
                        _signUpScreen.DisplayError(res.Error);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }
    }
}
