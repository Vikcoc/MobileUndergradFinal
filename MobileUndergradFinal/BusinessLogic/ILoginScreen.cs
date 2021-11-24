using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public interface ILoginScreen
    {
        string Username { get; }
        string UsernameError { get; set; }
        Action OnUsernameTouch { get; set; }
        string Password { get; }
        string PasswordError { get; set; }
        Action OnPasswordTouch { get; set; }

        Action OnSubmitButtonPress { get; set; }
        Func<Task> OnGoToSignUpPress { get; set; }
        Action OnGoBackPress { get; set; }

        ISignUpScreen GoToSignUp();
        IDashboardScreen GoToDashboard();
        void GoBack();

        void StartLoadingState();
        void EndLoadingState();
    }
}
