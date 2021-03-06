using System;
using System.Threading.Tasks;

namespace BusinessLogic.LoginRegister
{
    public interface ILoginScreen : IErrorScreen
    {
        string Username { get; }
        string UsernameError { get; set; }
        string Password { get; }
        string PasswordError { get; set; }

        Func<Task> OnSubmitButtonPress { get; set; }
        Action OnGoToSignUpPress { get; set; }
        Action OnGoBackPress { get; set; }

        void GoToSignUp();
        void GoToDashboard();
        void GoBack();

        void StartLoadingState();
        void EndLoadingState();

        string ErrorForUsername { get; }
        string ErrorForUsernameEmpty { get; }
        string ErrorForPassword { get; }
        string ErrorForPasswordEmpty { get; }

        string AccessToken { set; }
    }
}
