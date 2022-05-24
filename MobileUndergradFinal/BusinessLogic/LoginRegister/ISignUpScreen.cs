using System;
using System.Threading.Tasks;

namespace BusinessLogic.LoginRegister
{
    public interface ISignUpScreen : IErrorScreen
    {
        string Username { get; }
        string UsernameError { get; set; }
        Action OnUsernameTouch { get; set; }
        string Password { get; }
        string PasswordError { get; set; }
        Action OnPasswordTouch { get; set; }

        Func<Task> OnSubmitButtonPress { get; set; }
        Action OnGoToSignInPress { get; set; }
        Action OnGoBackPress { get; set; }
        void GoToDashboard();
        void GoBack();

        void StartLoadingState();
        void EndLoadingState();

        string ErrorForUsername { get; }
        string ErrorForPassword { get; }
        string AccessToken { set; }
    }
}
