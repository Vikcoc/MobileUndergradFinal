﻿namespace BusinessLogic.Dashboard
{
    public class DashboardLogic
    {
        private readonly IDashboardScreen _dashboardScreen;
        public DashboardLogic(IDashboardScreen dashboardScreen)
        {
            _dashboardScreen = dashboardScreen;
        }
    }
}