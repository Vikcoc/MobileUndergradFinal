using System;
using System.Threading.Tasks;
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
            _loginScreen.OnGoToSignUpPress = SignUp;
        }

        public void GoBack()
        {
            _loginScreen.GoBack();
        }

        public async Task SignUp()
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

                var net = new NetworkService("", null);

                await net.GetAsync<object>("", (a) =>
                {
                    var dashboardLogic = new DashboardLogic(_loginScreen.GoToDashboard());

                });

                _loginScreen.EndLoadingState();

            }
        }
    }
}
