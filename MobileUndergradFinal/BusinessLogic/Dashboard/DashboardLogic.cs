using Network;
using Network.Response;
using System;
using System.Threading.Tasks;

namespace BusinessLogic.Dashboard
{
    public class DashboardLogic
    {
        private readonly IDashboardScreen _dashboardScreen;
        private readonly NetworkService _networkService;
        public DashboardLogic(IDashboardScreen dashboardScreen)
        {
            _dashboardScreen = dashboardScreen;
            _networkService = new NetworkService();


            _dashboardScreen.OnAddNewFountainPress = _dashboardScreen.MoveToAddNewFountain;
            _dashboardScreen.OnSignOutPress = _dashboardScreen.SignOutAndMoveToLogin;
            _dashboardScreen.OnScreenVisible = LoadScreen;
        }

        public async Task LoadScreen()
        {
            _networkService.BearerToken = _dashboardScreen.AccessToken;
            var res = await _networkService.GetAsync<string>(RequestPaths.UserName);

            switch (res.ErrorType)
            {
                case ErrorType.None:
                {
                    _dashboardScreen.Welcome = _dashboardScreen.WelcomeText + " " + res.Data;
                    break;
                }
                case ErrorType.Actionable:
                    if(res.Error == ErrorStrings.Unauthorized)
                    {
                        _dashboardScreen.SignOutAndMoveToLogin();
                        return;
                    }

                    _dashboardScreen.DisplayError(res.Error);
                    break;
                case ErrorType.NonActionable:
                    _dashboardScreen.DisplayError(res.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}