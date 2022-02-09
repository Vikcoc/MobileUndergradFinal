﻿using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Communication.AccountDto;
using Network;

namespace BusinessLogic.LoginRegister
{
    public class SignUpLogic
    {
        private readonly ISignUpScreen _signUpScreen;
        private readonly NetworkService _networkService;

        public SignUpLogic(ISignUpScreen signUpScreen)
        {
            _signUpScreen = signUpScreen;


            _networkService = new NetworkService("http://192.168.0.152:5000/api");
            _signUpScreen.OnSubmitButtonPress += SignUp;
            _signUpScreen.OnGoToSignInPress += GoBack;
            _signUpScreen.OnGoBackPress += GoBack;
            _signUpScreen.OnUsernameTouch += UsernameTouch;
            _signUpScreen.OnPasswordTouch += PasswordTouch;
        }

        private void PasswordTouch()
        {
            _signUpScreen.PasswordError = "";
        }

        private void GoBack()
        {
            _signUpScreen.GoBack();
        }

        private void UsernameTouch()
        {
            _signUpScreen.UsernameError = "";
        }

        private async Task SignUp()
        {
            var makeRequest = true;
            var regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            if (!regex.IsMatch(_signUpScreen.Username))
            {
                makeRequest = false;
                _signUpScreen.UsernameError = _signUpScreen.ErrorForUsername;
            }

            var regex2 = new Regex("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@_$!%*?&-])[A-Za-z\\d@_$!%*?&-]{8,}");
            if (!regex2.IsMatch(_signUpScreen.Password))
            {
                makeRequest = false;
                _signUpScreen.PasswordError = _signUpScreen.ErrorForPassword;
            }

            if (makeRequest)
            {
                _signUpScreen.StartLoadingState();

                var signUp = new UserSignUpDto
                {
                    Email = _signUpScreen.Username,
                    Password = _signUpScreen.Password
                };

                await _networkService.PostAsync<string>(RequestPaths.SignUp, signUp, a =>
                {
                    Debug.WriteLine(a);
                    _signUpScreen.GoToDashboard();

                }, a =>
                {
                    Debug.WriteLine(a.Aggregate((x, y) => x + '\n' + y));
                });

                _signUpScreen.EndLoadingState();

            }
        }
    }
}