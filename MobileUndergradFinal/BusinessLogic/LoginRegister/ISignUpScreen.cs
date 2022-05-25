using System;
using System.Threading.Tasks;

namespace BusinessLogic.LoginRegister
{
    public interface ISignUpScreen : IErrorScreen
    {
        string Email { get; }
        string EmailError { get; set; }
        Action OnEmailTouch { get; set; }
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

        string ErrorForEmail { get; }
        string ErrorForUsername { get; }
        string ErrorForPassword { get; }
        string AccessToken { set; }
    }
}
