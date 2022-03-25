namespace BusinessLogic.Dashboard
{
    public class DashboardLogic
    {
        private readonly IDashboardScreen _dashboardScreen;
        public DashboardLogic(IDashboardScreen dashboardScreen)
        {
            _dashboardScreen = dashboardScreen;
            _dashboardScreen.OnAddNewFountainPress = _dashboardScreen.MoveToAddNewFountain;
            _dashboardScreen.OnSignOutPress = _dashboardScreen.SignOutAndMoveToLogin;
        }
    }
}